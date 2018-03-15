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
    }
}
