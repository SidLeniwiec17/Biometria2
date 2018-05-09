using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;

namespace VoiceCode
{
    public class Voice
    {
        public float[] Left { get; set; }
        public float[] Right { get; set; }
        public Bitmap VoiceBitmap { get; set; }

        public Voice()
        {

        }

        public float[] GetFloatedSound()
        {
            if (Left != null)
            {
                return Left;
            }
            else if (Right != null)
            {
                return Right;
            }
            else
            {
                return null;
            }
        }

        public bool NotNull()
        {
            if (Left != null)
            {
                return true;
            }
            else if (Right != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void BoostSound()
        {
            if (Left != null)
            {
                for (int i = 0; i < Left.Length; i++)
                {
                    Left[i] = Left[i] * 100f;
                }
            }
            if (Right != null)
            {
                for (int i = 0; i < Right.Length; i++)
                {
                    Right[i] = Right[i] * 100f;
                }
            }
        }

        public void CreateBitmap()
        {
            var data = this.GetFloatedSound();
            var height = (int)(data.Max() + (-data.Min()));
            drawGraph(data, (int)(0.15f * (float)data.Length), (int)(1.4f * (float)height), Color.Blue, Color.White);
        }

        public void drawGraph(float[] data, int width, int height, Color ForeColor, Color BackColor)
        {
            Bitmap bmp = new System.Drawing.Bitmap(width, height,
                                    PixelFormat.Format32bppArgb);
            Rectangle plotArea = new Rectangle(0, 0,
                           width,
                           height);
            using (Graphics g = Graphics.FromImage(bmp))
            using (Pen pen = new Pen(Color.FromArgb(224, ForeColor), 1.75f))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Silver);
                using (SolidBrush brush = new SolidBrush(BackColor))
                    g.FillRectangle(brush, plotArea);
                g.DrawRectangle(Pens.LightGoldenrodYellow, plotArea);

                g.TranslateTransform(plotArea.Left, plotArea.Top);

                g.DrawLine(Pens.White, 0, plotArea.Height / 2,
                       plotArea.Width, plotArea.Height / 2);


                float dataHeight = Math.Max(data.Max(), -data.Min()) * 2;

                float yScale = 1f * plotArea.Height / dataHeight;
                float xScale = 1f * plotArea.Width / data.Length;

               
                g.ScaleTransform(xScale, yScale);
                g.TranslateTransform(0, dataHeight / 2);

                var points = data.ToList().Select((y, x) => new { x, y })
                                 .Select(p => new PointF(p.x, p.y)).ToList();

                g.DrawLines(pen, points.ToArray());

                g.ResetTransform();                
            }
            VoiceBitmap = bmp;
        }               
    }
}
