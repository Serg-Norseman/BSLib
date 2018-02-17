/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

using System;

namespace BSLib.ArborGVT
{
    public struct ArborPoint
    {
        public static readonly ArborPoint Null = new ArborPoint(double.NaN, double.NaN);

        public double X;
        public double Y;

        public ArborPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool isNull()
        {
            return (double.IsNaN(X) && double.IsNaN(Y));
        }

        public static ArborPoint newRnd(double a = 5)
        {
            return new ArborPoint(2 * a * (ArborSystem.getRndDouble() - 0.5), 2 * a * (ArborSystem.getRndDouble() - 0.5));
        }

        public bool exploded()
        {
            return (double.IsNaN(X) || double.IsNaN(Y));
        }

        public ArborPoint add(ArborPoint a)
        {
            return new ArborPoint(X + a.X, Y + a.Y);
        }

        public ArborPoint sub(ArborPoint a)
        {
            return new ArborPoint(X - a.X, Y - a.Y);
        }

        public ArborPoint mul(double a)
        {
            return new ArborPoint(X * a, Y * a);
        }

        public ArborPoint div(double a)
        {
            return new ArborPoint(X / a, Y / a);
        }

        public double magnitude()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double magnitudeSquare()
        {
            return X * X + Y * Y;
        }

        public ArborPoint normalize()
        {
            return div(magnitude());
        }
    }
}
