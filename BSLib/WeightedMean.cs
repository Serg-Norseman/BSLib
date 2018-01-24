/*
 *  "BSLib".
 *  Copyright (C) 2015-2017 by Sergey V. Zhdanovskih.
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

namespace BSLib
{
    public sealed class WeightedMean
    {
        private double fResult;
        private double fSum;

        public WeightedMean()
        {
            fResult = 0.0d;
            fSum = 0.0d;
        }

        public void AddValue(double value, double weight)
        {
            fResult += (value * weight);
            fSum += weight;
        }

        public double GetResult()
        {
            return (fSum != 0.0d) ? fResult / fSum : double.NaN;
        }
    }
}
