using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VoiceCode
{
    /// <summary>
    /// Interaction logic for CostMatrix.xaml
    /// </summary>
    public partial class CostMatrix : Window
    {
        float[][] LocalCost;

        public CostMatrix(float[][] localCost, string title)
        {
            InitializeComponent();
            LocalCost = localCost;
            DrawBitmap();
            this.Title = title;
        }

        public void DrawBitmap()
        {
            Bitmap btm = CreateEmptyBitmap(LocalCost.Length, LocalCost[0].Length);
            ConvertLocalCost(btm.Width, btm.Height);
            float valMax = 0;
            float valMin = 0;

            for (int x = 0; x < LocalCost.Length; x++)
            {
                for (int y = 0; y < LocalCost[x].Length; y++)
                {
                    if ((float)LocalCost[x][y] > valMax)
                    {
                        valMax = (float)LocalCost[x][y];
                    }
                    if ((float)LocalCost[x][y] < valMin)
                    {
                        valMin = (float)LocalCost[x][y];
                    }
                }
            }

            for (int x = 0; x < LocalCost.Length; x++)
            {
                for (int y = 0; y < LocalCost[0].Length; y++)
                {
                    var newval = NormalizeColor(((LocalCost[x][y] - valMin) / (valMax - valMin)) * 255);
                    btm.SetPixel(x, y, System.Drawing.Color.FromArgb((int)newval, (int)newval, (int)newval));
                }
            }
            Console.WriteLine("Creating Local CostGraph...");
            this.Width = btm.Width + 35;
            this.Height = btm.Height + 55;
            localCost.Source = ToBitmapSource(btm);
        }

        public int NormalizeColor(float color)
        {
            if ((int)color <= 0)
            {
                return 0;
            }
            else if ((int)color >= 255)
            {
                return 255;
            }
            else
            {
                return (int)color;
            }
        }

        private void ConvertLocalCost(int width, int height)
        {
            int pixesPerPix = (int)((double)LocalCost.Length / (double)width);
            float[][] newWidth = new float[width][];
            for (int x = 0; x < width; x++)
            {
                newWidth[x] = new float[LocalCost[0].Length];
            }
            int counter = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < newWidth[x].Length; y++)
                {
                    float sum = 0.0f;
                    for (int i = 0; i < pixesPerPix; i++)
                    {
                        sum += LocalCost[counter + i][y];
                    }
                    newWidth[x][y] = sum / (float)pixesPerPix;
                }
                counter += pixesPerPix;
            }
            //now the same with heigh

            pixesPerPix = (int)((double)LocalCost[0].Length / (double)height);
            float[][] newHeight = new float[newWidth.Length][];
            for (int x = 0; x < newHeight.Length; x++)
            {
                newHeight[x] = new float[height];
            }

            for (int x = 0; x < newHeight.Length; x++)
            {
                counter = 0;
                for (int y = 0; y < newHeight[x].Length; y++)
                {
                    float sum = 0.0f;
                    for (int i = 0; i < pixesPerPix; i++)
                    {
                        sum += LocalCost[x][counter + i];
                    }
                    newHeight[x][y] = sum / (float)pixesPerPix;
                    counter += pixesPerPix;
                }
            }
            LocalCost = newHeight;
        }

        public Bitmap CreateEmptyBitmap(int w, int h)
        {
            if (w >= 1200)
            {
                double proportion = (double)h / (double)w;
                int width = 1200;
                int height = (int)(width * proportion);

                Bitmap btm = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                return btm;
            }
            else
            {
                Bitmap btm = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                return btm;
            }
        }

        public System.Windows.Media.ImageSource ToBitmapSource(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
