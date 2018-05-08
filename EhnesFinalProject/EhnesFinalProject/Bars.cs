using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Un4seen.Bass;

namespace EhnesFinalProject
{
    class Bars : IVisualizer
    {

        Canvas canvas;

        //  To keep track if points have been plotted
        bool hasBegun = false;

        //  This number can be changed, determines how many circles there are
        int numBars = 10;

        //  How much to move to the side
        double step;

        //  Used for getting to know demensions of validity, I get the values in the construtor
        double gridWidth;
        double gridHeight;

        //  Used for the width
        double width = 50;

        //  Used for the music stream
        int stream;

        //file path used to open the stram
        string songPath;

        //  For extracting information from stream 
        float[] buffer;

        //  Dispatcher used for updating the UI
        DispatcherTimer dispatcher = new DispatcherTimer();

        public Bars()
        {

        }

        public void GetSongPathAndArea(string songPath, Canvas canvas)
        {

            this.songPath = songPath;
            this.canvas = canvas;
            this.gridHeight = canvas.Height;
            this.gridWidth = canvas.Width;
            this.step = (gridWidth / numBars) * width;

            dispatcher.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcher.Tick += new EventHandler(Animate);

        }

        public void Restart(object sender, EventArgs e)
        {



        }

        public void SetUp()
        {

            // Now that this has been set to true, this method will not be called again until a new instance of the class is created (new song or leaving page)  
            hasBegun = true;

            Rectangle rectangle;

            //  Coords used for creating Points that will be assigned to each rectangle
            double xCoord;

            double newStep = 0;


            for (int i = 0; i < numBars; i++)
            {

                //  Generating all of the base circles, base properties
                rectangle = new Rectangle();
                rectangle.Width = width;
                rectangle.Height = 0;

                rectangle.Fill = Brushes.Aqua;

                xCoord = newStep;

                Canvas.SetTop(rectangle, 100);
                Canvas.SetLeft(rectangle, newStep);

                newStep += width;

                //  Adding completed base rectangles to the grid
                canvas.Children.Add(rectangle);

            }

            //  Setting up the Bass stream
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            stream = Bass.BASS_StreamCreateFile(songPath, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT);
            if (stream == 0)
            {
                Console.WriteLine(songPath);
                MessageBox.Show("Error opening stream");
                return;
            }

            Bass.BASS_ChannelPlay(stream, false);

        }


        //  will be started / stopped using function calls from VisualizerPage.cs
        public void Start()
        {

            //  Making sure everything is ready
            if (!hasBegun)
            {
                SetUp();
            }

            //  Getting status of stream
            BASSActive status = Bass.BASS_ChannelIsActive(stream);

            //  Stream is not playing
            if (!(status == BASSActive.BASS_ACTIVE_PLAYING))
            {
                Bass.BASS_Start();
            }

            dispatcher.Start();

        }

        public void Stop()
        {

            //  Returns false if pausing failed
            if (!Bass.BASS_Pause())
            {
                Console.WriteLine(Bass.BASS_ErrorGetCode());
            }

            dispatcher.Stop();

        }

        public void Exit()
        {

            //  Freeing the memory
            Bass.BASS_Stop();
            Bass.BASS_StreamFree(stream);
            Bass.BASS_Free();

            dispatcher.Stop();

        }

        public void ChangeVolume(double level)
        {
            Bass.BASS_SetVolume((float)level);
        }

        public void Animate(object sender, EventArgs e)
        {

            //  Getting samples of current spectrum
            int length = (int)Bass.BASS_ChannelSeconds2Bytes(stream, 0.03);
            buffer = new float[length / 2];
            length = Bass.BASS_ChannelGetData(stream, buffer, length);

            int count = 0;

            //  updating each ellipse
            foreach (Rectangle rectangle in canvas.Children)
            {

                rectangle.Height = (Math.Abs(150 * buffer[count]));
                count++;

            }

            canvas.UpdateLayout();

        }
    }
}
