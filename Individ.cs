using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace somethink
{
    class Individ : IComparable<Individ>
    {
        // :alien:
        public List<Point> Points;
        public double Cost;

        public Individ(List<Point> points)
        {
            Points = new List<Point>();
            Points.AddRange(points);
            DistanceOfWay(points);
        }
        public Individ(Individ individ)
        {
            Points = new List<Point>(individ.Points);
            DistanceOfWay(Points);
        }
        public Individ(List<Point> man, List<Point> woman)
        {
            Points = new List<Point>();
            var parentFirst = new List<Point>();
            var parentSecond = new List<Point>();
            parentFirst.AddRange(man);
            parentSecond.AddRange(woman);

            parentFirst.RemoveRange(parentFirst.Count / 2, parentFirst.Count / 2);
            parentSecond.RemoveRange(0, parentSecond.Count / 2);

            Points.AddRange(parentFirst);
            Points.AddRange(parentSecond);

            DistanceOfWay(Points);
        }

        public void DistanceOfWay(List<Point> points)
        {
            var sum = 0.0;
            for (var i = 0; i < points.Count - 1; i++)
            {
                sum += Distance(points[i], points[i + 1]);
            }
            sum += Distance(points[points.Count - 1], points[0]);
            Cost = sum;
        }
        private double Distance(Point firstPoint, Point secondPoint)
        {
            return Math.Sqrt((firstPoint.X - secondPoint.X) * (firstPoint.X - secondPoint.X) + (firstPoint.Y - secondPoint.Y) * (firstPoint.Y - secondPoint.Y));
        }

        public int CompareTo(Individ other)
        {
            return Cost.CompareTo(other.Cost);
        }
        public override string ToString()
        {
            return Points.Aggregate("", (current, point) => current + ("|" + point.X + "," + point.Y + "|  "));
        }
        public static bool operator >(Individ a, Individ b)
        {
            return (a.Cost > b.Cost);
        }
        public static bool operator <(Individ a, Individ b)
        {
            return (a.Cost < b.Cost);
        }         
    }

}
