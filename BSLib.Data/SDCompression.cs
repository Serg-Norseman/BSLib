/*
 *  "BSLib.Data".
 *  Copyright (C) 2019-2023 by Sergey V. Zhdanovskih.
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

namespace BSLib.Data
{
    public struct DataPoint
    {
        public readonly long Timestamp;
        public readonly double Value;

        public DataPoint(long timestamp, double value)
        {
            Timestamp = timestamp;
            Value = value;
        }
    }

    /// <summary>
    /// Possible ratios of the slope of the corridor doors.
    /// </summary>
    public enum SlopesRelation
    {
        srNone,
        srNewSUIsIsGreater, // the slope of the upper door increased
        srNewSLIsLess, // the slope of the lower door decreased
        srSUIsGreaterSL // the doors opened
    }

    /// <summary>
    /// The class implements the logic of selecting points that must be stored in the database (alg. of "Swinging Door").
    /// </summary>
    public class SDCompression
    {
        private bool fIsNeedInit;
        private readonly double fDeviation; // inaccuracy of measurements
        private readonly long fCorridorTimeSec; // maximum time in seconds during which at least one point must be stored
        private DateTime fLastRetainDate; // timestamp of the last data record
        private DataPoint fCurrPoint; // current point from the device
        private DataPoint fPrevPoint; // previous point from the device
        private DataPoint fCorridorStartPoint; // current starting point of the corridor
        private DataPoint fU, fL; // anchor points
        private double fSU, fSL; // angular coefficients of slope of the doors of the corridor


        public DataPoint CorridorStartPoint { get { return fCorridorStartPoint; } }
        public DataPoint CurrentPoint { get { return fCurrPoint; } }
        public DataPoint PreviousPoint { get { return fPrevPoint; } }
        public double SU { get { return fSU; } }
        public double SL { get { return fSL; } }
        public DataPoint PivotU { get { return fU; } }
        public DataPoint PivotL { get { return fL; } }


        public SDCompression(double deviation, long corridorTimeSec)
        {
            fIsNeedInit = true;
            fDeviation = deviation;
            fCorridorTimeSec = corridorTimeSec;
            fLastRetainDate = DateTime.FromBinary(0);
            fSU = 0;
            fSL = 0;
        }

        /// <summary>
        /// Register a new point.
        /// </summary>
        public bool ReceivePoint(ref DateTime timestamp, ref double value)
        {
            bool result = false;
            fPrevPoint = fCurrPoint;
            fCurrPoint = new DataPoint(timestamp.ToBinary(), value);

            // set the first point as a corridor point
            // calculate anchor points
            if (fIsNeedInit) {
                fCorridorStartPoint = fCurrPoint;
                EstablishPivotPoints();
                fIsNeedInit = false;

                // current point must be stored in the database
                fLastRetainDate = timestamp;
                result = true;
            } else {
                if ((fPrevPoint.Timestamp != fCorridorStartPoint.Timestamp) && (fPrevPoint.Value != fCorridorStartPoint.Value)) {
                    // calculate the slope of the corridor doors
                    switch (CalculateCurrentSlopes()) {
                        case SlopesRelation.srNone: // the received point enters the corridor
                        case SlopesRelation.srNewSUIsIsGreater: // the upper boundary of the corridor has changed
                        case SlopesRelation.srNewSLIsLess: // the lower boundary of the corridor has changed
                                // save point only if time is exceeded
                                result = IsCorridorTimeExpired(timestamp);
                                break;

                        case SlopesRelation.srSUIsGreaterSL: // the doors opened
                            {
                                // current point does not enter the corridor
                                // the previous point opens the corridor, the coefficients are recalculated
                                fCorridorStartPoint = fPrevPoint;
                                EstablishPivotPoints();
                                InitSlopes();
                                fLastRetainDate = timestamp;

                                // return the data of the previous point to save
                                timestamp = DateTime.FromBinary(fCorridorStartPoint.Timestamp);
                                value = fCorridorStartPoint.Value;

                                result = true;
                                break;
                            }
                    }
                } else {
                    InitSlopes();
                }
            }
            return result;
        }

        /// <summary>
        /// Calculation of the angular coefficients of the corridor.
        /// </summary>
        private SlopesRelation CalculateCurrentSlopes()
        {
            SlopesRelation result = SlopesRelation.srNone;

            double su, sl;
            su = (fCurrPoint.Value - fCorridorStartPoint.Value - fDeviation) / (fCurrPoint.Timestamp - fCorridorStartPoint.Timestamp);
            sl = (fCurrPoint.Value - fCorridorStartPoint.Value + fDeviation) / (fCurrPoint.Timestamp - fCorridorStartPoint.Timestamp);

            if (su > fSU) {
                fSU = su;
                result = SlopesRelation.srNewSUIsIsGreater;
            }

            if (sl < fSL) {
                fSL = sl;
                result = SlopesRelation.srNewSLIsLess;
            }

            if (fSU > fSL) {
                result = SlopesRelation.srSUIsGreaterSL;
            }

            return result;
        }

        /// <summary>
        /// Calculation of anchor points.
        /// </summary>
        private void EstablishPivotPoints()
        {
            fU = new DataPoint(fCorridorStartPoint.Timestamp, fCorridorStartPoint.Value + fDeviation);
            fL = new DataPoint(fCorridorStartPoint.Timestamp, fCorridorStartPoint.Value - fDeviation);
        }

        /// <summary>
        /// Initializing the angular coefficients of the corridor doors.
        /// </summary>
        private void InitSlopes()
        {
            fSU = (fCurrPoint.Value - fCorridorStartPoint.Value - fDeviation) / (fCurrPoint.Timestamp - fCorridorStartPoint.Timestamp);
            fSL = (fCurrPoint.Value - fCorridorStartPoint.Value + fDeviation) / (fCurrPoint.Timestamp - fCorridorStartPoint.Timestamp);
        }

        /// <summary>
        /// Returns True if the time of the last record exceeds CorridorTimeSec.
        /// </summary>
        private bool IsCorridorTimeExpired(DateTime timestamp)
        {
            bool result = ((timestamp - fLastRetainDate).TotalSeconds > fCorridorTimeSec);
            // if the time is up, then update the last save timestamp
            if (result) {
                fLastRetainDate = timestamp;
            }
            return result;
        }
    }
}
