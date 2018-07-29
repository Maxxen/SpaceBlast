using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point (int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2((float)X, (float)Y);
        }

        public static Point operator + (Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator - (Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        //public static bool operator == (Point p1, Point p2)
        //{
        //    return (p1.X == p2.X && p1.Y == p2.Y);
        //}

        //public static bool operator != (Point p1, Point p2)
        //{
        //    return (p1.X != p2.X || p1.Y != p2.Y);
        //}
    }
}
