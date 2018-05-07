using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void CreateBitmap()
        {

        }
    }
}
