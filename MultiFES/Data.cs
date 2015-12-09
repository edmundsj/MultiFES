using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MultiFES
{

    /// <summary>
    /// The class responsible for handling all data collected during stimulation.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Adds force data to either our experimental or our general data stores.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="val"></param>
        public static void AddData(double time, double val)
        {
            if (Timekeeeper.Experimental.IsRunning)
            {
                Data.Experimental.ForceData.addData(time, val);
            }
            if(Timekeeeper.General.IsRunning)
            {
                Data.General.ForceData.addData(time, val);
            }
        }

        /// <summary>
        /// Adds force data to either our experimental or our general data stores.
        /// </summary>
        /// <param name="times">The list of times to add</param>
        /// <param name="vals">The list of values to add</param>
        public static void AddData(List<double> times, List<double> vals)
        {
            if(Timekeeeper.Experimental.IsRunning)
            {
                Data.Experimental.ForceData.addData(times, vals);
            }
            if(Timekeeeper.General.IsRunning)
            {
                Data.General.ForceData.addData(times, vals);
            }
        }

        /// <summary>
        /// Adds force data to either our experimental or our general data stores.
        /// </summary>
        /// <param name="c">The capsule containing data to add</param>
        public static void AddData(Capsule c)
        {
            if(Timekeeeper.Experimental.IsRunning)
            {
                Data.Experimental.ForceData.addData(c);
            }
            if(Timekeeeper.General.IsRunning)
            {
                Data.General.ForceData.addData(c);
            }
        }

        /// <summary>
        /// Our data structure for holding force data. Essentially a 2-column table that holds
        /// timestamps and values for those timestamps.
        /// </summary>
        public class Capsule
        {
            /// <summary>
            ///  Constructs an empty capsule.
            /// </summary>
            public Capsule()
            {

            }

            /// <summary>
            /// Constructs a capsule with the contents of the lists passed in.
            /// Note: DOES NOT create a copy of these items, shares them with
            /// wherever you passed them in from, if that place exists.
            /// </summary>
            /// <param name="times">The list of timestamps</param>
            /// <param name="vals">The list of values</param>
            public Capsule(List<double> times, List<double> vals)
            {
                if (times.Count == vals.Count)
                {
                    this.Timestamps = times;
                    this.Values = vals;
                }
            }

            /// <summary>
            /// Clears all the contents from a capsule and resets the accessor.
            /// </summary>
            public virtual void Clear()
            {
                Timestamps.Clear();
                Values.Clear();
                last_accessed = 0;
            }

            /// <summary>
            /// Normalizes the data to a percentage of the user's maximum force.
            /// </summary>
            public virtual void Normalize()
            {
                // only proceed if it was not already normalized.
                if (this.is_normalized == false)
                {
                    for (int i = 0; i < this.Count; i++)
                    {
                        // only normalize if we have a valid normalization value.
                        if (Data.Experimental.MaxForce > 0)
                        {
                            this.Values[i] /= Data.Experimental.MaxForce; // divide by our baseline force
                        }
                    }
                
                    this.is_normalized = true;
                }
            }

            /// <summary>
            /// Removes
            /// </summary>
            public virtual void Clean() // cleans up our data
            {
                for(int i = this.Count - 1; i >= 0; i--)
                {

                    // Our cleaning conditions for normalized data
                    if(is_normalized) {
                        if(this.Values[i] < -0.05)
                        {
                            this.RemoveAt(i);
                        }
                        if(this.Values[i] > 1.1)
                        {
                            this.RemoveAt(i);
                        }
                        
                    }
                    // our cleaning conditions for non-normalized data.
                    else
                    {
                        if (this.Values[i] < -10)
                        {
                            this.RemoveAt(i);
                        }
                        if(this.Values[i] > 200)
                        {
                            this.RemoveAt(i);
                        }
                    }
                }
            }

            /// <summary>
            /// Safely adds data to a capsule.
            /// </summary>
            /// <param name="timestamp">The current time</param>
            /// <param name="value">The value at that time</param>
            public void addData(double timestamp, double value)
            {
                // first off, only add the data if our Timekeeper is running
                // first check if the timestamp is valid
                if (timestamp >= 0)
                {
                    if (value > this.minimum_value) // set the maximum value
                    {
                        this.maximum_value = value;
                    }
                    if (value < this.minimum_value) // set the minimum value
                    {
                        this.minimum_value = value;
                    }
                    Timestamps.Add(timestamp);
                    Values.Add(value);

                }
                else
                {
                    throw new Exception("Defined Error, addData(): Timestamp cannot be negative.");
                }
            }

            /// <summary>
            /// Allows us to safely add data from an existing capsule to this one.
            /// </summary>
            /// <param name="c">The capsule whose data you want to add</param>
            public void addData(Capsule c)
            {
                if (c.IsValid)
                {
                    for (int i = 0; i < c.Count; i++)
                    {
                        this.addData(c.Timestamps[i], c.Values[i]);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="times">A list of times to add</param>
            /// <param name="vals">A list of values to add</param>
            public void addData(List<double> times, List<double> vals)
            {
                if (times.Count == vals.Count)
                {
                    for (int i = 0; i < times.Count; i++)
                    {
                        this.addData(times[i], vals[i]);
                    }
                }
            }
            
            /// <summary>
            /// Trims a specific number of data points off the beginning or end 
            /// of the capsule in question.
            /// </summary>
            /// <param name="number_to_trim">The number of points to remove</param>
            /// <param name="start_point">the TrimPoint value (old data or new data)</param>
            public void Trim(int number_to_trim, TrimPoint start_point)
            {
                bool removed_minimum = false;
                bool removed_maximum = false;
                if (number_to_trim > this.Count)
                {
                    number_to_trim = this.Count;
                }


                if (start_point == TrimPoint.OLD)
                {
                    for (int i = 0; i < number_to_trim; i++)
                    {
                        if (this.Values[0] >= this.maximum_value)
                        {
                            removed_maximum = true;
                        }
                        if (this.Values[0] <= this.minimum_value)
                        {
                            removed_minimum = true;
                        }
                        this.RemoveAt(0);
                    }
                }

                else if (start_point == TrimPoint.NEW)
                {
                    for (int i = this.Count - 1; i > this.Count - 1 - number_to_trim; i--)
                    {
                        // if we are removing a minimum or maximim value, find the new minimum or maximum
                        if (this.Values[i] == this.maximum_value)
                        {
                            removed_maximum = true;
                        }
                        if (this.Values[i] == this.minimum_value)
                        {
                            removed_minimum = true;
                        }
                        this.RemoveAt(i);
                    }
                }
                // if we removed a minimum or maximum value find the new one
                if (removed_minimum == true)
                {
                    // find our new minimum
                    double new_minimum = 10e12;
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this.Values[i] <= new_minimum)
                        {
                            new_minimum = this.Values[i];
                        }
                    }
                    this.minimum_value = new_minimum;
                }
                if (removed_maximum == true)
                {
                    // find our new maximum
                    double new_maximum = 0;
                    for (int i = 0; i < this.Count; i++)
                    {
                        if (this.Values[i] >= new_maximum)
                        {
                            new_maximum = this.Values[i];
                        }
                    }
                    this.minimum_value = new_maximum;
                }
            }

            /// <summary>
            /// Safely removes data at a given index from our capsule
            /// </summary>
            /// <param name="index">The zero-based index of the data point to remove</param>
            public void RemoveAt(int index)
            {
                if (index < this.Count && index >= 0)
                {
                    this.Timestamps.RemoveAt(index);
                    this.Values.RemoveAt(index);
                }
            }

            /// <summary>
            /// Writes capsule data to file.
            /// </summary>
            /// <param name="filename"></param>
            /// <returns>0 for success, 1 for no data to write, 2 for data not successfully added, 3 for file in use</returns>
            public virtual int writeToFile(String filename)
            {
                DateTime t = DateTime.Now;
                String filename_ending = filename;
                filename_ending += "_";
                filename_ending += t.Month.ToString() + "-";
                filename_ending += (t.Day.ToString()) + "_";
                filename_ending += t.Hour.ToString() + "_";
                filename_ending += t.Minute.ToString() + ".csv";

                filename = Path.Combine(Settings.DataPath, filename_ending);

                String d = "";
                if(this.Timestamps.Count == 0)
                {
                    return 1; // no data to write
                }
                for (int i = 0; i < this.Timestamps.Count; i++)
                {
                    d += Convert.ToString(this.Timestamps[i]) + ",";
                    if (this.Count >= i)
                    {
                        d += Convert.ToString(this.Values[i]) + "\n";
                    }
                }
                try
                {
                    StreamWriter sw = new StreamWriter(filename);
                    sw.Write(d);
                    sw.Close();
                    if(d.Length == 0)
                    {
                        return 2; // there was data to write, but it did not get added
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (IOException)
                {
                    return 3;
                }
            }


            /// <summary>
            /// Writes capsule data to file, can be called as a background worker in the thread pool.
            /// </summary>
            /// <param name="obj"></param>
            public virtual void writeToFile(System.Object obj)
            {
                String filename = (String)obj;
                DateTime t = DateTime.Now;
                String filename_ending = filename;
                filename_ending += "_";
                filename_ending += t.Month.ToString() + "-";
                filename_ending += (t.Day.ToString()) + "_";
                filename_ending += t.Hour.ToString() + "_";
                filename_ending += t.Minute.ToString() + ".csv";

                filename = Path.Combine(Settings.DataPath, filename_ending);

                String d = "";
                for (int i = 0; i < this.Timestamps.Count; i++)
                {
                    d += Convert.ToString(this.Timestamps[i]) + ",";
                    if (this.Count >= i)
                    {
                        d += Convert.ToString(this.Values[i]) + "\n";
                    }
                }
                try
                {
                    StreamWriter sw = new StreamWriter(filename);
                    sw.Write(d);
                    sw.Close();
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Undefined Exception, writeToFile(): The file we tried to write to appears to be in use.\nError Code: " +
                        ex.Message, "File Writing Error");
                }
            }


            int last_accessed = 0;

            /// <summary>
            /// The time data for this capsule, stored as a one-dimensional list of doubles.
            /// </summary>
            public List<double> Timestamps { get; set; } = new List<double>();
            /// <summary>
            /// The values data for this capsule, stored as a one-dimensional list of doubles
            /// </summary>
            public List<double> Values { get; set; } = new List<double>();

            /// <summary>
            /// How many data points are in this capsule?
            /// </summary>
            public int Count
            {
                get
                {
                    return this.Timestamps.Count;
                }
            }

            double minimum_value;
            /// <summary>
            /// The value of the smallest value the capsule contains
            /// </summary>
            public double MinimumValue
            {
                get
                {
                    return minimum_value;
                }
            }

            double maximum_value;
            /// <summary>
            /// The value of the maximum value the capsule contains.
            /// </summary>
            public double MaximumValue
            {
                get
                {
                    return maximum_value;
                }
            }

            /// <summary>
            /// Returns the average data.
            /// </summary>
            public double Average
            {
                
                get
                {
                    double total = 0;
                    for(int i = 0; i < this.Count; i++)
                    {
                        total += this.Values[i];
                    }
                    total /= this.Count;
                    return total;
                }
            }

            /// <summary>
            /// Gets roughly the last 1.5 seconds of data.
            /// </summary>
            public double RecentAverage
            {
                get
                {
                    double total = 0;
                    // by default go back 300 data points (about 1.5 seconds)
                    int look_back_index = this.Count - 300;
                    if(look_back_index < 0)
                    {
                        look_back_index = 0;
                    }
                    for(int i = this.Count -1; i >= look_back_index; i--)
                    {
                        total += this.Values[i];
                    }
                    total /= (this.Count - look_back_index);
                    return total;
                }
            }

            /// <summary>
            /// Runs a check to ensure capsule is valid.
            /// </summary>
            public bool IsValid
            {
                get
                {
                    if (this.Timestamps.Count == this.Values.Count && this.last_accessed <= this.Count)
                    {
                        if(this.Timestamps.Count == 0)
                        {
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            bool is_normalized = false;
            /// <summary>
            /// Reports whether our dataset is normalized.
            /// </summary>
            public bool IsNormalized
            {
                get
                {
                    return is_normalized;
                }
            }
            

            /// <summary>
            /// The data to trim from the capsule, either old or new data
            /// </summary>
            public enum TrimPoint
            {
                /// <summary>
                /// Trims the oldest data added to the capsule (0 index = oldest)
                /// </summary>
                OLD,
                /// <summary>
                /// Trums the newest data added to the capsule
                /// </summary>
                NEW
            }
        }

        /// <summary>
        /// Contains all non-experimental data (calibration, testing, etc.)
        /// </summary>
        public static class General
        {
            /// <summary>
            /// Clears out all current data from the General static class
            /// </summary>
            public static void Clear()
            {
                ForceData.Clear();
                for (int i = 0; i < Amplitudes.Count; i++)
                {
                    Amplitudes[i].Clear();
                }
            }

            /// <summary>
            /// The capsule that holds General force data.
            /// </summary>
            public static Capsule ForceData = new Capsule();

            /// <summary>
            /// The list of capsules that holds General amplitude data.
            /// </summary>
            public static List<Capsule> Amplitudes = new List<Capsule>(); // stores our node amplitude data.
        }

        /// <summary>
        /// The class that holds all our Experimental data.
        /// </summary>
        public static class Experimental // our experimental data.
        {
            /// <summary>
            /// Clears out all current experimental data.
            /// </summary>
            public static void Clear() // clears all data
            {
                Rotations = 0;
                ForceData.Clear();
                for (int i = 0; i < Amplitudes.Count; i++)
                {
                    Amplitudes[i].Clear();
                }
            }
            /// <summary>
            /// The number of rotations that have been undergone in a given stimulation experiment.
            /// </summary>
            public static int Rotations { get; set; } // how many rotations have we gone through?
            /// <summary>
            /// The capsule that contains Experimental force data
            /// </summary>
            public static Capsule ForceData = new Capsule();
            /// <summary>
            /// The list of capsules that contains Experimental amplitudes
            /// </summary>
            public static List<Capsule> Amplitudes = new List<Capsule>(); // stores our node amplitude data.
            /// <summary>
            /// The maximum force that can be achieved, reported in kg. To get force in newtons, multiply by 9.8
            /// </summary>
            public static double MaxForce { set; get; } = 16;    // measured in Newtons / 9.8 (kg weight equivalents)
            /// <summary>
            /// The baseline (resting) force for a given user due to resting againsth the load cell
            /// or reading errors. Used to offset to get a true 0 value for 0.
            /// </summary>
            public static double BaselineForce { set; get; } // measured in normalized force (MVC)

        }

        /// <summary>
        /// Gets experimental data if an experiment is running and general data otherwise
        /// </summary>
        public static Capsule ForceData
        {
            get
            {
                if(Timekeeeper.Experimental.IsRunning)
                {
                    return Data.Experimental.ForceData;
                }
                else
                {
                    return Data.General.ForceData;
                }
            }
        } 

        /// <summary>
        /// Returns a list of amplitudes, either from the Experimental class if an experiment is running
        /// or from the General class if an experiment is not running.
        /// </summary>
        public static List<Capsule> Amplitudes
        {
            get
            {
                if (Timekeeeper.Experimental.IsRunning)
                {
                    return Data.Experimental.Amplitudes;
                }
                else
                {
                    return Data.General.Amplitudes;
                }
            }
        }
    }
}
