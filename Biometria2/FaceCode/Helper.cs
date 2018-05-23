using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceCode
{
    public class Helper
    {
        public static void GrayScale(ByteImage btm)
        {
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    var value = (0.2 * (double)oldColour[1]) + (0.7 * (double)oldColour[2]) + (0.1 * (double)oldColour[3]);
                    btm.setPixel(x, y, oldColour[0], (byte)value, (byte)value, (byte)value);
                }
            }
        }

        public static void Binarization(ByteImage btm, int suggestedtreshold)
        {
            int pixAmount = 0;
            double sum = 0;
            for (int x = (int)(0.15 * btm.Width); x < btm.Width - (int)(0.15 * btm.Width); x++)
            {
                for (int y = (int)(0.15 * btm.Height); y < btm.Height - (int)(0.15 * btm.Height); y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    sum += (double)oldColour[1];
                    pixAmount++;
                }
            }
            int treshold = (int)sum / pixAmount;
            if (suggestedtreshold > 0)
            {
                treshold = suggestedtreshold;
            }
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    var value = oldColour[1] < treshold ? 0 : 255;
                    btm.setPixel(x, y, oldColour[0], (byte)value, (byte)value, (byte)value);
                }
            }
        }

        public static void Test(ByteImage btm)
        {
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    var value = CastColor((byte)(Math.Abs((double)oldColour[1] - (double)oldColour[2])));
                    btm.setPixel(x, y, oldColour[0], (byte)value, (byte)value, (byte)value);
                }
            }
        }

        public static int GetTreshold(ByteImage btm)
        {
            int pixAmount = 0;
            double sum = 0;
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    sum += (double)oldColour[1];
                    pixAmount++;
                }
            }
            int treshold = (int)sum / pixAmount;
            return treshold;
        }

        public static int Blackout(ByteImage btm)
        {
            int pixAmount = 0;
            double sum = 0;
            for (int x = (int)(0.15 * btm.Width); x < btm.Width - (int)(0.15 * btm.Width); x++)
            {
                for (int y = (int)(0.15 * btm.Height); y < btm.Height - (int)(0.15 * btm.Height); y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    sum += (double)oldColour[1];
                    pixAmount++;
                }
            }
            int treshold = (int)sum / pixAmount;
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    var value = oldColour[1] < ((byte)(1.2 * treshold)) ? 0 : oldColour[1];
                    btm.setPixel(x, y, oldColour[0], (byte)value, (byte)value, (byte)value);
                }
            }
            return treshold;
        }

        public static byte CastColor(byte color)
        {
            return color > 255 ? (byte)255 : (color < 0 ? (byte)0 : color);
        }

        public static void LowPassFilter(ByteImage btm)
        {
            double a = 2.0;
            double[,] wages = new double[3, 3] { { 1, 1, 1 }, { 1, a, 1 }, { 1, 1, 1 } };

            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    var currpix = btm.getPixel(x, y);
                    double sumR = 0.0;
                    double sumG = 0.0;
                    double sumB = 0.0;
                    double dividor = 0.0;
                    for (int x2 = -1; x2 < 2; x2++)
                    {
                        for (int y2 = -1; y2 < 2; y2++)
                        {
                            if (x + x2 >= 0 && y + y2 >= 0 && x + x2 < btm.Width && y + y2 < btm.Height)
                            {
                                double currWage = wages[x2 + 1, y2 + 1];
                                var pix = btm.getPixel(x + x2, y + y2);
                                sumR += (double)(pix[1]) * currWage;
                                sumG += (double)(pix[2]) * currWage;
                                sumB += (double)(pix[3]) * currWage;
                                dividor += currWage;
                            }
                        }
                    }
                    btm.setPixel(x, y, currpix[0], CastColor((byte)(sumR / dividor)), CastColor((byte)(sumG / dividor)), CastColor((byte)(sumB / dividor)));
                }
            }
        }

        public static void StretchColors(ByteImage btm, int treshold, double a, double b)
        {
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    var value = oldColour[1] < treshold ? (byte)(a * (double)oldColour[1]) : (byte)(b * (double)oldColour[1]);
                    btm.setPixel(x, y, oldColour[0], (byte)value, (byte)value, (byte)value);
                }
            }
        }

        public static int[] GetHorizontalHistogram(ByteImage btm, byte comparedColor)
        {
            int[] histogram = new int[btm.Width];

            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    var value = oldColour[1] == comparedColor ? 1 : 0;
                    histogram[x] += value;
                }
            }

            return histogram;
        }

        public static int[] GetVerticalHistogram(ByteImage btm, byte comparedColor)
        {
            int[] histogram = new int[btm.Height];
            for (int y = 0; y < btm.Height; y++)
            {
                for (int x = 0; x < btm.Width; x++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    var value = oldColour[1] == comparedColor ? 1 : 0;
                    histogram[y] += value;
                }
            }
            return histogram;
        }

        public static byte[] MarkFaceCenter(ByteImage btm, int X, int Y)
        {
            var currPix = btm.getPixel(X, Y);
            byte[] newCol = new byte[] { currPix[0], (byte)220, (byte)0, (byte)0 };

            btm.setPixel(X, Y, newCol);
            btm.setPixel(X - 1, Y, newCol);
            btm.setPixel(X + 1, Y, newCol);

            btm.setPixel(X + 1, Y + 1, newCol);
            btm.setPixel(X, Y - 1, newCol);
            btm.setPixel(X, Y + 1, newCol);

            btm.setPixel(X - 1, Y - 1, newCol);
            btm.setPixel(X - 2, Y, newCol);
            btm.setPixel(X + 2, Y, newCol);

            btm.setPixel(X - 2, Y - 2, newCol);
            btm.setPixel(X + 2, Y + 2, newCol);
            btm.setPixel(X, Y - 2, newCol);
            btm.setPixel(X, Y + 2, newCol);

            return newCol;
        }

        public static void GaussFiltr(ByteImage btm)
        {
            double a = 2.0;
            double[,] wages = new double[5, 5] { { 1, 1, 1, 1, 1 }, { 1, 1, a, 1, 1 }, { 1, a, a * a, a, 1 }, { 1, 1, a, 1, 1 }, { 1, 1, 1, 1, 1 } };

            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    var currpix = btm.getPixel(x, y);
                    double sumR = 0.0;
                    double sumG = 0.0;
                    double sumB = 0.0;
                    double dividor = 0.0;
                    for (int x2 = -2; x2 < 3; x2++)
                    {
                        for (int y2 = -2; y2 < 3; y2++)
                        {
                            if (x + x2 >= 0 && y + y2 >= 0 && x + x2 < btm.Width && y + y2 < btm.Height)
                            {
                                double currWage = wages[x2 + 2, y2 + 2];
                                var pix = btm.getPixel(x + x2, y + y2);
                                sumR += (double)(pix[1]) * currWage;
                                sumG += (double)(pix[2]) * currWage;
                                sumB += (double)(pix[3]) * currWage;
                                dividor += currWage;
                            }
                        }
                    }
                    btm.setPixel(x, y, currpix[0], CastColor((byte)(sumR / dividor)), CastColor((byte)(sumG / dividor)), CastColor((byte)(sumB / dividor)));
                }
            }
        }

        public static int HorizontalCenter(int[] histogram)
        {
            int centerX = 0;
            var horizontalHistogram = Smooth(histogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);

            int min = horizontalHistogram.Max();
            int pos = 0;
            for (int i = horizontalHistogram.Length / 3; i < horizontalHistogram.Length - (horizontalHistogram.Length / 3); i++)
            {
                if (horizontalHistogram[i] < min)
                {
                    pos = i;
                    min = horizontalHistogram[i];
                }
            }
            centerX = pos;
            return centerX;
        }

        public static int VerticalCenter(int[] histogram)
        {
            int centerY = 0;
            var verticalHistogram = Smooth(histogram);
            verticalHistogram = Smooth(verticalHistogram);
            verticalHistogram = Smooth(verticalHistogram);
            verticalHistogram = Smooth(verticalHistogram);
            verticalHistogram = Smooth(verticalHistogram);
            verticalHistogram = Smooth(verticalHistogram);

            int min = verticalHistogram.Max();
            int pos = 0;
            for (int i = verticalHistogram.Length / 4; i < verticalHistogram.Length - (verticalHistogram.Length / 4); i++)
            {
                if (verticalHistogram[i] < min)
                {
                    pos = i;
                    min = verticalHistogram[i];
                }
            }
            centerY = pos;
            return centerY;
        }

        public static List<int> HorizontalLeftCenterRight(int[] histogram)
        {
            List<int> points = new List<int>();
            int firstLeft = histogram.Length;
            int firstRight = 0;

            for (int i = 0; i < histogram.Length; i++)
            {
                if (histogram[i] > 0)
                {
                    firstLeft = i;
                    break;
                }
            }
            for (int i = histogram.Length - 1; i >= 0; i--)
            {
                if (histogram[i] > 0)
                {
                    firstRight = i;
                    break;
                }
            }

            var horizontalHistogram = Smooth(histogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);



            int max = 0;
            int pos = 0;
            for (int i = firstLeft; i < firstRight; i++)
            {
                if (horizontalHistogram[i] > max)
                {
                    pos = i;
                    max = horizontalHistogram[i];
                }
            }
            points.Add(firstLeft);
            points.Add(pos);
            points.Add(firstRight);
            return points;
        }

        public static int[] Smooth(int[] histogram)
        {
            int[] newHistogram = new int[histogram.Length];
            List<int> currSum = new List<int>();
            for (int i = 0; i < histogram.Length; i++)
            {
                newHistogram[i] = 0;
            }
            currSum.Add(histogram[0]);
            currSum.Add(histogram[1]);
            currSum.Add(histogram[2]);
            currSum.Add(histogram[3]);
            currSum.Add(histogram[4]);
            currSum.Add(histogram[3]);
            currSum.Add(histogram[4]);

            for (int i = 3; i < histogram.Length - 3; i++)
            {
                newHistogram[i] = (int)currSum.Sum() / currSum.Count;
                currSum.RemoveAt(0);
                if (i + 4 < histogram.Length)
                {
                    currSum.Add(histogram[i + 4]);
                }
            }
            newHistogram[0] = newHistogram[3];
            newHistogram[1] = newHistogram[3];
            newHistogram[2] = newHistogram[3];
            newHistogram[histogram.Length - 3] = newHistogram[histogram.Length - 4];
            newHistogram[histogram.Length - 2] = newHistogram[histogram.Length - 4];
            newHistogram[histogram.Length - 1] = newHistogram[histogram.Length - 4];
            return newHistogram;
        }

        private static void FloodFill(ByteImage bmp, System.Drawing.Point pt, List<byte[]> targetColors, byte[] replacementColor)
        {
            List<Queue<System.Drawing.Point>> stackList = new List<Queue<System.Drawing.Point>>();
            bool[][] isAdded = new bool[bmp.Width][];
            for (int i = 0; i < isAdded.Length; i++)
            {
                isAdded[i] = new bool[bmp.Height];
                for (int j = 0; j < bmp.Height; j++)
                {
                    isAdded[i][j] = false;
                }
            }
            Queue<System.Drawing.Point> pixels = new Queue<System.Drawing.Point>();
            //ByteImage tempPict = new ByteImage(bmp);
            pixels.Enqueue(pt);
            stackList.Add(pixels);
            isAdded[pt.X][pt.Y] = true;
            int lB = 0;
            int rB = bmp.Width;
            int uB = bmp.Height;
            int bB = 0;

            int stackIndex = 0;
            while (ShouldContinue(stackList))
            {
                System.Drawing.Point a = stackList[stackIndex].Dequeue();

                if (a.X < rB && a.X >= lB &&
                        a.Y < uB && a.Y >= bB)//make sure we stay within bounds
                {
                    bool isPushed = false;
                    foreach (var c in targetColors)
                    {
                        if (!isPushed && bmp.getPixel(a.X, a.Y)[1] == c[1])
                        {
                            bmp.setPixel(a.X, a.Y, replacementColor);
                            if (a.X - 1 < rB && a.X - 1 >= lB && a.Y < uB && a.Y >= bB && !isAdded[a.X - 1][a.Y])
                            {
                                stackList[stackIndex].Enqueue(new System.Drawing.Point(a.X - 1, a.Y));
                                isAdded[a.X - 1][a.Y] = true;
                            }
                            if (a.X + 1 < rB && a.X + 1 >= lB && a.Y < uB && a.Y >= bB && !isAdded[a.X + 1][a.Y])
                            {
                                stackList[stackIndex].Enqueue(new System.Drawing.Point(a.X + 1, a.Y));
                                isAdded[a.X + 1][a.Y] = true;
                            }
                            if (a.X < rB && a.X >= lB && a.Y - 1 < uB && a.Y - 1 >= bB && !isAdded[a.X][a.Y - 1])
                            {
                                stackList[stackIndex].Enqueue(new System.Drawing.Point(a.X, a.Y - 1));
                                isAdded[a.X][a.Y - 1] = true;
                            }
                            if (a.X < rB && a.X >= lB && a.Y + 1 < uB && a.Y + 1 >= bB && !isAdded[a.X][a.Y + 1])
                            {
                                stackList[stackIndex].Enqueue(new System.Drawing.Point(a.X, a.Y + 1));
                                isAdded[a.X][a.Y + 1] = true;
                            }
                            isPushed = true;
                        }
                    }
                }
                if (stackList[stackIndex].Count > 30000)
                {
                    stackIndex++;
                    stackList.Add(new Queue<System.Drawing.Point>());
                    System.Drawing.Point b = stackList[stackIndex - 1].Dequeue();
                    stackList[stackIndex].Enqueue(b);
                }
                else if (stackList[stackIndex].Count == 0)
                {
                    stackList.RemoveAt(stackIndex);
                    stackIndex--;
                }
            }
        }

        private static bool ShouldContinue(List<Queue<System.Drawing.Point>> stackList)
        {
            bool isSomething = false;
            for (int i = 0; i < stackList.Count; i++)
            {
                if (stackList[i].Count > 0)
                {
                    isSomething = true;
                }
            }
            return isSomething;
        }

        private static bool CanAdd(List<Stack<System.Drawing.Point>> stackList, System.Drawing.Point point)
        {
            bool canAdd = true;
            for (int i = 0; i < stackList.Count; i++)
            {
                for (int j = 0; j < stackList[i].Count; j++)
                {
                    if (stackList[i].ElementAt(j) == point)
                    {
                        canAdd = false;
                    }
                }
            }
            return canAdd;
        }

        public static List<int> VerticalUpCenterBottom(int[] histogram)
        {
            List<int> points = new List<int>();
            int firstUp = histogram.Length;
            int firstDown = 0;

            for (int i = 0; i < histogram.Length; i++)
            {
                if (histogram[i] > 0)
                {
                    firstUp = i;
                    break;
                }
            }
            for (int i = histogram.Length - 1; i >= 0; i--)
            {
                if (histogram[i] > 0)
                {
                    firstDown = i;
                    break;
                }
            }

            var horizontalHistogram = Smooth(histogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);
            horizontalHistogram = Smooth(horizontalHistogram);



            int max = 0;
            int pos = 0;
            for (int i = firstUp; i < firstDown; i++)
            {
                if (horizontalHistogram[i] > max)
                {
                    pos = i;
                    max = horizontalHistogram[i];
                }
            }
            points.Add(firstUp);
            points.Add(pos);
            points.Add(firstDown);
            return points;
        }

        private static List<Point> DrawBorders(ByteImage ori, List<int> horizontalBorders, List<int> verticalBorders, byte[] color)
        {
            List<Point> corners = new List<Point>();
            Point center = new Point(horizontalBorders.ElementAt(1), verticalBorders.ElementAt(1));
            Point upLeft = new Point(horizontalBorders.ElementAt(0), verticalBorders.ElementAt(0));
            Point upRight = new Point(horizontalBorders.ElementAt(2), verticalBorders.ElementAt(0));
            Point bottomLeft = new Point(horizontalBorders.ElementAt(0), verticalBorders.ElementAt(2));
            Point bottomRight = new Point(horizontalBorders.ElementAt(2), verticalBorders.ElementAt(2));

            Point meanCenter = new Point((((Math.Abs(upRight.X - upLeft.X) / 2) + upLeft.X) + center.X) / 2, (((Math.Abs(upRight.Y - bottomRight.Y) / 2) + upRight.Y) + center.Y) / 2);

            int meanWidth = (Math.Abs(upLeft.X - meanCenter.X) + Math.Abs(meanCenter.X - upRight.X)) / 2;
            int meanHeigt = (Math.Abs(upLeft.Y - meanCenter.Y) + Math.Abs(meanCenter.Y - upRight.Y)) / 2;

            upLeft = CorrectPoint(new Point(meanCenter.X - meanWidth - (int)(0.1 * meanWidth), meanCenter.Y - meanHeigt - (int)(0.1 * meanHeigt)), ori);
            upRight = CorrectPoint(new Point(meanCenter.X + meanWidth + (int)(0.1 * meanWidth), meanCenter.Y - meanHeigt - (int)(0.1 * meanHeigt)), ori);
            bottomLeft = CorrectPoint(new Point(meanCenter.X - meanWidth - (int)(0.1 * meanWidth), meanCenter.Y + meanHeigt + (int)(0.1 * meanHeigt)), ori);
            bottomRight = CorrectPoint(new Point(meanCenter.X + meanWidth + (int)(0.1 * meanWidth), meanCenter.Y + meanHeigt + (int)(0.1 * meanHeigt)), ori);

            corners.Add(upLeft);
            corners.Add(upRight);
            corners.Add(bottomLeft);
            corners.Add(bottomRight);

            for (int x = bottomLeft.X; x < bottomRight.X; x++)
            {
                ori.setPixel(x, upLeft.Y, color);
                ori.setPixel(x, bottomLeft.Y, color);
            }

            for (int y = upLeft.Y; y < bottomLeft.Y; y++)
            {
                ori.setPixel(upLeft.X, y, color);
                ori.setPixel(upRight.X, y, color);
            }

            return corners;
        }

        public static Point CorrectPoint(Point point, ByteImage orig)
        {
            if (point.X >= orig.Width)
            {
                point.X = orig.Width - 1;
            }
            if (point.X < 0)
            {
                point.X = 0;
            }
            if (point.Y >= orig.Height)
            {
                point.Y = orig.Height - 1;
            }
            if (point.Y < 0)
            {
                point.Y = 0;
            }
            return point;
        }

        private static ByteImage CutOffFace(List<Point> corners, ByteImage ori)
        {
            int width = corners.ElementAt(1).X - corners.ElementAt(0).X;
            int height = corners.ElementAt(2).Y - corners.ElementAt(0).Y;
            ByteImage face = new ByteImage(width, height);

            for (int x = 0; x < face.Width; x++)
            {
                for (int y = 0; y < face.Height; y++)
                {
                    face.setPixel(x, y, ori.getPixel(x + corners.ElementAt(0).X, y + corners.ElementAt(0).Y));
                }
            }
            return face;
        }

        private static int GetSkinCollor(ByteImage picture)
        {
            int iteration = 15;
            int colorCounter = 3;
            int[] Sums = new int[colorCounter];
            int[] Counter = new int[colorCounter];
            int[] Values = new int[colorCounter];
            for (int i = 0; i < colorCounter; i++)
            {
                Values[i] = (255 / colorCounter) * i;
                Sums[i] = 0;
                Counter[i] = 0;
            }

            for (int i = 0; i < iteration; i++)
            {
                for (int x = 0; x < picture.Width; x++)
                {
                    for (int y = 0; y < picture.Height; y++)
                    {
                        byte[] color = picture.getPixel(x, y);
                        int dist = 255;
                        int indx = 0;
                        for (int c = 0; c < colorCounter; c++)
                        {
                            if (Math.Abs(color[1] - Values[c]) < dist)
                            {
                                dist = Math.Abs(color[1] - Values[c]);
                                indx = c;
                            }
                        }
                        Sums[indx] += color[1];
                        Counter[indx]++;
                    }
                }

                for (int c = 0; c < colorCounter; c++)
                {
                    Values[c] = Sums[c] / Counter[c];
                    Sums[c] = 0;
                    Counter[c] = 0;
                }
            }

            for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    byte[] color = picture.getPixel(x, y);
                    int dist = 255;
                    int indx = 0;
                    for (int c = 0; c < colorCounter; c++)
                    {
                        if (Math.Abs(color[1] - Values[c]) < dist)
                        {
                            dist = Math.Abs(color[1] - Values[c]);
                            indx = c;
                        }
                    }
                    Sums[indx] += color[1];
                    Counter[indx]++;
                }
            }

            int mostCommonCounter = 0;
            int mostCommonIndex = 0;
            for (int c = 0; c < colorCounter; c++)
            {
                if (Counter[c] > mostCommonCounter)
                {
                    mostCommonCounter = Counter[c];
                    mostCommonIndex = c;
                }
            }

            /*for (int x = 0; x < picture.Width; x++)
            {
                for (int y = 0; y < picture.Height; y++)
                {
                    byte[] color = picture.getPixel(x, y);
                    int dist = 255;
                    int indx = 0;
                    for (int c = 0; c < colorCounter; c++)
                    {
                        if (Math.Abs(color[1] - Values[c]) < dist)
                        {
                            dist = Math.Abs(color[1] - Values[c]);
                            indx = c;
                        }
                    }
                    picture.setPixel(x, y, color[0], (byte)Values[indx], (byte)Values[indx], (byte)Values[indx]);
                }
            }*/

            return Values[mostCommonIndex];
        }

        /* private static Point[] GetMouthLine(int[] horizontalHistogram, int[] verticalHistogram)
         {
             //od konjca pierwsze wzniesienie 
             Point pointLeft = new Point();
             Point pointRight = new Point();

             int lowLevelStart = (int)(0.45 * (double)(verticalHistogram.Max() - verticalHistogram.Min()));
             bool isInLowLevel = false;
             int currTop = 0;
             int currIndx = 0;

             for (int i = verticalHistogram.Length - 1; i > verticalHistogram.Length / 2; i--)
             {
                 if (verticalHistogram[i] <= lowLevelStart)
                 {
                     isInLowLevel = true;
                 }
                 if (isInLowLevel && verticalHistogram[i] > lowLevelStart)
                 {
                     if (verticalHistogram[i] > currTop)
                     {
                         currTop = verticalHistogram[i];
                         currIndx = i;
                     }
                 }
                 if (currTop != 0 && isInLowLevel && verticalHistogram[i] < lowLevelStart)
                 {
                     break;
                 }
             }
             int y = currIndx;
             pointLeft.X = 0;
             pointLeft.Y = y;
             pointRight.X = verticalHistogram.Length - 1;
             pointRight.Y = y;

             return new Point[] { pointLeft, pointRight };
         }*/

        private static Point[] GetMouthLine(int[] horizontalHistogram, int[] verticalHistogram)
        {
            //od konjca pierwsze wzniesienie 
            Point pointLeft = new Point();
            Point pointRight = new Point();


            //for (int i = verticalHistogram.Length - 1; i > verticalHistogram.Length / 2; i--)
            int currIndx = 0;
            int currTop = 0;
            bool searching = false;
            bool goingDown = false;
            bool goingUp = false;
            for (int i = verticalHistogram.Length - 1; i > verticalHistogram.Length / 2; i--)
            {
                if (verticalHistogram[i] > verticalHistogram[i - 1])
                {
                    goingUp = false;
                    goingDown = true;
                    if (searching)
                    {
                        break;
                    }
                }
                else if (goingDown && verticalHistogram[i] < verticalHistogram[i - 1])
                {
                    goingUp = true;
                    goingDown = false;
                    searching = true;
                }
                if (goingUp)
                {
                    if (verticalHistogram[i] >= currTop)
                    {
                        currTop = verticalHistogram[i];
                        currIndx = i;
                    }
                }
            }
            int y = currIndx;
            pointLeft.X = 0;
            pointLeft.Y = y;
            pointRight.X = verticalHistogram.Length - 1;
            pointRight.Y = y;

            return new Point[] { pointLeft, pointRight };
        }

        /*private static Point[] GetEyesLine(int[] horizontalHistogram, int[] verticalHistogram)
        {
            od poczatku pierwsze wzniesienie 
            Point pointLeft = new Point();
            Point pointRight = new Point();

            int lowLevelStart = (int)(0.35 * (double)(verticalHistogram.Max() - verticalHistogram.Min()));
            bool isInLowLevel = false;
            int currTop = 0;
            int currIndx = 0;

            for (int i = verticalHistogram.Length / 10; i < verticalHistogram.Length / 2; i++)
            {
                if (verticalHistogram[i] <= lowLevelStart)
                {
                    isInLowLevel = true;
                }
                if (isInLowLevel && verticalHistogram[i] > lowLevelStart)
                {
                    if (verticalHistogram[i] > currTop)
                    {
                        currTop = verticalHistogram[i];
                        currIndx = i;
                    }
                }
                if (currTop != 0 && isInLowLevel && verticalHistogram[i] < lowLevelStart)
                {
                    break;
                }
            }
            int y = currIndx;
            pointLeft.X = 0;
            pointLeft.Y = y;
            pointRight.X = verticalHistogram.Length - 1;
            pointRight.Y = y;
            

            return new Point[] { pointLeft, pointRight };
        }*/

        private static Point[] GetEyesLine(int[] horizontalHistogram, int[] verticalHistogram)
        {
            //od poczatku pierwsze wzniesienie 
            Point pointLeft = new Point();
            Point pointRight = new Point();

            int currIndx = 0;
            int currTop = 0;
            bool searching = false;
            bool goingDown = false;
            bool goingUp = false;
            for (int i = verticalHistogram.Length / 10; i < verticalHistogram.Length / 2; i++)
            {
                if (verticalHistogram[i] > verticalHistogram[1 + i])
                {
                    goingUp = false;
                    goingDown = true;
                    if (searching)
                    {
                        break;
                    }
                }
                else if (goingDown && verticalHistogram[i] < verticalHistogram[1 + i])
                {
                    goingUp = true;
                    goingDown = false;
                    searching = true;
                }
                if (goingUp)
                {
                    if (verticalHistogram[i] >= currTop)
                    {
                        currTop = verticalHistogram[i];
                        currIndx = i;
                    }
                }
            }

            int y = currIndx;
            pointLeft.X = 0;
            pointLeft.Y = y;
            pointRight.X = verticalHistogram.Length - 1;
            pointRight.Y = y;

            return new Point[] { pointLeft, pointRight };
        }

        private static Point[] GetFaceWidth(int[] horizontalHistogram, int[] verticalHistogram)
        {
            //od poczatku pierwsze wzniesienie 
            Point pointLeft = new Point();
            Point pointRight = new Point();

            int leftBorder = 0;
            int rightBorder = 0;

            int searchRegion = (int)(horizontalHistogram.Length / 20);
            int jumpSize = (int)(0.3 * (double)(horizontalHistogram.Max() - horizontalHistogram.Min()));

            for (int i = 0; i < horizontalHistogram.Length - searchRegion; i++)
            {
                int currLevel = horizontalHistogram[i];
                int nextLevel = horizontalHistogram[i + searchRegion];
                if (Math.Abs(currLevel - nextLevel) >= jumpSize)
                {
                    leftBorder = ((2 * i) + searchRegion) / 2;
                    break;
                }
            }

            for (int i = horizontalHistogram.Length - 1; i > searchRegion; i--)
            {
                int currLevel = horizontalHistogram[i];
                int nextLevel = horizontalHistogram[i - searchRegion];
                if (Math.Abs(currLevel - nextLevel) >= jumpSize)
                {
                    rightBorder = ((2 * i) + searchRegion) / 2;
                    break;
                }
            }

            pointLeft.X = leftBorder;
            pointLeft.Y = 0;
            pointRight.X = horizontalHistogram.Length - rightBorder;
            pointRight.Y = 0;

            return new Point[] { pointLeft, pointRight };
        }

        private static Point[] GetFaceHeight(int[] horizontalHistogram, int[] verticalHistogram)
        {
            //od poczatku pierwsze wzniesienie 
            Point pointLeft = new Point();
            Point pointRight = new Point();

            int leftBorder = 0;
            int rightBorder = 0;

            int searchRegion = (int)(verticalHistogram.Length / 20);
            int jumpSize = (int)(0.35 * (double)(verticalHistogram.Max() - verticalHistogram.Min()));

            for (int i = 0; i < verticalHistogram.Length / 2; i++)
            {
                int currLevel = verticalHistogram[i];
                int nextLevel = verticalHistogram[i + searchRegion];
                if (Math.Abs(currLevel - nextLevel) >= jumpSize)
                {
                    leftBorder = ((2 * i) + searchRegion) / 2;
                    break;
                }
            }

            for (int i = verticalHistogram.Length - 1; i > verticalHistogram.Length / 2; i--)
            {
                int currLevel = verticalHistogram[i];
                int nextLevel = verticalHistogram[i - searchRegion];
                if (Math.Abs(currLevel - nextLevel) >= (0.8 * jumpSize))
                {
                    rightBorder = (i + searchRegion);
                    break;
                }
            }

            pointLeft.X = 0;
            pointLeft.Y = leftBorder + 10;
            pointRight.X = 0;
            pointRight.Y = verticalHistogram.Length - (int)(1.5 * rightBorder);

            return new Point[] { pointLeft, pointRight };
        }

        private static Point[] GetMidLine(int[] horizontalHistogram, int[] verticalHistogram)
        {
            Point pointLeft = new Point();
            Point pointRight = new Point();

            int min = horizontalHistogram.Max();
            int indx = 0;

            for (int i = horizontalHistogram.Length / 4; i < horizontalHistogram.Length - (horizontalHistogram.Length / 4); i++)
            {
                if (horizontalHistogram[i] < min)
                {
                    min = horizontalHistogram[i];
                    indx = i;
                }
            }

            pointLeft.X = indx;
            pointLeft.Y = 0;
            pointRight.X = indx;
            pointRight.Y = verticalHistogram.Length - 1;

            return new Point[] { pointLeft, pointRight };
        }

        public static void DrawLine(Point[] points, byte[] color, ByteImage picture)
        {
            bool horizontal = false;


            if (points[1].X - points[0].X > 0)
            {
                horizontal = true;
            }

            if (horizontal)
            {
                for (int x = points[0].X; x < points[1].X; x++)
                {
                    picture.setPixel(x, points[0].Y, color);
                }
            }
            else
            {
                for (int y = points[0].Y; y < points[1].Y; y++)
                {
                    picture.setPixel(points[0].X, y, color);
                }
            }

        }

        public static float[] GetFloatFeatures(Point[] wysokosc, Point[] szerokosc, Point[] usta, Point[] oczy)
        {
            float[] features = new float[4];
            //szerokosc do dlugosci
            double width = Math.Abs(szerokosc[0].X - szerokosc[1].X);
            double height = Math.Abs(wysokosc[0].Y - wysokosc[1].Y);
            features[0] = (float)(width / height);
            //rozstaw oczy i usta
            double eyeMouthDistance = Math.Abs(usta[0].Y - oczy[0].Y);
            features[1] = (float)(eyeMouthDistance / height);
            //lokalizacja ust
            double eyeLocalisation = Math.Abs(wysokosc[1].Y - oczy[0].Y);
            features[2] = (float)(eyeLocalisation / height);
            //lokalizacja oczu
            double mouthLocalisation = Math.Abs(wysokosc[1].Y - usta[0].Y);
            features[3] = (float)(mouthLocalisation / height);

            for (int i = 0; i < features.Length; i++)
            {
                if (features[i] < 0.0f)
                {
                    features[i] = 0.0f;
                }
                else if (features[i] > 1.0f)
                {
                    features[i] = 1.0f;
                }
            }
            return features;
        }

        public static Tuple<ByteImage, float[]> ProcessPicture(ByteImage picture)
        {
            float[] features = new float[4];
            ByteImage ori = new ByteImage(picture);
            ByteImage onlyFace = new ByteImage(picture);
            int CenterX = 0;
            int CenterY = 0;
            Test(picture);
            int treshold = Blackout(picture);
            GaussFiltr(picture);

            StretchColors(picture, (int)(1.2 * (double)treshold), 0.1, 1.5);
            Binarization(picture, (int)(1.6 * (double)treshold));
            int[] horizontalHistogram = GetHorizontalHistogram(picture, 0);
            int[] verticalHistogram = GetVerticalHistogram(picture, 0);
            //WYGLADZANIE !! moving average
            horizontalHistogram = Smooth(horizontalHistogram);
            verticalHistogram = Smooth(verticalHistogram);
            //Znalezienie minimow !! moje algo
            CenterX = HorizontalCenter(horizontalHistogram);
            CenterY = VerticalCenter(verticalHistogram);

            var newColor = MarkFaceCenter(picture, CenterX, CenterY);
            FloodFill(picture, new System.Drawing.Point(CenterX, CenterY), new List<byte[]>() { new byte[] { 255, 255, 255, 255 }, newColor }, newColor);

            int[] horizontaRedlHistogram = GetHorizontalHistogram(picture, newColor[1]);
            int[] verticalRedHistogram = GetVerticalHistogram(picture, newColor[1]);

            //WYGLADZANIE !! moving average
            horizontaRedlHistogram = Smooth(horizontaRedlHistogram);
            verticalRedHistogram = Smooth(verticalRedHistogram);

            List<int> horizontalBorders = HorizontalLeftCenterRight(horizontaRedlHistogram);
            List<int> verticalBorders = VerticalUpCenterBottom(verticalRedHistogram);

            GrayScale(ori);
            MarkFaceCenter(ori, horizontalBorders.ElementAt(1), verticalBorders.ElementAt(1));


            //TU MAMY W MIARE TWARZ

            List<System.Drawing.Point> corners = DrawBorders(ori, horizontalBorders, verticalBorders, newColor);
            onlyFace = CutOffFace(corners, onlyFace);
            picture = new ByteImage(onlyFace);

            GrayScale(picture);
            int skinColor = GetSkinCollor(picture);
            Binarization(picture, (int)(0.7 * (double)skinColor));
            horizontalHistogram = GetHorizontalHistogram(picture, 0);
            verticalHistogram = GetVerticalHistogram(picture, 0);
            for (int i = 0; i < 10; i++)
            {
                horizontalHistogram = Smooth(horizontalHistogram);
                verticalHistogram = Smooth(verticalHistogram);
            }

            Point[] liniaSrodka = GetMidLine(horizontalHistogram, verticalHistogram);
            Point[] wysokoscTwarzy = GetFaceHeight(horizontalHistogram, verticalHistogram);
            wysokoscTwarzy[0].X = liniaSrodka[0].X;
            wysokoscTwarzy[1].X = liniaSrodka[0].X;
            Point[] szerokoscTwarzy = GetFaceWidth(horizontalHistogram, verticalHistogram);
            szerokoscTwarzy[0].Y = Math.Abs(wysokoscTwarzy[1].Y - wysokoscTwarzy[0].Y);
            szerokoscTwarzy[1].Y = Math.Abs(wysokoscTwarzy[1].Y - wysokoscTwarzy[0].Y);
            Point[] liniaOczu = GetEyesLine(horizontalHistogram, verticalHistogram);
            Point[] lniaUst = GetMouthLine(horizontalHistogram, verticalHistogram);


            DrawLine(liniaSrodka, new byte[] { 255, 0, 255, 0 }, picture); //Green
            DrawLine(wysokoscTwarzy, new byte[] { 255, 255, 0, 0 }, picture); //Red
            DrawLine(szerokoscTwarzy, new byte[] { 255, 255, 0, 0 }, picture); //Red
            DrawLine(liniaOczu, new byte[] { 255, 0, 0, 255 }, picture); //Blue
            DrawLine(lniaUst, new byte[] { 255, 0, 255, 255 }, picture); //Turkus 
            features = GetFloatFeatures(wysokoscTwarzy, szerokoscTwarzy, lniaUst, liniaOczu);


            return new Tuple<ByteImage, float[]>(picture, features);
        }
    }
}
