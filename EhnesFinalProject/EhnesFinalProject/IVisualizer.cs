using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EhnesFinalProject
{
    public interface IVisualizer
    {
        //  Start / stop Bass.Net stream
        void Start();
        void Stop();

        //  Used for getting the media player and the rectangle from the controller
        void GetSongPathAndArea(string songPath, Canvas canvas);

        //  Used for beginning set up (place shapes etc)
        void SetUp();

        //  Continually called by timer while music is being played, this is where the magic happens
        void Animate(object sender, EventArgs e);

        //  Called when back button is pressed and thread is destroyed
        void Exit();

        //  Action triggers this to be called by the Visualization page
        void ChangeVolume(double level);

    }
}
