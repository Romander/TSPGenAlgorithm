using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace somethink
{
    public partial class MainWindow 
    {
        private List<Point> _vertexes = new List<Point>();

        public MainWindow()
        {
            InitializeComponent();
            ReadVertexesToFile();
            PaintBasis();
        }

        private void MainCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            WriteVertexToFile(mousePosition);
            MainCanvas.Children.Add(CreateEllipse(10, 10, mousePosition.X, mousePosition.Y));
            ReadVertexesToFile();
            PaintBasis();
        }
        private void MainCanvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(this);
            var radius = 10;

            for (var i = 0; i < _vertexes.Count; i++)
            {
                var circleFormula = Math.Sqrt(Math.Pow(mousePosition.X - _vertexes[i].X, 2) + Math.Pow(mousePosition.Y - _vertexes[i].Y, 2));
                if (circleFormula <= radius) _vertexes.Remove(_vertexes[i]);
            }  
            PaintBasis();
            WriteVertexesToFile();
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {    
            PaintWay();
        }

        private void ReadVertexesToFile()
        {
            try
            {
                _vertexes = new List<Point>();
                var lines = File.ReadAllLines("data.txt");
                foreach (var line in lines)
                {
                    var words = line.Split(' ');
                    var vertex = new Point(double.Parse(words.First()), double.Parse(words.Last()));
                    _vertexes.Add(vertex);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Could not read the file");
            }
        }
        private void WriteVertexToFile(Point point)
        {
            try
            {
                var file = new StreamWriter("data.txt", true);
                file.WriteLine(point.X + " " + point.Y);
                file.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Could not write the file");
            }
        }
        private void WriteVertexesToFile()
        {
            try
            {
                using (var file = new StreamWriter("data.txt", false))
                {
                    foreach (var vertex in _vertexes)
                    {
                        file.WriteLine(vertex.X + " " + vertex.Y);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Could not write the file");
            }
        }

        private void PaintBasis()
        {
            MainCanvas.Children.Clear();
            DrawLines(_vertexes.ToArray(), Brushes.Gray, 0.5);
            DrawVertexes(_vertexes.ToArray());    
        }
        private void PaintWay()
        {
            MainCanvas.Children.Clear();
            DrawLines(_vertexes.ToArray(), Brushes.Gray, 0.5);
            DrawVertexes(_vertexes.ToArray());
            _vertexes = GenAlgorithm().Points;
            DrawWay(_vertexes.ToArray(), Brushes.Red, 2);
        }

        private void DrawLines(Point[] points, SolidColorBrush brushColor, double thickness)
        {
            foreach (var myline in points.SelectMany(pointMain => points.Select(pointOver => new Line
            {
                Stroke = brushColor,
                StrokeThickness = thickness,
                X1 = pointMain.X,
                Y1 = pointMain.Y,
                X2 = pointOver.X,
                Y2 = pointOver.Y
            })))
            {
                MainCanvas.Children.Add(myline);
            }
        }

        private void DrawWay(Point[] points, SolidColorBrush brushColor, double thickness)
        {
            for (var i = 0; i < points.Length - 1; i++)
            {
                DrawLine(points[i], points[i + 1], brushColor, thickness);
            }
            DrawLine(points[points.Length - 1], points[0], brushColor, thickness);
        }
        private void DrawLine(Point pointFirst, Point pointSecond, SolidColorBrush brushColor, double thickness)
        {
            var line = new Line
            {
                Stroke = brushColor,
                StrokeThickness = thickness,
                X1 = pointFirst.X,
                Y1 = pointFirst.Y,
                X2 = pointSecond.X,
                Y2 = pointSecond.Y
            };


            MainCanvas.Children.Add(line);
        }

        private void DrawVertexes(Point[] points)
        {
            foreach (var point in points)
            {
                MainCanvas.Children.Add(CreateEllipse(10, 10, point.X, point.Y));
            }
        }
        private Ellipse CreateEllipse(double width, double height, double desiredCenterX, double desiredCenterY)
        {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            EllipseColored(ellipse, 1, Colors.Yellow, Colors.Black);
            return ellipse;
        }
        private void EllipseColored(Ellipse ellipse, int strokeThickness, Color strokeColor, Color fillColor)
        {
            var blueBrush = new SolidColorBrush {Color = strokeColor};
            var blackBrush = new SolidColorBrush {Color = fillColor};

            ellipse.StrokeThickness = strokeThickness;
            ellipse.Stroke = blackBrush;
            ellipse.Fill = blueBrush;
        }

        private Individ GenAlgorithm()
        {
            List<Individ> populations = new List<Individ>();
            var n = 0;
            while (n != 100)
            {
                Shuffle(_vertexes);
                populations.Add(new Individ(_vertexes));
                n++;
            }
            populations.Sort();
            populations.RemoveRange(populations.Count / 2, populations.Count / 2);   
            
            n = 0;
            while (n != 5)
            {
                var mutPop = new List<Individ>();
                mutPop.AddRange(populations);

                foreach (var e in mutPop)
                {
                    e.Mutation((int)percentMutation.Value);
                }

                populations.AddRange(mutPop);
                populations.Sort();
                populations.RemoveRange(populations.Count / 2, populations.Count / 2);
                Debug.WriteLine(populations.Count);  
                Debug.WriteLine(populations[0].Cost);
                n++;
            }
            Debug.WriteLine(populations.Count);  
            return populations[0];
        }

        private static readonly Random Rng = new Random();
        public void Shuffle(List<Point> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                Point value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
