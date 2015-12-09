using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Diagnostics;

namespace MultiFES
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Initializes our main UI with all its components and goodies.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            InitializeOtherThings();
        }

        /// <summary>
        /// Initializes all the dynamically created objects that cannot be known before runtime.
        /// </summary>
        private void InitializeOtherThings()
        {
            // XMLWrapper.loadXMLSettings();            // load our initial settings data
            if(Settings.loadDefaultSettings() == false)
            {

            }
            else 
            {
                if (Nodes.Initialize() == false)
                {
                    MessageBox.Show("There was an error with the settings. Please check the default node array XML file.",
                         "Connection Failure", MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation);
                }
                else
                {
                    initializePadArray();                   // create our Nodes pad on the UI with all the buttons.
                    Nodes.setGlobalFrequency();         // set our initial frequencies to what we want
                    
                }
            }
            // initialize communications
            if(Comms.Initialize() == false)
            {
                MessageBox.Show("Could not connect to Arduino. Please ensure it is plugged in, then click File-> Arduino -> Connect",
                     "Connection Failure", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
            }
            else
            {
            }
            
            Timekeeeper.Initialize();               // initializes our timekeeper
            
            force_chart.DataSource = Data.Experimental.ForceData;

            ui_timer.Enabled = true;
            updateUI();

            // so in the "background", or asynchronously, we need to recieve MVC data from the arduino
        }

        // this creates our pad and it's dependent graph
        private void initializePadArray()
        {

            for (int i = 0; i < Nodes.Count; i++)
            {
                // first we create our array pad
                Button temp_button = new Button();
                temp_button.Text = "CH " + Convert.ToString(Nodes.getNode(i).ID); // get the ID of the node
                temp_button.Name = Convert.ToString(Nodes.getNode(i).ID); // how we will identify our node
                temp_button.Location = Nodes.getPoint(i);
                temp_button.Width = Nodes.getNode(i).Width;
                temp_button.Height = Nodes.getNode(i).Height;
                temp_button.Click += new System.EventHandler(Nodes.padClicked);
                Nodes.UIButtons.Add(temp_button);
                array_pad.Controls.Add(Nodes.getButtonByIndex(Nodes.getNode(i).ID));

                // now we create our data variables
                Data.Experimental.Amplitudes.Add(new Data.Capsule());
                Data.General.Amplitudes.Add(new Data.Capsule());
                
            }
        }

        /// <summary>
        /// Updates our Node Pad UI to make our selected node orange.
        /// </summary>
        private void updatePadUI()
        {
            // first we want to highlight our selected node
            for (int i = 0; i < Nodes.UIButtons.Count; i++)
            {
                Nodes.UIButtons[i].BackColor = Color.LightGray;
            }
            
            Nodes.getButtonByIndex(Nodes.SelectedNode.ID).BackColor = Color.Coral;
            
        }

        /// <summary>
        /// Updates all UI components to contain the proper values and elements.
        /// </summary>
        private void updateUI()
        {
            updatePadUI();
            if (Timekeeeper.IsRunning)
            {
                if (Data.ForceData.Count > 0)
                {
                    if (Data.ForceData.IsValid)
                    {
                        
                        // first check and make sure we don't need to zero the x-axis
                        if(Data.ForceData.Timestamps[Data.ForceData.Count - 1] < force_chart.ChartAreas[0].AxisX.Minimum)
                        {
                            force_chart.ChartAreas[0].AxisX.Minimum = 0;
                            ClearGraph();
                        }
                        
                        if (force_chart.Series[0].Points.Count > 0)
                        {
                            force_chart.ChartAreas[0].AxisX.Minimum = force_chart.Series[0].Points[0].XValue;
                            force_chart.ChartAreas[0].AxisX.Maximum = Data.ForceData.Timestamps[Data.ForceData.Count - 1]; // the new maximum
                            /*
                            if (Data.ForceData.Values[Data.ForceData.Count - 1] < force_chart.Series[0].Points.FindMinByValue().YValues[0])
                            {
                                force_chart.ChartAreas[0].AxisY.Minimum = force_chart.Series[0].Points.FindMinByValue().YValues[0];
                            }
                            */
                        }
                        

                        force_chart.Series[0].Points.Add(new DataPoint(Data.ForceData.Timestamps[Data.ForceData.Count - 1],
                            Data.ForceData.Values[Data.ForceData.Count - 1]));
                        if (force_chart.Series[0].Points.Count > 75)
                        {
                            force_chart.Series[0].Points.RemoveAt(0);

                        }

                    }
                    // problem: I think while we are trying to bind the data (which is an iterative process)
                    // we get new data and we add it. This is likely due to the change of the Comms timer to 
                    // a windows forms timer.
                }
            }

            // update our labels and sliders according to the current valid frequency.

            frequency_label.Text = Convert.ToString(Nodes.SelectedNode.Frequency);
            frequency_trackbar.Value = Convert.ToInt32(Nodes.SelectedNode.Frequency);

            amplitude_label.Text = Convert.ToString(Nodes.SelectedNode.Amplitude);
            amplitude_trackbar.Value = Convert.ToInt32(Nodes.SelectedNode.Amplitude);

            // here we update all our variables
            // here we update the debug field.
            if (Debug.Enabled == true)
            {
                debug_box.Visible = true;
                while (!Debug.IsEmpty)
                {
                    debug_box.AppendText(Debug.PopStatement() + Environment.NewLine);
                }

            }
            else
            {
                debug_box.Visible = false;
            }

            // here we add the running time.
            if (Timekeeeper.IsRunning == true)
            {
                time_elapsed.Text = Convert.ToString(Timekeeeper.ElapsedSeconds);
            }
            else
            {
                stimulate_button.Text = "STIMULATE";
            }
        }

        /// <summary>
        /// Clears the force chart for use.
        /// </summary>
        public void ClearGraph()
        {
            force_chart.Refresh();
            force_chart.Series[0].Points.Clear();
        }

        /// <summary>
        /// Begins our FES stimulation in response to the user clicking the "stimulate" button,
        /// or ends the stimulation in response to the user clicking the "abort" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stimulate_button_Click(object sender, EventArgs e)
        {
            // if the user clicks "STIMULATE"
            if (stimulate_button.Text == "STIMULATE")
            {
                if (Comms.Open() == true)
                {
                    Timekeeeper.General.Start();
                    stimulate_button.Text = "ABORT";
                }
            }
            // if the user clicks "ABORT"
            else
            {
                Comms.Close();

                stimulate_button.Text = "STIMULATE";

                // reset our timekeepers
                Timekeeeper.Stop();

            }
            // then we set the button's text to the opposite of what it was
        }

        /// <summary>
        /// Event handler for frequency trackbar scrolling
        /// </summary>
        private void frequency_trackbar_Scroll(object sender, EventArgs e)
        {
            // make sure the selected node is valid.
            Nodes.SelectedNode.Frequency = ((TrackBar)sender).Value;
            
        }

        /// <summary>
        /// Event handler for the amplitude trackbar scrolling.
        /// </summary>
        private void amplitude_trackbar_Scroll(object sender, EventArgs e)
        {
            Nodes.SelectedNode.Amplitude = ((TrackBar)sender).Value;
            if (Nodes.SelectedNode.Amplitude > Nodes.SelectedNode.MaximumAmplitude)
            {
                Nodes.SelectedNode.MaximumAmplitude = Convert.ToInt16(Nodes.SelectedNode.Amplitude);
            }

        }
        

        /// <summary>
        /// Event handler for Debug -> Output menu item
        /// </summary>
        private void arduinoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Comms.Output.DebugEnabled == false)
            {
                Comms.Output.DebugEnabled = true;
            }
            else
            {
                Comms.Output.DebugEnabled = false;
            }

        }
        
        /// <summary>
        /// Event handler that saves the data from the previous stimulation run.
        /// </summary>
        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create the save box dialog
            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.InitialDirectory = Settings.DataPath;


            if (save_file_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Data.ForceData.Clean();
                // NOTE: DATA.EXPERIMENTAL.FORCEDATA WILL ONLY HAVE DATA IF YOU ARE RUNNING AN EXPERIMENT (duh)
                int force_result = Data.ForceData.writeToFile(save_file_dialog.FileName + "_mvc-data_");  // write the file
                int amplitude_result = 0;
                // write our amplitude data to file.
                for (int i = 0; i < Data.Amplitudes.Count; i++)
                {
                    int temp_result = Data.Amplitudes[i].writeToFile(save_file_dialog.FileName + "CH-"
                        + i.ToString() + "_amplitude-data_");
                    if(temp_result != amplitude_result && amplitude_result == 0)
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

                Comms.Input.Buffer.Empty(); // make sure our program doesn't crash.
                Data.Experimental.Clear();
            }
        }

        /// <summary>
        /// Event handler for the button than forcibly rotates electrodes. Only works if a timekeeper is on.
        /// </summary>
        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Timekeeeper.IsRunning)
            {
                if (Timekeeeper.Experimental.IsRunning == false) // only allow explicit node rotation if we aren't
                    // currently running an experiment
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Nodes.rotateNodesAsync));
                }
            }
        }

        // set interval to 5s
        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 5;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 10;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 15;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 20;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 25;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 30;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 45;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 60;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 90;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem9_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 120;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem10_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 150;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void sToolStripMenuItem11_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 180;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        /// <summary>
        /// Begins an experiment.
        /// </summary>
        private void beginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool begin_experiment = true;
            // only start an experiment if our max amplitudes are greater than zero.
            
            if (begin_experiment == true)
            {
                // begin the experiment by opening the communications port
                if (Comms.IsOpen == false) // if we successfully connect to the arduino
                    Comms.Open();

                // first we need to set all amplitudes that aren't the selected node to zero, and that to max.
                Data.Experimental.Clear(); // clear out our old experimental data
                Comms.Input.Buffer.Empty(); // empty out our input buffer of force data
                Nodes.setAllZero();

                Timekeeeper.Experimental.Start(); // start our experimental timer

                // also change our stimulate button so it reads abort
                stimulate_button.Text = "ABORT";

                // set our selected node to the maximum amplitude

                Nodes.SelectedNode.Amplitude = Nodes.SelectedNode.MaximumAmplitude;
            }
            else
            {
            }

        }



        // our ui timer
        private void ui_timer_Tick(object sender, EventArgs e)
        {
            updateUI(); // this updates our UI
        }
        
        
        // sets our ramping down delay to whatever we need 0.1-0.8
        private void rampUpFirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Experimental.DownDelay = Convert.ToDouble(((ToolStripMenuItem)sender).Text);
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        // changes the experiment to single channel type
        private void singlechannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Experimental.CurrentType = Settings.Experimental.ExperimentType.SingleChannel;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        // changes the experiment to multi-channel type
        private void multichannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Experimental.CurrentType = Settings.Experimental.ExperimentType.MultiChannel;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }
        
        // this binds some data to our graph
        private void graphToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event handler for the button that hides or unhides the UI's graphs. 
        /// Found under Experiment -> Hide/Unhide graphs
        /// </summary>
        private void hideGraphsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.force_chart.Visible == true)
            {
                hide_unhide_graphs.Text = "Unhide Graphs";
                this.force_chart.Visible = false;
                this.amplitude_chart.Visible = false;
            }
            else {
                hide_unhide_graphs.Text = "Hide Graphs";
                this.force_chart.Visible = true;
                this.amplitude_chart.Visible = true;
            }

            
        }
        /// <summary>
        /// Event handler that reconnects the serial port if the user did not plug it in when 
        /// they should have. Found under File -> Arduino -> Connect
        /// </summary>
        private void connectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Comms.Destroy();
            if (Comms.Initialize())
            {
                MessageBox.Show("Successfully connected to Arduino.",
                    "Connected Successfully", MessageBoxButtons.OK,
                   MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Could not connect to Arduino. Please ensure it is plugged in.",
                     "Connection Failure", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
            }
            Comms.Open();
        }

        private void uploadCodeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int return_val = Comms.UploadCode();

            if (return_val == 0)
            {
                MessageBox.Show("Uploaded Code successfully!",
                     "Upload Success", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
            }
            else if (return_val == 1)
            {
                MessageBox.Show("There was a problem uploading code to the arduino. Check avrdude settings.",
                     "Upload failure", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
            }
            else if (return_val == 2)
            {
                MessageBox.Show("Could not connect to Arduino. Please ensure it is plugged in, then click File -> Arduino -> Connect",
                     "Connection failure", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// This unchecks all other menu items for a given ToolStripMenuItem
        /// </summary>
        /// <param name="selectedMenuItem"></param>
        public void UncheckOtherToolStripMenuItems(ToolStripMenuItem selectedMenuItem)
        {
            selectedMenuItem.Checked = true;

            // Select the other MenuItens from the ParentMenu(OwnerItens) and unchecked this,
            // The current Linq Expression verify if the item is a real ToolStripMenuItem
            // and if the item is a another ToolStripMenuItem to uncheck this.
            foreach (var ltoolStripMenuItem in (from object
                                                    item in selectedMenuItem.Owner.Items
                                                let ltoolStripMenuItem = item as ToolStripMenuItem
                                                where ltoolStripMenuItem != null
                                                where !item.Equals(selectedMenuItem)
                                                select ltoolStripMenuItem))
                (ltoolStripMenuItem).Checked = false;
        }

        private void inputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Comms.Input.DebugEnabled == false)
            {
                Comms.Input.DebugEnabled = true;
            }
            else
            {
                Comms.Input.DebugEnabled = false;
            }
        }

        /// <summary>
        /// The button we press when we want to test something.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Comms.Close();
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            Settings.Experimental.RampTime = 2.0;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            Settings.Experimental.RampTime = 3.0;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            Settings.Experimental.RampTime = 4.0;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
        }

        /// <summary>
        /// Calibrates the user's maximum force
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calibrateForceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Comms.Open();
            Timekeeeper.General.Start();
            // now record some data...
            MessageBox.Show("Please flex as hard as you can.");
            Thread.Sleep(2000);

            Data.ForceData.Clean(); // remove junk values
            Data.Experimental.MaxForce = Data.ForceData.Average;
            Data.ForceData.Clear(); // destroy the data, we got what we needed.

            Timekeeeper.General.Stop();
            Comms.Close();
        }
    }
}
