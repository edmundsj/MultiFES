﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace MultiFES
{
    /// <summary>
    /// This class handles all our node-related information, including current amplitudes,
    /// statuses, and frequencies. Setting of these values should be done through this class.
    /// </summary>
    public static class Nodes
    {
        /// <summary>
        /// Gets a node by its zero-based index. 
        /// Throws error code 0x008 if the index is out of range.
        /// </summary>
        /// <param name="index">The zero-based node index.</param>
        /// <returns>A Node with the specified index.</returns>
        public static Node getNode(int index)
        {
            if (index < nodes.Count)
            {
                return nodes[index];
            }
            else
            {
                throw new ArgumentOutOfRangeException("Node index out of range. Error code 0x008.");
            }
        }

        /// <summary>
        /// Gets a node with the given node ID
        /// </summary>
        /// <param name="node_id">The node ID on the FES board</param>
        /// <returns>The node with the given ID</returns>
        public static Node getNode(uint node_id)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ID == node_id)
                {
                    return nodes[i];
                }
            }
            throw new ArgumentOutOfRangeException("Defined Error, getNode(): Node with ID " +
                node_id.ToString() + "does not exist");
        }

        /// <summary>
        /// Rotates our nodes asynchronously. I wouldn't touch this. It's a little bleh.
        /// </summary>
        /// <param name="thread_info"></param>
        public static void rotateNodesAsync(Object thread_info)
        {
            // only proceed if we have an active timekeeper
            if (Timekeeeper.IsRunning)
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
                try {
                    Node ramping_up_node = Nodes.getNode(Settings.NodeStatus.DORMANT);
                    Node ramping_down_node = Nodes.getNode(Settings.NodeStatus.ACTIVE);// the slopes are in units of amplitudes per step
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
                            if (current_time > (ramping_down_start_time +
                                    ramping_down_step_duration * (ramping_down_counter + 1)))
                            {
                                ramping_down_node.Amplitude -= ramping_down_slope;
                                ramping_down_counter++;
                            }



                        }



                        // if we have not got te the max amplitude on our ramping up node
                        if (ramping_up_node.Amplitude < ramping_up_node.MaximumAmplitude)
                        {
                            // if the current time is greater than the ramping up start time
                            if (current_time > ramping_up_start_time)
                            {
                                if (elapsed_time > ramping_up_step_duration * (ramping_up_counter + 1))
                                {
                                    ramping_up_node.Amplitude += ramping_up_slope;
                                    ramping_up_counter++;
                                }
                            }
                        }

                        Thread.Sleep(1); // don't blow up our CPU. I don't like it, but no other choice here.
                    } // end while loop
                }
                catch(ArgumentOutOfRangeException out_ex)
                {
                    MessageBox.Show("Cannot rotate nodes. There are either not enough active or not enough dormant nodes. " +
                       out_ex.Message);
                }
            } // end if Timekeeper running conditional
        } // end function
        
        /// <summary>
        /// Initializes our node array from the previously loaded settings
        /// </summary>
        public static bool Initialize()
        {
            destroyArray(); // destroy the old array

            for (int i = 0; i < Settings.NodeIDs.Count; i++)
            {
                Node temp_node = new Node((int)Settings.NodeIDs[i]);
                temp_node.XCoor = Settings.NodeXCoors[i];
                temp_node.YCoor = Settings.NodeYCoors[i];
                temp_node.Width = Settings.Widths[i];
                temp_node.Height = Settings.Heights[i];

                nodes.Add(temp_node);
            }

            if (nodes.Count > 0)
            {
                SelectedNode = Nodes.nodes[0];
            }
            else
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Destroys all current nodes and sets selected node to -1.
        /// </summary>
        public static void destroyArray()
        {
            selected_node_id = -1;
            // clear out our old arrays.
            nodes.Clear();
        }

        /// <summary>
        /// This gets the first node with a given status
        /// Throws error code 0x004 if it cannot find a node with that status.
        /// </summary>
        /// <param name="status">Status of the node</param>
        /// <returns>Returns the actual node.</returns>
        public static Node getNode(Settings.NodeStatus status)
        {
            // for now, return the first node with that status
            for (int i = 0; i < Count; i++)
            {
                if (nodes[i].Status == status)
                {
                    return nodes[i];
                }
            }
            throw new ArgumentOutOfRangeException("Unable to find node with status " +
                Convert.ToString(status) + ". Error code 0x004.");
        }
        
        /// <summary>
        /// Gets a node by its zero-based channel ID as seen on the FES board.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The node with a given id</returns>
        public static Node getNodeByID(int id)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ID == id)
                {
                    return nodes[i];
                }
            }
            throw new Exception("Defined Error, getNodeByIndex(): Could not find the node you were looking for. ");
        }

        /// <summary>
        /// Gets a UI button we dynamically created upon initialization of this class.
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public static Button getButtonByIndex(int ind)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ID == ind)
                {
                    return UIButtons[i];
                }
            }
            return new Button();
        }

        /// <summary>
        /// Sets the current frequency on all nodes to the global frequency.
        /// </summary>
        public static void setGlobalFrequency()
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Frequency = Settings.GlobalFrequency;
            }
        }

        /// <summary>
        /// Sets all nodes to an amplitude of zero.
        /// </summary>
        public static void setAllZero()
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Amplitude = 0;
            }
        }

        /// <summary>
        /// The event handler responsible for a user clicking on one of the nodes
        /// buttons on the pad.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void padClicked(Object sender, System.EventArgs e)
        {
            Button intermediary = (Button)sender; // we know the object we are dealing with is a button
            Nodes.selected_node_id = Convert.ToUInt16(intermediary.Name);
        }

        /// <summary>
        /// Gets a point from a given node index for placing the node on the UI.
        /// Returns error code 0x007 if the node cannot be found.
        /// </summary>
        /// <param name="node_index"></param>
        /// <returns>Returns a Point</returns>
        public static Point getPoint(int node_index)
        {

            const int ugliness_offset = 15;
            if (node_index < Nodes.Count)
            {
                return new Point(nodes[node_index].XCoor + ugliness_offset - 5, nodes[node_index].YCoor + ugliness_offset);
            }
            else throw new ArgumentException("There appears to be a problem loading the Settings. Error code 0x007.");
        }
        
        /// <summary>
        /// An individual node. Contains all information necessary for the UI and an active node.
        /// </summary>
        public class Node
        {
            /// <summary>
            /// Constructor that takes the node ID as a mandatory argument.
            /// </summary>
            /// <param name="id">The ID of the node.</param>
            public Node(int id)
            {
                this.ID = id;
                this.Index = Nodes.Count;
                this.status = Settings.NodeStatus.DORMANT;
            }
            double amplitude;
            /// <summary>
            /// The current amplitude of a node. Set it using this property.
            /// </summary>
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
                        ID, Convert.ToInt16(amplitude)).Bytes);

                    // and we add the amplitude to our data if an experiment is running.
                    if (Timekeeeper.IsRunning)
                    {
                        Data.Amplitudes[Nodes.getNodeByID(ID).Index].addData(Timekeeeper.ElapsedSeconds,
                            amplitude);
                    }
                }
            }

            int maximum_amplitude;
            /// <summary>
            /// The maximum amplitude of a given node
            /// </summary>
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
            /// <summary>
            /// The current frequency of a given node
            /// </summary>
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
                            ID, Convert.ToInt16(value)).Bytes);

                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Defined Error: Cannot set Frequency to " +
                            value.ToString());
                    }
                }
            }
            
            /// <summary>
            /// The node channel identifier (from the FES board)
            /// </summary>
            public int ID { get; set; } = -1;
            /// <summary>
            /// The zero-based node index as a member of the Nodes class.
            /// </summary>
            public int Index { get; }

            Settings.NodeStatus status;
            /// <summary>
            /// The current node status.
            /// </summary>
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
                        ID, (int)value).Bytes);
                }
            }
            /// <summary>
            /// The width of the button for this node on the UI
            /// </summary>
            public int Width { get; set; }
            /// <summary>
            /// The height of the button for this node on the UI
            /// </summary>
            public int Height { get; set; }
            /// <summary>
            /// The X coordinate of the button for this node on the UI
            /// </summary>
            public int XCoor { get; set; }
            /// <summary>
            /// The Y coordinate of the button for this node on the UI.
            /// </summary>
            public int YCoor { get; set; }

        }

        /// <summary>
        /// The current number of nodes
        /// </summary>
        public static int Count
        {
            get { return nodes.Count; }
        }
        static List<Node> nodes = new List<Node>();
        /// <summary>
        /// A list of buttons that are painted to our UI.
        /// </summary>
        public static List<Button> UIButtons { get; set; } = new List<Button>();
        static int selected_node_id = -1; // no selected node
        /// <summary>
        /// The currently selected node on the UI.
        /// </summary>
        public static Node SelectedNode 
        {
           
            get
            {
                return getNodeByID(selected_node_id);
            }
            set
            {
                selected_node_id = value.ID;
            }
        }
    }
}