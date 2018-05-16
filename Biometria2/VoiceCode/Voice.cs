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
        public float[] Simplyfied { get; set; }
        public Bitmap VoiceBitmap { get; set; }
        public Bitmap SimpleVoiceBitmap { get; set; }
        public float MaxVal { get; set; }
        public float MinVal { get; set; }

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

        public void Simplyfy()
        {
            int simplyfieSize = 8000;
            float[] original = GetFloatedSound();
            MinVal = original.Min();
            MaxVal = original.Max();
            if (original.Length > simplyfieSize)
            {


                float[] simplyfied = new float[simplyfieSize];
                int pointsToSimplyfy = original.Length / simplyfieSize;

                int counter = 0;
                for (int i = 0; i < simplyfieSize; i++)
                {
                    float sum = 0.0f;
                    for (int y = 0; y < pointsToSimplyfy; y++)
                    {
                        sum += original[counter + y];
                    }
                    counter += pointsToSimplyfy;
                    simplyfied[i] = sum / pointsToSimplyfy;
                }
                float[] originalNoEnd = CutOffEnd(simplyfied);
                float[] originalCutted = CutOffBeggining(originalNoEnd);
                Simplyfied = originalCutted;
            }
            else
            {
                Simplyfied = original;
            }

            MinVal = Simplyfied.Min();
            MaxVal = Simplyfied.Max();
        }

        public float[] CutOffEnd(float[] original)
        {
            int index = original.Length;
            float diff = MaxVal - MinVal;
            float jump = 0.02f * diff;
            for (int x = original.Length - 1; x > 1; x--)
            {
                if (Math.Abs(original[x]) > jump)
                {
                    index = x;
                    break;
                }
            }

            float[] newOriginal = new float[index];
            for (int x = 0; x < newOriginal.Length; x++)
            {
                newOriginal[x] = original[x];
            }
            return newOriginal;
        }

        public float[] CutOffBeggining(float[] original)
        {
            int index = 0;
            float diff = MaxVal - MinVal;
            float jump = 0.02f * diff;
            for (int x = 0; x < original.Length; x++)
            {
                if (Math.Abs(original[x]) > jump)
                {
                    index = x;
                    break;
                }
            }

            float[] newOriginal = new float[original.Length - index];
            for (int x = 0; x < newOriginal.Length; x++)
            {
                newOriginal[x] = original[index + x];
            }
            return newOriginal;
        }

        public void CreateBitmap()
        {
            var data = this.GetFloatedSound();
            var height = (int)(data.Max() + (-data.Min()));
            VoiceBitmap = drawGraph(data, (int)(0.15f * (float)data.Length), (int)(1.4f * (float)height), Color.Blue, Color.White);
        }

        public void CreateSimplyBitmap()
        {
            var data = this.Simplyfied;
            var height = (int)(data.Max() + (-data.Min()));
            SimpleVoiceBitmap = drawGraph(data, (int)(0.15f * (float)data.Length), (int)(1.4f * (float)height), Color.Blue, Color.White);
        }

        public Bitmap drawGraph(float[] data, int width, int height, Color ForeColor, Color BackColor)
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
            return bmp;
        }

        /*public static float[] GetNewLine(float[] oldLine, int newSize)
        {
            float[] newLine = new float[newSize];

            for (int i = 0; i < newLine.Length; i++)
            {
                double percentPosition = (double)i / (double)newLine.Length;
                int middlePos = (int)(percentPosition * (double)oldLine.Length);
                int rightPos = middlePos + 1;
                int leftPos = middlePos - 1;
                if (rightPos >= oldLine.Length)
                {
                    leftPos = oldLine.Length - 3;
                    rightPos = oldLine.Length - 1;
                    middlePos = oldLine.Length - 2;
                }
                if (leftPos < 0)
                {
                    leftPos = 0;
                    rightPos = 2;
                    middlePos = 1;
                }

                float[] factors = GetParabolaFactors(leftPos, oldLine[leftPos], middlePos, oldLine[middlePos], rightPos, oldLine[rightPos]);

                float newVal = (float)GetInterpolatedValue(factors, percentPosition * (double)oldLine.Length);
                newLine[i] = newVal;
            }
            return newLine;
        }

        public static float[] GetParabolaFactors(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float[] factors = new float[3];

            var denom = (x1 - x2) * (x1 - x3) * (x2 - x3);
            factors[0] = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
            factors[1] = (x3 * x3 * (y1 - y2) + x2 * x2 * (y3 - y1) + x1 * x1 * (y2 - y3)) / denom;
            factors[2] = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;

            return factors;
        }

        public static double GetInterpolatedValue(float[] factors, double x)
        {
            double val = (factors[0] * (x * x)) + (factors[1] * x) + factors[2];
            return val;
        }*/
    }
}
