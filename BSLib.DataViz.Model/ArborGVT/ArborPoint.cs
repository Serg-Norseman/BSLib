/*
 *  ArborGVT - a graph vizualization toolkit
 *
 *  Physics code derived from springy.js, copyright (c) 2010 Dennis Hotson
 *  JavaScript library, copyright (c) 2011 Samizdat Drafting Co.
 *
 *  Fork and C# implementation, copyright (c) 2012,2016 by Serg V. Zhdanovskih.
 */

using System;

namespace BSLib.DataViz.ArborGVT
{
    public struct ArborPoint : IEquatable<ArborPoint>
    {
        public static readonly ArborPoint Null = new ArborPoint(double.NaN, double.NaN);
        public static readonly ArborPoint Zero = new ArborPoint(0.0f, 0.0f);

        public double X;
        public double Y;

        public ArborPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(ArborPoint other)
        {
            return other.X == X && other.Y == Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is ArborPoint) {
                ArborPoint other = (ArborPoint)obj;
                return other.X == X && other.Y == Y;
            }
            return false;
        }

        public bool IsNull()
        {
            return (double.IsNaN(X) && double.IsNaN(Y));
        }

        public bool IsExploded()
        {
            return (double.IsNaN(X) || double.IsNaN(Y));
        }

        public ArborPoint Add(ArborPoint a)
        {
            return new ArborPoint(X + a.X, Y + a.Y);
        }

        public ArborPoint Sub(ArborPoint a)
        {
            return new ArborPoint(X - a.X, Y - a.Y);
        }

        public ArborPoint Mul(double a)
        {
            return new ArborPoint(X * a, Y * a);
        }

        public ArborPoint Div(double a)
        {
            return new ArborPoint(X / a, Y / a);
        }

        public double Magnitude()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double MagnitudeSquare()
        {
            return X * X + Y * Y;
        }

        public ArborPoint Normalize()
        {
            return Div(Magnitude());
        }
    }
}
