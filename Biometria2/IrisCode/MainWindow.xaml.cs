using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IrisCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Bitmap newBmp;
        public WriteableBitmap WnewBmp;
        public ByteImage newBmpTbl;

        public Bitmap originalBitmap;
        public WriteableBitmap WoriginalBitmap;
        public ByteImage originalBitmapTbl;

        public Bitmap codeBitmap;
        public WriteableBitmap WcodeBitmap;
        public ByteImage codeBitmapTbl;

        public int borderColor = 255;
        public int borderIrisColor = 255;

        public Bitmap newBmp2;
        public WriteableBitmap WnewBmp2;
        public ByteImage newBmpTbl2;

        public Bitmap originalBitmap2;
        public WriteableBitmap WoriginalBitmap2;
        public ByteImage originalBitmapTbl2;

        public Bitmap codeBitmap2;
        public WriteableBitmap WcodeBitmap2;
        public ByteImage codeBitmapTbl2;

        public int borderColor2 = 255;
        public int borderIrisColor2 = 255;

        public int pupilX = 0;
        public int pupilY = 0;
        public int pupilR = 0;
        public int irisX = 0;
        public int irisY = 0;
        public int irisR = 0;

        public int pupilX2 = 0;
        public int pupilY2 = 0;
        public int pupilR2 = 0;
        public int irisX2 = 0;
        public int irisY2 = 0;
        public int irisR2 = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Load1_Button(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "pic";
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (dialog.ShowDialog() == true)
            {
                BlakWait.Visibility = Visibility.Visible;
                pupilX = 0;
                pupilY = 0;
                pupilR = 0;
                irisX = 0;
                irisY = 0;
                irisR = 0;

                await LoadImages(new BitmapImage(new Uri(dialog.FileName)), dialog.FileName, 0);
                BlakWait.Visibility = Visibility.Collapsed;
            }
        }

        private async void CutOffIris1_Button(object sender, RoutedEventArgs e)
        {
            if (!(newBmp != null && newBmpTbl != null) && !(newBmp2 != null && newBmpTbl2 != null))
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;

            await CutOffIris(0);
            await CutOffIris(1);

            BlakWait.Visibility = Visibility.Collapsed;
            if (newBmpTbl != null)
                img.Source = newBmpTbl.ToBitmapSource();
            if (newBmpTbl2 != null)
                img2.Source = newBmpTbl2.ToBitmapSource();
            Console.WriteLine("Done.");
        }

        private async void Load2_Button(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "pic";
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (dialog.ShowDialog() == true)
            {
                BlakWait.Visibility = Visibility.Visible;
                pupilX2 = 0;
                pupilY2 = 0;
                pupilR2 = 0;
                irisX2 = 0;
                irisY2 = 0;
                irisR2 = 0;

                await LoadImages(new BitmapImage(new Uri(dialog.FileName)), dialog.FileName, 1);
                BlakWait.Visibility = Visibility.Collapsed;
            }
        }

        private void Compare_Button(object sender, RoutedEventArgs e)
        {

        }

        private async void Lines1_Button(object sender, RoutedEventArgs e)
        {
            if ((!(newBmp != null && newBmpTbl != null) && pupilR != 0 && irisR != 0) && !(newBmp2 != null && newBmpTbl2 != null) && pupilR2 != 0 && irisR2 != 0)
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;

            if ((newBmp != null && newBmpTbl != null) && pupilR != 0 && irisR != 0)
            {
                newBmpTbl = new ByteImage(originalBitmapTbl);
                await RunGreyScale(0);
                await DrawLines(0);
                newBmpTbl = new ByteImage(originalBitmapTbl);
                await CutOffStuff(0);
                img.Source = newBmpTbl.ToBitmapSource();
                code.Source = codeBitmapTbl.ToBitmapSource();
            }

            if ((newBmp2 != null && newBmpTbl2 != null) && pupilR2 != 0 && irisR2 != 0)
            {
                newBmpTbl2 = new ByteImage(originalBitmapTbl2);
                await RunGreyScale(1);
                await DrawLines(1);
                newBmpTbl2 = new ByteImage(originalBitmapTbl2);
                await CutOffStuff(1);
                img2.Source = newBmpTbl2.ToBitmapSource();
                code2.Source = codeBitmapTbl2.ToBitmapSource();
            }

            BlakWait.Visibility = Visibility.Collapsed;
            Console.WriteLine("Test.");
        }

        private async Task LoadImages(BitmapImage btmi, string FileName, int mode)
        {
            if (mode == 0)
            {
                img.Source = btmi;
                ori.Source = btmi;

                await Task.Run(() => performLoadingPictures(btmi, FileName, mode));
            }
            else if (mode == 1)
            {
                img2.Source = btmi;
                ori2.Source = btmi;

                await Task.Run(() => performLoadingPictures(btmi, FileName, mode));
            }
        }

        private void performLoadingPictures(BitmapImage btmi, string FileName, int mode)
        {
            if (mode == 0)
            {
                newBmp = (Bitmap)Bitmap.FromFile(FileName);
                var temp = new BitmapImage(new Uri(FileName));
                WnewBmp = new WriteableBitmap(temp);
                newBmpTbl = new ByteImage(WnewBmp, newBmp);

                originalBitmap = (Bitmap)Bitmap.FromFile(FileName);
                WoriginalBitmap = new WriteableBitmap(temp);
                originalBitmapTbl = new ByteImage(WoriginalBitmap, newBmp);

                codeBitmap = (Bitmap)Bitmap.FromFile(FileName);
                WcodeBitmap = new WriteableBitmap(temp);
                codeBitmapTbl = new ByteImage(WoriginalBitmap, newBmp);
            }
            else if (mode == 1)
            {
                newBmp2 = (Bitmap)Bitmap.FromFile(FileName);
                var temp2 = new BitmapImage(new Uri(FileName));
                WnewBmp2 = new WriteableBitmap(temp2);
                newBmpTbl2 = new ByteImage(WnewBmp2, newBmp2);

                originalBitmap2 = (Bitmap)Bitmap.FromFile(FileName);
                WoriginalBitmap2 = new WriteableBitmap(temp2);
                originalBitmapTbl2 = new ByteImage(WoriginalBitmap2, newBmp2);

                codeBitmap2 = (Bitmap)Bitmap.FromFile(FileName);
                WcodeBitmap2 = new WriteableBitmap(temp2);
                codeBitmapTbl2 = new ByteImage(WoriginalBitmap2, newBmp2);
            }
        }

        private async Task CutOffIris(int mode)
        {
            if (mode == 0)
            {
                VERYBAD:
                pupilX = 0;
                pupilY = 0;
                pupilR = 0;
                irisX = 0;
                irisY = 0;
                irisR = 0;
                newBmpTbl = new ByteImage(originalBitmapTbl);

                await RunGreyScale(0);
                await ThreeColors(0);
                for (int i = 0; i < 2; i++)
                {
                    await RunGaussFilter(0);
                    await RunBlack(0);
                }
                await RunRemoveSingleNoises(0);
                await Task.Run(() =>
                {
                    Tuple<int, int, int> PupCenter = Helper.PupilCenter(borderColor, newBmpTbl);
                    pupilX = PupCenter.Item1;
                    pupilY = PupCenter.Item2;
                    pupilR = PupCenter.Item3;
                });

                //--------------
                newBmpTbl = new ByteImage(originalBitmapTbl);
                await RunContrast(0);
                await RunGreyScale(0);
                await RunContrast(0);
                await RunGaussFilter(0);

                await RunBlack(0);
                await FiveColors(0);
                //await RunFullBlack();

                await RunRemoveSingleNoises(0);

                await Task.Run(() =>
                {
                    Tuple<int, int, int> IrisCenter = Helper.Iris(borderIrisColor, newBmpTbl, pupilX, pupilY, pupilR);
                    irisX = IrisCenter.Item1;
                    irisY = IrisCenter.Item2;
                    irisR = IrisCenter.Item3;
                });
                if (irisR == -1)
                {
                    goto VERYBAD;
                }
                newBmpTbl = new ByteImage(originalBitmapTbl);
                await CutOffStuff(0);
                img.Source = newBmpTbl.ToBitmapSource();
            }
            else if(mode == 1)
            {
                VERYBAD2:
                pupilX2 = 0;
                pupilY2 = 0;
                pupilR2 = 0;
                irisX2 = 0;
                irisY2 = 0;
                irisR2 = 0;
                newBmpTbl2 = new ByteImage(originalBitmapTbl2);

                await RunGreyScale(1);
                await ThreeColors(1);
                for (int i = 0; i < 2; i++)
                {
                    await RunGaussFilter(1);
                    await RunBlack(1);
                }
                await RunRemoveSingleNoises(1);
                await Task.Run(() =>
                {
                    Tuple<int, int, int> PupCenter = Helper.PupilCenter(borderColor2, newBmpTbl2);
                    pupilX2 = PupCenter.Item1;
                    pupilY2 = PupCenter.Item2;
                    pupilR2 = PupCenter.Item3;
                });

                //--------------
                newBmpTbl2 = new ByteImage(originalBitmapTbl2);
                await RunContrast(1);
                await RunGreyScale(1);
                await RunContrast(1);
                await RunGaussFilter(1);

                await RunBlack(1);
                await FiveColors(1);
                //await RunFullBlack();

                await RunRemoveSingleNoises(1);

                await Task.Run(() =>
                {
                    Tuple<int, int, int> IrisCenter = Helper.Iris(borderIrisColor2, newBmpTbl2, pupilX2, pupilY2, pupilR2);
                    irisX2 = IrisCenter.Item1;
                    irisY2 = IrisCenter.Item2;
                    irisR2 = IrisCenter.Item3;
                });
                if (irisR2 == -1)
                {
                    goto VERYBAD2;
                }
                newBmpTbl2 = new ByteImage(originalBitmapTbl2);
                await CutOffStuff(1);
                img2.Source = newBmpTbl2.ToBitmapSource();
            }
        }

        public async Task FiveColors(int mode)
        {
            if (mode == 0)
            {
                await Task.Run(() =>
                {
                    borderIrisColor = Helper.FiveColors(newBmpTbl);
                });
            }
            else if (mode == 1)
            {
                await Task.Run(() =>
                {
                    borderIrisColor = Helper.FiveColors(newBmpTbl2);
                });
            }
        }

        public async Task CutOffStuff(int mode)
        {
            if (mode == 0)
            {
                await Task.Run(() =>
                {
                    Helper.CuttOffIris(newBmpTbl, new Tuple<System.Drawing.Point, int>(new System.Drawing.Point(pupilX, pupilY), pupilR + 5), new Tuple<System.Drawing.Point, int>(new System.Drawing.Point(irisX, irisY), irisR - 5));
                });
            }
            else if(mode == 1)
            {
                await Task.Run(() =>
                {
                    Helper.CuttOffIris(newBmpTbl2, new Tuple<System.Drawing.Point, int>(new System.Drawing.Point(pupilX2, pupilY2), pupilR2 + 5), new Tuple<System.Drawing.Point, int>(new System.Drawing.Point(irisX2, irisY2), irisR2 - 5));
                });
            }
        }

        public async Task RunFullBlack()
        {
            await Task.Run(() =>
            {
                Helper.RunFullBlack(newBmpTbl);
            });
        }

        public async Task RunGreyScale(int mode)
        {
            if (mode == 0)
            {
                await Task.Run(() =>
                {
                    Helper.GrayScale(newBmpTbl);
                });
            }
            else if(mode == 1)
            {
                await Task.Run(() =>
                {
                    Helper.GrayScale(newBmpTbl2);
                });
            }
        }

        public async Task ThreeColors(int mode)
        {
            if (mode == 0)
            {
                await Task.Run(() =>
                {
                    borderColor = Helper.ThreeColors(newBmpTbl);
                });
            }
            else if (mode == 1)
            {
                await Task.Run(() =>
                {
                    borderColor2 = Helper.ThreeColors(newBmpTbl2);
                });
            }
        }

        public async Task RunContrast(int mode)
        {
            if (mode == 0)
            {
                await Task.Run(() =>
                {
                    Helper.Contrast(newBmpTbl);
                });
            }
            else if (mode == 1)
            {
                await Task.Run(() =>
                {
                    Helper.Contrast(newBmpTbl2);
                });
            }
        }

        public async Task RunGaussFilter(int mode)
        {
            if (mode == 0)
            {
                await Task.Run(() =>
                {
                    Helper.Gauss(newBmpTbl);
                });
            }
            else if (mode == 1)
            {
                await Task.Run(() =>
                {
                    Helper.Gauss(newBmpTbl2);
                });
            }
        }

        public async Task RunBlack(int mode)
        {
            if (mode == 0)
            {
                await Task.Run(() =>
                {
                    Helper.RunBlack(newBmpTbl);
                });
            }
            else if (mode == 1)
            {
                await Task.Run(() =>
                {
                    Helper.RunBlack(newBmpTbl2);
                });
            }
        }

        public async Task RunRemoveSingleNoises(int mode)
        {
            if (mode == 0)
            {
                await Task.Run(() =>
                {
                    Helper.RemoveSingleNoises(newBmpTbl);
                });
            }
            else if (mode == 1)
            {
                await Task.Run(() =>
                {
                    Helper.RemoveSingleNoises(newBmpTbl2);
                });
            }
        }

        public async Task DrawLines(int mode)
        {
            if (mode == 0)
            {
                await Task.Run(() =>
                {
                    codeBitmapTbl = Helper.DrawLines(newBmpTbl, pupilX, pupilY, pupilR, irisX, irisY, irisR);
                });
            }
            else if(mode == 1)
            {
                await Task.Run(() =>
                {
                    codeBitmapTbl2 = Helper.DrawLines(newBmpTbl2, pupilX2, pupilY2, pupilR2, irisX2, irisY2, irisR2);
                });
            }
        }
    }
}
