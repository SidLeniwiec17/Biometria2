using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Biometria2
{
    public class BitmapTable
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private Color[][] Image;

        public BitmapTable(Bitmap bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            Image = new Color[Width][];

            for (int x = 0; x < Width; x++)
            {
                Image[x] = new Color[Height];
                for (int y = 0; y < Height; y++)
                {
                    Image[x][y] = bitmap.GetPixel(x, y);
                }
            }
        }

        public BitmapTable(BitmapTable bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            Image = new Color[Width][];
            for (int x = 0; x < Width; x++)
            {
                Image[x] = new Color[Height];
                for (int y = 0; y < Height; y++)
                {
                    Image[x][y] = Color.FromArgb(bitmap.Image[x][y].A, bitmap.Image[x][y]);
                }
            }
        }

        public Color getPixel(int x, int y)
        {
            return Image[x][y];
        }

        public void setPixel(int x, int y, Color color)
        {
            Image[x][y] = color;
        }

        public System.Windows.Media.ImageSource ToBitmapSource()
        {
            Bitmap p_bitmap = toBitmap();
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

        public Bitmap toBitmap()
        {
            Bitmap btm = new Bitmap(Width, Height);
            for(int x = 0; x < Width; x ++)
            {
                for(int y =  0; y < Height; y++)
                {
                    btm.SetPixel(x, y, Image[x][y]);
                }
            }
            return btm;
        }
    }
}
