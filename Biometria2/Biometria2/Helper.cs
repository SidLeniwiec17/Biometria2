using System;
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
            return pupil;
        }

        public void GetItDone(int[][][] spectres, int width, int height, int maxR, BitmapTable bitmap)
        {
            int maxLeft = width;
            int maxRight = 0;
            for (int y = 0; y < height; y++)
            {

                int edge1 = 0;
                int edge2 = 0;
                for (int x = 0; x < width; x++)
                {
                    var t = Cluster(spectres[x][y]);
                    edge1 += t.Item1;
                    edge2 += t.Item2;
                }
                edge1 = edge1 / maxR;
                bool el = true;
                bool er = true;

                edge2 = edge2 / maxR;
                for (int i = 0; i < maxR; i++)
                {
                    if (er && Math.Abs(bitmap.getPixel(width / 2 + i, y).R - edge2) > (0.9 * edge2))
                    {
                        er = false;
                    }
                    if (er && Math.Abs(bitmap.getPixel(width/2 + i , y).R - edge1) < (0.9 * edge1) && (width / 2 + i) > maxRight)
                    {
                        maxRight = width / 2 + i;
                    }
                    if (el && Math.Abs(bitmap.getPixel(width / 2 - i, y).R - edge2) > (0.9 * edge2))
                    {
                        el = false;
                    }
                    if (el && Math.Abs(bitmap.getPixel(width / 2 - i, y).R - edge1) < (0.9 * edge1) && (width / 2 - i) < maxLeft)
                    {
                        maxRight = width / 2 + i;
                    }
                }
            }

            int CenterX = (maxRight - maxLeft) / 2;
            int R = maxRight - CenterX;
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

        private Tuple<int, int> Cluster(int[] values)
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
                min = mins / minC;
                mid = mids / midC;
                max = maxs / maxC;
            }
            return new Tuple<int, int>(mid, max);
        }
    }
}
