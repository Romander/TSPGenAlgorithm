using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace somethink
{
    class Individ : IComparable<Individ>
    {
        public List<Point> Points;
        public double Cost;

        public Individ(List<Point> points)
        {
            Points = new List<Point>();
            Points.AddRange(points);
            DistanceOfWay(points);
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

        private void DistanceOfWay(List<Point> points)
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

        public void Mutation(int percentOfMutation)
        {
            int numbersOfMuteted = Points.Count / 2 * percentOfMutation / 100;
            Random rnd = new Random();
            for (var i = 0; i < numbersOfMuteted; i++)
            {
                var firstRand = rnd.Next(0, Points.Count);
                var secondRand = rnd.Next(0, Points.Count);

                Point tmp = Points[firstRand];
                Points[firstRand] = Points[secondRand];
                Points[secondRand] = tmp;
            }
            Points = new List<Point>();
            Points.AddRange(Points);
            DistanceOfWay(Points);
        }

        public override string ToString()
        {
            return Points.Aggregate("", (current, point) => current + ("|" + point.X + "," + point.Y + "|  "));
        }

        public int CompareTo(Individ other)
        {
            return Cost.CompareTo(other.Cost);
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
