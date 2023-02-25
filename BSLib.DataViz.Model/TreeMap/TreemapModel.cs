/*
 *  "BSLib.DataViz".
 *  Copyright (C) 2017-2023 by Sergey V. Zhdanovskih.
 *
 *  This file is part of "BSLib".
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;

namespace BSLib.DataViz.TreeMap
{
    public class TMHintRequestEventArgs : EventArgs
    {
        public MapItem MapItem { get; private set; }

        public string Hint { get; set; }

        public TMHintRequestEventArgs(MapItem mapItem)
        {
            MapItem = mapItem;
        }
    }


    public delegate void TMHintRequestEventHandler(object sender, TMHintRequestEventArgs args);


    public delegate MapItem CreateItemEventHandler(MapItem parent, string name, double size);


    /// <summary>
    /// TreemapModel. Model object used to represent data for a treemap.
    /// 
    /// Implements the Squarified Treemap layout published by Mark Bruls, Kees
    /// Huizing, and Jarke J. van Wijk
    ///
    /// Squarified Treemaps https://www.win.tue.nl/~vanwijk/stm.pdf
    /// </summary>
    public class TreemapModel
    {
        private readonly List<MapItem> fItems;


        /// <summary>
        /// Get the list of items in this model.
        /// </summary>
        /// <returns>An array of the MapItem objects in this MapModel.</returns>
        public List<MapItem> Items
        {
            get { return fItems; }
        }


        public event CreateItemEventHandler CreatingItem;


        /// <summary>
        /// Creates a Map Model instance based on the relative size of the mappable items and the frame size.
        /// </summary>
        public TreemapModel()
        {
            fItems = new List<MapItem>();
        }

        public MapItem CreateItem(MapItem parent, string name, double size)
        {
            MapItem result;

            var handler = CreatingItem;
            if (handler != null) {
                result = handler(parent, name, size);
            } else {
                result = new MapItem(parent, name, size);
            }

            if (parent == null) {
                fItems.Add(result);
            } else {
                parent.AddItem(result);
            }

            return result;
        }

        public MapItem FindByCoord(int x, int y)
        {
            int num = fItems.Count;
            for (int i = 0; i < num; i++) {
                MapItem item = fItems[i];
                MapItem found = item.FindByCoord(x, y);
                if (found != null) {
                    return found;
                }
            }

            return null;
        }

        public MapItem FindByCoord(int x, int y, out MapItem upperItem)
        {
            return FindByCoord(fItems, x, y, out upperItem);
        }

        public MapItem FindByCoord(List<MapItem> itemsList, int x, int y, out MapItem upperItem)
        {
            upperItem = null;

            int num = itemsList.Count;
            for (int i = 0; i < num; i++) {
                MapItem item = itemsList[i];
                MapItem found = item.FindByCoord(x, y);
                if (found != null) {
                    upperItem = item;
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Arrange the items in the given MapModel to fill the given rectangle.
        /// </summary>
        /// <param name="bounds">The bounding rectangle for the layout.</param>
        public void CalcLayout(List<MapItem> items, MapRect bounds, int headerHeight, int padding = 4)
        {
            // calculate all true sizes for treemap
            int num = items.Count;
            for (int i = 0; i < num; i++) {
                MapItem item = items[i];
                item.CalculateSize();
            }

            // calculate bounds of item for all levels
            CalcRecursiveLayout(items, bounds, headerHeight, padding);
        }

        private static void CalcRecursiveLayout(List<MapItem> items, MapRect bounds, int headerHeight, int padding = 4)
        {
            if (items == null) return;

            int itemsNum = items.Count;
            if (itemsNum <= 0) return;

            // calculate sum for current level
            double sum = 0;
            for (int i = 0; i < itemsNum; i++) {
                MapItem item = items[i];
                sum += item.GetCalcSize();
            }

            // calculate relative sizes for current level
            double totalArea = bounds.W * bounds.H;
            for (int i = 0; i < itemsNum; i++) {
                MapItem item = items[i];
                item.Ratio = (totalArea / sum * item.GetCalcSize());
            }

            items.Sort(ItemsCompare);

            CalcLayout(items, 0, itemsNum - 1, bounds);

            for (int i = 0; i < itemsNum; i++) {
                MapItem item = items[i];
                int subCount = item.Items.Count;
                if (subCount > 0) {
                    var clientBounds = item.Bounds;
                    if (subCount > 1) {
                        clientBounds.Inflate(headerHeight, padding);
                    }
                    CalcRecursiveLayout(item.Items, clientBounds, headerHeight, padding);
                }
            }
        }

        private static int ItemsCompare(MapItem it1, MapItem it2)
        {
            return -it1.Ratio.CompareTo(it2.Ratio);
        }

        private static int CalcLayout(List<MapItem> items, int start, int end, MapRect bounds)
        {
            if (start > end) {
                return -1;
            }
            if (start == end) {
                items[start].Bounds = bounds;
            }

            int mid = start;
            while (mid < end) {
                if (GetHighestAspect(items, start, mid, bounds) > GetHighestAspect(items, start, mid + 1, bounds)) {
                    mid++;
                } else {
                    MapRect newBounds = LayoutRow(items, start, mid, bounds);
                    int res = CalcLayout(items, mid + 1, end, newBounds);
                    if (res != -1) mid = res;
                }
            }
            return mid;
        }

        private static float GetHighestAspect(List<MapItem> items, int start, int end, MapRect bounds)
        {
            LayoutRow(items, start, end, bounds);

            float max = float.MinValue;
            for (int i = start; i <= end; i++) {
                float aspectRatio = items[i].Bounds.GetAspectRatio();
                if (max < aspectRatio) {
                    max = aspectRatio;
                }
            }
            return max;
        }

        private static MapRect LayoutRow(IList<MapItem> items, int start, int end, MapRect bounds)
        {
            bool isHorizontal = bounds.W > bounds.H;
            float total = bounds.W * bounds.H;

            double rowTotalRatio = 0;
            for (int i = start; i <= end; i++) {
                rowTotalRatio += items[i].Ratio;
            }

            float rowRatio = (float)(rowTotalRatio / total);
            float offset = 0;

            for (int i = start; i <= end; i++) {
                MapItem item = items[i];
                float ratio = (float)(item.Ratio / rowTotalRatio);

                float rX, rY, rW, rH;
                if (isHorizontal) {
                    rX = bounds.X;
                    rW = bounds.W * rowRatio;
                    rY = bounds.Y + bounds.H * offset;
                    rH = bounds.H * ratio;
                } else {
                    rX = bounds.X + bounds.W * offset;
                    rW = bounds.W * ratio;
                    rY = bounds.Y;
                    rH = bounds.H * rowRatio;
                }
                item.SetBounds(rX, rY, rW, rH);

                offset += ratio;
            }

            if (isHorizontal) {
                return new MapRect(bounds.X + bounds.W * rowRatio, bounds.Y, bounds.W - bounds.W * rowRatio, bounds.H);
            } else {
                return new MapRect(bounds.X, bounds.Y + bounds.H * rowRatio, bounds.W, bounds.H - bounds.H * rowRatio);
            }
        }
    }
}
