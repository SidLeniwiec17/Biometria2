using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceCode
{
    public class BigMatrixWrapper
    {
        public string FilePath { get; set; }

        public BigMatrixWrapper()
        {
            string path = Directory.GetCurrentDirectory();
            string fileName = Path.Combine(path, "matrix.txt");
            FilePath = fileName;
        }

        public void CreateFile()
        {
            string path = Directory.GetCurrentDirectory();
            string fileName = Path.Combine(path, "matrix.txt");
            FilePath = fileName;

            if ((!File.Exists(FilePath))) //Checking if scores.txt exists or not
            {
                FileStream fs = File.Create(FilePath); //Creates Scores.txt
                fs.Close(); //Closes file stream
            }
        }

        public void InitializeFile(int width, int height)
        {
            float[][] matrix = new float[width][];
            for(int x = 0; x < width; x++)
            {
                matrix[x] = new float[height];
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    matrix[x][y] = 0.0f;
                }
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath, true))
            {
                for(int i = 0; i < height; i++)
                file.WriteLine(ArrayToString(matrix, i));
            }
        }

        public string ArrayToString(float[][] array, int lineIndex)
        {
            string line = "";
            for(int i = 0; i < array.Length; i++)
            {
                line += array[i][lineIndex].ToString();
                if(i < array.Length -1)
                {
                    line += ";";
                }
            }
            return line;
        }

        public float[] RowFromFile(int row)
        {
            string line = File.ReadLines(FilePath).Skip(row).Take(1).First();
            string[] splitted = line.Split(';');
            float[] floats = new float[splitted.Length];
            for(int i = 0; i < floats.Length; i++)
            {
                floats[i] = float.Parse(splitted[i]);
            }
            return floats;
        }


        public float FloatFromFile(int x, int y)
        {
            string line = File.ReadLines(FilePath).Skip(y).Take(1).First();
            string[] splitted = line.Split(';');
            
            return float.Parse(splitted[x]);
        }
    }
}
