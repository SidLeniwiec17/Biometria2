using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace IrisCode
{
    public class Helper
    {
        public static System.Windows.Media.ImageSource ToBitmapSource(Bitmap p_bitmap)
        {
            IntPtr hBitmap = p_bitmap.GetHbitmap();
            System.Windows.Media.ImageSource wpfBitmap;
            try
            {
                wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                //p_bitmap.Dispose();
            }
            return wpfBitmap;
        }

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

        public static void Gauss(ByteImage btm)
        {
            double a = 2;
            double[,] wages = new double[3, 3] { { 1, a, 1 }, { a, a * a, a }, { 1, a, 1 } };
            GaussFilter(btm, a, wages);
        }

        public static void Contrast(ByteImage btm)
        {
            PrimitiveContrast pc = new PrimitiveContrast(30, 255, 255, 0);
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    btm.setPixel(x, y, pc.GetColor(btm.getPixel(x, y)));
                }
            }
        }

        public static void GaussFilter(ByteImage btm, double a, double[,] wages)
        {
            ByteImage tempPict = new ByteImage(btm);

            for (int x = 0; x < tempPict.Width; x++)
            {
                for (int y = 0; y < tempPict.Height; y++)
                {
                    var currPix = tempPict.Pixels[tempPict.getPixelIndex(x, y)];
                    double sumR = 0.0;
                    double sumG = 0.0;
                    double sumB = 0.0;
                    double dividor = 0.0;
                    for (int x2 = -1; x2 < 2; x2++)
                    {
                        for (int y2 = -1; y2 < 2; y2++)
                        {
                            if (x + x2 >= 0 && y + y2 >= 0 && x + x2 < tempPict.Width && y + y2 < tempPict.Height)
                            {
                                double currWage = wages[x2 + 1, y2 + 1];
                                var xx = x + x2;
                                var yy = y + y2;
                                sumR += tempPict.Pixels[tempPict.getPixelIndex(xx, yy) + 1] * currWage;
                                sumG += tempPict.Pixels[tempPict.getPixelIndex(xx, yy) + 2] * currWage;
                                sumB += tempPict.Pixels[tempPict.getPixelIndex(xx, yy) + 3] * currWage;
                                dividor += currWage;
                            }
                        }
                    }
                    btm.setPixel(x, y, currPix, (byte)FromInterval((int)(sumR / dividor)), (byte)FromInterval((int)(sumG / dividor)), (byte)FromInterval((int)(sumB / dividor)));
                }
            }
        }

        private static int FromInterval(int col)
        {
            if (col <= 255 && col >= 0)
                return col;
            else if (col > 255)
                return 255;
            else
                return 0;
        }

        public static void RunBlack(ByteImage newBmpTbl)
        {
            int bigCounter = 3;
            while (bigCounter > 2)
            {
                ByteImage tempPict = new ByteImage(newBmpTbl);
                bigCounter = 0;
                for (int x = (int)newBmpTbl.Width / 8; x < newBmpTbl.Width - newBmpTbl.Width / 7; x++)
                {
                    for (int y = (int)newBmpTbl.Height / 8; y < newBmpTbl.Height - newBmpTbl.Height / 7; y++)
                    {
                        int counter = 0;
                        for (int x2 = -1; x2 < 2; x2++)
                        {
                            for (int y2 = -1; y2 < 2; y2++)
                            {
                                if (newBmpTbl.Pixels[newBmpTbl.getPixelIndex(x, y) + 1] != 0 && x + x2 >= 0 && y + y2 >= 0 && x + x2 < newBmpTbl.Width && y + y2 < newBmpTbl.Height && newBmpTbl.Pixels[newBmpTbl.getPixelIndex(x + x2, y + y2) + 1] <= (255 / 6))
                                {
                                    counter++;
                                }
                            }
                        }
                        if (counter >= 5)
                        {
                            tempPict.setPixel(x, y, 255, 0, 0, 0);
                            bigCounter++;
                        }
                    }
                }
                Console.WriteLine("Blacked " + bigCounter + " pixels.");
                for (int x = 0; x < newBmpTbl.Width; x++)
                {
                    for (int y = 0; y < newBmpTbl.Height; y++)
                    {
                        newBmpTbl.setPixel(x, y, tempPict.getPixel(x, y));
                    }
                }
            }
        }

        public static void RunFullBlack(ByteImage newBmpTbl)
        {
            int bigCounter = 3;
            while (bigCounter > 2)
            {
                ByteImage tempPict = new ByteImage(newBmpTbl);
                bigCounter = 0;
                for (int x = 0; x < newBmpTbl.Width; x++)
                {
                    for (int y = 0; y < newBmpTbl.Height; y++)
                    {
                        int counter = 0;
                        for (int x2 = -1; x2 < 2; x2++)
                        {
                            for (int y2 = -1; y2 < 2; y2++)
                            {
                                if (newBmpTbl.Pixels[newBmpTbl.getPixelIndex(x, y) + 1] != 0 && x + x2 >= 0 && y + y2 >= 0 && x + x2 < newBmpTbl.Width && y + y2 < newBmpTbl.Height && newBmpTbl.Pixels[newBmpTbl.getPixelIndex(x + x2, y + y2) + 1] <= (255 / 6))
                                {
                                    counter++;
                                }
                            }
                        }
                        if (counter >= 5)
                        {
                            tempPict.setPixel(x, y, 255, 0, 0, 0);
                            bigCounter++;
                        }
                    }
                }
                Console.WriteLine("Blacked " + bigCounter + " pixels.");
                for (int x = 0; x < newBmpTbl.Width; x++)
                {
                    for (int y = 0; y < newBmpTbl.Height; y++)
                    {
                        newBmpTbl.setPixel(x, y, tempPict.getPixel(x, y));
                    }
                }
            }
        }

        internal static void DrawLines(ByteImage newBmpTbl, int pupilX, int pupilY, int pupilR, int irisX, int irisY, int irisR)
        {
            byte[] firstLine;
            byte[] secondLine;
            byte[] thirdLine;
            byte[] forthLine;
            byte[] fifthLine;
            byte[] sixthLine;
            byte[] seventhLine;
            byte[] eighthLine;

            double[] coss = new double[360];
            double[] sins = new double[360];
            for (int t = 0; t < 360; t++)
            {
                coss[t] = Math.Cos(t * Math.PI / 180);
                sins[t] = Math.Sin(t * Math.PI / 180);
            }

            int dist = ((irisR - ((irisR - pupilR) / 10)) - pupilR) / 8;


            firstLine = GetSingleLine(newBmpTbl, pupilX, pupilY, pupilR + (1 * dist), coss, sins);
            secondLine = GetSingleLine(newBmpTbl, pupilX, pupilY, pupilR + (2 * dist), coss, sins);
            thirdLine = GetSingleLine(newBmpTbl, pupilX, pupilY, pupilR + (3 * dist), coss, sins);
            forthLine = GetSingleLine(newBmpTbl, pupilX, pupilY, pupilR + (4 * dist), coss, sins);
            fifthLine = GetSingleLine(newBmpTbl, pupilX, pupilY, pupilR + (5 * dist), coss, sins);
            sixthLine = GetSingleLine(newBmpTbl, pupilX, pupilY, pupilR + (6 * dist), coss, sins);
            seventhLine = GetSingleLine(newBmpTbl, pupilX, pupilY, pupilR + (7 * dist), coss, sins);
            eighthLine = GetSingleLine(newBmpTbl, pupilX, pupilY, pupilR + (8 * dist), coss, sins);
            ;
        }

        internal static void DrawInnerCircle(ByteImage originalBitmapTbl, Tuple<int, int, int> pupCenter)
        {
            int x = pupCenter.Item1;
            int y = pupCenter.Item2;
            int r = pupCenter.Item3;
            try
            {
                originalBitmapTbl.setPixel(x, y, 255, 255, 0, 0);
                originalBitmapTbl.setPixel(x + 1, y, 255, 255, 0, 0);
                originalBitmapTbl.setPixel(x - 1, y, 255, 255, 0, 0);
                originalBitmapTbl.setPixel(x, y + 1, 255, 255, 0, 0);
                originalBitmapTbl.setPixel(x, y - 1, 255, 255, 0, 0);

                for (int t = 0; t < 360; t++)
                {
                    var a = (int)(x - (r * Math.Cos(t * Math.PI / 180)));
                    var b = (int)(y - (r * Math.Sin(t * Math.PI / 180)));
                    if (a >= 0 && b >= 0 && a < originalBitmapTbl.Width && b < originalBitmapTbl.Height)
                    {
                        try
                        {
                            originalBitmapTbl.setPixel(a, b, 255, 0, 0, 255);
                        }
                        catch (Exception e) { };
                    }
                }
            }
            catch (Exception ex) { }
        }

        public static void IrisContrast(ByteImage newBmpTbl)
        {
            PrimitiveContrast pc = new PrimitiveContrast(0, 255, 185, 0);
            for (int x = 0; x < newBmpTbl.Width; x++)
            {
                for (int y = 0; y < newBmpTbl.Height; y++)
                {
                    newBmpTbl.setPixel(x, y, pc.GetColor(newBmpTbl.getPixel(x, y)));
                }
            }
        }

        public static Tuple<int, int, int> Iris(int borderColor, ByteImage bitmap, int px, int py, int pr)
        {
            int threads = 8;
            ByteImage[] pictures = new ByteImage[threads];
            ConcurrentBag<int> Xs = new ConcurrentBag<int>();
            ConcurrentBag<int> Ys = new ConcurrentBag<int>();
            ConcurrentBag<int> Rs = new ConcurrentBag<int>();

            for (int i = 0; i < threads; i++)
            {
                pictures[i] = new ByteImage(bitmap);
            }
            int borderValue = (int)(((double)borderColor / 5) * 3);
            int centerX = px;
            int centerY = py;
            int R = pr;

            int w = 12 / threads;

            Parallel.For(0, threads, i =>
            //for (int i = 0; i < threads; i++)
            {
                int tempX = 0;
                int tempY = 0;
                int tempR = 0;
                for (int x = px - (i * w); x < px + ((i + 1) * w); x++)
                {
                    for (int y = py - 9; y < py + 9; y++)
                    {
                        if (pictures[i].Pixels[pictures[i].getPixelIndex(x, y) + 1] == 0)
                        {
                            int currR = 0;
                            int[] contrasts = FindContrasts((int)pictures[i].Width / 4 + pr, x, y, pictures[i], (pr + 10));
                            for (int c = 0; c < contrasts.Length; c++)
                            {
                                if (contrasts[c] >= borderValue)
                                {
                                    currR = c;
                                    break;
                                }
                            }
                            if (currR > tempR)
                            {
                                tempX = x;
                                tempY = y;
                                tempR = currR;
                            }
                        }
                    }
                }
                Xs.Add(tempX);
                Ys.Add(tempY);
                Rs.Add(tempR);
            });

            for (int i = 0; i < threads; i++)
            {
                if (Rs.ElementAt(i) > R)
                {
                    R = Rs.ElementAt(i);
                    centerX = Xs.ElementAt(i);
                    centerY = Ys.ElementAt(i);
                }
            }
            R = R + pr;
            return new Tuple<int, int, int>(centerX, centerY, R);
        }

        public static int FiveColors(ByteImage newBmpTbl)
        {
            int borderColor = 255;
            int threads = 16;
            ByteImage[] pictures = new ByteImage[threads];
            ConcurrentBag<int> mins = new ConcurrentBag<int>();
            ConcurrentBag<int> maxs = new ConcurrentBag<int>();
            ConcurrentBag<int> sums = new ConcurrentBag<int>();
            ConcurrentBag<int> counters = new ConcurrentBag<int>();

            for (int i = 0; i < threads; i++)
            {
                pictures[i] = new ByteImage(newBmpTbl);
            }

            Parallel.For(0, threads, i =>
            {
                int min = 255;
                int max = 0;
                int totSum = 0;
                int counter = 0;
                int w = (int)pictures[i].Width / threads;

                for (int x = i * w; x < (i + 1) * w; x++)
                {
                    for (int y = 0; y < pictures[i].Height; y++)
                    {
                        var col = pictures[i].Pixels[pictures[i].getPixelIndex(x, y) + 1];
                        if (col > max)
                            max = col;
                        if (col < min)
                            min = col;
                        if (col <= 215)
                        {
                            totSum += col;
                            counter++;
                        }
                    }
                }
                mins.Add(min);
                maxs.Add(max);
                sums.Add(totSum);
                counters.Add(counter);
            });

            int fmin = mins.Min();
            int fmax = maxs.Max();
            int fmid = 0;
            if (counters.Sum() != 0)
            {
                fmid = sums.Sum() / counters.Sum();
            }
            int fmid1 = (fmid + fmin) / 2;
            int fmid2 = (fmax + fmid) / 2;

            borderColor = fmid2;

            for (int x = 0; x < newBmpTbl.Width; x++)
            {
                for (int y = 0; y < newBmpTbl.Height; y++)
                {
                    var col = newBmpTbl.Pixels[newBmpTbl.getPixelIndex(x, y) + 1];

                    int distMin = Math.Abs(col - fmin);
                    int distMid1 = Math.Abs(col - fmid1);
                    int distMid2 = Math.Abs(col - fmid2);
                    int distMax = Math.Abs(col - fmax);

                    int minV = Math.Min(distMin, Math.Min(distMid1, Math.Min(distMid2, distMax)));
                    if (minV == distMin)
                    {
                        newBmpTbl.setPixel(x, y, 255, 0, 0, 0);
                    }
                    else if (minV == distMid1)
                    {
                        newBmpTbl.setPixel(x, y, 255, (byte)fmid1, (byte)fmid1, (byte)fmid1);
                    }
                    else if (minV == distMid2)
                    {
                        newBmpTbl.setPixel(x, y, 255, (byte)fmid2, (byte)fmid2, (byte)fmid2);
                    }
                    else
                    {
                        newBmpTbl.setPixel(x, y, 255, 255, 255, 255);
                    }
                }
            }
            return borderColor;
        }

        public static void RemoveSingleNoises(ByteImage btm)
        {
            ByteImage tempPict = new ByteImage(btm);
            int counter = 0;
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    var oldColour = btm.getPixel(x, y);
                    if (oldColour[1] == 0)
                    {
                        bool isAlone = true;

                        for (int x2 = -1; x2 <= 1; x2++)
                        {
                            for (int y2 = -1; y2 <= 1; y2++)
                            {
                                if (x + x2 >= 0 && x + x2 < btm.Width && y + y2 >= 0 && y + y2 < btm.Height)
                                {
                                    var col = btm.Pixels[btm.getPixelIndex(x + x2, y + y2) + 1];
                                    if (col == 0)
                                    {
                                        isAlone = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (isAlone)
                        {
                            tempPict.setPixel(x, y, 255, 0, 0, 0);
                        }
                    }
                }
            }
            Console.WriteLine(counter + " pixels removed");
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    btm.setPixel(x, y, tempPict.getPixel(x, y));
                }
            }
        }

        public static int ThreeColors(ByteImage newBmpTbl)
        {
            int borderColor = 255;
            int threads = 16;
            ByteImage[] pictures = new ByteImage[threads];
            ConcurrentBag<int> mins = new ConcurrentBag<int>();
            ConcurrentBag<int> maxs = new ConcurrentBag<int>();
            ConcurrentBag<int> sums = new ConcurrentBag<int>();
            ConcurrentBag<int> counters = new ConcurrentBag<int>();

            for (int i = 0; i < threads; i++)
            {
                pictures[i] = new ByteImage(newBmpTbl);
            }


            Parallel.For(0, threads, i =>
            {
                int min = 255;
                int max = 0;
                int totSum = 0;
                int counter = 0;
                int w = (int)pictures[i].Width / threads;

                for (int x = i * w; x < (i + 1) * w; x++)
                {
                    for (int y = 0; y < pictures[i].Height; y++)
                    {
                        var col = pictures[i].getPixel(x, y)[1];
                        if (col > max)
                            max = col;
                        if (col < min)
                            min = col;
                        if (col <= 215)
                        {
                            totSum += col;
                            counter++;
                        }
                    }
                }
                mins.Add(min);
                maxs.Add(max);
                sums.Add(totSum);
                counters.Add(counter);
            });

            int fmin = mins.Min();
            int fmax = maxs.Max();
            int fmid = sums.Sum() / counters.Sum();
            int fmid1 = (fmid + fmin) / 2;
            int fmid2 = (fmax + fmid) / 2;

            borderColor = fmid2;

            for (int x = 0; x < newBmpTbl.Width; x++)
            {
                for (int y = 0; y < newBmpTbl.Height; y++)
                {
                    var col = newBmpTbl.Pixels[newBmpTbl.getPixelIndex(x, y) + 1];

                    int distMin = Math.Abs(col - fmin);
                    int distMid1 = Math.Abs(col - fmid1);
                    int distMid2 = Math.Abs(col - fmid2);
                    int distMax = Math.Abs(col - fmax);

                    int minV = Math.Min(distMin, Math.Min(distMid1, Math.Min(distMid2, distMax)));
                    if (minV == distMin)
                    {
                        newBmpTbl.setPixel(x, y, 255, 0, 0, 0);
                    }
                    else if (minV == distMid1)
                    {
                        newBmpTbl.setPixel(x, y, 255, (byte)fmid2, (byte)fmid2, (byte)fmid2);
                    }
                    else if (minV == distMid2)
                    {
                        newBmpTbl.setPixel(x, y, 255, (byte)fmid2, (byte)fmid2, (byte)fmid2);
                    }
                    else
                    {
                        newBmpTbl.setPixel(x, y, 255, 255, 255, 255);
                    }
                }
            }
            return borderColor;
        }

        public static byte SinglePoints(ByteImage bitmap, int a, int b)
        {
            int sum = 0;
            int counter = 0;
            for (int x2 = -1; x2 < 2; x2++)
            {
                for (int y2 = -1; y2 < 2; y2++)
                {
                    if (a + x2 >= 0 && b + y2 >= 0 && a + x2 < bitmap.Width && b + y2 < bitmap.Height)
                    {
                        sum += bitmap.Pixels[bitmap.getPixelIndex(a + x2, b + y2) + 1];
                        counter++;
                    }
                }
            }
            if (counter > 0)
            {
                return ((byte)(FromInterval(sum / counter)));
            }
            else
            {
                return (255);
            }
        }

        public static byte[] GetSingleLine(ByteImage bitmap, int x, int y, int R, double[] coss, double[] sins)
        {
            int wid = (int)bitmap.Width;
            int hig = (int)bitmap.Height;

            List<byte> pixels = new List<byte>();

            for (int t = 0; t < 359; t++)
            {
                var a = (int)(x - (R * coss[t]));
                var b = (int)(y - (R * sins[t]));
                var a2 = (int)(x - (R * coss[t + 1]));
                var b2 = (int)(y - (R * sins[t + 1]));

                if (Math.Abs(a2 - a) == 0 && Math.Abs(b2 - b) == 0)
                {
                    pixels.Add(SinglePoints(bitmap, a, b));
                }
                else if (a2 - a > b2 - b && Math.Abs(a2 - a) != 0 && Math.Abs(b2 - b) != 0)
                {
                    //bardziej poziomo
                    pixels.AddRange(MoreHorizontal(bitmap, a, b, a2, b2));
                }
                else if (a2 - a <= b2 - b && Math.Abs(a2 - a) != 0 && Math.Abs(b2 - b) != 0)
                {
                    //bardziej pionowo
                    pixels.AddRange(MoreVertical(bitmap, a, b, a2, b2));
                }
                else if (Math.Abs(a2 - a) == 0 && Math.Abs(b2 - b) != 0)
                {
                    //pionowo
                    pixels.AddRange(Vertical(bitmap, a, b, a2, b2));
                }
                else if (Math.Abs(a2 - a) != 0 && Math.Abs(b2 - b) != 0)
                {
                    //poziomo
                    pixels.AddRange(Horizontal(bitmap, a, b, a2, b2));
                }
            }
            return pixels.ToArray();
        }

        private static List<byte> Horizontal(ByteImage bitmap, int a, int b, int a2, int b2)
        {
            List<byte> list = new List<byte>();
            for (int i = -Math.Abs(b2 - b) / 2; i < Math.Abs(b2 - b) / 2; i++)
            {
                int sum = 0;
                int counter = 0;
                for (int x2 = -1; x2 < 2; x2++)
                {
                    for (int y2 = -1; y2 < 2; y2++)
                    {
                        if (a + x2 + i >= 0 && b + y2 >= 0 && a + x2 + i < bitmap.Width && b + y2 < bitmap.Height)
                        {
                            sum += bitmap.Pixels[bitmap.getPixelIndex(a + x2 + i, b + y2) + 1];
                            counter++;
                        }
                    }
                }
                if (counter > 0)
                {
                    list.Add((byte)(FromInterval(sum / counter)));
                }
                else
                {
                    list.Add(255);
                }
            }
            return list;
        }

        private static List<byte> Vertical(ByteImage bitmap, int a, int b, int a2, int b2)
        {
            List<byte> list = new List<byte>();
            for (int i = -Math.Abs(a2 - a) / 2; i < Math.Abs(a2 - a) / 2; i++)
            {
                int sum = 0;
                int counter = 0;
                for (int x2 = -1; x2 < 2; x2++)
                {
                    for (int y2 = -1; y2 < 2; y2++)
                    {
                        if (a + x2 >= 0 && b + y2 + i >= 0 && a + x2 < bitmap.Width && b + y2 + i < bitmap.Height)
                        {
                            sum += bitmap.Pixels[bitmap.getPixelIndex(a + x2, b + y2 + i) + 1];
                            counter++;
                        }
                    }
                }
                if (counter > 0)
                {
                    list.Add((byte)(FromInterval(sum / counter)));
                }
                else
                {
                    list.Add(255);
                }
            }
            return list;
        }

        public static List<byte> MoreVertical(ByteImage bitmap, int a, int b, int a2, int b2)
        {
            var list = new List<byte>();
            for (int i = 0; i < Math.Abs(b2 - b); i++)
            {
                int nY = b + i;
                int nX = (int)(((b2 - ((b2 - b) / (a2 - a))) - nY) * ((a2 - a) / (b2 - b)));

                int sum = 0;
                int counter = 0;
                for (int x2 = -1; x2 < 2; x2++)
                {
                    for (int y2 = -1; y2 < 2; y2++)
                    {
                        if (nX + x2 >= 0 && nY + y2 >= 0 && nX + x2 < bitmap.Width && nY + y2 < bitmap.Height)
                        {
                            sum += bitmap.Pixels[bitmap.getPixelIndex(nX + x2, nY + y2) + 1];
                            counter++;
                        }
                    }
                }
                if (counter > 0)
                {
                    list.Add((byte)(FromInterval(sum / counter)));
                }
                else
                {
                    list.Add(255);
                }
            }
            return list;
        }

        public static List<byte> MoreHorizontal(ByteImage bitmap, int a, int b, int a2, int b2)
        {
            var list = new List<byte>();
            for (int i = 0; i < Math.Abs(a2 - a); i++)
            {
                int nX = a + i;
                int nY = (int)(((b2 - b) / (a2 - a) * nX) + (b2 - ((b2 - b) / (a2 - a) * a2)));

                int sum = 0;
                int counter = 0;
                for (int x2 = -1; x2 < 2; x2++)
                {
                    for (int y2 = -1; y2 < 2; y2++)
                    {
                        if (nX + x2 >= 0 && nY + y2 >= 0 && nX + x2 < bitmap.Width && nY + y2 < bitmap.Height)
                        {
                            sum += bitmap.Pixels[bitmap.getPixelIndex(nX + x2, nY + y2) + 1];
                            counter++;
                        }
                    }
                }
                if (counter > 0)
                {
                    list.Add((byte)(FromInterval(sum / counter)));
                }
                else
                {
                    list.Add(255);
                }
            }
            return list;
        }

        public static int[] FindContrasts(int maxR, int x, int y, ByteImage bitmap, int minR = 0)
        {
            double[] coss = new double[360];
            double[] sins = new double[360];
            for (int t = 0; t < 360; t++)
            {
                coss[t] = Math.Cos(t * Math.PI / 180);
                sins[t] = Math.Sin(t * Math.PI / 180);
            }
            int wid = (int)bitmap.Width;
            int hig = (int)bitmap.Height;
            int[] cList = new int[maxR];
            for (int r = 0; r < maxR; r++)
            {
                int sum = 0;
                int counter = 0;
                for (int t = 0; t < 360; t++)
                {
                    var a = (int)(x - ((r + minR) * coss[t]));
                    var b = (int)(y - ((r + minR) * sins[t]));
                    if (a >= 0 && b >= 0 && a < wid && b < hig)
                    {
                        sum += bitmap.Pixels[bitmap.getPixelIndex(a, b) + 1];
                        counter++;
                    }
                }
                if (counter > 0)
                {
                    cList[r] = sum / counter;
                }
                else
                {
                    cList[r] = 0;
                }
            }

            return cList;
        }

        public static Tuple<int, int, int> PupilCenter(int borderColor, ByteImage bitmap)
        {
            int threads = 16;
            ByteImage[] pictures = new ByteImage[threads];
            ConcurrentBag<int> Xs = new ConcurrentBag<int>();
            ConcurrentBag<int> Ys = new ConcurrentBag<int>();
            ConcurrentBag<int> Rs = new ConcurrentBag<int>();

            for (int i = 0; i < threads; i++)
            {
                pictures[i] = new ByteImage(bitmap);
            }
            int borderValue = (int)((0.85 * (double)borderColor) / 3);
            int centerX = (int)bitmap.Width / 2;
            int centerY = (int)bitmap.Height / 2;
            int R = 1;
            int verticalMargin = (int)bitmap.Height / 4;
            int horizontalMargin = (int)bitmap.Width / 4;

            int w = (int)bitmap.Width / threads;

            Parallel.For(0, threads, i =>
            //for (int i = 0; i < threads; i++)
            {
                int tempX = 0;
                int tempY = 0;
                int tempR = 0;
                for (int x = i * w; x < (i + 1) * w; x++)
                {
                    for (int y = 0; y < pictures[i].Height; y++)
                    {
                        if (x > horizontalMargin && x < pictures[i].Width - horizontalMargin && y > verticalMargin && y < pictures[i].Height - verticalMargin)
                        {
                            if (x == pictures[i].Width / 2 && y == pictures[i].Height / 2)
                            {
                                Console.WriteLine("test");
                            }
                            if (pictures[i].Pixels[pictures[i].getPixelIndex(x, y) + 1] == 0)
                            {
                                int currR = 0;
                                int[] contrasts = FindContrasts((int)pictures[i].Width / 4, x, y, pictures[i]);
                                for (int c = 0; c < contrasts.Length; c++)
                                {
                                    if (contrasts[c] >= borderValue)
                                    {
                                        currR = c;
                                        break;
                                    }
                                }
                                if (currR > tempR)
                                {
                                    tempX = x;
                                    tempY = y;
                                    tempR = currR;
                                }
                            }
                        }
                    }
                }
                Xs.Add(tempX);
                Ys.Add(tempY);
                Rs.Add(tempR);
            });

            for (int i = 0; i < threads; i++)
            {
                if (Rs.ElementAt(i) > R)
                {
                    R = Rs.ElementAt(i);
                    centerX = Xs.ElementAt(i);
                    centerY = Ys.ElementAt(i);
                }
            }

            return new Tuple<int, int, int>(centerX, centerY, R);
        }

        public static void CuttOffIris(ByteImage btm, Tuple<System.Drawing.Point, int> pupil, Tuple<System.Drawing.Point, int> iris)
        {
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    if (IsInsideCircle(pupil, x, y) || !IsInsideCircle(iris, x, y))
                    {
                        btm.setPixel(x, y, 255, 255, 255, 255);
                    }
                }
            }
        }

        public static bool IsInsideCircle(Tuple<System.Drawing.Point, int> circle, int x, int y)
        {
            var dist = Math.Sqrt(((circle.Item1.X - x) * (circle.Item1.X - x)) + ((circle.Item1.Y - y) * (circle.Item1.Y - y)));
            return dist <= circle.Item2;
        }
    }
}