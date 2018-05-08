using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Un4seen.Bass;

namespace EhnesFinalProject
{

    //  Rotating circles that change color and size

    class Circles : IVisualizer
    {

        Canvas canvas;

        //  To keep track if points have been plotted
        bool hasBegun = false;

        //  This number can be changed, determines how many circles there are
        int numCircles = 15;

        //  This can be changed, determines how fast the circles spin (timer interval)
        double rotationSpeed = 0.0005;

        //  This determines what the circles can not be smaller than
        double minRadius = 12;

        //  Size is used for setting the width and height (default minRadius)
        double size = 75;

        //  Max radius for the large overall circle
        double max = 2 * Math.PI;

        //  How much to rotate per point recalculation, this is not the same as rotation speed
        double step;

        //  Used for color calculations when changing color
        double startHue = 700.0;

        //  Used for getting to know demensions of validity, I get the values in the construtor
        double gridWidth;
        double gridHeight;

        //  Used for the big circles radius
        double radius;

        //  Keeps track of all the circles
        List<Ellipse> ellipses;

        //  Used for the music stream
        int stream;

        //file path used to open the stram
        string songPath;

        //  For extracting information from stream 
        float[] buffer;
             
        //  Dispatcher used for updating the UI
        DispatcherTimer dispatcher = new DispatcherTimer();

        public Circles()
        {
            
        }

        public void GetSongPathAndArea(string songPath, Canvas canvas)
        {

            this.songPath = songPath;
            this.canvas = canvas;
            this.gridHeight = canvas.Height;
            this.gridWidth = canvas.Width;
            this.radius = gridWidth / 2;
            this.step = max / numCircles;

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

            //  Initializing the ellipse list
            ellipses = new List<Ellipse>();

            Ellipse ellipse;

            //  Coords used for creating Points that will be assigned to each ellipse
            double yCoord, xCoord;

            double value = 0;

            for (int i = 0; i < numCircles; i++)
            {

                //  Generating all of the base circles, base properties
                ellipse = new Ellipse();
                ellipse.Width = size;
                ellipse.Height = size;
                ellipse.StrokeThickness = 1;

                ellipse.Stroke = Brushes.HotPink;

                //  Generating the points to form a circle and assigning each point to an ellipse
                value = i * step + 100;

                //  160 and 250 are there just to center it inside the canvas, does not affect the shape
                xCoord = 160 + (radius * Math.Cos(value));
                yCoord = 250 + (radius * Math.Sin(value));

                Canvas.SetTop(ellipse, yCoord);
                Canvas.SetLeft(ellipse, xCoord);

                //  Adding completed base ellipses to the grid
                canvas.Children.Add(ellipse);
                
                //  Adding to list to keep track of them all
                ellipses.Add(ellipse);
                
            }

            //  Setting up the Bass stream
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            stream = Bass.BASS_StreamCreateFile(songPath, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT);
            if (stream == 0)
            {
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

            double yCoord, xCoord;

            double value = 0;
            int count = 0;

            //  Getting samples of current spectrum
            int length = (int)Bass.BASS_ChannelSeconds2Bytes(stream, 0.03);
            buffer = new float[length / 2];
            length = Bass.BASS_ChannelGetData(stream, buffer, length);

            double newRadius = radius + (50 * (double)Math.Abs(buffer[0]));

            //  updating each ellipse
            foreach (Ellipse ellipse in canvas.Children)
            {

                rotationSpeed += .00005;
                value = (count * step) - rotationSpeed;
                ellipse.Width = (Math.Abs(100 * (double)buffer[count]) + size);
                ellipse.Height = (Math.Abs(100 * (double)buffer[count]) + size);
                xCoord = 160 + (newRadius * Math.Cos(value));
                yCoord = 250 + (newRadius * Math.Sin(value));

                Canvas.SetTop(ellipse, yCoord);
                Canvas.SetLeft(ellipse, xCoord);

                count++;

            }

            canvas.UpdateLayout();

        }

    }
}
