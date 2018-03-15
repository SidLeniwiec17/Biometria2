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

namespace Biometria2
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

        private async Task LoadImages( BitmapImage btmi, string FileName)
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

        private async void GrayScale_Button(object sender, RoutedEventArgs e)
        {
            if (!(newBmp != null && newBmpTbl != null))
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            await RunGreyScale();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = newBmpTbl.ToBitmapSource();
            Console.WriteLine("Gray Scale.");
        }

        private async void Gauss_Button(object sender, RoutedEventArgs e)
        {
            if (!(newBmp != null && newBmpTbl != null))
            {
                MessageBox.Show("Load image!");
                return;
            }
            BlakWait.Visibility = Visibility.Visible;
            await RunGaussFilter();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = newBmpTbl.ToBitmapSource();
            Console.WriteLine("Gauss Filter.");
        }

        private async void Clear_Button(object sender, RoutedEventArgs e)
        {
            BlakWait.Visibility = Visibility.Visible;
            await ClearPic();
            BlakWait.Visibility = Visibility.Collapsed;
            img.Source = newBmpTbl.ToBitmapSource();
        }

        public async Task ClearPic()
        {
            await Task.Run(() =>
            {
                newBmpTbl = new BitmapTable(originalBitmapTbl);
            });
        }

        public async Task RunGreyScale()
        {
            await Task.Run(() =>
            {
                Helper.GrayScale(newBmpTbl);
            });
        }

        public async Task RunGaussFilter()
        {
            await Task.Run(() =>
            {
                Helper.Gauss(newBmpTbl);
            });
        }
    }
}
