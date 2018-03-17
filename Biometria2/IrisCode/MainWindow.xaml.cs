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
        public BitmapTable newBmpTbl;
        public Bitmap originalBitmap;
        public BitmapTable originalBitmapTbl;
        public int borderColor = 255;
        public int borderIrisColor = 255;

        public int pupilX = 0;
        public int pupilY = 0;
        public int pupilR = 0;
        public int irisX = 0;
        public int irisY = 0;
        public int irisR = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Load_Button(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "pic";
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (dialog.ShowDialog() == true)
            {
                BlakWait.Visibility = Visibility.Visible;
                await LoadImages(new BitmapImage(new Uri(dialog.FileName)), dialog.FileName);
                BlakWait.Visibility = Visibility.Collapsed;
            }
        }

        private async void CutOffIris_Button(object sender, RoutedEventArgs e)
        {
            if (!(newBmp != null && newBmpTbl != null))
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            await CutOffIris();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = newBmpTbl.ToBitmapSource();
            Console.WriteLine("Done.");
        }

        private async Task LoadImages(BitmapImage btmi, string FileName)
        {
            img.Source = btmi;
            ori.Source = btmi;

            await Task.Run(() => performLoadingPictures(btmi, FileName));
        }

        private void performLoadingPictures(BitmapImage btmi, string FileName)
        {
            newBmp = (Bitmap)Bitmap.FromFile(FileName);
            originalBitmap = (Bitmap)Bitmap.FromFile(FileName);
            newBmpTbl = new BitmapTable(newBmp);
            originalBitmapTbl = new BitmapTable(newBmpTbl);
        }

        private async Task CutOffIris()
        {
            await RunGreyScale();
            await ThreeColors();
            for (int i = 0; i < 4; i++)
            {
                await RunGaussFilter();
                await RunBlack();
            }
            await RunRemoveSingleNoises();
            await Task.Run(() =>
            {
                Tuple<int, int, int> PupCenter = Helper.PupilCenter(borderColor, newBmpTbl);
                pupilX = PupCenter.Item1;
                pupilY = PupCenter.Item2;
                pupilR = PupCenter.Item3;
            });

            //--------------
            newBmpTbl = new BitmapTable(originalBitmapTbl);
            await RunContrast();
            await RunGreyScale();
            await RunContrast();
            await RunGaussFilter();

            await RunBlack();
            await FiveColors();
            await RunFullBlack();

            await RunRemoveSingleNoises();

            await Task.Run(() =>
            {
                Tuple<int, int, int> IrisCenter = Helper.Iris(borderIrisColor, newBmpTbl, pupilX, pupilY, pupilR);
                irisX = IrisCenter.Item1;
                irisY = IrisCenter.Item2;
                irisR = IrisCenter.Item3;
            });
            newBmpTbl = new BitmapTable(originalBitmapTbl);
            await CutOffStuff();
            img.Source = newBmpTbl.ToBitmapSource();
        }

        public async Task FiveColors()
        {
            await Task.Run(() =>
            {
                borderIrisColor = Helper.FiveColors(newBmpTbl);
            });
        }

        public async Task CutOffStuff()
        {
            await Task.Run(() =>
            {
                Helper.CuttOffIris(newBmpTbl, new Tuple<System.Drawing.Point, int>(new System.Drawing.Point(pupilX, pupilY), pupilR + 5), new Tuple<System.Drawing.Point, int>(new System.Drawing.Point(irisX, irisY), irisR - 5));
            });
        }

        public async Task RunFullBlack()
        {
            await Task.Run(() =>
            {
                Helper.RunFullBlack(newBmpTbl);
            });
        }

        public async Task RunGreyScale()
        {
            await Task.Run(() =>
            {
                Helper.GrayScale(newBmpTbl);
            });
        }

        public async Task ThreeColors()
        {
            await Task.Run(() =>
            {
                borderColor = Helper.ThreeColors(newBmpTbl);
            });
        }

        public async Task RunContrast()
        {
            await Task.Run(() =>
            {
                Helper.Contrast(newBmpTbl);
            });
        }

        public async Task RunGaussFilter()
        {
            await Task.Run(() =>
            {
                Helper.Gauss(newBmpTbl);
            });
        }

        public async Task RunBlack()
        {
            await Task.Run(() =>
            {
                Helper.RunBlack(newBmpTbl);
            });
        }

        public async Task RunRemoveSingleNoises()
        {
            await Task.Run(() =>
            {
                Helper.RemoveSingleNoises(newBmpTbl); ;
            });
        }
    }
}
