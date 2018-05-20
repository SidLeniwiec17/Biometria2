using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FaceCode
{
    public class ByteImage
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Stride { get; set; }
        public byte[] Pixels { get; set; }
        public Bitmap Bitmap { get; set; }

        public ByteImage(WriteableBitmap bitmap, Bitmap _bitmap)
        {
            Width = bitmap.PixelWidth;
            Height = bitmap.PixelHeight;
            Stride = bitmap.PixelWidth * 4;
            int size = (Width * Height * 4);
            Pixels = new byte[size];
            bitmap.CopyPixels(Pixels, Stride, 0);
            ReMakePixelsOrder();
            Bitmap = new Bitmap(_bitmap);
        }

        public ByteImage(ByteImage bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            int size = Width * Height * 4;
            Pixels = new byte[size];
            Stride = bitmap.Stride;
            for (int x = 0; x < size; x++)
            {
                Pixels[x] = bitmap.Pixels[x];
            }
            Bitmap = new Bitmap(bitmap.Bitmap);
        }

        public ByteImage(int width, int height)
        {
            Width = width;
            Height = height;
            int size = Width * Height * 4;
            Pixels = new byte[size];
            Stride = Width * 4;
            Bitmap = new Bitmap(width, height);
        }

        public byte[] getPixel(int x, int y)
        {
            int offset = y * Stride + x * 4;
            return new byte[] { Pixels[offset], Pixels[offset + 1], Pixels[offset + 2], Pixels[offset + 3] };
        }

        public int getPixelIndex(int x, int y)
        {
            return y * Stride + x * 4;
        }

        public void setPixel(int x, int y, byte[] color)
        {
            int offset = y * Stride + x * 4;
            Pixels[offset] = color[0];
            Pixels[offset + 1] = color[1];
            Pixels[offset + 2] = color[2];
            Pixels[offset + 3] = color[3];
        }

        public void setPixel(int x, int y, byte a, byte r, byte g, byte b)
        {
            int offset = y * Stride + x * 4;
            Pixels[offset] = a;
            Pixels[offset + 1] = r;
            Pixels[offset + 2] = g;
            Pixels[offset + 3] = b;
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
            Bitmap image = new Bitmap(Bitmap);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    byte[] bits = getPixel(x, y);
                    image.SetPixel(x, y, Color.FromArgb((int)bits[0], (int)bits[1], (int)bits[2], (int)bits[3]));
                }
            }
            return image;
        }

        public void ReMakePixelsOrder()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    byte[] bits = getPixel(x, y);
                    setPixel(x, y, new byte[] { bits[3], bits[2], bits[1], bits[0] });
                }
            }
        }
    }
}