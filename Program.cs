using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaSlicer {
    class Program {
        static void Main(string[] args) {

            int rows;
            int columns;
            int minIngredients;
            int maxCells;
            char[,] pizza;
            List<Shape> shapes = new List<Shape>();
            List<Slice> slices = new List<Slice>();

            using (TextReader r = new StreamReader("input.in")) {

                var line = r.ReadLine();
                var values = line.Split(' ');
                rows = int.Parse(values[0] + " ");
                columns = int.Parse(values[1] + " ");
                minIngredients = int.Parse(values[2] + " ");
                maxCells = int.Parse(values[3] + " ");
                pizza = new char[rows, columns];
                for (int i = 0; i < rows; i++) {
                    line = r.ReadLine();
                    for (int x = 0; x < line.Length; x++) {
                        pizza[i, x] = line[x];
                    }
                }

                int blocks = maxCells;
                while (blocks > minIngredients*2) {
                    if (blocks % 2 == 0) {
                        var width = blocks;
                        var currentWidth = blocks;
                        var height = 1;
                        shapes.Add(new Shape() { Width = width, Height = height });
                        while (height <= currentWidth) {
                            height++;
                            if (width/height % 2 == 0) {
                                currentWidth = width / height;
                                shapes.Add(new Shape() { Width = currentWidth, Height = height });
                                shapes.Add(new Shape() { Width = height, Height = currentWidth });
                            }
                        }
                    }
                    else {
                        shapes.Add(new Shape() { Width = blocks, Height = 1 });
                        shapes.Add(new Shape() { Width = 1, Height = blocks });
                    }
                    blocks--;
                }
                shapes.OrderByDescending(x => x.Area);

                for (int i = 0; i < pizza.GetLength(0); i++) {
                    for (int x = 0; x < pizza.GetLength(1); x++) {
                        foreach (var item in shapes) {
                            if (IsValid(pizza, i, x, item, slices, minIngredients,rows,columns )) {
                                var slice = new Slice() { StartX = i, StartY = x, Cut = item };
                                slice.Coords = slice.getCoords();
                                slices.Add(slice);
                                Console.WriteLine(slice.ToString());
                                break;
                            }
                        }
                    }
                }

                using (TextWriter w = new StreamWriter("out.txt")) {
                    w.WriteLine(slices.Count);
                    foreach (var item in slices) {
                        w.WriteLine(item.ToString());
                    }
                }
                

            }

        }

        public class Shape {

            public int Width { get; set; }
            public int Height { get; set; }
            public int Area {
                get {
                    return Width * Height;
                }
            }
        }

        public class Slice {

            public int StartX { get; set; }
            public int StartY { get; set; }
            public Shape Cut { get; set; }

            public List<Coordinate> Coords {

                get; set;

            }

            public List<Coordinate> getCoords() {

                List<Coordinate> coords = new List<Coordinate>();
                for (int x = StartX; x<StartX+Cut.Width; x++) {
                    for (int y = StartY; y<StartY+Cut.Height; y++) {
                        coords.Add(new Coordinate() { X = x, Y = y });
                    }
                }
                return coords;
            }

            public override string ToString() {
                return StartX + " " + StartY + " " + (StartX + Cut.Width-1) + " " + (StartY + Cut.Height-1);
            }
        }

        public class Coordinate {
            public int X { get; set; }
            public int Y { get; set; }

            public override bool Equals(object obj) {
                var item = (Coordinate)obj;
                return item.X == X && item.Y == Y;
            }

            public override int GetHashCode() {
                var hashCode = 1861411795;
                hashCode = hashCode * -1521134295 + X.GetHashCode();
                hashCode = hashCode * -1521134295 + Y.GetHashCode();
                return hashCode;
            }
        }

        public static bool IsValid(char[,] pizza, int X, int Y, Shape cut, List<Slice> slices, int minIngredients, int height, int width) {
            
            List<char> ingredients = new List<char>();
            List<Coordinate> coords = new List<Coordinate>();

            if (X + cut.Width+1 > height) {
                return false;
            }

            if (Y + cut.Height+1 > width) {
                return false;
            }

            for (int i = Y; i < Y+cut.Height; i++) {
                for (int x = X; x < X + cut.Width; x++) {
                    ingredients.Add(pizza[x, i]);
                    coords.Add(new Coordinate() { X = x, Y = i });
                }
            }

            if (!(ingredients.Count(x => x == 'T') >= minIngredients) || !(ingredients.Count(x => x == 'M') >= minIngredients)) {
                return false;
            }

            foreach (var item in slices.Where(x => (x.StartX >= X - cut.Area && x.StartX <= X+cut.Area)|| (x.StartY >= Y - cut.Area && x.StartY <= Y + cut.Area))) {                
                
                if (item.Coords.Intersect(coords).Count() > 0) {
                    return false;
                }
            }

            return true;

        }
    }
}
