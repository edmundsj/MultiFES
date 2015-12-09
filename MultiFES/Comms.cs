using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace MultiFES
{
    /// <summary>
    /// This class handles all communication to and from the Arduino.
    /// Everything you send and do should be through this class.
    /// </summary>
    public static class Comms
    {
        /// <summary>
        /// Initializes our Comms class by detecting any available serial ports and setting
        /// the first detected serial port as the active serial port.
        /// </summary>
        public static bool Initialize()  
        {
            if (initialized == false)
            {
                ports = new List<SerialPort>();
                port_names = new List<String>();

                comm_timer.Interval = 10;
                comm_timer.Tick += new System.EventHandler(comm_timer_Tick);
                initialized = true;

                // now we try to find the serial port if there exists one that is newer
                try
                {
                    String[] possible_ports = SerialPort.GetPortNames();
                    for(int i = 0; i < possible_ports.Length; i++)
                    {
                        ports.Add(new SerialPort(possible_ports[i], Comms.BaudRate));
                        ports[i].DataReceived += new SerialDataReceivedEventHandler(Comms.Input.dataReceived);
                        port_names.Add(possible_ports[i]);
                        if(i == 0)
                        {
                            active_port = i;
                        }
                    }
                    if(possible_ports.Length == 0)
                    {
                        return false;
                    }

                } catch(Win32Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Detsroys our Comms class variables so it can be re-initialized. This should be called
        /// if the user needs to plug in a new device or plug an old one back in.
        /// </summary>
        public static void Destroy()
        {
            port_names.Clear();
            ports.Clear();
            initialized = false;
        }

        /// <summary>
        /// Uploads code to the arduino.
        /// </summary>
        /// <returns>0 for success, 1 for avrdude failure, 2 for connection failure</returns>
        public static int UploadCode()
        {
            if (Comms.Close()) // if we can successfully close the port
            {
                var proc = new Process();
                proc.StartInfo.FileName = Settings.AvrdudePath + "avrdude.exe";
                proc.StartInfo.Arguments = "-C\"" + Settings.AvrdudePath
                    + "avrdude.conf\" -cwiring -P" + Comms.ActivePort + " -patmega2560 -b115200 -D -Uflash:w:\""
                    + Settings.ArduinoPath + "binary_code.mega.hex:i";
                //proc.StartInfo.CreateNoWindow = true;
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                proc.Start();
                proc.WaitForExit();
                var exitCode = proc.ExitCode;
                proc.Close();
                if (exitCode == 0)
                {
                    return 0;
                }
                else
                {
                    return 1; // the exit code for an AVRDUDE failure
                }
            }
            else return 2; // the exit code for a communication failure
        }

        /// <summary>
        /// Opens the active serial port for communication. Returns true if successful.
        /// </summary>
        /// <returns>Returns true if successful.</returns>
        public static bool Open() // this opens our communication with the arduino by opening the port
        {
            if (IsOpen == false)
            {
                try
                {
                    if (ports[active_port].IsOpen == false)
                    {
                        ports[active_port].Open();
                    }
                    Comms.comm_timer.Enabled = true;
                    Comms.comm_timer.Start();
                    return true;

                }
                catch (NullReferenceException null_exception)
                {
                    MessageBox.Show("Please plug in your device and Click File->Connect.\n"
                        + null_exception.Message, "Arduino Error", MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation);
                    return false;
                }
                catch (Exception any_exception)
                {
                    MessageBox.Show("Unable to open communication with the Arduino. Please ensure it is " +
                       "properly plugged in. " + any_exception.Message, "Arduino Error", MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            else return true;
        }

        /// <summary>
        /// This opens a new port with a specific name for further use, and then 
        /// sets that port to the new active port. Returns true if successful
        /// </summary>
        /// <param name="port_name"></param>
        /// <returns>Returns true if successful.</returns>
        public static bool Open(String port_name) // this opens our communication with the arduino by opening the port
        {
            try
            {
                if (ports[active_port].IsOpen == false)
                {
                    if(port_names.Contains(port_name))
                    {
                        ports[port_names.IndexOf(port_name)].Open();
                        active_port = port_names.IndexOf(port_name);
                    }
                    ports[active_port].Open();
                }
                Comms.comm_timer.Enabled = true;
                Comms.comm_timer.Start();
                return true;
            }
            catch (Exception base_exception)
            {
                MessageBox.Show("Unable to open communication with the Arduino. Please ensure it is " +
                   "properly plugged in. " + base_exception.Message, "Arduino Error", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
                return false;
            }
        }

        /// <summary>
        /// Closes the active port. This also sends an abort signal to the FES system
        /// and sets all our node amplitudes to zero.
        /// </summary>
        /// <returns>Returns true if successful.</returns>
        public static bool Close()
        {
            try {
                if (active_port < ports.Count)
                {
                    if (ports[active_port].IsOpen == false)
                    {
                        ports[active_port].Open();

                    }
                }
                else return false;
                
                Comms.Abort();
                ports[active_port].Close();

                return true;
            }
            catch(System.IO.IOException io_ex)
            {
                MessageBox.Show("Unable to close communication with the Arduino. Please ensure it is " +
                   "properly plugged in. " + io_ex.Message, "Arduino Error", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
                return false;
            }
        }

        /// <summary>
        /// This enables our comm_timer so as to allow queued commands to be executed.
        /// </summary>
        public static void Enable()
        {
            comm_timer.Enabled = true;
        }

        /// <summary>
        /// This disables our comm_timer so as to prevent any queued commands from being executed.
        /// </summary>
        public static void Disable()
        {
            comm_timer.Enabled = false;
        }

        /// <summary>
        /// This sends the abort signal to the FES board.
        /// </summary>
        /// <returns>True if successfully sent, false if otherwise.</returns>
        public static bool Abort()
        {
            if (Comms.IsOpen == false)
            {
                Comms.Open(); // if the port is not open, open it.
            }

            Comms.Disable(); // disables the comm_timer to prevent further command execution
            Comms.Empty(); // empties out the buffer to stop any running command execution
            Thread.Sleep(6); // gives the arduino time to process any command we just sent to it.

            if (Comms.Send(new Command(Command.CommandType.EMERGENCY_OFF)) == true) // send abort signal
            {
                Nodes.setAllZero(); // set all our local variables to 0
                return true; // success.
            }
            return false;

        }

        /// <summary>
        /// Forces an execute of the commands in the queue.
        /// </summary>
        public static void Execute()
        {
            while(!Comms.Output.Buffer.IsEmpty)
            {
                Comms.Send(Comms.Output.Buffer.Pop());
            }
        }

        /// <summary>
        /// Reads a line from the active serial port.
        /// </summary>
        /// <returns>Returns the line read from the serial port</returns>
        public static string ReadLine()
        {
            try {
                if (Comms.IsOpen)
                {
                    return Comms.ports[active_port].ReadLine();
                }
            }
            catch(InvalidOperationException)
            {

            }
            catch (FormatException)
            {
                // do nothing. The arduino has been known to do this.
            }
            catch (IOException)
            {
                // OutputDebugMessage(L"We must have exited while receiving data. Awk.\n");
            }
            catch (OverflowException)
            {
                // OutputDebugMessage(L"The arduino is trying to beat us to death with data.\n")
            }

            return ""; // a null value
        }
        
        /// <summary>
        /// This is the base Send method of which all others are based. Sends a stream of raw bytes
        /// through the serial port to the arduino for processing.
        /// </summary>
        /// <param name="data"></param>
        public static bool Send(byte[] data)
        {
            if(ports[active_port].IsOpen)
            {
                Comms.ports[active_port].Write(data, 0, data.Length);
                Thread.Sleep(6); // so the arduino has time to process the data sent. Do not remove.
                if (Comms.Output.DebugEnabled == true)
                {
                    Debug.addStatement(data);
                }
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Sendns a user-created command readable by the arduino.
        /// </summary>
        /// <param name="c">Command to be sent.</param>
        public static bool Send(Command c)
        {
            return Comms.Send(c.Bytes);
        }
        

        /// <summary>
        /// This adds a set of bytes to the output buffer to be sent.
        /// All commands should be added to the queue during FES stimulation
        /// rather than using send directly. All commands sent not during stimulation
        /// should not use this method. This also makes sudden changes less sudden.
        /// </summary>
        /// <param name="data"></param>
        public static void Queue(byte[] data) // TO ADD: THE ABILITY OF THIS FUNCTION TO 
        {
            Comms.Output.Buffer.Add(data);
        }

        /// <summary>
        /// This empties the output buffer of commands. Note: does not empty the input buffer.
        /// </summary>
        public static void Empty()
        {
            Comms.Output.Buffer.Empty();
        }

        /// <summary>
        /// This executes every 10ms in the background once Comms.Open() is called and sends
        /// the next command in the queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static private void comm_timer_Tick(object sender, EventArgs e)
        {
            while (!Comms.Output.Buffer.IsEmpty) 
            {
               
                Comms.Send(Comms.Output.Buffer.Pop());
            }
        }

        /// <summary>
        /// This class handles all input sent to from the arduino. This includes load cell
        /// and potentially in the future, EMG data.
        /// </summary>
        public static class Input
        {
            /// <summary>
            /// Event handler that gets data from the arduino as we recieve it.
            /// </summary>
            public static void dataReceived(Object sender, SerialDataReceivedEventArgs args)
            {
                String data = Comms.ReadLine();
                Input.Buffer.addAndProcess(data);
                if (Comms.Input.DebugEnabled == true)
                {
                    Debug.addStatement("IN: " + data);
                }
            }

            /// <summary>
            /// This contains all data ever sent by the arduino since the opening of the serial port.
            /// </summary>
            public static class Buffer
            {
                /// <summary>
                /// Adds load cell data to the buffer after normalizing it to a percentage
                /// of the user's maximum voluntary contraction (MVC).
                /// </summary>
                /// <param name="data"></param>
                public static void addAndProcess(String data)
                {
                    int converted_data;
                    if (int.TryParse(data, out converted_data))
                    {
                        // first, we convert it to an unsigned 16-bit integer. 
                        //int converted_data = Convert.ToInt16(data);
                        double result = 0;
                        switch (Settings.LoadCellVoltage)
                        {
                            case 9:
                                {
                                    switch (Settings.LoadCellVoltage)
                                    {
                                        case 5:
                                            result = (-0.0089 * converted_data + 8.5675);
                                            break;
                                        case 10:
                                            result = (-0.0168 * converted_data + 12.62);
                                            break;
                                        case 20:
                                            result = (-0.0335 * converted_data + 21.209);
                                            break;
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    switch (Settings.LoadCellResistance)
                                    {
                                        case 5:
                                            result = (-0.0092 * converted_data + 8.4856);
                                            break;
                                        case 10:
                                            result = (-0.017 * converted_data + 12.857);
                                            break;
                                        case 20:
                                            result = (-0.0333 * converted_data + 21.25);
                                            break;
                                    }
                                    break;
                                }
                        }
                        result *= 9.8; // this allows the force to be represented in newtons.

                            /*
                        if (Data.Experimental.BaselineForce > -0.03 && Data.Experimental.BaselineForce < 0.2) // the normalized baseline
                        { // if our baseline is within the expected range
                            result -= Data.Experimental.BaselineForce;
                        }
                        */

                        // now we check our calibrated slopes to get a "real" value.

                        // add our data to the buffer
                        Contents.addData(Timekeeeper.ElapsedSeconds, result);

                        Data.AddData(Timekeeeper.ElapsedSeconds, result);
                        // also, if we are running an experiment, add the data to the experimental data
                    }
                }

                /// <summary>
                /// Empties the contents of the input buffer.
                /// </summary>
                public static void Empty()
                {
                    Contents.Clear();
                }
                
                /// <summary>
                /// The capsule contained by our input buffer that stores data
                /// </summary>
                public static Data.Capsule Contents { get; set; } = new Data.Capsule();
            }

            static bool debug_enabled;
            /// <summary>
            /// Set to true when output debugging is enabled.
            /// </summary>
            public static bool DebugEnabled
            {
                get
                {
                    return debug_enabled;
                }
                set
                {
                    debug_enabled = value;
                    // if the output debug is the only one to be true, set debug to true
                    if (value == true && Comms.Output.DebugEnabled == false)
                    {
                        Debug.Enabled = value;
                    }
                    else if (value == false && Comms.Output.DebugEnabled == false)
                    {
                        Debug.Enabled = value;
                    }
                }
            }
        }
        
        /// <summary>
        /// This class handles all the output going to the serial ports. However, it is only a structure.
        /// All communication should be done directly through the Comms class.
        /// </summary>
        public static class Output
        {
            /// <summary>
            /// This class contains the buffer of commands to be sent.
            /// Throws error code 0x003 if the byte array to send exceeds 100 bytes in length.
            /// </summary>
            public static class Buffer
            {
                /// <summary>
                /// Add content to the buffer
                /// </summary>
                /// <param name="byte_arr">The byte array of data to add</param>
                public static void Add(byte[] byte_arr)
                {
                    // check that the byte array is not of unreasonable length.
                    if(byte_arr.Length < 100)
                    {
                        bytes_to_send.Add(byte_arr);
                    }
                    else
                    {
                        throw new ArgumentException("Byte array to send is exceedingly large. Error code 0x003.");
                    }
                }

                /// <summary>
                /// Clears out the buffer of commands.
                /// </summary>
                public static void Empty()
                {
                    bytes_to_send.Clear();
                }

                /// <summary>
                /// Removes the byte array of the next command to send and returns it.
                /// Throws Error code 0x002 if there are no commands left to pop.
                /// </summary>
                /// <returns>The next byte array to send.</returns>
                internal static byte[] Pop()
                {
                    if (bytes_to_send.Count > 0)
                    {
                        byte[] new_arr = bytes_to_send[0];
                        bytes_to_send.RemoveAt(0);
                        return new_arr;
                    }
                    else
                    {
                        throw new IndexOutOfRangeException("No commands left to pop. Error code 0x002.");
                    }
                }

                /// <summary>
                /// Checks to see if the comms Output buffer is empty.
                /// </summary>
                public static bool IsEmpty
                {
                    get
                    {
                        if (bytes_to_send.Count == 0) return true;
                        else return false;
                    }
                }

                /// <summary>
                /// The raw bytes to be sent to the arduino.
                /// </summary>
                static List<byte[]> bytes_to_send = new List<byte[]>();
            }
            static bool debug_enabled;
            /// <summary>
            /// Set to true when output debugging is enabled.
            /// </summary>
            public static bool DebugEnabled
            {
                get
                {
                    return debug_enabled;
                }
                set
                {
                    debug_enabled = value;
                    // if the output debug is the only one to be true, set debug to true
                    if(value == true && Comms.Input.DebugEnabled == false)
                    {
                        Debug.Enabled = value;
                    }
                    else if(value == false && Comms.Input.DebugEnabled == false)
                    {
                        Debug.Enabled = value;
                    }
                }
            }
        }
        
        static bool initialized;

        /// <summary>
        /// The list of actual serial ports that perform communication. 
        /// These are set up at runtime based on curretnly connected devices with serial capability.
        /// </summary>
        static List<SerialPort> ports = null;

        /// <summary>
        /// The default baud rate of all devices communicating through the Comms class.
        /// </summary>
        public static int BaudRate { get; set; } = 19200;

        /// <summary>
        /// The list of port names (COM1, COM2, etc.) Set during runtime.
        /// </summary>
        static List<String> port_names;

        static int active_port;
        /// <summary>
        /// This is the index value of the active port
        /// </summary>
        public static String ActivePort {
            get
            {
                return port_names[active_port];
            }
            set
            {
                if (port_names.Contains(value))
                {
                    active_port = port_names.IndexOf(value);
                }
                else
                {
                    throw new ArgumentException("Cannot set the port to " + value + ", no such port exists.");
                }
            }
        }

        /// <summary>
        /// This class generates arduino-readable commands to be send
        /// via serial port with the Comms.Send(Command c) method.
        /// </summary>
        public class Command
        {
            /// <summary>
            /// This creates an arduino-readable command
            /// </summary>
            /// <param name="t">The type of command to execute (status change, amplitude change, frequency change)</param>
            /// <param name="id">The node ID on which to execute this command</param>
            /// <param name="v">The value to set this command (i.e. amplitude of 255)</param>
            public Command(CommandType t, int id, int v)
            {
                type = t;
                node_id = id;
                val = v;
                /*
                Our CommandType directly correlateds with the Arduino's commandtype
                According to our current encoding scheme, we do things like so:

                [00000000]  [0000]      [00]
                Value       Node        Command
                */
                uint data = 0;
                data = Convert.ToUInt16(type); // set to the Commandtype
                data += Convert.ToUInt16(node_id * 4);
                data += Convert.ToUInt16(val * 4 * 16);

                bytes[0] = (byte)Debug.extractBitData(data, 0, 7); // safe because we are only returning 8 places
                bytes[1] = (byte)Debug.extractBitData(data, 8, 15); // the rest of our data 
                Text = Convert.ToString(Convert.ToInt16(bytes[0]) + Convert.ToInt16(bytes[1]) * Convert.ToInt16(Math.Pow(2, 8)));
            }

            /// <summary>
            ///  This funciot allows us to create an EMERGENCY_OFF command with only one argument.
            ///  Throws error code 0x001 if the argument is not EMERGENCY_OFF
            /// </summary>
            /// <param name="t">can only be CommandType.EMERGENCY_OFF</param>
            public Command(CommandType t)
            {
                if (t == CommandType.EMERGENCY_OFF)
                {
                    this.bytes[0] = 3;
                    Thread.Sleep(10);
                }
                else
                {
                    throw new ArgumentException("Cannot create command with only argument type " + Convert.ToString(t)
                        + "; Error code 0x001");
                }
            }

            int node_id;
            /// <summary>
            /// The node id this command acts on.
            /// </summary>
            public int NodeID
            {
                get
                {
                    return node_id;
                }
            }

            /// <summary>
            /// The type of command to be sent (see Comms.Command.CommandType)
            /// </summary>
            public CommandType type { get; set; }

            int val;

            /// <summary>
            /// The value for a given command. 
            /// </summary>
            public int Value
            {
                get
                {
                    return val;
                }
                set
                {
                    val = value;
                }
            }
            byte[] bytes = new byte[2];

            /// <summary>
            /// The raw data a command contains.
            /// </summary>
            public byte[] Bytes { get
                {
                    return bytes;
                }
            }

            /// <summary>
            /// The raw text of the command
            /// </summary>
            public String Text { get; set; }
            

            /// <summary>
            /// This enumeration is directly linked to the binary data the arduino expects to receive. 
            /// As such it should not be changed. ever. Don't add things, don't move them around.
            /// </summary>
            public enum CommandType
            {
                /// <summary>
                /// The command that changes the amplitude for a given node
                /// </summary>
                AMPLITUDE_CHANGE,
                /// <summary>
                /// The command that sets a node to ACTIVE or DORMANT.
                /// </summary>
                STATUS_CHANGE,
                /// <summary>
                /// Command that changes the frequency of a node
                /// </summary>
                FREQUENCY_CHANGE,
                /// <summary>
                /// Emergency abort signal. Turns off all nodes.
                /// </summary>
                EMERGENCY_OFF
            };

            /// <summary>
            /// This enumeration is also directly linked to the binary data the arduino expects to receive
            /// As such it should not be changed. ever. Don't add things, don't move them around.
            /// </summary>
            public enum StatusType
            {
                /// <summary>
                /// Node will be pulsed at the frequency specified on the FES system
                /// </summary>
                ACTIVE,
                /// <summary>
                /// Node will not be pulsed, (not equivalent to zero amplitude)
                /// </summary>
                DORMANT
            }
        }
        /// <summary>
        /// The timer used to send communication in real-time to the arduino.
        /// </summary>
        static System.Windows.Forms.Timer comm_timer = new System.Windows.Forms.Timer();
        
        /// <summary>
        /// See whether our active communications port is open.
        /// </summary>
        public static bool IsOpen
        {
            get
            {
                if(Comms.ports[active_port].IsOpen == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    
}
