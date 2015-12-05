using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace CSharpProject
{
    static class Nodes
    {
        public static Node getNode(int index)
        {
            if (index < nodes.Count)
            {
                return nodes[index];
            }
            else
            {
                throw new ArgumentOutOfRangeException("Defined Error, Node[]: Node index out of range. ");
            }
        }

        public static Node getNode(uint node_id)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Index == node_id)
                {
                    return nodes[i];
                }
            }
            throw new ArgumentOutOfRangeException("Defined Error, getNode(): Node with ID " +
                node_id.ToString() + "does not exist");
        }

        // this rotates our nodes, selecting both (1) active node and (1) dormant node.
        public static void rotateNodesAsync(Object thread_info)
        {
            // only proceed if we have an active timekeeper
            if (Timekeeeper.Experimental.Running || Timekeeeper.General.Running)
            {
                // first we get the current time
                // if we are running an experiment, the start time is determined by the Experimental time
                double start_time, end_time, elapsed_time, ramping_down_start_time, ramping_up_start_time,
                    ramping_up_slope, ramping_down_slope, current_time, ramping_up_step_duration, 
                    ramping_down_step_duration, ramping_down_duration;

                int ramping_down_counter = 0;
                int ramping_up_counter = 0;

                start_time = Timekeeeper.ElapsedSeconds;

                end_time = start_time + Settings.Experimental.RampTime; // this is our enforced ending time
                ramping_down_start_time = start_time + (end_time - start_time) * Settings.Experimental.DownDelay;
                ramping_up_start_time = start_time;
                ramping_down_duration = (end_time - start_time) * (1.0 - Settings.Experimental.DownDelay);

                Node ramping_up_node = Nodes.selectNode(Settings.NodeStatus.DORMANT);
                Node ramping_down_node = Nodes.selectNode(Settings.NodeStatus.ACTIVE);

                // the slopes are in units of amplitudes per step
                ramping_up_slope = ramping_up_node.MaximumAmplitude / Settings.Experimental.RampTime /
                    Settings.Experimental.StepsPerSecond;
                // the step duration is in units of seconds per step
                ramping_up_step_duration = 1.0 / Settings.Experimental.StepsPerSecond / Settings.Experimental.RampTime;

                ramping_down_slope = ramping_down_node.MaximumAmplitude / ramping_down_duration /
                    Settings.Experimental.StepsPerSecond;
                ramping_down_step_duration = 1.0 / Settings.Experimental.StepsPerSecond / ramping_down_duration;

                // ideally before we execute the below code we need some way of locking out the user so they can't fuck with us.
                while (ramping_up_node.Amplitude < ramping_up_node.MaximumAmplitude ||
                    ramping_down_node.Amplitude > 0)
                {
                    current_time = Timekeeeper.ElapsedSeconds;
                    elapsed_time = current_time - start_time;

                    // if we have not yet got to zero amplitude on our ramping down node
                    if (ramping_down_node.Amplitude > 0)
                    {
                        // if the current time is greater than the ramping down start time
                        if (current_time > ramping_down_start_time)
                        {
                            // if we have exceeded the time it takes to make a step, do one
                            if (elapsed_time > ramping_down_step_duration * (ramping_down_counter + 1))
                            {
                                ramping_down_node.Amplitude -= ramping_down_slope;
                                ramping_down_counter++;
                            }
                        }
                    }

                    // if we have not got te the max amplitude on our ramping up node
                    if (ramping_up_node.Amplitude < ramping_up_node.MaximumAmplitude)
                    {
                        // if the current time is greater than the ramping up start time
                        if (current_time > ramping_up_start_time)
                        {
                            if(elapsed_time > ramping_up_step_duration * (ramping_up_counter + 1))
                            {
                                ramping_up_node.Amplitude += ramping_up_slope;
                                ramping_up_counter++;
                            }
                        }
                    }

                    Thread.Sleep(1); // don't blow up our CPU. I don't like it, but no other choice here.
                } // end while loop
            } // end if Timekeeper running conditional
        } // end function

        public static void Add(Node n)
        {
            nodes.Add(n);
        }
        // here we set up our node array from our settings class
        public static void Initialize()
        {
            try {
                destroyArray(); // destroy the old array

                List<int> node_vars = Settings.popNodeData(); // pop data from our settings
                while (node_vars.Count > 0)
                {
                    if (node_vars.Count == 5) // all good. we can proceed
                    {
                        Node temp_node = new Node(node_vars[0]);
                        nodes.Add(temp_node);
                        NodeIDs.Add(node_vars[0]);
                        NodeXCoors.Add(node_vars[1]);
                        NodeYCoors.Add(node_vars[2]);
                        Widths.Add(node_vars[3]);
                        Heights.Add(node_vars[4]);
                    }
                    else if (node_vars.Count > 0 && node_vars.Count != 5)
                    {
                        throw new Exception("Defined Error, initializeArray(): There appears to be a problem with the number "
                            + "of arguments passed to us by the settings popNode function.");
                    }
                    node_vars.Clear();
                    node_vars = Settings.popNodeData();
                }
                if (nodes.Count > 0)
                {
                    SelectedNode = Nodes.nodes[0];
                }
                else
                {
                    SelectedNode = new Node(0); // bugy as hell.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // destroys all the information contained in the Nodes class.
        public static void destroyArray()
        {
            selected_node = -1;
            // clear out our old arrays.
            nodes.Clear();
            NodeIDs.Clear();
            NodeXCoors.Clear();
            NodeYCoors.Clear();
            Widths.Clear();
            Heights.Clear();
        }

        // selects a node for us
        public static void selectNode(uint selector)
        {
            selected_node = Convert.ToInt32(selector);
        }

        public static Node selectNode(Settings.NodeStatus status)
        {
            // for now, return the first active node
            for (int i = 0; i < Count; i++)
            {
                if (nodes[i].Status == status)
                {
                    return nodes[i];
                }
            }
            throw new ArgumentOutOfRangeException("Defined Error, selectNode(): Unable to find node with status " +
                Convert.ToString(status));
        }

        public static int getReverseIndex(int id)
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                if(nodes[i].Index == id)
                {
                    return i;
                }
            }
            throw new Exception("Defined Error, getReverseIndex(): Could not find node.");
        }

        public static Node getNodeByID(int ind)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Index == ind)
                {
                    return nodes[i];
                }
            }
            throw new Exception("Defined Error, getNodeByIndex(): Could not find the node you were looking for. ");
        }

        public static Button getButtonByIndex(int ind)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].Index == ind)
                {
                    return UIButtons[i];
                }
            }
            return new Button();
        }

        public static int getSelectedNodeIndex()
        {
            return selected_node;
        }

        public static void setGlobalFrequency()
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Frequency = Settings.GlobalFrequency;
            }
        }

        // sets all our node variables to zero (except max amplitude)
        public static void setAllZero()
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Amplitude = 0;
            }
        }

        public static void padClicked(Object sender, System.EventArgs e)
        {
            Button intermediary = (Button)sender; // we know the object we are dealing with is a button
            Nodes.selectNode(Convert.ToUInt16(intermediary.Name));
        }

        public static Point getPoint(int node_index)
        {

            const int ugliness_offset = 15;
            if (node_index < Nodes.Count && NodeXCoors.Count == Nodes.Count && NodeYCoors.Count == Nodes.Count)
            {
                return new Point(NodeXCoors[node_index] + ugliness_offset - 5, NodeYCoors[node_index] + ugliness_offset);
            }
            return new Point(0, 0);
        }

        // returns the width of a node
        public static int getWidth(int node_index)
        {
            if (node_index < Nodes.Count && node_index < Widths.Count && Nodes.Count == Widths.Count)
            {
                return Widths[node_index];
            }
            else
            {
                throw new Exception("Defined Exception, getWidth(): Something is wrong with the settings.");
            }
        }

        // returns the height of a node
        public static int getHeight(int node_index)
        {
            if (node_index < nodes.Count && node_index < Heights.Count && nodes.Count == Heights.Count)
            {
                return Heights[node_index];
            }
            else
            {
                throw new Exception("Defined Exception, getHeight(): Something is wrong with the settings.");
            }
        }

        public class Node
        {
            public Node(int ind)
            {
                this.Index = ind;
                this.Status = Settings.NodeStatus.DORMANT;
            }
            double amplitude;
            public double Amplitude
            {
                get { return amplitude; }
                set
                {
                    // if we set the value too low, now set it to zero
                    if (value < 0)
                    {
                        value = 0.0;
                    }
                    // if we set the value too high, set it to its maximum
                    if (value > 255.0)
                    {
                        value = 255.0;
                    }

                    double previous_amplitude = amplitude;
                    int current_frequency = frequency;
                    amplitude = value;
                    double current_amplitude = value;

                    if (previous_amplitude == 0 && current_amplitude != 0 && current_frequency != 0) // We are changing the frequency from zero. If the amplitude is nonzero,
                                                                                                     // this means we want to turn on the node.
                    {
                        // if we set the amplitude to nonzero from zero, turn the node on.
                        Status = Settings.NodeStatus.ACTIVE;
                    }
                    else if (previous_amplitude != 0 && current_amplitude == 0)
                    {
                        // if we set the amplitude to zero, make sure the node is turned off
                        Status = Settings.NodeStatus.DORMANT;
                    }
                    // now we actually send the command.
                    Comms.Queue(new Comms.Command(Comms.Command.CommandType.AMPLITUDE_CHANGE,
                        Index, Convert.ToInt16(amplitude)).Bytes);

                    // and we add the amplitude to our data if an experiment is running.
                    if (Timekeeeper.Experimental.Running)
                    {
                        Data.Experimental.Amplitudes[Nodes.getReverseIndex(Index)].addData(Timekeeeper.ElapsedSeconds,
                            amplitude);
                    }
                }
            }

            int maximum_amplitude;
            public int MaximumAmplitude
            {
                get { return maximum_amplitude; }
                set
                {
                    if(value < 0)
                    {
                        value = 0;
                    }
                    if (value <= 255)
                    {
                        maximum_amplitude = value;
                        if(value > amplitude) // if our new max amplitude exceeds our current amplitude, reduce our current amplitude.
                        {
                            Amplitude = maximum_amplitude;
                        }
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Defined Error: Cannot set a value of " +
                            Convert.ToString(value) + "to MaximumAmplitude");
                    }
                }
            }

            int frequency;
            public int Frequency
            {
                get { return frequency; }
                set
                {
                    if(value < 0)
                    {
                        value = 0;
                    }
                    if (value <= 127)
                    {

                        int previous_frequency = frequency;
                        double current_amplitude = amplitude;

                        frequency = value;
                        int current_frequency = value;

                        if (previous_frequency == 0 && current_amplitude != 0 && current_frequency != 0) // We are changing the frequency from zero. If the amplitude is nonzero,
                                                                                                         // this means we want to turn on the node.
                        {
                            Status = Settings.NodeStatus.ACTIVE;
                        }
                        else if (previous_frequency != 0 && current_frequency == 0)
                        {
                            Status = Settings.NodeStatus.DORMANT;
                        }

                        // now we actually change the frequency
                        Comms.Queue(new Comms.Command(Comms.Command.CommandType.FREQUENCY_CHANGE,
                            Index, Convert.ToInt16(value)).Bytes);

                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Defined Error: Cannot set Frequency to " +
                            value.ToString());
                    }
                }
            }

            public bool IsValid
            {
                get
                {
                    if (Index == -1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            public int Index { get; set; } = -1;
            Settings.NodeStatus status;
            public Settings.NodeStatus Status
            {
                get
                {
                    return status;
                }
                set
                {
                    // first we set the value
                    status = value;

                    // then we generate the command
                    Comms.Queue(new Comms.Command(Comms.Command.CommandType.STATUS_CHANGE,
                        Index, (int)value).Bytes);
                }
            }

        }

        public static int Count
        {
            get { return nodes.Count; }
        }
        static List<Node> nodes = new List<Node>();
        public static List<Button> UIButtons { get; set; } = new List<Button>();
        static int selected_node = -1; // no selected node
        public static Node SelectedNode 
        {
           
            get
            {
                return getNodeByID(selected_node);
            }
            set
            {
                selected_node = value.Index;
            }
        }

        // our setup-specific variables
        public static List<int> NodeIDs { get; set; } = new List<int>();
        public static List<int> NodeXCoors { get; set; } = new List<int>();
        public static List<int> NodeYCoors { get; set; } = new List<int>();
        public static List<int> Heights { get; set; } = new List<int>();
        public static List<int> Widths { get; set; } = new List<int>();
    }
}

/*
           try {
               // this tells us where the line of intersection should be between the node ramping up
               // and the node ramping down.
               double portion_active = (double)thread_info;
               // first, we select both an active node and a dormant node.

               if (portion_active <= 1.0)
               {
                   Node active_node = selectNode(Settings.NodeStatus.ACTIVE);
                   Node dormant_node = selectNode(Settings.NodeStatus.DORMANT);


                   int dormant_node_step = Convert.ToInt16((double)dormant_node.MaximumAmplitude /
                       Settings.Experimental.MaximumSteps / (Settings.Experimental.RampTime));
                   int dormant_node_step_duration = Convert.ToInt16(255.0 / dormant_node_step * 
                       Settings.Experimental.RampTime); // in milliseconds, the 0.6 to correct for the fact we are using a sleep.
                   if(Settings.Experimental.RampTime >= 1.5)
                   {
                       dormant_node_step_duration = Convert.ToInt16(dormant_node_step_duration * 0.6);
                   }
                   int active_node_step = Convert.ToInt16((double)active_node.Amplitude /
                       Settings.Experimental.MaximumSteps / (Settings.Experimental.RampTime * portion_active));

                   // the 0.1 is to correct for the rounding errors encountered.
                   double active_node_delay = (1.0 - portion_active) * Settings.Experimental.RampTime;


                   while (active_node.Amplitude > 0 || dormant_node.Amplitude < dormant_node.MaximumAmplitude)
                   {
                       // this is our delay
                       if (Timekeeeper.Experimental.ElapsedSeconds -
                           Data.Experimental.Rotations * Settings.Experimental.Interval
                           > active_node_delay)
                       {
                           if (active_node.Amplitude > 0) // don't call this any more than we need to
                               active_node.Amplitude -= active_node_step;
                       }

                       if (dormant_node.Amplitude < dormant_node.MaximumAmplitude)
                       {
                           if (dormant_node.Amplitude + dormant_node_step > dormant_node.MaximumAmplitude)
                               dormant_node.Amplitude = dormant_node.MaximumAmplitude;
                           else
                           {
                               dormant_node.Amplitude += dormant_node_step;
                           }
                       }
                       Thread.Sleep(dormant_node_step_duration);
                   }
               }
           }
           catch (ArgumentOutOfRangeException our_ex)
           {
               MessageBox.Show("Defined Error, rotateNodes(): " + our_ex.Message);
           }
           */
