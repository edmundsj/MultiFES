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

namespace CSharpProject
{
    public partial class Form1 : Form
    {
        
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
                    Nodes.setGlobalFrequency();         // set our initial frequencies to what we want
                }
                else
                {
                    initializePadArray();                   // create our Nodes pad on the UI with all the buttons.
                }
            }
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
                force_chart.Series[0].Points.DataBindXY(Data.ForceData.Timestamps,
                    Data.ForceData.Values);
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
                Timekeeeper.Reset();

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
            if (Comms.DebugEnabled == false)
            {
                Comms.DebugEnabled = true;
            }
            else
            {
                Comms.DebugEnabled = false;
            }

        }
        
        // saves the data we had just recorded
        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create the save box dialog
            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.InitialDirectory = Settings.CSV_PATH;


            if (save_file_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
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

        // this forcibly rotates our electrodes. It can only be called if the system is running.
        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Timekeeeper.IsRunning)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Nodes.rotateNodesAsync));
            }
        }

        // set interval to 5s
        private void sToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 5;
        }

        private void sToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 10;
        }

        private void sToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 15;
        }

        private void sToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 20;
        }

        private void sToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 25;
        }

        private void sToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Interval = 30;
        }

        private void sToolStripMenuItem6_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 45;
        }

        private void sToolStripMenuItem7_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 60;
        }

        private void sToolStripMenuItem8_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 90;
        }

        private void sToolStripMenuItem9_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 120;
        }

        private void sToolStripMenuItem10_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 150;
        }

        private void sToolStripMenuItem11_Click(object sender, EventArgs e)
        {
            Settings.Experimental.Duration = 180;
        }

        // begins the experiment.
        private void beginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // begin the experiment by opening the communications port
            if (Comms.Open() == true) // if we successfully connect to the arduino
            {
                // first we need to set all amplitudes that aren't the selected node to zero, and that to max.
                Data.Experimental.Clear();
                Nodes.setAllZero();

                // also change our stimulate button so it reads abort
                stimulate_button.Text = "ABORT";

                // set our selected node to the maximum amplitude

                Nodes.SelectedNode.Amplitude = Nodes.SelectedNode.MaximumAmplitude;

                Timekeeeper.Experimental.Reset();
                Timekeeeper.Experimental.Start();
            }
        }



        // our ui timer
        private void ui_timer_Tick(object sender, EventArgs e)
        {

            updateUI(); // update our UI here

        }

        private void fastStartToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // sets our ramping down delay to 0.1
        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            Settings.Experimental.DownDelay = 0.1;
        }

        // sets our ramping down delay to 0.2
        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            Settings.Experimental.DownDelay = 0.2;
        }


        // sets our ramping down delay to 0.3
        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Settings.Experimental.DownDelay = 0.3;
        }

        // sets our ramping down delay to 0.8
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Settings.Experimental.DownDelay = 0.4;
        }

        // sets our ramping down delay to 0.8
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Settings.Experimental.DownDelay = 0.5;
        }

        // sets our ramping down delay to 0.6
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Settings.Experimental.DownDelay = 0.6;
        }

        // sets our ramping down delay to 0.7
        private void sameTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Experimental.DownDelay = 0.7;
        }

        // sets our ramping down delay to 0.8
        private void rampUpFirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Experimental.DownDelay = 0.8;
        }

        // this begins single-channel stimulation
        private void typeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        // changes the experiment to single channel type
        private void singlechannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Experimental.CurrentType = Settings.Experimental.ExperimentType.SingleChannel;
        }

        // changes the experiment to multi-channel type
        private void multichannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Experimental.CurrentType = Settings.Experimental.ExperimentType.MultiChannel;
        }
        
        // this binds some data to our graph
        private void graphToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // hides or unhides the graphs so our user cannot see them.
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
        /// This reconnects the serial port if the user did not plug it in when they should have.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
    }
}
