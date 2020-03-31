/*
 *  "BSLib.Timeline".
 *  Copyright (C) 2019-2020 by Sergey V. Zhdanovskih.
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace BSLib.Timeline
{
    /// <summary>
    ///   Event arguments for an event that notifies about a change in the selection of tracks.
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        ///   The tracks that were selected in the operation.
        /// </summary>
        public IEnumerable<ITimeObject> Selected { get; private set; }

        /// <summary>
        ///   The track elements that were deselected in the operation.
        /// </summary>
        public IEnumerable<ITimeObject> Deselected { get; private set; }

        /// <summary>
        ///   Construct a new SelectionChangedEventArgs instance.
        /// </summary>
        /// <param name="selected">The track elements that were deselected in the operation.</param>
        /// <param name="deselected">The tracks that were selected in the operation.</param>
        public SelectionChangedEventArgs(IEnumerable<ITimeObject> selected, IEnumerable<ITimeObject> deselected)
        {
            Selected = selected;
            Deselected = deselected;
        }

        /// <summary>
        ///   An empty instance of the <see cref="SelectionChangedEventArgs"/> class.
        /// </summary>
        public new static SelectionChangedEventArgs Empty
        {
            get { return new SelectionChangedEventArgs(null, null); }
        }
    }


    /// <summary>
    ///   The main host control.
    /// </summary>
    public class TimelineViewer : Panel
    {
        /// <summary>
        ///   Enumerates states the timeline can be in.
        ///   These are usually invoked through user interaction.
        /// </summary>
        private enum BehaviorMode
        {
            /// <summary>
            ///   The timeline is idle or not using any more specific state.
            /// </summary>
            Idle,
            /// <summary>
            ///   The user is currently in the process of selecting items on the timeline.
            /// </summary>
            Selecting,
            /// <summary>
            ///   The user is scrubbing the playhead.
            /// </summary>
            TimeScrub
        }


        private Color fBackgroundColor;
        private Pen fBrightPen;
        private double fClockValue;
        private BehaviorMode fCurrentMode;
        private Range<float> fDataRange;
        private int fGridAlpha;
        private Pen fGridPen;
        private EventFrame fHighlightedFrame;
        private ExtSize fImageSize;
        private readonly Font fLabelFont;
        private readonly StringFormat fLabelsFormat;
        private Pen fMinorGridPen;
        private PointF fPanOrigin;
        private SizeF fPlayheadExtents;
        private PointF fRenderingOffset;
        private PointF fRenderingOffsetBeforePan;
        private float fRenderingScale;
        private readonly List<EventFrame> fSelectedFrames;
        private Point fSelectionOrigin;
        private readonly Pen fSelectionPen;
        private Rectangle fSelectionRectangle;
        private readonly ToolTip fToolTip;
        private List<string> fToolTipText;
        private int fTrackBorderSize;
        private List<Color> fTrackColors;
        private int fTrackHeight;
        private int fTrackLabelWidth;
        private int fTrackSpacing;
        private readonly List<Track> fTracks;


        /// <summary>
        ///   The background color of the timeline.
        /// </summary>
        [Description("The background color of the timeline.")]
        [Category("Drawing")]
        public Color BackgroundColor
        {
            get { return fBackgroundColor; }
            set { fBackgroundColor = value; }
        }

        /// <summary>
        ///   The transparency of the background grid.
        /// </summary>
        [Description("The transparency of the background grid.")]
        [Category("Drawing")]
        public int GridAlpha
        {
            get { return fGridAlpha; }
            set {
                fGridAlpha = value;
                RecreatePens();
            }
        }

        /// <summary>
        ///   Which tracks are currently selected?
        /// </summary>
        public IEnumerable<EventFrame> SelectedFrames
        {
            get { return fSelectedFrames; }
        }

        /// <summary>
        ///   Invoked when the selection of track elements changed.
        ///   Inspect <see cref="SelectedFrames"/> to see the current selection.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        ///   How high a single track should be.
        /// </summary>
        [Description("How high a single track should be.")]
        [Category("Layout")]
        public int TrackHeight
        {
            get { return fTrackHeight; }
            set { fTrackHeight = value; }
        }

        /// <summary>
        ///   How wide/high the border on a track item should be.
        ///   This border allows you to interact with an item.
        /// </summary>
        [Description("How wide/high the border on a track item should be.")]
        [Category("Layout")]
        public int TrackBorderSize
        {
            get { return fTrackBorderSize; }
            set { fTrackBorderSize = value; }
        }

        /// <summary>
        ///   How much space should be left between every track.
        /// </summary>
        [Description("How much space should be left between every track.")]
        [Category("Layout")]
        public int TrackSpacing
        {
            get { return fTrackSpacing; }
            set { fTrackSpacing = value; }
        }

        /// <summary>
        ///   The width of the label section before the tracks.
        /// </summary>
        [Description("The width of the label section before the tracks.")]
        [Category("Layout")]
        private int TrackLabelWidth
        {
            get { return fTrackLabelWidth; }
            set { fTrackLabelWidth = value; }
        }


        /// <summary>
        ///   Construct a new timeline.
        /// </summary>
        public TimelineViewer()
        {
            AutoScroll = true;
            ResizeRedraw = true;

            BackColor = Color.FromArgb(64, 64, 64);
            DoubleBuffered = true;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.Opaque |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.Selectable |
                ControlStyles.UserPaint, true);

            fToolTip = new ToolTip();
            fToolTip.AutoPopDelay = 15000;
            fToolTip.InitialDelay = 250;
            fToolTip.OwnerDraw = true;
            fToolTip.Draw += ToolTip_Draw;
            fToolTip.Popup += ToolTip_Popup;

            fBackgroundColor = Color.Black;
            fGridAlpha = 40;
            fImageSize = ExtSize.Empty;
            fLabelsFormat = new StringFormat(StringFormatFlags.FitBlackBox);
            fPlayheadExtents = new SizeF(5, 16);
            fRenderingOffset = PointF.Empty;
            fRenderingOffsetBeforePan = PointF.Empty;
            fRenderingScale = 1.0f;
            fSelectedFrames = new List<EventFrame>();
            fSelectionPen = new Pen(Color.LightGray, 1) { DashStyle = DashStyle.Dot };
            fSelectionRectangle = Rectangle.Empty;
            fTrackBorderSize = 2;
            fTrackHeight = 20;
            fTrackLabelWidth = 100;
            fTrackSpacing = 1;
            fTracks = new List<Track>();

            // Set up the font to use to draw the track labels
            float emHeightForLabel = EmHeightForLabel("WM_g^~", TrackHeight);
            fLabelFont = new Font(DefaultFont.FontFamily, emHeightForLabel - 2);

            RecreatePens();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                fSelectionPen.Dispose();
                fLabelsFormat.Dispose();

                if (fGridPen != null) fGridPen.Dispose();
                if (fMinorGridPen != null) fMinorGridPen.Dispose();
                if (fBrightPen != null) fBrightPen.Dispose();
            }
            base.Dispose(disposing);
        }

        public void Clear()
        {
            fTracks.Clear();
            Recalculate();
            Invalidate();
        }

        /// <summary>
        ///   Add an event frame to the timeline.
        /// </summary>
        /// <param name="eventFrame">The event frame to add.</param>
        public void AddEventFrame(EventFrame eventFrame)
        {
            fTracks.Add(new Track(eventFrame));
            Recalculate();
            Invalidate();
        }

        /// <summary>
        ///   Add a track to the timeline which contains multiple event frames.
        /// </summary>
        /// <param name="track"></param>
        public void AddTrack(Track track)
        {
            fTracks.Add(track);
            Recalculate();
            Invalidate();
        }

        /// <summary>
        ///   Recalculates appropriate values for scrollbar bounds.
        /// </summary>
        private void Recalculate()
        {
            fDataRange = GetDataBounds();
            float distance = fDataRange.End - fDataRange.Start;
            fImageSize = new ExtSize((int)(distance * fRenderingScale),
                                     (int)((fTracks.Count * (TrackHeight + TrackSpacing)) * fRenderingScale));
            AdjustViewport(fImageSize);

            // Generate colors for the tracks.
            fTrackColors = GetRandomColors(fTracks.Count);
        }

        /// <summary>
        ///   Calculate the rectangle within which track should be drawn.
        /// </summary>
        /// <returns>The rectangle within which all tracks should be drawn.</returns>
        private Rectangle GetTracksAreaBounds()
        {
            Rectangle clientRect = ClientRectangle;

            Rectangle tracksArea = new Rectangle();

            // Start after the track labels
            tracksArea.X = TrackLabelWidth - -AutoScrollPosition.X;
            // Start at the top (later, we'll deduct the playhead and time label height)
            tracksArea.Y = (int)fPlayheadExtents.Height - -AutoScrollPosition.Y;
            // Deduct scrollbar width.
            tracksArea.Width = clientRect.Width - TrackLabelWidth;
            // Deduct scrollbar height.
            tracksArea.Height = clientRect.Height - (int)fPlayheadExtents.Height;

            return tracksArea;
        }

        private Range<float> GetDataBounds()
        {
            float min, max;

            if (fTracks.Count > 0) {
                min = float.MaxValue;
                max = float.MinValue;
            } else {
                min = 0;
                max = 0;
            }

            foreach (Track track in fTracks) {
                foreach (EventFrame frame in track.Frames) {
                    float frameStart = (float)frame.Start.ToOADate();
                    float frameEnd = (float)frame.End.ToOADate();

                    min = Math.Min(min, frameStart);
                    max = Math.Max(max, frameEnd);
                }
            }

            min *= 0.999f;
            max *= 1.001f;

            if (float.IsNaN(min) || float.IsInfinity(min) || float.IsNaN(max) || float.IsInfinity(max)) {
                min = 0;
                max = 0;
            }

            return new Range<float>(min, max);
        }

        /// <summary>
        ///   Check if an event frame is located at the given position.
        /// </summary>
        /// <param name="test">The point to test for.</param>
        /// <returns>
        ///   The <see cref="EventFrame" /> if there is one under the given point; <see langword="null" /> otherwise.
        /// </returns>
        private EventFrame HitTest(PointF test)
        {
            foreach (EventFrame frame in fTracks.SelectMany( t => t.Frames )) {
                // The extent of the frame, including the border
                RectangleF frameExtent = GetFrameExtents(frame);

                if (frameExtent.Contains(test)) {
                    return frame;
                }
            }

            return null;
        }

        /// <summary>
        ///   Check if a track label is located at the given position.
        /// </summary>
        /// <param name="test">The point to test for.</param>
        /// <returns>The index of the track the hit label belongs to, if one was hit; -1 otherwise.</returns>
        private int TrackLabelHitTest(PointF test)
        {
            if (test.X > 0 && test.X < TrackLabelWidth) {
                for (int index = 0; index < fTracks.Count; index++) {
                    Track track = fTracks[index];
                    RectangleF frameExtent = GetFrameExtents(track.Frames.First());

                    if (frameExtent.Top < test.Y && frameExtent.Bottom > test.Y) {
                        return index;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        ///   Calculate an Em-height for a font to fit within a given height.
        /// </summary>
        /// <param name="label">The text to use for the measurement.</param>
        /// <param name="maxHeight">The largest height the text must fit into.</param>
        /// <returns>An Em-height that can be used to construct a font that will fit into the given height.</returns>
        private float EmHeightForLabel(string label, float maxHeight)
        {
            float size = DefaultFont.Size;
            Font currentFont = new Font(DefaultFont.FontFamily, size);
            Graphics graphics = Graphics.FromHwnd(this.Handle);
            SizeF measured = graphics.MeasureString(label, currentFont);
            while (measured.Height < maxHeight) {
                size += 1;
                currentFont = new Font(DefaultFont.FontFamily, size);
                measured = graphics.MeasureString(label, currentFont);
            }
            return size - 1;
        }

        /// <summary>
        ///   Set the clock to a position that relates to a given position on the playhead area.
        ///   Current rendering offset and scale will be taken into account.
        /// </summary>
        /// <param name="location">The location on the playhead area.</param>
        private void SetClockFromMousePosition(PointF location)
        {
            Rectangle trackAreaBounds = GetTracksAreaBounds();
            // Calculate a clock value for the current X coordinate.
            fClockValue = (location.X - fRenderingOffset.X - trackAreaBounds.X) * (1 / fRenderingScale);
        }

        private void RecreatePens()
        {
            fGridPen = new Pen(Color.FromArgb(GridAlpha, GridAlpha, GridAlpha));
            fMinorGridPen = new Pen(Color.FromArgb(30, 30, 30)) { DashStyle = DashStyle.Dot };

            int minutePenColor = (int)(255 * Math.Min(255, GridAlpha * 2) / 255f);
            fBrightPen = new Pen(Color.FromArgb(minutePenColor, minutePenColor, minutePenColor));
        }

        /// <summary>
        ///   Draws the background of the control.
        /// </summary>
        private void DrawBackground(Graphics graphics)
        {
            Rectangle trackAreaBounds = GetTracksAreaBounds();

            // Draw horizontal grid.
            // Grid is white so just take the alpha as the white value.
            // Calculate the Y position of the first line.
            int firstLineY = (int)(TrackHeight * fRenderingScale + trackAreaBounds.Y + fRenderingOffset.Y);
            // Calculate the distance between each following line.
            int actualRowHeight = (int)((TrackHeight) * fRenderingScale + TrackSpacing);
            actualRowHeight = Math.Max(1, actualRowHeight);
            // Draw the actual lines.
            for (int y = firstLineY; y < Height; y += actualRowHeight) {
                graphics.DrawLine(fGridPen, trackAreaBounds.X, y, trackAreaBounds.Width, y);
            }

            // The distance between the minor ticks.
            float minorTickDistance = fRenderingScale;
            int minorTickOffset = (int)(fRenderingOffset.X % minorTickDistance);

            // The distance between the regular ticks.
            int tickDistance = Math.Max(1, (int)(10f * fRenderingScale));

            // The distance between minute ticks
            int minuteDistance = tickDistance * 6;

            // Draw a vertical grid. Every 10 ticks, we place a line.
            int tickOffset = (int)(fRenderingOffset.X % tickDistance);
            int minuteOffset = (int)(fRenderingOffset.X % minuteDistance);

            // Calculate the distance between each column line.
            int columnWidth = Math.Max(1, (int)(10 * fRenderingScale));

            // Should we draw minor ticks?
            if (minorTickDistance > 5.0f) {
                for (float x = minorTickOffset; x < Width; x += minorTickDistance) {
                    graphics.DrawLine(fMinorGridPen, trackAreaBounds.X + x, trackAreaBounds.Y, trackAreaBounds.X + x, trackAreaBounds.Height);
                }
            }

            // We start one tick distance after the offset to draw the first line that is actually in the display area
            // The one that is only tickOffset pixels away it behind the track labels.
            for (int x = tickOffset + tickDistance; x < Width; x += columnWidth) {
                // Every 60 ticks, we put a brighter, thicker line.
                Pen penToUse = ((x - minuteOffset) % minuteDistance == 0) ? fBrightPen : fGridPen;
                graphics.DrawLine(penToUse, trackAreaBounds.X + x, trackAreaBounds.Y, trackAreaBounds.X + x, trackAreaBounds.Height);
            }
        }

        /// <summary>
        ///   Draw a list of frames onto the timeline.
        /// </summary>
        /// <param name="frames">The frames to draw.</param>
        private void DrawFrames(IEnumerable<EventFrame> frames, Graphics graphics)
        {
            Rectangle tracksAreaBounds = GetTracksAreaBounds();

            foreach (EventFrame frame in frames) {
                // The extent of the track, including the border
                Rectangle frameExtent = GetFrameExtents(frame);

                // Don't draw track elements that aren't within the target area.
                if (!tracksAreaBounds.IntersectsWith(frameExtent)) {
                    continue;
                }

                // The index of this track (or the one it's a substitute for).
                int trackIndex = GetTrackIndexForFrame(frame);

                // Determine colors for this track
                Color trackBaseColor = fTrackColors[trackIndex];
                Color trackColor = AdjustColor(trackBaseColor, 0, -0.1, -0.2);
                Color borderColor = Color.FromArgb(128, Color.Black);

                if (fSelectedFrames.Contains(frame)) {
                    borderColor = Color.WhiteSmoke;
                }

                bool isHighlighted = (frame == fHighlightedFrame);
                if (isHighlighted) {
                    trackColor = AdjustColor(trackColor, 0, +0.3, +0.6);
                }

                // Draw the main track area.
                using (var trackBrush = new LinearGradientBrush(frameExtent, trackColor, trackBaseColor, LinearGradientMode.Vertical)) {
                    graphics.FillRectangle(trackBrush, frameExtent);
                }

                // Compensate for border size
                frameExtent.X += (int)(TrackBorderSize / 2f);
                frameExtent.Y += (int)(TrackBorderSize / 2f);
                frameExtent.Height -= TrackBorderSize;
                frameExtent.Width -= TrackBorderSize;

                graphics.DrawRectangle(new Pen(borderColor, TrackBorderSize), frameExtent.X, frameExtent.Y, frameExtent.Width, frameExtent.Height);
            }
        }

        /// <summary>
        ///   Draw the labels next to each track.
        /// </summary>
        private void DrawTrackLabels(Graphics graphics)
        {
            foreach (Track track in fTracks) {
                if (!track.Frames.Any())
                    continue;

                // We just need the height and Y-offset, so we get the extents of the first track
                RectangleF frameExtents = GetFrameExtents(track.Frames.First());
                RectangleF labelRect = new RectangleF(0, frameExtents.Y, TrackLabelWidth, frameExtents.Height);

                graphics.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 30)), labelRect);
                graphics.DrawString(track.Name, fLabelFont, Brushes.LightGray, labelRect, fLabelsFormat);
            }
        }

        /// <summary>
        ///   Draw a playhead on the timeline.
        ///   The playhead indicates a time value.
        /// </summary>
        private void DrawPlayhead(Graphics graphics)
        {
            // Calculate the position of the playhead.
            Rectangle trackAreaBounds = GetTracksAreaBounds();

            // Draw a background for the playhead. This also overdraws elements that drew into the playhead area.
            graphics.FillRectangle(Brushes.Black, 0, 0, Width, fPlayheadExtents.Height);

            float playheadOffset = (float)(trackAreaBounds.X + fClockValue * fRenderingScale) + fRenderingOffset.X;
            // Don't draw when not in view.
            if (playheadOffset < trackAreaBounds.X || playheadOffset > trackAreaBounds.X + trackAreaBounds.Width) {
                return;
            }

            // Draw the playhead as a single line.
            graphics.DrawLine(Pens.SpringGreen, playheadOffset, trackAreaBounds.Y, playheadOffset, trackAreaBounds.Height);

            graphics.FillRectangle(Brushes.SpringGreen, playheadOffset - fPlayheadExtents.Width / 2, 0, fPlayheadExtents.Width, fPlayheadExtents.Height);
        }

        /// <summary>
        ///   Draws the selection rectangle the user is drawing.
        /// </summary>
        private void DrawSelectionRectangle(Graphics graphics)
        {
            graphics.DrawRectangle(fSelectionPen, fSelectionRectangle);
        }

        /// <summary>
        ///   Retrieve the index of the track of a given frame.
        /// </summary>
        /// <param name="frame">The frame for which to retrieve the index.</param>
        /// <returns>The index of the track or the index the track is a substitute for.</returns>
        private int GetTrackIndexForFrame(EventFrame frame)
        {
            return fTracks.FindIndex(t => t.Frames.Contains(frame));
        }

        private void SetHighlightedTrack(EventFrame frame)
        {
            if (fHighlightedFrame != frame) {
                fHighlightedFrame = frame;

                fToolTipText = new List<string>();
                if (fHighlightedFrame != null) {
                    fToolTipText.Add(fHighlightedFrame.Name);
                    fToolTipText.Add(fHighlightedFrame.Start.ToString());
                    fToolTipText.Add(fHighlightedFrame.End.ToString());
                    fToolTip.Show(".", this);
                } else {
                    fToolTip.Hide(this);
                }

                Invalidate();
            }
        }

        #region Event handlers

        /// <summary>
        ///   Invoked when the control is repainted
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var gfx = e.Graphics;

            // Clear the buffer
            gfx.Clear(fBackgroundColor);

            DrawBackground(gfx);
            DrawFrames(fTracks.SelectMany(t => t.Frames), gfx);
            DrawSelectionRectangle(gfx);
            DrawTrackLabels(gfx);
            DrawPlayhead(gfx);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            Recalculate();

            base.OnResize(eventargs);
        }

        /// <summary>
        ///   Invoked when the cursor is moved over the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Store the current mouse position.
            Point location = new Point(e.X, e.Y);
            // Check if there is a track at the current mouse position.
            EventFrame focusedFrame = HitTest(location);

            // Is the left mouse button pressed?
            if ((e.Button & MouseButtons.Left) != 0) {
                if (fCurrentMode == BehaviorMode.Selecting) {
                    // Set the appropriate cursor for a selection action.
                    Cursor = Cursors.Cross;

                    // Construct the correct rectangle spanning from the selection origin to the current cursor position.
                    fSelectionRectangle = Normalize(fSelectionOrigin, location);
                    Invalidate();
                } else if (fCurrentMode == BehaviorMode.TimeScrub) {
                    SetClockFromMousePosition(location);
                    Invalidate();
                }
            } else if ((e.Button & MouseButtons.Middle) != 0) {
                // Pan the view
                // Calculate the movement delta.
                PointF delta = PointF.Subtract(location, new SizeF(fPanOrigin));
                // Now apply the delta to the rendering offsets to pan the view.
                fRenderingOffset = PointF.Add(fRenderingOffsetBeforePan, new SizeF(delta));

                // Make sure to stay within bounds.
                fRenderingOffset.X = Math.Max(-fImageSize.Width, Math.Min(0, fRenderingOffset.X));
                fRenderingOffset.Y = Math.Max(-fImageSize.Height, Math.Min(0, fRenderingOffset.Y));

                // Update scrollbar positions. This will invoke a redraw.
                UpdateScrollPosition((int)(-fRenderingOffset.X), (int)(-fRenderingOffset.Y));
            } else {
                // No mouse button is being pressed
                Cursor = Cursors.Arrow;
                SetHighlightedTrack(focusedFrame);
            }
        }

        /// <summary>
        ///   Invoked when the user presses a mouse button over the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!Focused)
                Focus();

            // Store the current mouse position.
            Point location = new Point(e.X, e.Y);

            if ((e.Button & MouseButtons.Left) != 0) {
                // Check if there is a track at the current mouse position.
                EventFrame focusedFrame = HitTest(location);

                if (focusedFrame != null) {
                    // Was this track already selected?
                    if (!fSelectedFrames.Contains(focusedFrame)) {
                        // Tell the track that it was selected.
                        InvokeSelectionChanged(new SelectionChangedEventArgs(focusedFrame.Yield<ITimeObject>(), null));
                        // Clear the selection, unless the user is picking
                        if ((ModifierKeys & Keys.Control) == 0) {
                            InvokeSelectionChanged(new SelectionChangedEventArgs(null, (IEnumerable<ITimeObject>)fSelectedFrames));
                            fSelectedFrames.Clear();
                        }

                        // Add track to selection
                        fSelectedFrames.Add(focusedFrame);

                        // If the track was already selected and Ctrl is down
                        // then the user is picking and we want to remove the track from the selection
                    } else if ((ModifierKeys & Keys.Control) != 0) {
                        fSelectedFrames.Remove(focusedFrame);
                        InvokeSelectionChanged(new SelectionChangedEventArgs(null, focusedFrame.Yield<ITimeObject>()));
                    }
                } else if (location.Y < fPlayheadExtents.Height) {
                    fCurrentMode = BehaviorMode.TimeScrub;
                    SetClockFromMousePosition(location);
                } else {
                    // Clear the selection, unless the user is picking
                    if ((ModifierKeys & Keys.Control) == 0) {
                        InvokeSelectionChanged(new SelectionChangedEventArgs(null, (IEnumerable<ITimeObject>)fSelectedFrames));
                        fSelectedFrames.Clear();
                    }

                    // Remember this location as the origin for the selection.
                    fSelectionOrigin = location;
                    fCurrentMode = BehaviorMode.Selecting;
                }
            } else if ((e.Button & MouseButtons.Middle) != 0) {
                fPanOrigin = location;
                fRenderingOffsetBeforePan = fRenderingOffset;
            }

            Invalidate();
        }

        /// <summary>
        ///   Invoked when the user releases the mouse cursor over the control.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // Store the current mouse position.
            Point location = new Point(e.X, e.Y);

            if ((e.Button & MouseButtons.Left) != 0) {
                if (fCurrentMode == BehaviorMode.Selecting) {
                    // Are we on the track label column?
                    int trackIndex = TrackLabelHitTest(location);
                    if (trackIndex >= 0) {
                        Track track = fTracks[trackIndex];

                        foreach (EventFrame frame in track.Frames) {
                            // Toggle track in and out of selection.
                            if (fSelectedFrames.Contains(frame)) {
                                fSelectedFrames.Remove(frame);
                                InvokeSelectionChanged(new SelectionChangedEventArgs(null, frame.Yield<ITimeObject>()));
                            } else {
                                fSelectedFrames.Add(frame);
                                InvokeSelectionChanged(new SelectionChangedEventArgs(frame.Yield<ITimeObject>(), null));
                            }
                        }
                    } else {
                        // If we were selecting, it's now time to finalize the selection
                        // Construct the correct rectangle spanning from the selection origin to the current cursor position.
                        Rectangle selectionRectangle = Normalize(fSelectionOrigin, location);

                        foreach (EventFrame frame in fTracks.SelectMany( t => t.Frames )) {
                            Rectangle boundingRectangle = GetFrameExtents(frame);

                            bool isSelected;
                            if ((ModifierKeys & Keys.Alt) != 0) {
                                isSelected = selectionRectangle.Contains(boundingRectangle);
                            } else {
                                isSelected = selectionRectangle.IntersectsWith(boundingRectangle);
                            }

                            // Check if the track item is selected by the selection rectangle.
                            if (isSelected) {
                                // Toggle track in and out of selection.
                                if (fSelectedFrames.Contains(frame)) {
                                    fSelectedFrames.Remove(frame);
                                    InvokeSelectionChanged(new SelectionChangedEventArgs(null, frame.Yield<ITimeObject>()));
                                } else {
                                    fSelectedFrames.Add(frame);
                                    InvokeSelectionChanged(new SelectionChangedEventArgs(frame.Yield<ITimeObject>(), null));
                                }
                            }
                        }
                    }
                }

                // Reset cursor
                Cursor = Cursors.Arrow;
                // Reset selection origin.
                fSelectionOrigin = Point.Empty;
                // And the selection rectangle itself.
                fSelectionRectangle = Rectangle.Empty;
                // Reset mode.
                fCurrentMode = BehaviorMode.Idle;
            } else if ((e.Button & MouseButtons.Middle) != 0) {
                fPanOrigin = PointF.Empty;
                fRenderingOffsetBeforePan = PointF.Empty;
            }

            Invalidate();
        }

        /// <summary>
        ///   Invoked when a key is released.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.A && e.Control) {
                // Ctrl+A - Select all
                InvokeSelectionChanged(new SelectionChangedEventArgs(null, (IEnumerable<ITimeObject>)fSelectedFrames));
                fSelectedFrames.Clear();
                foreach (EventFrame frame in fTracks.SelectMany( t => t.Frames )) {
                    fSelectedFrames.Add(frame);
                }
                InvokeSelectionChanged(new SelectionChangedEventArgs((IEnumerable<ITimeObject>)fSelectedFrames, null));

                Invalidate();
            } else if (e.KeyCode == Keys.D && e.Control) {
                // Ctrl+D - Deselect all
                InvokeSelectionChanged(new SelectionChangedEventArgs(null, (IEnumerable<ITimeObject>)fSelectedFrames));
                fSelectedFrames.Clear();

                Invalidate();
            }
        }

        /// <summary>
        ///   Invoked when the user scrolls the mouse wheel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            var scrollPos = this.AutoScrollPosition;

            if ((ModifierKeys & Keys.Alt) != 0) {
                // If Alt is down, we're zooming.
                float amount = e.Delta / 1200f;
                Rectangle trackAreaBounds = GetTracksAreaBounds();

                fRenderingScale += amount;
                // Don't zoom below 1%
                fRenderingScale = Math.Max(0.01f, fRenderingScale);

                // We now also need to move the rendering offset so that the center of focus stays at the mouse cursor.
                fRenderingOffset.X -= trackAreaBounds.Width * ((e.Location.X - trackAreaBounds.X) / (float)trackAreaBounds.Width) * amount;
                fRenderingOffset.X = Math.Min(0, fRenderingOffset.X);

                fRenderingOffset.Y -= trackAreaBounds.Height * ((e.Location.Y - trackAreaBounds.Y) / (float)trackAreaBounds.Height) * amount;
                fRenderingOffset.Y = Math.Min(0, fRenderingOffset.Y);

                // Update scrollbar position.
                scrollPos.X = (int)(-fRenderingOffset.X);
                scrollPos.Y = (int)(-fRenderingOffset.Y);

                UpdateScrollPosition(scrollPos.X, scrollPos.Y);
                Recalculate();
            } else {
                // If Alt isn't down, we're scrolling/panning.
                if ((ModifierKeys & Keys.Shift) != 0) {
                    // If Shift is down, we're scrolling horizontally.
                    AdjustScroll(-e.Delta / 10, 0);
                } else {
                    // If no modifier keys are down, we're scrolling vertically.
                    AdjustScroll(0, -e.Delta / 10);
                }
            }

            Invalidate();
        }

        #endregion

        /// <summary>
        ///   Invoke the <see cref="SelectionChanged" /> event.
        /// </summary>
        /// <param name="eventArgs">The arguments to pass with the event.</param>
        private void InvokeSelectionChanged(SelectionChangedEventArgs eventArgs = null)
        {
            if (SelectionChanged != null) {
                SelectionChanged.Invoke(this, eventArgs ?? SelectionChangedEventArgs.Empty);
            }
        }

        /// <summary>
        ///   Calculate the bounding rectangle for a frame in screen-space.
        /// </summary>
        /// <param name="frame">The frame for which to calculate the bounding rectangle.</param>
        /// <returns>The bounding rectangle for the given frame.</returns>
        private Rectangle GetFrameExtents(EventFrame frame)
        {
            int trackIndex = GetTrackIndexForFrame(frame);

            float frameStart = (float)frame.Start.ToOADate();
            float frameEnd = (float)frame.End.ToOADate();

            float xStart = frameStart - fDataRange.Start;
            float xEnd = frameEnd - fDataRange.Start;
            float xWidth = Math.Max(1, xEnd - xStart);
            RectangleF frameRect = new RectangleF(xStart, 0, xWidth, 0);

            Rectangle trackAreaBounds = GetTracksAreaBounds();

            int actualRowHeight = (int)((TrackHeight) * fRenderingScale + TrackSpacing);

            // Calculate the Y offset for the frame
            int frameOffsetY = (int)(trackAreaBounds.Y + (actualRowHeight * trackIndex) + fRenderingOffset.Y);
            // Calculate the X offset for frame
            int frameOffsetX = (int)(trackAreaBounds.X + (frameRect.X * fRenderingScale) + fRenderingOffset.X);

            // The extent of the frame, including the border
            return new Rectangle(frameOffsetX, frameOffsetY,
                                 (int)(frameRect.Width * fRenderingScale), (int)(TrackHeight * fRenderingScale));
        }

        /// <summary>
        ///   Create a rectangle from two points. The smaller coordinates will define the upper left corner,
        ///   the larger coordinates will define the lower right corner.
        /// </summary>
        /// <param name="start">The first point to use.</param>
        /// <param name="end">The second point to use.</param>
        /// <returns>A valid rectangle.</returns>
        private static Rectangle Normalize(Point start, Point end)
        {
            Rectangle result = new Rectangle();
            if (end.X < start.X) {
                result.X = end.X;
                result.Width = start.X - result.X;
            } else {
                result.X = start.X;
                result.Width = end.X - result.X;
            }
            if (end.Y < start.Y) {
                result.Y = end.Y;
                result.Height = start.Y - result.Y;
            } else {
                result.Y = start.Y;
                result.Height = end.Y - result.Y;
            }
            return result;
        }

        #region Color functions

        /// <summary>
        /// Get a list of random colors.
        /// </summary>
        /// <param name="count">How many colors to generate.</param>
        /// <returns>A list of random colors.</returns>
        private static List<Color> GetRandomColors(int count)
        {
            double step = 360.0 / count;
            List<Color> colors = new List<Color>();
            for (uint i = 0; i < count; ++i) {
                double value = i * step;
                colors.Add(ColorFromHSV(value, 0.6, 0.8));
            }
            return colors;
        }

        /// <summary>
        /// Convert a color to HSV values.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <param name="hue">The hue of the color.</param>
        /// <param name="saturation">The saturation of the color.</param>
        /// <param name="value">The value of the color.</param>
        private static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        /// <summary>
        /// Convert HSV values to a color.
        /// </summary>
        /// <param name="hue">The hue of the color.</param>
        /// <param name="saturation">The saturation of the color.</param>
        /// <param name="value">The value of the color.</param>
        /// <returns>The color appropriate for the given values.</returns>
        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value).Clamp(0, 255);
            int p = Convert.ToInt32(value * (1 - saturation)).Clamp(0, 255);
            int q = Convert.ToInt32(value * (1 - f * saturation)).Clamp(0, 255);
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation)).Clamp(0, 255);

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            return Color.FromArgb(255, v, p, q);
        }

        /// <summary>
        /// Adjust the hue, saturation and/or value of a given color.
        /// </summary>
        /// <param name="color">The color to adjust.</param>
        /// <param name="hue">The hue of the color.</param>
        /// <param name="saturation">The saturation of the color.</param>
        /// <param name="value">The value of the color.</param>
        /// <returns>The adjusted color value.</returns>
        private static Color AdjustColor(Color color, double hue, double saturation, double value)
        {
            double oldHue;
            double oldSaturation;
            double oldValue;
            ColorToHSV(color, out oldHue, out oldSaturation, out oldValue);

            double newHue = oldHue + hue;
            double newSaturation = oldSaturation + saturation;
            double newValue = oldValue + value;

            return ColorFromHSV(newHue, newSaturation, newValue);
        }

        #endregion

        #region Scrolling support

        protected Rectangle GetClientRect(bool includePadding)
        {
            int left = 0;
            int top = 0;
            int width = ClientSize.Width;
            int height = ClientSize.Height;

            if (includePadding) {
                left += Padding.Left;
                top += Padding.Top;
                width -= Padding.Horizontal;
                height -= Padding.Vertical;
            }

            return new Rectangle(left, top, width, height);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.ScrollableControl.Scroll" /> event.
        /// </summary>
        /// <param name="se">
        /// A <see cref="T:System.Windows.Forms.ScrollEventArgs" /> that contains the event data.
        /// </param>
        protected override void OnScroll(ScrollEventArgs se)
        {
            if (se.Type == ScrollEventType.ThumbTrack) {
                switch (se.ScrollOrientation) {
                    case ScrollOrientation.HorizontalScroll:
                        AutoScrollPosition = new Point(se.NewValue, -AutoScrollPosition.Y);
                        Invalidate();
                        break;

                    case ScrollOrientation.VerticalScroll:
                        AutoScrollPosition = new Point(-AutoScrollPosition.X, se.NewValue);
                        Invalidate();
                        break;
                }
            }
            base.OnScroll(se);
        }

        /// <summary>
        /// Updates the scroll position.
        /// </summary>
        /// <param name="posX">The X position.</param>
        /// <param name="posY">The Y position.</param>
        protected void UpdateScrollPosition(int posX, int posY)
        {
            AutoScrollPosition = new Point(posX, posY);
        }

        /// <summary>
        /// Adjusts the scroll.
        /// </summary>
        /// <param name="dx">The x.</param>
        /// <param name="dy">The y.</param>
        protected void AdjustScroll(int dx, int dy)
        {
            UpdateScrollPosition(HorizontalScroll.Value + dx, VerticalScroll.Value + dy);
        }

        protected void AdjustViewport(ExtSize imageSize, bool noRedraw = false)
        {
            if (AutoScroll && !imageSize.IsEmpty) {
                AutoScrollMinSize = new Size(imageSize.Width + Padding.Horizontal, imageSize.Height + Padding.Vertical);
            }

            if (!noRedraw)
                Invalidate();
        }

        protected Point GetImageRelativeLocation(PointF mpt)
        {
            return new Point((int)mpt.X + Math.Abs(AutoScrollPosition.X), (int)mpt.Y + Math.Abs(AutoScrollPosition.Y));
        }

        #endregion

        #region ToolTip

        private void ToolTip_Draw(Object sender, DrawToolTipEventArgs e)
        {
            if (fToolTipText == null || fToolTipText.Count == 0) {
                return;
            }

            e.Graphics.FillRectangle(Brushes.AntiqueWhite, e.Bounds);
            e.DrawBorder();

            int titleHeight = 14;
            int fontHeight = 12;

            // Draws the line just below the title
            e.Graphics.DrawLine(Pens.Black, 0, titleHeight, e.Bounds.Width, titleHeight);

            // Draws the title
            string title = fToolTipText[0];
            using (Font font = new Font(e.Font, FontStyle.Bold)) {
                int x = (int)(e.Bounds.Width - e.Graphics.MeasureString(title, font).Width) / 2;
                int y = (int)(titleHeight - e.Graphics.MeasureString(title, font).Height) / 2;
                e.Graphics.DrawString(title, font, Brushes.Black, x, y);
            }

            // Draws the lines
            for (int line = 1; line < fToolTipText.Count; line++) {
                string str = fToolTipText[line];

                int x = 5;
                int y = (int)(titleHeight - fontHeight - e.Graphics.MeasureString(str, e.Font).Height) / 2 + 10 + (line * 14);
                e.Graphics.DrawString(str, e.Font, Brushes.Black, x, y);
            }
        }

        private void ToolTip_Popup(Object sender, PopupEventArgs e)
        {
            if (fToolTipText == null || fToolTipText.Count == 0 || fToolTipText[0].Length == 0) {
                e.ToolTipSize = new Size(0, 0);
                return;
            }

            // resizes the ToolTip window
            int height = 18 + (fToolTipText.Count * 15);
            e.ToolTipSize = new Size(200, height);
        }

        #endregion
    }
}
