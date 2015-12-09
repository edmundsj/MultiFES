using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MultiFES
{
    /// <summary>
    /// This static class keeps track of time, both experimental time and general time.
    /// </summary>
    public static class Timekeeeper
    {
        /// <summary>
        /// Initializes our timekeepers by adding event handlers 
        /// </summary>
        public static void Initialize()
        {
            if (initialized == false)
            {
                experimental_timer.Interval = 10;
                experimental_timer.Elapsed += new System.Timers.ElapsedEventHandler(experimental_timer_Tick);
                initialized = true;
            }
        }
        /// <summary>
        /// Stops our General and Experimental stopwatches
        /// </summary>
        public static void Stop()
        {
            General.Stop();
            Experimental.Stop();
            experimental_timer.Stop();
        }

        /// <summary>
        /// Resets our General and Experimental stopwatches
        /// </summary>
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
            /// <summary>
            /// Resets and starts our General stopwatch
            /// </summary>
            public static void Start()
            {
                watch.Reset();
                watch.Start();
                is_running = true;
            }

            /// <summary>
            /// Stops our General stopwatch
            /// </summary>
            public static void Stop()
            {
                watch.Stop();
                is_running = false;
            }

            /// <summary>
            /// Resets our General stopwatch
            /// </summary>
            public static void Reset()
            {
                watch.Reset();
                Data.General.Clear();
            }

            static bool is_running;
            /// <summary>
            /// checks whether our General stopwatch is running.
            /// </summary>
            public static bool IsRunning
            {
                get
                {
                    return is_running;
                }
            }

            /// <summary>
            /// Gets the elapsed seconds of our Experimental Timekeeper
            /// </summary>
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

            /// <summary>
            /// Resets then starts our Experimental stopwatch
            /// </summary>
            public static void Start()
            {
                watch.Reset();
                watch.Start();
                experimental_timer.Enabled = true;
                experimental_timer.Start();
                is_running = true;
            }

            /// <summary>
            /// Stops our Experimental stopwatch
            /// </summary>
            public static void Stop()
            {
                watch.Stop();
                is_running = false;
                experimental_timer.Stop();
                experimental_timer.Enabled = false;
            }

            /// <summary>
            /// Resets our Experimental stopwatch
            /// </summary>
            public static void Reset()
            {
                watch.Reset();
            }

            static bool is_running;
            /// <summary>
            /// Checks whether the Experimental stopwatch is running or not.
            /// </summary>
            public static bool IsRunning {
                get
                {
                    return is_running;
                }
            }
            /// <summary>
            /// The number of elapsed seconds for our experimental Timekeeper
            /// </summary>
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
        /// This contains all the necessary information for the Autorun() function
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
                Timekeeeper.Stop();
                Comms.Close();
                // write our MVC data to file.
                int force_result = Data.Experimental.ForceData.writeToFile("temp_mvc-data_");  // write the file
                int amplitude_result = 0;
                // write our amplitude data to file.
                for (int i = 0; i < Data.Experimental.Amplitudes.Count; i++)
                {
                    int temp_result = Data.Experimental.Amplitudes[i].writeToFile("temp_CH-"
                        + i.ToString() + "_amplitude-data_");
                    if (temp_result != amplitude_result && amplitude_result == 0)
                    {
                        amplitude_result = temp_result;
                    }
                }
                if (force_result == 0 && amplitude_result == 0)
                {
                    MessageBox.Show("Successfully wrote all data to file.",
                        "Data Write Successful.", MessageBoxButtons.OK,
                       MessageBoxIcon.Information);
                }
                else
                {
                    String result = "There was an error writing the ";
                    if (force_result != 0) result += "force data: Exit code " + force_result.ToString() + ",";
                    if (amplitude_result != 0) result += " amplitude data: Exit code " + amplitude_result.ToString();

                    MessageBox.Show(result, "Data write Failure", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }

        }
        static System.Timers.Timer experimental_timer = new System.Timers.Timer();
        static bool initialized;
    }
}
