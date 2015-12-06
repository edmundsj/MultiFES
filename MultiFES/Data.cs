using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace CSharpProject
{

    class Data // this is our data class that stores everything we need. Ported over from Comms.
    {
        public class Capsule
        {
            public Capsule()
            {

            }

            public Capsule(List<double> times, List<double> vals)
            {
                this.Timestamps = times;
                this.Values = vals;
            }

            public virtual void Clear()
            {
                Timestamps.Clear();
                Values.Clear();
                last_accessed = 0;
            }

            // safely adds data to an MVC capsule
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

            // this allows us to add additional capsule data.
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

            // trims data off the beginning or end of a capsule
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

            // removes data at a given index
            public void RemoveAt(int index)
            {
                if (index < this.Count && index >= 0)
                {
                    this.Timestamps.RemoveAt(index);
                    this.Values.RemoveAt(index);
                }
            }

            // this gets data that has not yet been accessed by the capsule.
            public virtual Capsule getRecentData()
            {
                // now we peer back at the data we got
                List<double> new_timestamps = this.Timestamps.GetRange(this.last_accessed,
                    this.Count);
                List<double> new_values = this.Values.GetRange(this.last_accessed,
                    this.Count); // there is a problem here.
                return new Capsule(new_timestamps, new_values);
            }

            public virtual void popData()
            {
                if (this.Timestamps.Count > 0)
                {
                    this.Timestamps.RemoveAt(0);
                    this.Values.RemoveAt(0);
                }
            }

            public void updateData() // gets the most recent points from our input buffer
            {
                this.Timestamps = Comms.Input.Buffer.Contents.Timestamps;
                this.Values = Comms.Input.Buffer.Contents.Values;
            }


            // allows us to write any capsule data to file.
            public virtual void writeToFile(String filename)
            {

                DateTime t = DateTime.Now;
                String filename_ending = filename;
                filename_ending += "_";
                filename_ending += t.Month.ToString() + "-";
                filename_ending += (t.Day.ToString()) + "_";
                filename_ending += t.Hour.ToString() + "_";
                filename_ending += t.Minute.ToString() + ".csv";

                filename = Path.Combine(Settings.CSV_PATH, filename_ending);

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


            // allows us to write any capsule data to file. THIS ALLOWS IT TO BE RUN IN THE BACKGROUND
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

                filename = Path.Combine(Settings.CSV_PATH, filename_ending);

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

            public List<double> Timestamps { get; set; } = new List<double>();
            public List<double> Values { get; set; } = new List<double>();

            public int Count
            {
                get
                {
                    return this.Timestamps.Count;
                }
            }

            double minimum_value;
            public double MinimumValue
            {
                get
                {
                    return minimum_value;
                }
            }

            double maximum_value;
            public double MaximumValue
            {
                get
                {
                    return maximum_value;
                }
            }


            // checks if the valid capsule is empty. An empty capsule is valid.
            public bool IsValid
            {
                get
                {
                    if (this.Timestamps.Count == this.Values.Count)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public enum TrimPoint
            {
                OLD,
                NEW
            }
        }

        public static class General
        {
            public static List<Capsule> Amplitudes = new List<Capsule>();
        }

        public static class Experimental // our experimental data.
        {
            public static void Clear() // clears all data
            {
                Rotations = 0;
                ForceData.Clear();
                for (int i = 0; i < Amplitudes.Count; i++)
                {
                    Amplitudes[i].Clear();
                }
            }
            public static int Rotations { get; set; } // how many rotations have we gone through?

            public static Capsule ForceData = new Capsule();

            public static List<Capsule> Amplitudes = new List<Capsule>(); // stores our node amplitude data.

            public static double MaxForce { set; get; } = 16;    // measured in Newtons / 9.8 (kg weight equivalents)
            public static double BaselineForce { set; get; } // measured in normalized force (MVC)

        }

        // this returns experimental data if an experiment is running and our input buffer otherwise
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
                    return Comms.Input.Buffer.Contents;
                }
            }
        } 

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
