using System;
using System.Collections.Generic;
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

        public static void Binarization(ByteImage btm)
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
                    var value = CastColor( (byte)(Math.Abs((double)oldColour[1] - (double)oldColour[2])));
                    btm.setPixel(x, y, oldColour[0], (byte)value, (byte)value, (byte)value);
                }
            }
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
            int a = 4;
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

        public static void StretchColors(ByteImage btm, int treshold)
        {
            for (int x = 0; x < btm.Width; x++)
            {
                for (int y = 0; y < btm.Height; y++)
                {
                    byte[] oldColour;
                    oldColour = btm.getPixel(x, y);
                    var value = oldColour[1] < treshold ? (byte)(0.6 * (double)oldColour[1]) : (byte)(1.4 * (double)oldColour[1]);
                    btm.setPixel(x, y, oldColour[0], (byte)value, (byte)value, (byte)value);
                }
            }
        }

        public static void ProcessPicture(ByteImage picture)
        {
            int CenterX = 0;
            int CenterY = 0;
            Test(picture);
            int treshold = Blackout(picture);
            LowPassFilter(picture);
            StretchColors(picture, treshold);
        }
    }
}
