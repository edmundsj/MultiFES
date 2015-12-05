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

        // this is the class that will handle all of our graphing operations.
        public class Graph
        {
            // this initializes a graph with a certain data capsule as its data source
            public Graph(Capsule s)
            {
                wrapped_chart = new Chart();
                wrapped_area = new ChartArea();
                wrapped_chart.ChartAreas.Add(wrapped_area);

                this.sources.Add(s);
                Series new_series = new Series(series.Count.ToString()); // set the name of the series to the count
                this.series.Add(new_series);
                this.wrapped_chart.Series.Add(this.series[this.series.Count - 1]);
                currently_showing.Add(new Capsule());
            }

            public Graph(List<Capsule> capsule_list)
            {
                wrapped_chart = new Chart();
                wrapped_area = new ChartArea();
                wrapped_chart.ChartAreas.Add(wrapped_area);
                wrapped_chart.Size = new Size(200, 200);

                for (int i = 0; i < capsule_list.Count; i++)
                {
                    this.sources.Add(capsule_list[i]);
                    Series new_series = new Series(series.Count.ToString()); // set the name of the series to the count
                    this.series.Add(new_series);
                    this.wrapped_chart.Series.Add(this.series[this.series.Count - 1]);
                    currently_showing.Add(new Capsule());
                }

            }

            // this updates our graph with the most recent data obtained from the capsule.
            public void Update()
            {
                /*  1. Get the recent data from our data sources
                    2. Set our showing data to include the most recently obtained data
                    3. Check to see if our showing data exceeds the maximum resolution
                    4. Set the new axis values
                    5. Remove the number of points necessary to have the maximum resolution 
                    6. Add the new points
                    7. Profit.
                */

                List<int> exceeded_by = new List<int>();
                List<Capsule> to_add = new List<Capsule>();
                double x_axis_minimum = 10e12;
                double x_axis_maximum = 0;
                double y_axis_minimum = 10e12;
                double y_axis_maximum = 0;


                for (int i = 0; i < this.sources.Count; i++)
                {
                    exceeded_by.Add(0);
                    to_add.Add(sources[i].getRecentData());
                    this.currently_showing[i].addData(to_add[i]); // add the recently obtained data

                    // check and see if this exceeds the number of points we can have exceeds max resolution
                    if (this.currently_showing[i].Count > this.MaxResolution)
                    {
                        exceeded_by[i] = this.MaxResolution - this.currently_showing[i].Count;
                        this.currently_showing[i].Trim(exceeded_by[i], Capsule.TrimPoint.OLD);
                    }

                    // now we set our new axes value


                    // now we remove the points from each series to achieve the max resolution
                    for (int x = 0; x < exceeded_by[i]; x++)
                    {
                        series[i].Points.RemoveAt(0);
                    }
                    // now we set the axes to the minimum/maximum value to our axes
                    if (this.currently_showing[i].Count > 0)
                    {
                        if (this.currently_showing[i].Timestamps[0] <= x_axis_minimum)
                        {
                            x_axis_minimum = this.currently_showing[i].Timestamps[0];
                        }

                        if (this.currently_showing[i].Timestamps[this.currently_showing.Count - 1] >= x_axis_maximum)
                        {
                            x_axis_maximum = this.currently_showing[i].Timestamps[this.currently_showing.Count - 1];
                        }

                        // set our Y axis minimum and maximums
                        if (this.currently_showing[i].MinimumValue <= y_axis_minimum)
                        {
                            y_axis_minimum = this.currently_showing[i].MinimumValue;
                        }

                        if (this.currently_showing[i].MaximumValue >= y_axis_maximum)
                        {
                            y_axis_maximum = this.currently_showing[i].MaximumValue;
                        }
                    }
                }

                // make sure to check our values for validity
                if (x_axis_minimum > x_axis_maximum)
                {
                    x_axis_minimum = x_axis_maximum;
                }
                if (y_axis_minimum > y_axis_maximum)
                {
                    y_axis_minimum = y_axis_maximum;
                }

                // now we set our axes
                this.wrapped_area.AxisX.Minimum = x_axis_minimum;
                this.wrapped_area.AxisX.Maximum = x_axis_maximum;
                this.wrapped_area.AxisY.Minimum = y_axis_minimum;
                this.wrapped_area.AxisY.Maximum = y_axis_maximum;

                // and, finally, we add our points.
                for (int i = 0; i < to_add.Count; i++)
                {
                    for (int x = 0; x < to_add[i].Count; x++)
                    {
                        this.series[i].Points.Add(new DataPoint(to_add[i].Timestamps[x], to_add[i].Values[x]));
                    }
                }
            }

            public Chart wrapped_chart;
            private ChartArea wrapped_area;
            private List<Series> series = new List<Series>(); // our list of serieses

            private List<Capsule> sources = new List<Capsule>(); // our list of data sources
            private List<Capsule> currently_showing = new List<Capsule>(); // our list of currently showing data

            int max_resolution;
            public int MaxResolution
            {
                get
                {
                    return max_resolution;
                }
                set
                {
                    if (value < 1000 && value > 0)
                    {
                        max_resolution = value;
                    }
                }
            }

        }
    }
}
