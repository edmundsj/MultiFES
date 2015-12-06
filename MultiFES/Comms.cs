using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace CSharpProject
{
    // this class handles communication to and from the arduino
    static class Comms
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
                comm_timer.Tick += new EventHandler(comm_timer_Tick);
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
                proc.StartInfo.FileName = Settings.AVRDUDE_PATH + "avrdude.exe";
                proc.StartInfo.Arguments = "-C\"" + Settings.AVRDUDE_PATH
                    + "avrdude.conf\" -cwiring -P" + Comms.ActivePort + " -patmega2560 -b115200 -D -Uflash:w:\""
                    + Settings.ARDUINO_PATH + "binary_code.mega.hex:i";
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
            try {
                if (ports[active_port].IsOpen == false)
                {
                    ports[active_port].Open();
                }
                Comms.comm_timer.Enabled = true;
                Comms.comm_timer.Start();
                return true;
                
            }
            catch(NullReferenceException null_exception)
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

                Nodes.setAllZero(); // I would prefer not to have this here, but I'll live for now...
                Comms.Send(new Command(Comms.Command.CommandType.EMERGENCY_OFF));
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
        /// Reads a line from the active serial port.
        /// </summary>
        /// <returns>Returns the line read from the serial port</returns>
        public static string ReadLine()
        {
            try {
                return Comms.ports[active_port].ReadLine();
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
        public static void Send(byte[] data)
        {
            if(ports[active_port].IsOpen)
            {
                Comms.ports[active_port].Write(data, 0, data.Length);
                Thread.Sleep(6); // so the arduino has time to process the data sent. Do not remove.
            }
            if(DebugEnabled == true)
            {
                Debug.addStatement(data);
            }
        }

        /// <summary>
        /// Sendns a user-created command readable by the arduino.
        /// </summary>
        /// <param name="c">Command to be sent.</param>
        public static void Send(Command c)
        {
            if (ports[active_port].IsOpen)
            {
                Comms.Send(c.Bytes);
            }
        }

        /// <summary>
        /// This adds a set of bytes to the output buffer to be sent.
        /// All commands should be added to the queue during FES stimulation
        /// rather than using send directly. All commands sent not during stimulation
        /// should not use this method.
        /// </summary>
        /// <param name="data"></param>
        public static void Queue(byte[] data)
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
            public static void dataReceived(Object sender, SerialDataReceivedEventArgs args)
            {
                String data = Comms.ReadLine();
                Input.Buffer.addAndProcess(data);
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
                    // first, we convert it to an unsigned 16-bit integer. 
                    int converted_data = Convert.ToInt16(data);
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
                    if (Data.Experimental.MaxForce > 4)
                    { // our max MVC should have at least a value of 4kg.
                        result /= Data.Experimental.MaxForce;
                    }

                    if (Data.Experimental.BaselineForce > -0.03 && Data.Experimental.BaselineForce < 0.2) // the normalized baseline
                    { // if our baseline is within the expected range
                        result -= Data.Experimental.BaselineForce;
                    }

                    // now we check our calibrated slopes to get a "real" value.
                    
                    // add our data to the buffer
                    Contents.addData(Timekeeeper.ElapsedSeconds, result);

                    // also, if we are running an experiment, add the data to the experimental data
                    if(Timekeeeper.Experimental.IsRunning)
                    {
                        Data.Experimental.ForceData.addData(Timekeeeper.ElapsedSeconds, result);
                    }
                }

                /// <summary>
                /// Empties the contents of the input buffer.
                /// </summary>
                public static void Empty()
                {
                    Contents.Clear();
                }
                
                
                public static Data.Capsule Contents { get; set; } = new Data.Capsule();
                public static List<String> Inputs { get; set; } = new List<String>();
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
                public static void Add(byte[] add_these)
                {
                    // check that the byte array is not of unreasonable length.
                    if(add_these.Length < 100)
                    {
                        bytes_to_send.Add(add_these);
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
                Debug.Enabled = value;
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
            /// <param name="type">The type of command to execute (status change, amplitude change, frequency change)</param>
            /// <param name="node id">The node ID on which to execute this command</param>
            /// <param name="data value">The value to set this command (i.e. amplitude of 255)</param>
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

            }

            /// <summary>
            ///  This funciot allows us to create an EMERGENCY_OFF command with only one argument.
            ///  Throws error code 0x001 if the argument is not EMERGENCY_OFF
            /// </summary>
            /// <param name="type">can only be CommandType.EMERGENCY_OFF</param>
            public Command(CommandType t)
            {
                if (t == CommandType.EMERGENCY_OFF)
                {
                    this.bytes[0] = 3;
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
            /// This enumeration is directly linked to the binary data the arduino expects to receive. 
            /// As such it should not be changed. ever. Don't add things, don't move them around.
            /// </summary>
            public enum CommandType
            {
                AMPLITUDE_CHANGE,
                STATUS_CHANGE,
                FREQUENCY_CHANGE,
                EMERGENCY_OFF
            };

            /// <summary>
            /// This enumeration is also directly linked to the binary data the arduino expects to receive
            /// As such it should not be changed. ever. Don't add things, don't move them around.
            /// </summary>
            public enum StatusType
            {
                ACTIVE,
                DORMANT
            }
        }
        /// <summary>
        /// The timer used to send communication in real-time to the arduino.
        /// </summary>
        static System.Windows.Forms.Timer comm_timer = new System.Windows.Forms.Timer();

    }
    
}
