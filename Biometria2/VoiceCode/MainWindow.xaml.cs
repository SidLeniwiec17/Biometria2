using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace VoiceCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Voice voice1;
        public Voice voice2;

        public MainWindow()
        {
            InitializeComponent();
            voice1 = new Voice();
            voice2 = new Voice();
        }

        private async void Load1_Button(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "audio";
            dialog.Filter = "Wav files |*.wav;";

            if (dialog.ShowDialog() == true)
            {
                BlakWait.Visibility = Visibility.Visible;
                await LoadAudio1(dialog.FileName);
                BlakWait.Visibility = Visibility.Collapsed;
            }
        }

        private async void Load2_Button(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Title = "audio";
            dialog.Filter = "Wav files |*.wav;";

            if (dialog.ShowDialog() == true)
            {
                BlakWait.Visibility = Visibility.Visible;
                await LoadAudio2(dialog.FileName);
                BlakWait.Visibility = Visibility.Collapsed;
            }
        }

        private async void Action_Button(object sender, RoutedEventArgs e)
        {
            if(voice1.NotNull() && voice2.NotNull())
            {
                BlakWait.Visibility = Visibility.Visible;
                await Compare();
                BlakWait.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("At least one voice record is not loaded !");
            }
        }

        private async Task LoadAudio1(string fileName)
        {
            await Task.Run(() =>
            {
                float[] tempLeft = null;
                float[] tempRight = null;
                if (Helper.readWav(fileName, out tempLeft, out tempRight))
                {
                    voice1.Left = tempLeft != null ? tempLeft : null;
                    voice1.Right = tempRight != null ? tempRight : null;
                }
                else
                {
                    MessageBox.Show("Cannot load voice record !");
                }
            });
        }

        private async Task LoadAudio2(string fileName)
        {
            await Task.Run(() =>
            {
                float[] tempLeft = null;
                float[] tempRight = null;
                if (Helper.readWav(fileName, out tempLeft, out tempRight))
                {
                    voice2.Left = tempLeft != null ? tempLeft : null;
                    voice2.Right = tempRight != null ? tempRight : null;
                }
                else
                {
                    MessageBox.Show("Cannot load voice record !");
                }
            });
        }

        private async Task Compare()
        {
            await Task.Run(() =>
            {

            });
        }
    }
}
