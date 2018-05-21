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
            for (int x = 0; x < btm.Width ; x++)
            {
                for (int y = 0; y < btm.Height ; y++)
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

        public static ByteImage ProcessPicture(ByteImage picture)
        {
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
            //GaussFiltr(picture);
            treshold = GetTreshold(picture);
            //StretchColors(picture, (int)(0.8 * (double)treshold), 0.8, 1.2);
            Binarization(picture, (int)(1.0* (double)treshold));
            // horizontalHistogram = GetHorizontalHistogram(picture, 0);
            // verticalHistogram = GetVerticalHistogram(picture, 0);           
            // horizontalHistogram = Smooth(horizontalHistogram);
            //verticalHistogram = Smooth(verticalHistogram);

            return picture;
        }
    }
}
