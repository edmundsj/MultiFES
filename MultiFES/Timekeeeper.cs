using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace CSharpProject
{
    static class Timekeeeper
    {

        public static void Initialize()
        {
            if (initialized == false)
            {
                experimental_timer.Interval = 10;
                experimental_timer.Tick += new EventHandler(experimental_timer_Tick);
                initialized = true;
            }
        }
        public static void Stop()
        {
            Initialize();
            General.Stop();
            Experimental.Stop();
            experimental_timer.Stop();
        }

        public static void Reset()
        {
            Initialize();
            General.Reset();
            Experimental.Reset();
        }
        public static class General
        {
            // starts our stopwatch
            public static void Start()
            {
                watch.Start();
                Running = true;
            }

            // stops our stopwatch
            public static void Stop()
            {
                watch.Stop();
                Running = false;
            }

            // resets our stopwatch
            public static void Reset()
            {
                watch.Reset();
            }

            public static bool Running { get; set; }
            public static double ElapsedSeconds
            {
                get
                {
                    return watch.ElapsedMilliseconds / 1000.0;
                }
            }

            static Stopwatch watch = new Stopwatch();
        }
        public static class Experimental
        {

            // starts our stopwatch
            public static void Start()
            {
                watch.Start();
                Running = true;
            }

            // stops our stopwatch
            public static void Stop()
            {
                watch.Stop();
                Running = false;
            }

            // resets our stopwatch
            public static void Reset()
            {
                watch.Reset();
            }

            public static bool Running { get; set; }
            public static double ElapsedSeconds
            {
                get
                {
                    return watch.ElapsedMilliseconds / 1000.0;
                }
            }

            static Stopwatch watch = new Stopwatch();
        }

        public static double ElapsedSeconds
        {
            get
            {
                if (Timekeeeper.Experimental.Running)
                {
                    return Timekeeeper.Experimental.ElapsedSeconds;
                }
                else if (Timekeeeper.Experimental.Running == false && Timekeeeper.General.Running == true)
                {
                    return Timekeeeper.General.ElapsedSeconds;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public static bool IsRunning
        {
            get
            {
                if (Timekeeeper.General.Running || Timekeeeper.Experimental.Running)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // this is our Settings.Experimental timer. We use it to determine whether we should rotate or not.
        private static void experimental_timer_Tick(object sender, EventArgs e)
        {
            if (Settings.Experimental.CurrentType == Settings.Experimental.ExperimentType.MultiChannel)
            {
                if (Timekeeeper.ElapsedSeconds > (Settings.Experimental.Interval +
                    Settings.Experimental.Interval * Data.Experimental.Rotations))
                {
                    Data.Experimental.Rotations++;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Nodes.rotateNodesAsync));
                }

            }

            if (Timekeeeper.ElapsedSeconds > Settings.Experimental.Duration)
            {
                Stop();
                Comms.Close();
                // write our MVC data to file.
                ThreadPool.QueueUserWorkItem(new WaitCallback(Data.Experimental.ForceData.writeToFile), "temp_mvc-data");

                // write our amplitude data to file.
                for (int i = 0; i < Data.Experimental.Amplitudes.Count; i++)
                {
                    String str = "temp_CH-" + i.ToString() + "_amplitude_data";
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Data.Experimental.Amplitudes[i].writeToFile), str);
                }
                MessageBox.Show("Experiment Concluded. Data now being saved to temporary storage . . .");

            }

        }
        static System.Windows.Forms.Timer experimental_timer = new System.Windows.Forms.Timer();
        static bool initialized;
    }
}
