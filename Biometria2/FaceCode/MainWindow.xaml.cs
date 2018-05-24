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

namespace FaceCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool pic1Loaded;
        bool pic1Processed;
        bool pic2Loaded;
        bool pic2Processed;

        public Bitmap pic1OriginalBtm;
        public WriteableBitmap WriteablePic1OriginalBtm;
        public ByteImage pic1OriginalByteImage;

        public Bitmap pic1ProcessedBtm;
        public WriteableBitmap WriteablePic1ProcessedBtm;
        public ByteImage pic1ProcessedByteImage;

        public Bitmap pic2OriginalBtm;
        public WriteableBitmap WriteablePic2OriginalBtm;
        public ByteImage pic2OriginalByteImage;

        public Bitmap pic2ProcessedBtm;
        public WriteableBitmap WriteablePic2ProcessedBtm;
        public ByteImage pic2ProcessedByteImage;

        float[] features1;
        float[] features2;

        public MainWindow()
        {
            InitializeComponent();
            pic1Loaded = false;
            pic1Processed = false;
            pic2Loaded = false;
            pic2Processed = false;
        }

        private async void LoadPicture1_Button(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "pic";
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (dialog.ShowDialog() == true)
            {
                BlakWait.Visibility = Visibility.Visible;
                await LoadImages(new BitmapImage(new Uri(dialog.FileName)), dialog.FileName, 0);
                AnswerLabel.Content = "";
                BlakWait.Visibility = Visibility.Collapsed;
            }
        }

        private async void LoadPicture2_Button(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "pic";
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png;*.bmp|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png";

            if (dialog.ShowDialog() == true)
            {
                BlakWait.Visibility = Visibility.Visible;
                await LoadImages(new BitmapImage(new Uri(dialog.FileName)), dialog.FileName, 1);
                AnswerLabel.Content = "";
                BlakWait.Visibility = Visibility.Collapsed;
            }
        }

        private async void GetFeatures_Button(object sender, RoutedEventArgs e)
        {
            if ((pic1Loaded && !pic1Processed) || (pic2Loaded && !pic2Processed))
            {
                BlakWait.Visibility = Visibility.Visible;
                await GetFeatures();
                BlakWait.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("There is no image to process !");
            }
        }

        private void Compare_Button(object sender, RoutedEventArgs e)
        {
            if(features1?.Length > 0 && features2?.Length > 0)
            {
                float totalSum = 0.0f;
                for (int i = 0; i < features1.Length; i++)
                {
                    totalSum += Math.Abs(features1[i] - features2[i]);
                }
                double answ = (1 - (double)(totalSum / (features2.Length * 1.0f))) * 100.0;
                AnswerLabel.Content = Math.Round(answ,2) + " %";
            }
        }

        private async Task LoadImages(BitmapImage btmi, string FileName, int mode)
        {
            if (mode == 0)
            {
                face1_original.Source = btmi;
                await Task.Run(() => performLoadingPictures(btmi, FileName, 0));
            }
            else if (mode == 1)
            {
                face2_original.Source = btmi;
                await Task.Run(() => performLoadingPictures(btmi, FileName, 1));
            }
        }

        private async Task GetFeatures()
        {
            if (pic1Loaded && !pic1Processed)
            {
                await Task.Run(() =>
                {
                    pic1ProcessedByteImage = new ByteImage(pic1OriginalByteImage);
                    var a = Helper.ProcessPicture(pic1ProcessedByteImage);
                    pic1ProcessedByteImage = a.Item1;
                    features1 = a.Item2;
                    pic1Processed = true;
                });
            }
            if (pic2Loaded && !pic2Processed)
            {
                await Task.Run(() =>
                {
                    pic2ProcessedByteImage = new ByteImage(pic2OriginalByteImage);
                    var b = Helper.ProcessPicture(pic2ProcessedByteImage);
                    pic2ProcessedByteImage = b.Item1;
                    features2 = b.Item2;
                    pic2Processed = true;
                });
            }
            if (pic1ProcessedByteImage != null && pic1Processed)
                face1_processed.Source = pic1ProcessedByteImage.ToBitmapSource();
            if (pic2ProcessedByteImage != null && pic2Processed)
                face2_processed.Source = pic2ProcessedByteImage.ToBitmapSource();
        }

        private void performLoadingPictures(BitmapImage btmi, string FileName, int mode)
        {
            if (mode == 0)
            {
                pic1Loaded = false;
                pic1OriginalBtm = (Bitmap)Bitmap.FromFile(FileName);
                var temp = new BitmapImage(new Uri(FileName));
                WriteablePic1OriginalBtm = new WriteableBitmap(temp);
                pic1OriginalByteImage = new ByteImage(WriteablePic1OriginalBtm, pic1OriginalBtm);

                pic1ProcessedBtm = (Bitmap)Bitmap.FromFile(FileName);
                temp = new BitmapImage(new Uri(FileName));
                WriteablePic1ProcessedBtm = new WriteableBitmap(temp);
                pic1ProcessedByteImage = new ByteImage(WriteablePic1ProcessedBtm, pic1ProcessedBtm);
                pic1Loaded = true;
                pic1Processed = false;
                features1 = null;
            }
            else if (mode == 1)
            {
                pic2Loaded = false;
                pic2OriginalBtm = (Bitmap)Bitmap.FromFile(FileName);
                var temp = new BitmapImage(new Uri(FileName));
                WriteablePic2OriginalBtm = new WriteableBitmap(temp);
                pic2OriginalByteImage = new ByteImage(WriteablePic2OriginalBtm, pic2OriginalBtm);

                pic2ProcessedBtm = (Bitmap)Bitmap.FromFile(FileName);
                temp = new BitmapImage(new Uri(FileName));
                WriteablePic2ProcessedBtm = new WriteableBitmap(temp);
                pic2ProcessedByteImage = new ByteImage(WriteablePic2ProcessedBtm, pic2ProcessedBtm);
                pic2Loaded = true;
                pic2Processed = false;
                features2 = null;
            }
        }
    }
}
