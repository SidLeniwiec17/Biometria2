using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceCode
{
    public class Helper
    {
        public static bool readWav(string filename, out float[] L, out float[] R)
        {
            L = R = null;
            //float [] left = new float[1];

            //float [] right;
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

                    // chunk 0
                    int chunkID = reader.ReadInt32();
                    int fileSize = reader.ReadInt32();
                    int riffType = reader.ReadInt32();


                    // chunk 1
                    int fmtID = reader.ReadInt32();
                    int fmtSize = reader.ReadInt32(); // bytes for this chunk
                    int fmtCode = reader.ReadInt16();
                    int channels = reader.ReadInt16();
                    int sampleRate = reader.ReadInt32();
                    int byteRate = reader.ReadInt32();
                    int fmtBlockAlign = reader.ReadInt16();
                    int bitDepth = reader.ReadInt16();

                    if (fmtSize == 18)
                    {
                        // Read any extra values
                        int fmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(fmtExtraSize);
                    }

                    // chunk 2
                    int dataID = reader.ReadInt32();
                    int bytes = reader.ReadInt32();

                    // DATA!
                    byte[] byteArray = reader.ReadBytes(bytes);

                    int bytesForSamp = bitDepth / 8;
                    int samps = bytes / bytesForSamp;


                    float[] asFloat = null;
                    switch (bitDepth)
                    {
                        case 64:
                            double[]
                            asDouble = new double[samps];
                            Buffer.BlockCopy(byteArray, 0, asDouble, 0, bytes);
                            asFloat = Array.ConvertAll(asDouble, e => (float)e);
                            break;
                        case 32:
                            asFloat = new float[samps];
                            Buffer.BlockCopy(byteArray, 0, asFloat, 0, bytes);
                            break;
                        case 16:
                            Int16[]
                            asInt16 = new Int16[samps];
                            Buffer.BlockCopy(byteArray, 0, asInt16, 0, bytes);
                            asFloat = Array.ConvertAll(asInt16, e => e / (float)Int16.MaxValue);
                            break;
                        default:
                            return false;
                    }

                    switch (channels)
                    {
                        case 1:
                            L = asFloat;
                            R = null;
                            return true;
                        case 2:
                            L = new float[samps];
                            R = new float[samps];
                            for (int i = 0, s = 0; i < samps; i++)
                            {
                                L[i] = asFloat[s++];
                                R[i] = asFloat[s++];
                            }
                            return true;
                        default:
                            return false;
                    }
                }
            }
            catch
            {
                Console.WriteLine("...Failed to load note: " + filename);
                return false;
                //left = new float[ 1 ]{ 0f };
            }
            return false;
        }

        public static float[] Reverse(float[] voice)
        {
            float[] reverted = new float[voice.Length];
            for(int i = 0; i < voice.Length; i++)
            {
                reverted[i] = voice[voice.Length - i -1];
            }
            return reverted;
        }

        public static Tuple<double, float[][], float[][]> Compare(Voice voice1, Voice voice2)
        {
            double answer = 0.0;
            float[] voiceInColumns = voice1.Simplyfied;
            float[] voiceInRows = Reverse(voice2.Simplyfied);

            //BigMatrixWrapper wrapper = new BigMatrixWrapper();
           // wrapper.InitializeFile(voiceInColumns.Length, voiceInRows.Length);

            float[][] localCost = new float[voiceInColumns.Length][];
            for (int i = 0; i < localCost.Length; i++)
            {
                localCost[i] = new float[voiceInRows.Length];
            }

            for (int x = 0; x < localCost.Length; x++)
            {
                for (int y = 0; y < localCost[x].Length; y++)
                {
                    localCost[x][y] = (float)(Math.Abs(voiceInColumns[x] - voiceInRows[y]));
                }
            }
            Console.WriteLine("Local cost matrix prepared.");


            float[][] globalCost = new float[voiceInColumns.Length][];
            for (int i = 0; i < globalCost.Length; i++)
            {
                globalCost[i] = new float[voiceInRows.Length];
            }


            for (int x = 1; x < globalCost.Length; x++)
            {
                for (int y = 1; y < globalCost[x].Length; y++)
                {
                    globalCost[x][y] = GetGlobalCost(x, y, localCost, globalCost);
                }
            }

            Console.WriteLine("Global cost matrix prepared.");

            float bestPathCost = 0.0f;


            int x2 = globalCost.Length - 1;
            int y2 = globalCost[x2].Length - 1;

            while (x2 > 0 && y2 > 0)
            {
                bestPathCost += globalCost[x2][y2];
                //w lewo
                if (globalCost[x2 - 1][y2] <= globalCost[x2 - 1][y2 - 1] && globalCost[x2 - 1][y2] <= globalCost[x2][y2 - 1])
                {
                    x2 = x2 - 1;
                }
                //w skos
                else if (globalCost[x2 - 1][y2 - 1] <= globalCost[x2][y2 - 1] && globalCost[x2 - 1][y2 - 1] <= globalCost[x2 - 1][y2])
                {
                    x2 = x2 - 1;
                    y2 = y2 - 1;
                }
                //w dół
                else if (globalCost[x2][y2 - 1] <= globalCost[x2 - 1][y2 - 1] && globalCost[x2][y2 - 1] <= globalCost[x2 - 1][y2])
                {
                    y2 = y2 - 1;
                }
            }

            Console.WriteLine("Best Path calculated.");
            float maxPossibleCost = globalCost.Length * (Math.Max(voice1.MaxVal, voice2.MaxVal) - Math.Min(voice2.MinVal, voice1.MinVal));
            float goodCost = maxPossibleCost - bestPathCost;
            answer = (double)(((double)goodCost * 100.0) / ((double)maxPossibleCost));

            Console.WriteLine("Answer calculated.");            

            return new Tuple<double, float[][], float[][]>(Math.Round(answer, 2), localCost, globalCost);
        }

        public static float GetGlobalCost(int x, int y, float[][] localCost, float[][] globalCost)
        {
            float cost = 0.0f;
            // try
            //{
            if (x == 1)
            {
                for (int i = 1; i < localCost[x].Length; i++)
                {
                    cost += localCost[x][i];
                }
            }
            else if (y == 1)
            {
                for (int i = 1; i < localCost.Length; i++)
                {
                    cost += localCost[i][y];
                }
            }
            else
            {
                cost = Math.Min(globalCost[x - 1][y - 1], Math.Min(globalCost[x - 1][y], globalCost[x][y - 1])) + localCost[x][y];
            }
            //    }
            /* catch(Exception ex)
             {
                 Console.WriteLine("Ups...");
             }*/
            return cost;
        }
    }
}
