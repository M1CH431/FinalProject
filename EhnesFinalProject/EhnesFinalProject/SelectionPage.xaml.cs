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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EhnesFinalProject
{
    /// <summary>
    /// Interaction logic for SelectionPage.xaml
    /// </summary>
    public partial class SelectionPage : Page
    {

        //  List of paths to songs
        List<string> songList = new List<string>();
        List<IVisualizer> visualizerList;

        public SelectionPage()
        {
            InitializeComponent();
            LoadLists();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new LoginPage());
        }

        private void VisualizeButton_Click(object sender, RoutedEventArgs e)
        {

            //  Checking to make sure that there is a song and visualizer selected
            if(SongListView.SelectedItems.Count == 0 || VisualizerListView.SelectedItems.Count == 0)
            {
                WarningBox.Text = "Please select a song and visualizer";
            }
            else
            {
                this.NavigationService.Navigate(new VisualizePage(songList.ElementAt(SongListView.SelectedIndex), visualizerList.ElementAt(VisualizerListView.SelectedIndex)));
            }
           

        }

        private void LoadLists()
        {

            string path = Directory.GetCurrentDirectory();

            //  Should only return one directory
            string[] songDirectory = Directory.GetDirectories(path, "Music");
            string songPath = songDirectory[0];
            //  Excpetion handling for the case where someone puts a non mp3 file in
            string[] songs = Directory.GetFiles(songPath, "*.mp3");

            //  Getting songs and loading them in as strings so that they are playable by Media Player
            foreach(string song in songs)
            { 

                //  Adding song path to list
                songList.Add(song);

                //  Getting name of file to add to ListView
                var songName = System.IO.Path.GetFileNameWithoutExtension(song);

                //  Adding the name to the list. This way the index of the SongListView and the songList are synced up so I can directly index the songList using the selected index of SongListView
                SongListView.Items.Add(songName);

            }

            //  Adding visualizers
            visualizerList = new List<IVisualizer>();

            visualizerList.Add(new Circles());
            VisualizerListView.Items.Add("Circles");

            visualizerList.Add(new Bars());
            VisualizerListView.Items.Add("Bars");

        }

    }
}
