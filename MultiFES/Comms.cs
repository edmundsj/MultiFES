using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

namespace CSharpProject
{
    // this class handles communication to and from the arduino
    static class Comms
    {
        public static void Initialize() // this initializes all things necessary. 
        {
            if (initialized == false)
            {
                comm_timer.Interval = 10;
                comm_timer.Tick += new EventHandler(comm_timer_Tick);
                initialized = true;

                // now we try to find the serial port if there exists one that is newer
                try
                {
                    String[] possible_ports = SerialPort.GetPortNames();
                    if(possible_ports.Length == 1)
                    {
                        Comms.PortName = possible_ports[0];
                    }

                    Comms.arduino_mega.initializePort();

                } catch(Win32Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public static void Open() // this opens our communication with the arduino by opening the port
        {
            Initialize();
            Empty();
            Comms.arduino_mega.openPort();
            Comms.comm_timer.Enabled = true;
            Comms.comm_timer.Start();
        }

        public static void Close() // this closes communication with the arduino
        {
            if(arduino_mega.IsOpen == false)
            {
                arduino_mega.openPort();
            }
            
            arduino_mega.sendAbortSignal();
            arduino_mega.closePort();
            Nodes.setAllZero(); // I would prefer not to have this here, but I'll live for now...

        }

        public static void Send(byte[] data)
        {
            Initialize();
            if(arduino_mega.IsOpen)
            {
                Comms.arduino_mega.sendRawData(data);
            }
        }

        // sends a command
        public static void Send(Command c)
        {
            Initialize();
            if (arduino_mega.IsOpen)
            {
                Comms.arduino_mega.sendRawData(c.Bytes);
            }
        }

        public static void Queue(byte[] data)
        {
            Comms.Output.Buffer.Add(data);
        }

        public static void Empty()
        {
            Initialize();
            Comms.Output.Buffer.Empty();
        }

        static private void comm_timer_Tick(object sender, EventArgs e)
        {
            while (!Comms.Output.Buffer.IsEmpty) 
            {
                Comms.Send(Comms.Output.Buffer.Pop());
            }
        }

        // this class handles all the data we are recieving
        public static class Input
        {
            public static void dataReceived(Object sender, SerialDataReceivedEventArgs args)
            {
                try
                {
                    String data = arduino_mega.readData();
                    Input.Buffer.addAndProcess(data);
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

            }

            // our input buffer
            public static class Buffer
            {
                // adds data to the buffer after some preprocessing
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
                    if(Timekeeeper.Experimental.Running)
                    {
                        Data.Experimental.ForceData.addData(Timekeeeper.ElapsedSeconds, result);
                    }
                }

                // empty the contents of the buffer
                public static void Clear()
                {
                    Contents.Clear();
                }
                
                public static void writeToFile(String filename)
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
                    for (int i = 0; i < Contents.Timestamps.Count; i++)
                    {
                        d += Convert.ToString(Contents.Timestamps[i]) + ",";
                        if (Contents.Count >= i)
                        {
                            d += Convert.ToString(Contents.Values[i]) + "\n";
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
                public static Data.Capsule Contents { get; set; } = new Data.Capsule();
                public static List<String> Inputs { get; set; } = new List<String>();
            }

            static bool debug_enabled;
            public static bool DebugEnabled
            {
                get
                {
                    return debug_enabled;
                }
                set // make sure to also set our debug class values
                {
                    debug_enabled = value;
                    Debug.Enabled = value;
                }
            }

        }
        // this class handles all the data we are sending
        public static class Output
        {
            // our output buffer
            public static class Buffer
            {
                public static void Add(byte[] add_these)
                {
                    // check that the byte array is not of unreasonable length.
                    if(add_these.Length < 100)
                    {
                        bytes_to_send.Add(add_these);
                    }
                }

                // clears out our bytes to send
                public static void Empty()
                {
                    bytes_to_send.Clear();
                }

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
                        throw new IndexOutOfRangeException("No commands left to pop. ");
                    }
                }

                public static bool IsEmpty
                {
                    get
                    {
                        if (bytes_to_send.Count == 0) return true;
                        else return false;
                    }
                }

                static List<byte[]> bytes_to_send = new List<byte[]>();
            }
        }

        // this class handles the implementation of communication with the arduino
        private class Arduino
        {

            public String readData()
            {
                if(this.IsOpen)
                {
                    return this.port.ReadLine();
                    
                }
                return "";
            }
            

            // initializes our arduino port for use, including adding our event handler.
            public void initializePort()
            {
                try {
                    if (this.port == null)
                    {
                        this.port = new SerialPort(Comms.PortName, Comms.BaudRate);
                        this.port.DataReceived += new SerialDataReceivedEventHandler(Comms.Input.dataReceived);
                    }
                    else
                    {
                        throw new IOException("Defined Error: Port has already been initialized. Try destroying the port first. ");
                    }
                }
                catch(IOException io_e)
                {
                    MessageBox.Show(io_e.Message);
                }
            }


            // destroys the arduino port.
            public void destroyPort()
            {
                try {
                    if (this.IsOpen == true)
                    {
                        throw new IOException("Defined Error: Please close the port before destroying. ");
                    }
                    this.port = null;
                }
                catch(IOException io_e)
                {
                    MessageBox.Show(io_e.Message);
                }
            }

            // opens the arduino port. This is the base function implement
            public void openPort()
            {
                try
                {
                    if (this == null)
                    {
                        throw new System.IO.IOException("Error: Port has not been initialized. ");
                    }
                    if (this.IsOpen == false) // only open the port if it is not already open.
                    {
                        this.port.Open();
                    }
                }

                catch (IOException io_e) // check and see if we properly initialized our port
                {
                    MessageBox.Show(io_e.Message);
                }
            }

            // closes the arduino port
            public void closePort()
            {
                try
                {
                    if(this == null)
                    {
                        throw new IOException("Defined Error: Port has not been initialized. ");
                    }

                    if(this.IsOpen == true)
                    {
                        this.port.Close();
                    }
                }
                catch(IOException io_e)
                {
                    MessageBox.Show(io_e.Message);
                }


            }
            
            public void sendAbortSignal()
            {
                if (this.IsOpen == false)
                {
                    this.openPort();
                }
                byte[] byte_arr = new byte[1];
                byte_arr[0] = 3;
                sendRawData(byte_arr); // this is the abort signal 
                this.closePort();
            }
            
            // this sends the raw data (only two bytes)
            public void sendRawData(byte[] byte_arr)
            {
                try
                {
                    this.port.Write(byte_arr, 0, byte_arr.Length);
                }
                catch (IOException io_ex)
                {
                    MessageBox.Show("Please try connecting the Arduino " + io_ex.Message);
                }
            }
            public bool IsOpen
            {
                get
                {
                    return port.IsOpen;
                }
            }
            SerialPort port = null;
        }

        static bool debug_enabled;
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
        static Arduino arduino_mega = new Arduino();
        static bool initialized;

        public static int BaudRate { get; set; } = 19200;
        public static String PortName { get; set; } = "COM3";
        public class Command
        {
            public Command(CommandType t, int id, int v)
            {
                type = t;
                node_id = id;
                val = v;
                // now the actual generation of the command (the encoding)
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

            public int NodeID // the node ID the command refers to 
            {
                get
                {
                    return node_id;
                }
                set
                {
                    node_id = value;
                }
            }

            public CommandType type { get; set; } // the command type (amplitude change, frequency change, etc.)
            int node_id;
            int val;
            public int Value // the value the command has 
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
            public byte[] Bytes { get
                {
                    return bytes;
                }
            }
            // a type of command we can send (i.e. status change, emergency off
            public enum CommandType
            {
                AMPLITUDE_CHANGE,
                STATUS_CHANGE,
                FREQUENCY_CHANGE,
                EMERGENCY_OFF
            };

            // a status that a node can have
            public enum StatusType
            {
                ACTIVE,
                DORMANT,
                RAMPING_UP,
                RAMPING_DOWN
            }
        }
        static System.Windows.Forms.Timer comm_timer = new System.Windows.Forms.Timer();
    }
    
}
