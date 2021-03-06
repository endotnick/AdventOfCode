using System;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace Advent
{
    public class DayEight2016

    {
        public static void Entry()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //readfile
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://raw.githubusercontent.com/endotnick/AdventOfCode/master/AdventOfCode/Input/2016d8");
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            //convert file to lines
            string[] lines = content.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            //create grid 50x6 (0,0) to (49,5)
            List<KeyValuePair<int, int>> baseSet = rectXY(50, 6, watch);
            Dictionary<KeyValuePair<int, int>, bool> grid = new Dictionary<KeyValuePair<int, int>, bool>();
            foreach (var pair in baseSet)
            {
                grid.Add(pair, false);
            }

            //init grid drawing
            string[] stringRows = drawGrid(grid);

            //parse lines
            parseLines(grid, lines, watch);

            //draw final grid
            stringRows = drawGrid(grid);
            outputGrid(stringRows, watch);
            watch.Stop();
        }

        public static void parseLines(Dictionary<KeyValuePair<int, int>, bool> grid, string[] lines, Stopwatch watch)
        {
            string[] rectSplitters = new string[] { " ", "x" };
            string[] shiftSplitters = new string[] { "=", " by " };
            //search line for rect,row,col
            foreach (string line in lines)
            {
                //if rect
                if (line.Contains("rect"))
                {
                    string[] result = line.Split(rectSplitters, StringSplitOptions.None);
                    int width = int.Parse(result[1]);
                    int height = int.Parse(result[2]);
                    //create rect
                    grid = rectCreation(grid, width, height, watch);
                    continue;
                }

                //if row
                if (line.Contains("row"))
                {
                    //split string on '='
                    string[] result = line.Split(shiftSplitters, StringSplitOptions.None);
                    //row is selected from the top
                    int rowIndex = (5 - int.Parse(result[1]));
                    int rowShift = int.Parse(result[2]);
                    //execute shift
                    grid = rowRotation(grid, rowIndex, rowShift, watch);
                    continue;
                }

                //if col
                if (line.Contains("column"))
                {
                    //split string on '='
                    string[] result = line.Split(shiftSplitters, StringSplitOptions.None);
                    int columnIndex = int.Parse(result[1]);
                    int columnShift = int.Parse(result[2]);
                    //execute shift
                    grid = columnRotation(grid, columnIndex, columnShift, watch);
                    continue;
                }
            }
        }

        //given width and height x,y, created a rectangle
        public static Dictionary<KeyValuePair<int, int>, bool> rectCreation(Dictionary<KeyValuePair<int, int>, bool> grid, int width, int height, Stopwatch watch)
        {
            List<KeyValuePair<int, int>> rectInstruction = rectXY(width, height, watch);
            foreach (var entry in rectInstruction)
            {
                grid[entry] = true;
            }
            return grid;
        }

        //if rotate row y=A by B, shift row A right B, wrapping
        public static Dictionary<KeyValuePair<int, int>, bool> rowRotation(Dictionary<KeyValuePair<int, int>, bool> grid, int rowIndex, int rowShift, Stopwatch watch)
        {
            //holds lit pixels
            List<KeyValuePair<int, int>> set = new List<KeyValuePair<int, int>>();
            //select previously lit pixels in the selected row
            foreach (var pair in grid)
            {
                if (pair.Key.Value == rowIndex && pair.Value == true)
                {
                    set.Add(pair.Key);
                }
            }

            //holds pixels to be lit
            List<KeyValuePair<int, int>> shiftedXPairs = shiftX(set, watch, rowShift);
            //light new set of pixels
            foreach (var entry in shiftedXPairs)
            {
                grid[entry] = true;
            }

            //remove previously lit pixels
            foreach (var entry in set)
            {
                if (!shiftedXPairs.Contains(entry))
                {
                    grid[entry] = false;
                }
            }

            return grid;
        }

        public static Dictionary<KeyValuePair<int, int>, bool> columnRotation(Dictionary<KeyValuePair<int, int>, bool> grid, int columnIndex, int columnShift, Stopwatch watch)
        {
            //holds lit pixels
            List<KeyValuePair<int, int>> set = new List<KeyValuePair<int, int>>();
            //select previously lit pixels in the selected column
            foreach (var pair in grid)
            {
                if (pair.Key.Key == columnIndex && pair.Value == true)
                {
                    set.Add(pair.Key);
                }
            }

            //holds pixels to be lit
            List<KeyValuePair<int, int>> shiftedYPairs = shiftY(set, watch, columnShift);
            //light new set of pixels
            foreach (var entry in shiftedYPairs)
            {
                grid[entry] = true;
            }

            //remove previously lit pixels	 	
            foreach (var entry in set)
            {
                if (!shiftedYPairs.Contains(entry))
                {
                    grid[entry] = false;
                }
            }

            return grid;
        }

        public static void outputGrid(string[] stringRows, Stopwatch watch)
        {
            //row 5 is the top of the display, need to read the array in reverse
            for (int i = 5; i >= 0; i--)
            {
                Console.WriteLine("@ {0} - entry: {1}", watch.Elapsed, stringRows[i]);
            }
        }

        public static string[] drawGrid(Dictionary<KeyValuePair<int, int>, bool> grid)
        {
            string[] stringRows = new string[6];
            int litPixels = 0;
            foreach (var entry in grid)
            {
                int row = entry.Key.Value;
                if (entry.Value)
                {
                    stringRows[row] += "#";
                    litPixels++;
                }
                else
                {
                    stringRows[row] += ".";
                }
            }

            Console.WriteLine("litPixels == {0}", litPixels);
            return stringRows;
        }

        public static List<KeyValuePair<int, int>> rectXY(int X, int Y, Stopwatch watch)
        {
            //holds the generated rectangle
            List<KeyValuePair<int, int>> rect = new List<KeyValuePair<int, int>>();
            for (int x = 0; x < X; x++)
            {
                for (int i = 0; i < Y; i++)
                {
                    //we have to draw the rectangles from the top left
                    int y = 5 - i;
                    rect.Add(new KeyValuePair<int, int>(x, y));
                }
            }

            return rect;
        }

        public static List<KeyValuePair<int, int>> shiftX(List<KeyValuePair<int, int>> set, Stopwatch watch, int count)
        {
            //holds new x,y
            List<KeyValuePair<int, int>> shiftedPairs = new List<KeyValuePair<int, int>>();
            //given x,y, shift x right by the given int "count"
            foreach (var orderedPair in set)
            {
                KeyValuePair<int, int> shiftedPair = new KeyValuePair<int, int>((orderedPair.Key + count) % 50, orderedPair.Value);
                shiftedPairs.Add(shiftedPair);
            }

            return shiftedPairs;
        }

        public static List<KeyValuePair<int, int>> shiftY(List<KeyValuePair<int, int>> set, Stopwatch watch, int count)
        {
            //holds new x,y
            List<KeyValuePair<int, int>> shiftedPairs = new List<KeyValuePair<int, int>>();
            //given x,y shift y down by the given int "count"
            foreach (var orderedPair in set)
            {
                int x = orderedPair.Value - (count % 6);
                //handle negatives
                if (x < 0)
                {
                    int i = Math.Abs(x);
                    x = 6 - i;
                }

                KeyValuePair<int, int> shiftedPair = new KeyValuePair<int, int>(orderedPair.Key, x);
                shiftedPairs.Add(shiftedPair);
            }

            return shiftedPairs;
        }
    }
}
