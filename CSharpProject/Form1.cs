﻿using System;
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

        // now we create our graphs
        Data.Graph force_graph = new Data.Graph(Comms.Input.Buffer.Contents);
        Data.Graph amplitude_graph;

        public Form1()
        {
            InitializeComponent();
            InitializeOtherThings();
        }

        private void InitializeOtherThings()
        {
            // XMLWrapper.loadXMLSettings();            // load our initial settings data
            Settings.loadDefaultSettings();
            Comms.Initialize();
            Timekeeeper.Initialize();               // initializes our timekeeper
            Nodes.Initialize();                // initialize our node array based on the loaded settings
            initializeArray();                                // create our Nodes pad on the UI with all the buttons.
            updateUI();
            Nodes.setGlobalFrequency();         // set our initial frequencies to what we want
            //force_groupbox.Controls.Add(force_graph.wrapped_chart);
            // amplitude_groupbox.Controls.Add(amplitude_graph.wrapped_chart);

            Data.Capsule main_mvc_capsule = new Data.Capsule();


            // so in the "background", or asynchronously, we need to recieve MVC data from the arduino
        }

        // this creates our pad and it's dependent graph
        private void initializeArray()
        {

            for (int i = 0; i < Nodes.Count; i++)
            {
                // first we create our array pad
                Button temp_button = new Button();
                temp_button.Text = "CH " + Convert.ToString(Nodes.getNode(i).Index); // get the index of the node
                temp_button.Name = Convert.ToString(Nodes.getNode(i).Index); // how we will identify our node
                temp_button.Location = Nodes.getPoint(i);
                temp_button.Width = Nodes.getWidth(i);
                temp_button.Height = Nodes.getHeight(i);
                temp_button.Click += new System.EventHandler(Nodes.padClicked);
                Nodes.UIButtons.Add(temp_button);
                array_pad.Controls.Add(Nodes.getButtonByIndex(Nodes.getNode(i).Index));

                // now we create our data variables
                Data.Experimental.Amplitudes.Add(new Data.Capsule());

            }
            amplitude_graph = new Data.Graph(Data.Experimental.Amplitudes);

        }

        // updates the pad section of our UI with pretty colors.
        private void updatePadUI()
        {
            // first we want to highlight our selected node
            for (int i = 0; i < Nodes.UIButtons.Count; i++)
            {
                Nodes.UIButtons[i].BackColor = Color.LightGray;
            }
            
            Nodes.getButtonByIndex(Nodes.SelectedNode.Index).BackColor = Color.Coral;
            
        }

        // here we update all our UI components with relevant information from the settings and our
        // base MVC capsule.
        private void updateUI()
        {

            updatePadUI();
            //force_graph.Update();

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
                    debug_box.AppendText(Debug.chopStatement() + Environment.NewLine);
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

        // begins stimulation
        private void stimulate_button_Click(object sender, EventArgs e)
        {
            // if the user clicks "STIMULATE"
            if (stimulate_button.Text == "STIMULATE")
            {
                try
                {
                    Comms.Open();
                    Timekeeeper.General.Start();
                    stimulate_button.Text = "ABORT";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Defined Error, stimulate_button_Click(): There was a problem when trying to begin stimulation."
                        + ex.Message);
                }
            }
            // if the user clicks "ABORT"
            else
            {
                try
                {
                    Nodes.setAllZero();
                    Comms.Close();

                    stimulate_button.Text = "STIMULATE";
                    
                    // reset our timekeepers
                    Timekeeeper.Stop();
                    Timekeeeper.Reset();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Defined Error, stimulate_button_Click(): There was a problem when trying to begin stimulation."
                        + ex.Message);
                }
            }
            // then we set the button's text to the opposite of what it was
        }

        // this happens when we change a value on our frequency trackbar
        private void frequency_trackbar_Scroll(object sender, EventArgs e)
        {
            // make sure the selected node is valid.
            Nodes.SelectedNode.Frequency = ((TrackBar)sender).Value;
            
        }

        // this happens when we change the value on our amplitude trackbar
        private void amplitude_trackbar_Scroll(object sender, EventArgs e)
        {
            Nodes.SelectedNode.Amplitude = ((TrackBar)sender).Value;
            if (Nodes.SelectedNode.Amplitude > Nodes.SelectedNode.MaximumAmplitude)
            {
                Nodes.SelectedNode.MaximumAmplitude = Convert.ToInt16(Nodes.SelectedNode.Amplitude);
            }

        }

        // allows for debugging of our program
        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        // debugs the Arduino Output by pasting it all to the screen first
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

        // debugs the Arduino Input by pasting it all to the screen in raw form.
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

        // saves the data we had just recorded
        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // create the save box dialog
            SaveFileDialog save_file_dialog = new SaveFileDialog();
            save_file_dialog.InitialDirectory = Settings.CSV_PATH;


            if (save_file_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // NOTE: DATA.EXPERIMENTAL.FORCEDATA WILL ONLY HAVE DATA IF YOU ARE RUNNING AN EXPERIMENT (duh)
                Data.Experimental.ForceData.writeToFile(save_file_dialog.FileName + "_mvc-data_");  // write the file

                // write our amplitude data to file.
                for (int i = 0; i < Data.Experimental.Amplitudes.Count; i++)
                {
                    Data.Experimental.Amplitudes[i].writeToFile(save_file_dialog.FileName + "CH-"
                        + i.ToString() + "_amplitude-data_");
                }
                Comms.Input.Buffer.Clear(); // make sure our program doesn't crash.
                Data.Experimental.Clear();
            }
        }

        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // theoreticall, this should do stuff in the backgronud.
            Timekeeeper.Experimental.Start();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Nodes.rotateNodesAsync));
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

        // this uploads our arduino code.
        private void uploadCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] my_arr = System.IO.File.ReadAllBytes(Settings.ARDUINO_PATH + "\\binary_code.mega.hex");

                var proc = new Process();
                // this is the command that works that we want to execute
                // avrdude -C./avrdude.conf -cwiring -PCOM3 -patmega2560 -b115200 -D -Uflash:w:../binary_code.mega.hex:i
                // with fully qualified filenames
                // avrdude "-CC:/Users/Xerxes/Google Drive/BCI_Implant_Group/FES/code/GUI/CSharpProject/CSharpProject/Arduino/avrdude/avrdude.conf" -cwiring -PCOM3 -patmega2560 -b115200 -D -Uflash:w:"C:/Users/Xerxes/Google Drive/BCI_Implant_Group/FES/code/GUI/CSharpProject/CSharpProject/Arduino/binary_code.mega.hex:i"
                // THIS works: 
                //avrdude "-CC:/Users/Xerxes/Google Drive/BCI_Implant_Group/FES/code/GUI/CSharpProject/CSharpProject/Arduino/avrdude/avrdude.conf" -cwiring -PCOM3 -patmega2560 -b115200 -D -Uflash:w:"C:/Users/Xerxes/Google Drive/BCI_Implant_Group/FES/code/GUI/CSharpProject/CSharpProject/Arduino/binary_code.mega.hex:i"
                proc.StartInfo.FileName = Settings.AVRDUDE_PATH + "avrdude.exe";
                proc.StartInfo.Arguments = "-C\"" + Settings.AVRDUDE_PATH +
                    "avrdude.conf\" -cwiring -P" + Comms.PortName + " -patmega2560 -b115200 -D -Uflash:w:\"" + Settings.ARDUINO_PATH + "binary_code.mega.hex:i";
                //proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                proc.Start();
                proc.WaitForExit();
                var exitCode = proc.ExitCode;
                proc.Close();

            }
            catch (System.IO.IOException io_ex)
            {
                MessageBox.Show("The arduino binary files appear to have been moved. Please move them back. " + io_ex.Message);
            }
        }
    }
}
