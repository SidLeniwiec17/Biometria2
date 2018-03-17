using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IrisCode
{
    public class PrimitiveContrast
    {
        public System.Windows.Point[] listContrast;

        public PrimitiveContrast(int x1, int y1, int x2, int y2)
        {
            List<System.Windows.Point> temp = new List<System.Windows.Point>();
            System.Windows.Point firstPoint = new System.Windows.Point();
            System.Windows.Point firstMidPoint = new System.Windows.Point();
            System.Windows.Point lastMidPoint = new System.Windows.Point();
            System.Windows.Point lastPoint = new System.Windows.Point();
            firstPoint.X = 0;
            lastPoint.X = 255;
            firstMidPoint.X = x1;
            lastMidPoint.X = x2;
            firstPoint.Y = 255;
            lastPoint.Y = 0;
            firstMidPoint.Y = y1;
            lastMidPoint.Y = y2;
            temp.Add(firstPoint);
            temp.Add(firstMidPoint);
            temp.Add(lastMidPoint);
            temp.Add(lastPoint);
            listContrast = temp.ToArray();
        }

        private int findValue(System.Windows.Point[] lista, int value)
        {
            System.Windows.Point a = new System.Windows.Point();
            System.Windows.Point b = new System.Windows.Point();

            for (int i = 0; i < lista.Length - 1; i++)
            {
                if ((value >= lista[i].X && value <= lista[i + 1].X))
                {
                    a = lista[i];
                    b = lista[i + 1];
                    break;
                }
            }
            
            a.Y = 255 - a.Y;
            b.Y = 255 - b.Y;

            return (int)(((b.Y - a.Y) / (b.X - a.X) * value) + (b.Y - ((b.Y - a.Y) / (b.X - a.X) * b.X)));
        }

        public Color GetColor(Color c)
        {
            int R = findValue(listContrast, c.R);
            int G = findValue(listContrast, c.G);
            int B = findValue(listContrast, c.B);
            
            return Color.FromArgb(FromInterval(R), FromInterval(G), FromInterval(B));
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
