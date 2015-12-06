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
    /// <summary>
    /// This static class keeps track of time, both experimental time and general time.
    /// </summary>
    static class Timekeeeper
    {
        /// <summary>
        /// Initializes our timekeepers by adding event handlers 
        /// </summary>
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
            General.Stop();
            Experimental.Stop();
            experimental_timer.Stop();
        }

        public static void Reset()
        {
            General.Reset();
            Experimental.Reset();
        }

        /// <summary>
        /// This class is used for non-experimental timing operations.
        /// </summary>
        public static class General
        {
            // starts our stopwatch
            public static void Start()
            {
                watch.Start();
                IsRunning = true;
            }

            // stops our stopwatch
            public static void Stop()
            {
                watch.Stop();
                IsRunning = false;
            }

            // resets our stopwatch
            public static void Reset()
            {
                watch.Reset();
            }

            public static bool IsRunning { get; set; }
            public static double ElapsedSeconds
            {
                get
                {
                    if (watch.IsRunning)
                    {
                        return watch.ElapsedMilliseconds / 1000.0;
                    }
                    else return 0.0;
                }
            }

            static Stopwatch watch = new Stopwatch();
        }

        /// <summary>
        /// Used exclusively for Experimental timing operations. Do not use for anything else.
        /// </summary>
        public static class Experimental
        {

            // starts our stopwatch
            public static void Start()
            {
                watch.Start();
                IsRunning = true;
            }

            // stops our stopwatch
            public static void Stop()
            {
                watch.Stop();
                IsRunning = false;
            }

            // resets our stopwatch
            public static void Reset()
            {
                watch.Reset();
            }

            public static bool IsRunning { get; set; }
            public static double ElapsedSeconds
            {
                get
                {
                    if (watch.IsRunning)
                    {
                        return watch.ElapsedMilliseconds / 1000.0;
                    }
                    else return 0.0;
                }
            }

            static Stopwatch watch = new Stopwatch();
        }

        /// <summary>
        /// Checks to see what the elapsed time is. Experimental time overrides General time.
        /// </summary>
        public static double ElapsedSeconds
        {
            get
            {
                if (Timekeeeper.Experimental.IsRunning)
                {
                    return Timekeeeper.Experimental.ElapsedSeconds;
                }
                else if (Timekeeeper.Experimental.IsRunning == false && Timekeeeper.General.IsRunning == true)
                {
                    return Timekeeeper.General.ElapsedSeconds;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        /// <summary>
        /// Checks to see if any timekeeper is running.
        /// </summary>
        public static bool IsRunning
        {
            get
            {
                if (Timekeeeper.General.IsRunning || Timekeeeper.Experimental.IsRunning)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// The experimental timer tick 
        /// </summary>
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
