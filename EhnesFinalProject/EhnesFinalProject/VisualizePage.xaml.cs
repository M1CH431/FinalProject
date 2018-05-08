using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EhnesFinalProject
{
    /// <summary>
    /// Interaction logic for VisualizePage.xaml
    /// </summary>
    public partial class VisualizePage : Page
    {

        IVisualizer visualizer;
        string songPath;
        string finalSongPath;

        public VisualizePage()
        {
            InitializeComponent();
        }

        //  Constructing using selected song and visualizer
        public VisualizePage(string song, IVisualizer visualizer)
        {
            InitializeComponent();
            this.songPath = song;
            this.visualizer = visualizer;

            //  Setting slider properties
            Volume.Minimum = 0.0;
            Volume.Maximum = 1.0;
            Volume.Value = Volume.Maximum;

            // need to add @"Music\\" becuase the BASS stream used in Circles takes a relative path starting in the executable directory
            finalSongPath = @"Music\\" + System.IO.Path.GetFileName(songPath);

            //  Sending over song path and rectangleL
            visualizer.GetSongPathAndArea(finalSongPath, VisualizationZone);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            visualizer.Exit();
            this.NavigationService.Navigate(new SelectionPage());
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {

            if (PlayPauseButton.Content.Equals("Play"))
            {
                visualizer.Start();
                PlayPauseButton.Content = "Pause";
            }
            else
            {
                visualizer.Stop();
                PlayPauseButton.Content = "Play";
            }

        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            visualizer.ChangeVolume(Volume.Value);
        }
    }
}
