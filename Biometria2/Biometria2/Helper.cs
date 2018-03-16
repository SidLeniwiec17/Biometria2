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

namespace Biometria2
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

        public static void GrayScale(BitmapTable btm)
        {
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    System.Drawing.Color oldColour;
                    oldColour = btm.getPixel(x, y);
                    var value = (0.2 * (double)oldColour.R) + (0.7 * (double)oldColour.G) + (0.1 * (double)oldColour.B);
                    btm.setPixel(x, y, System.Drawing.Color.FromArgb((int)value, (int)value, (int)value));
                }
            }
        }

        public static void Gauss(BitmapTable btm)
        {
            double a = 2;
            double[,] wages = new double[3, 3] { { 1, a, 1 }, { a, a * a, a }, { 1, a, 1 } };
            GaussFilter(btm, a, wages);
        }

        public static void Contrast(BitmapTable btm)
        {
            PrimitiveContrast pc = new PrimitiveContrast();
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    var nCol = pc.GetColor(btm.getPixel(x, y));
                    btm.setPixel(x, y, System.Drawing.Color.FromArgb(nCol.R, nCol.G, nCol.B));
                }
            }
        }

        public static void GaussFilter(BitmapTable btm, double a, double[,] wages)
        {
            BitmapTable tempPict = new BitmapTable(btm);

            for (int x = 0; x < tempPict.Width; x++)
            {
                for (int y = 0; y < tempPict.Height; y++)
                {
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
                                sumR += tempPict.getPixel(x + x2, y + y2).R * currWage;
                                sumG += tempPict.getPixel(x + x2, y + y2).G * currWage;
                                sumB += tempPict.getPixel(x + x2, y + y2).B * currWage;
                                dividor += currWage;
                            }
                        }
                    }
                    btm.setPixel(x, y, System.Drawing.Color.FromArgb(FromInterval((int)(sumR / dividor)), FromInterval((int)(sumG / dividor)), FromInterval((int)(sumB / dividor))));
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

        public static Tuple<int, int, int> PupilCenter(BitmapTable bitmap)
        {
            int threads = 8;

            Tuple<int, int, int> pupil = new Tuple<int, int, int>(0, 0, 0);
            int cX = bitmap.Width / 2;
            int cY = bitmap.Height / 2;
            int minX = cX - bitmap.Width / 5;
            int maxX = cX + bitmap.Width / 5;
            int minY = cY - bitmap.Width / 5;
            int maxY = cY + bitmap.Width / 5;
            int maxR = bitmap.Height > bitmap.Width ? (bitmap.Width / 5) * 2 : (bitmap.Height / 5) * 2;

            List<int[][][]> totalSpectres = new List<int[][][]>();
            int[][][] finalSpectres = new int[maxX - minX][][];
            BitmapTable[] pictures = new BitmapTable[threads];

            for (int i = 0; i < threads; i++)
            {
                pictures[i] = new BitmapTable(bitmap);
                totalSpectres.Add(new int[maxX - minX][][]);
                for (int x = 0; x < maxX - minX; x++)
                {
                    totalSpectres[i][x] = new int[maxY - minY][];
                    if (i == 0)
                    {
                        finalSpectres[x] = new int[maxY - minY][];
                    }
                }
            }

            int w = (maxX - minX) / threads;
            Parallel.For(0, threads, i =>
            {
                for (int x = i * w; x < (i + 1) * w; x++)
                {
                    for (int y = 0; y < maxY - minY; y++)
                    {
                        if (x + minX == 250 && y + minY == 145)
                        {
                            Console.WriteLine("test");
                        }
                        totalSpectres[i][x][y] = FindContrasts(maxR, x + minX, y + minY, pictures[i]);
                    }
                }
            });

            for (int i = 0; i < threads; i++)
            {
                for (int x = 0; x < maxX - minX; x++)
                {
                    for (int y = 0; y < maxY - minY; y++)
                    {
                        if (finalSpectres[x][y] == null)
                        {
                            finalSpectres[x][y] = totalSpectres[i][x][y];
                        }
                    }
                }
            }
            return GetItDone(finalSpectres, bitmap.Width, bitmap.Height, maxR, pictures, threads);
        }

        public static void RunBlack(BitmapTable newBmpTbl)
        {
            int bigCounter = 3;

            while (bigCounter > 2)
            {
                BitmapTable tempPict = new BitmapTable(newBmpTbl);
                bigCounter = 0;
                for (int x = newBmpTbl.Width / 10; x < newBmpTbl.Width - newBmpTbl.Width / 10; x++)
                {
                    for (int y = newBmpTbl.Height / 10; y < newBmpTbl.Height - newBmpTbl.Height / 10; y++)
                    {
                        int counter = 0;
                        for (int x2 = -1; x2 < 2; x2++)
                        {
                            for (int y2 = -1; y2 < 2; y2++)
                            {
                                if (newBmpTbl.getPixel(x, y).R != 0 && x + x2 >= 0 && y + y2 >= 0 && x + x2 < newBmpTbl.Width && y + y2 < newBmpTbl.Height && newBmpTbl.getPixel(x + x2, y + y2).R <= (255 / 6))
                                {
                                    counter++;
                                }
                            }
                        }
                        if (counter >= 5)
                        {
                            tempPict.setPixel(x, y, Color.FromArgb(0, 0, 0));
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

        public static void ThreeColors(BitmapTable newBmpTbl)
        {
            int threads = 8;
            BitmapTable[] pictures = new BitmapTable[threads];
            ConcurrentBag<int> mins = new ConcurrentBag<int>();
            ConcurrentBag<int> maxs = new ConcurrentBag<int>();
            ConcurrentBag<int> sums = new ConcurrentBag<int>();
            ConcurrentBag<int> counters = new ConcurrentBag<int>();

            for (int i = 0; i < threads; i++)
            {
                pictures[i] = new BitmapTable(newBmpTbl);
            }

            Parallel.For(0, threads, i =>
            {
                int min = 255;
                int max = 0;
                int totSum = 0;
                int counter = 0;
                int w = pictures[i].Width / threads;

                for (int x = i * w; x < (i + 1) * w; x++)
                {
                    for (int y = 0; y < pictures[i].Height; y++)
                    {
                        var col = pictures[i].getPixel(x, y).R;
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

            for (int x = 0; x < newBmpTbl.Width; x++)
            {
                for (int y = 0; y < newBmpTbl.Height; y++)
                {
                    var col = newBmpTbl.getPixel(x, y).R;

                    int distMin = Math.Abs(col - fmin);
                    int distMid1 = Math.Abs(col - fmid1);
                    int distMid2 = Math.Abs(col - fmid2);
                    int distMax = Math.Abs(col - fmax);

                    int minV = Math.Min(distMin, Math.Min(distMid1, Math.Min(distMid2, distMax)));
                    if (minV == distMin)
                    {
                        newBmpTbl.setPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                    else if (minV == distMid1)
                    {
                        newBmpTbl.setPixel(x, y, Color.FromArgb(fmid2, fmid2, fmid2));
                    }
                    else if (minV == distMid2)
                    {
                        newBmpTbl.setPixel(x, y, Color.FromArgb(fmid2, fmid2, fmid2));
                    }
                    else
                    {
                        newBmpTbl.setPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                }
            }
        }

        public static int[][] GetEdgesForCircles(int[][][] spectres, int threads)
        {
            int w = spectres[0].Length / threads;
            int maxR = spectres[0][0].Length;

            int[][] edges = new int[spectres[0].Length][];

            Parallel.For(0, threads, j =>
            {
                for (int y = j * w; y < (j + 1) * w; y++)
                {
                    int vedge1 = 0;
                    int vedge2 = 0;
                    for (int x = 0; x < spectres.Length; x++)
                    {
                        var t = Cluster(spectres[x][y]);
                        vedge1 += t.Item1;
                        vedge2 += t.Item2;
                    }
                    vedge1 = vedge1 / maxR;
                    vedge2 = vedge2 / maxR;

                    edges[y] = new int[] { vedge1, vedge2 };
                }
            });

            return edges;
        }

        public static Tuple<int, int, int> GetItDone(int[][][] spectres, int width, int height, int maxR, BitmapTable[] pictures, int threads)
        {
            int maxLeft = width;
            int maxRight = 0;

            int[][] edges = GetEdgesForCircles(spectres, threads);

            ConcurrentBag<int> OutCols = new ConcurrentBag<int>();
            ConcurrentBag<int> MaxRights = new ConcurrentBag<int>();
            ConcurrentBag<int> MaxLefts = new ConcurrentBag<int>();

            int outCol = 0;
            int w = edges.Length / threads;
            int margin = (width / 2) - (edges.Length / 2);
            Parallel.For(0, threads, j =>
            {
                int locOutCol = 0;
                int locMaxRight = 0;
                int locmaxLeft = width;
                //for (int x = i * w; x < (i + 1) * w; x++)
                for (int y = j * w; y < (j + 1) * w; y++)
                {
                    bool el = true;
                    bool er = true;

                    for (int i = 0; i < width / 2; i++)
                    {
                        int vedge2 = edges[y][1];
                        int vedge1 = edges[y][0];

                        var rPix = pictures[j].getPixel(width / 2 + i, y).R;
                        var lPix = pictures[j].getPixel(width / 2 - i, y).R;
                        if (er && Math.Abs(rPix - vedge2) > (0.9 * vedge2))
                        {
                            er = false;
                        }
                        if (er && Math.Abs(rPix - vedge1) < (0.9 * vedge1) && (width / 2 + i) > locMaxRight)
                        {
                            locMaxRight = width / 2 + i;
                            locOutCol = rPix;
                        }
                        if (el && Math.Abs(lPix - vedge2) > (0.9 * vedge2))
                        {
                            el = false;
                        }
                        if (el && Math.Abs(lPix - vedge1) < (0.9 * vedge1) && (width / 2 - i) < locmaxLeft)
                        {
                            locmaxLeft = width / 2 - i;
                        }
                    }
                }

                OutCols.Add(locOutCol);
                MaxRights.Add(locMaxRight);
                MaxLefts.Add(locmaxLeft);
            });

            for (int q = 0; q < threads; q++)
            {
                outCol = OutCols.ElementAt(q) > outCol ? OutCols.ElementAt(q) : outCol;
                maxRight = MaxRights.ElementAt(q) > maxRight ? MaxRights.ElementAt(q) : maxRight;
                maxLeft = MaxLefts.ElementAt(q) < maxLeft ? MaxLefts.ElementAt(q) : maxLeft;
            }

            int CenterX = (maxRight - maxLeft) / 2;
            int R = maxRight - CenterX;
            int maxUp = height;
            int maxDown = 0;

            for (int y = 0; y < height / 2; y++)
            {
                var uPix = pictures[0].getPixel(CenterX, height / 2 - y).R;
                var dPix = pictures[0].getPixel(CenterX, height / 2 + y).R;

                if (Math.Abs(uPix - outCol) < (0.9 * outCol))
                {
                    maxUp = height / 2 - y;
                }

                if (Math.Abs(dPix - outCol) < (0.9 * outCol))
                {
                    maxDown = height / 2 + y;
                }

            }
            int CenterY = (maxDown - maxUp) / 2;

            Console.WriteLine("center: " + CenterX + " , " + CenterY + " radius: " + R);
            return new Tuple<int, int, int>(CenterX, CenterY, R);
        }

        public static int[] FindContrasts(int maxR, int x, int y, BitmapTable bitmap)
        {
            double[] coss = new double[360];
            double[] sins = new double[360];
            for (int t = 0; t < 360; t++)
            {
                coss[t] = Math.Cos(t * Math.PI / 180);
                sins[t] = Math.Sin(t * Math.PI / 180);
            }
            int wid = bitmap.Width;
            int hig = bitmap.Height;
            int[] cList = new int[maxR];
            int minR = 0;
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
                        sum += bitmap.getPixel(a, b).R;
                        counter++;
                    }
                }
                cList[r] = sum / counter;
            }

            return cList;
        }

        private static Tuple<int, int> Cluster(int[] values)
        {
            int min = values.Min();
            int max = values.Max();
            int mid = (min + max) / 2;

            int mins = 0;
            int mids = 0;
            int maxs = 0;
            int minC = 0;
            int midC = 0;
            int maxC = 0;

            int iters = 5;

            for (int i = 0; i < iters; i++)
            {
                mins = 0;
                mids = 0;
                maxs = 0;
                minC = 0;
                midC = 0;
                maxC = 0;

                for (int v = 0; v < values.Length; v++)
                {
                    int distMin = Math.Abs(values[v] - min);
                    int distMid = Math.Abs(values[v] - mid);
                    int distMax = Math.Abs(values[v] - max);

                    int minV = Math.Min(distMin, Math.Min(distMid, distMax));
                    if (minV == distMin)
                    {
                        mins += values[v];
                        minC++;
                    }
                    else if (minV == distMid)
                    {
                        mids += values[v];
                        midC++;
                    }
                    else
                    {
                        maxs += values[v];
                        maxC++;
                    }
                }
                if (minC > 0)
                    min = mins / minC;
                if (midC > 0)
                    mid = mids / midC;
                if (maxC > 0)
                    max = maxs / maxC;
            }
            return new Tuple<int, int>(mid, max);
        }
    }
}
