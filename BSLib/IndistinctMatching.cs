﻿/*
 *  "BSLib".
 *  Copyright (C) 2009-2017 by Sergey V. Zhdanovskih.
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

namespace BSLib
{
    public static class IndistinctMatching
    {
        // quality checked
        private static int Minimum(int a, int b, int c)
        {
            int min = a;
            if (b < min) min = b;
            if (c < min) min = c;
            return min;
        }

        // quality checked, reduced memory useage
        public static int LevenshteinDistance(string value1, string value2)
        {
            // Return trivial case - where they are equal
            if (value1.Equals(value2)) return 0;

            if (string.IsNullOrEmpty(value1))
            {
                return string.IsNullOrEmpty(value2) ? 0 : value2.Length;
            }

            if (string.IsNullOrEmpty(value2)) {
                return value1.Length;
            }

            if (value1.Length > value2.Length) {
                string temp = value2;
                value2 = value1;
                value1 = temp;
            }

            // Return trivial case - where value1 is contained within value2
            if (value2.Contains(value1)) return value2.Length - value1.Length;

            int m = value2.Length;
            int n = value1.Length;

            int[,] dist = new int[2, m + 1];
            for (int j = 1; j <= m; j++) dist[0, j] = j;

            int curRow = 0;
            for (int i = 1; i <= n; ++i) {
                curRow = i & 1;
                dist[curRow, 0] = i;
                int prevRow = curRow ^ 1;

                for (int j = 1; j <= m; j++) {
                    int cost = (value2[j - 1] == value1[i - 1] ? 0 : 1);
                    dist[curRow, j] = Minimum(dist[prevRow, j] + 1, dist[curRow, j - 1] + 1, dist[prevRow, j - 1] + cost);
                }
            }

            return dist[curRow, m];
        }


        public static int DamerauLevenshteinDistance(string value1, string value2)
        {
            // Return trivial case - where they are equal
            if (value1.Equals(value2))
                return 0;

            // Return trivial case - where one is empty
            if (string.IsNullOrEmpty(value1) || string.IsNullOrEmpty(value2))
                return (value1 ?? "").Length + (value2 ?? "").Length;


            // Ensure value2 (inner cycle) is longer
            if (value1.Length > value2.Length)
            {
                string tmp = value1;
                value1 = value2;
                value2 = tmp;
            }

            // Return trivial case - where value1 is contained within value2
            if (value2.Contains(value1))
                return value2.Length - value1.Length;

            int length1 = value1.Length;
            int length2 = value2.Length;

            int[,] d = new int[length1 + 1, length2 + 1];

            for (int i = 0; i <= d.GetUpperBound(0); i++)
                d[i, 0] = i;

            for (int i = 0; i <= d.GetUpperBound(1); i++)
                d[0, i] = i;

            for (int i = 1; i <= d.GetUpperBound(0); i++)
            {
                for (int j = 1; j <= d.GetUpperBound(1); j++)
                {
                    int cost = value1[i - 1] == value2[j - 1] ? 0 : 1;

                    int del = d[i - 1, j] + 1;
                    int ins = d[i, j - 1] + 1;
                    int sub = d[i - 1, j - 1] + cost;

                    d[i, j] = Math.Min(del, Math.Min(ins, sub));

                    if (i > 1 && j > 1 && value1[i - 1] == value2[j - 2] && value1[i - 2] == value2[j - 1])
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }


        public static float GetSimilarity(string value1, string value2)
        {
            if (value1 == null)
                throw new ArgumentNullException("value1");
            if (value2 == null)
                throw new ArgumentNullException("value2");

            if (value1 == value2)
                return 1;

            int longestLenght = Math.Max(value1.Length, value2.Length);
            int distance = DamerauLevenshteinDistance(value1, value2);
            float percent = distance / (float)longestLenght;
            return 1.0f - percent;
        }
    }
}
