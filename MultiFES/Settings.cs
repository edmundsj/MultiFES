using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace MultiFES
{
    /// <summary>
    /// The class responsible for loading of XML configuration files and storing some
    /// runtime variables important to stimulation paramaters.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Loads the XML file for our node array
        /// </summary>
        /// <param name="filename"></param>
        public static void loadArray(String filename)
        {
            filename = Path.Combine(DataPath, filename); // add the CSV path
            XmlReader xreader = XmlReader.Create(filename);
            int active_node = 0;
            Property current_attribute = 0;
            int temp_x_coor = -1;
            int temp_y_coor = -1;
            int temp_width = -1;
            int temp_height = -1;
            uint temp_id = 0;

            while (xreader.Read())
            {
                if (xreader.NodeType == XmlNodeType.Element)
                {
                    if (xreader.Name == "Node")
                    { // we have begun reading data from a node
                      // do nothing
                    }
                    else if (xreader.Name == "node_id")
                    {
                        current_attribute = Property.INDEX;
                    }
                    else if (xreader.Name == "x_coordinate")
                    {
                        current_attribute = Property.X_COORDINATE;
                    }
                    else if (xreader.Name == "y_coordinate")
                    {
                        current_attribute = Property.Y_COORDINATE;
                    }
                    else if (xreader.Name == "shape")
                    {
                        current_attribute = Property.SHAPE;
                    }
                    else if (xreader.Name == "width")
                    {
                        current_attribute = Property.WIDTH;
                    }
                    else if (xreader.Name == "height")
                    {
                        current_attribute = Property.HEIGHT;
                    }

                }
                else if (xreader.NodeType == XmlNodeType.Text)
                {
                    switch (current_attribute)
                    {
                        case Property.INDEX:
                            {
                                temp_id = System.Convert.ToUInt16(xreader.Value);
                                break;
                            }
                        case Property.X_COORDINATE:
                            {
                                temp_x_coor = System.Convert.ToInt16(xreader.Value);
                                break;
                            }
                        case Property.Y_COORDINATE:
                            {
                                temp_y_coor = System.Convert.ToInt16(xreader.Value);
                                break;
                            }
                        case Property.WIDTH:
                            {
                                temp_width = System.Convert.ToInt16(xreader.Value);
                                break;
                            }
                        case Property.HEIGHT:
                            {
                                temp_height = System.Convert.ToInt16(xreader.Value);
                                break;
                            }

                    }
                }
                else if (xreader.NodeType == XmlNodeType.EndElement)
                { // we have reached the end of a node.
                    if (xreader.Name == "Node")
                    {
                        NodeIDs.Add(temp_id);
                        NodeXCoors.Add(temp_x_coor);
                        NodeYCoors.Add(temp_y_coor);
                        Widths.Add(temp_width);
                        Heights.Add(temp_height);
                        active_node++;
                    }
                }
            }
            xreader.Close();
        }

        /// <summary>
        /// Loads information about our load cell from XML.
        /// </summary>
        /// <param name="filename"></param>
        public static void loadLoadCell(String filename)
        {
            ProtocolVariable current_property = 0;
            filename = Path.Combine(ConfigPath, filename);
            XmlReader xreader = XmlReader.Create(filename);
            while (xreader.Read())
            {
                if (xreader.NodeType == XmlNodeType.Element)
                {
                    if (xreader.Name == "voltage")
                    {
                        current_property = ProtocolVariable.VOLTAGE;
                    }
                    else if (xreader.Name == "resistance")
                    {
                        current_property = ProtocolVariable.RESISTANCE;
                    }
                }
                else if (xreader.NodeType == XmlNodeType.Text)
                {
                    switch (current_property)
                    {
                        case ProtocolVariable.VOLTAGE:
                            LoadCellVoltage = Convert.ToUInt32(xreader.Value);
                            break;
                        case ProtocolVariable.RESISTANCE:
                            LoadCellResistance = Convert.ToUInt32(xreader.Value);
                            break;
                        default:
                            XmlException xml_e = new XmlException("In Capsule.Settings.LoadCell.loadLoadCell(): Unable to find protocol variable " +
                                 System.Convert.ToString(current_property));
                            throw xml_e;
                    }
                }
            }
            xreader.Close();
        }

        /// <summary>
        /// Loads information about our microcontroller defaults from XML.
        /// </summary>
        /// <param name="filename"></param>
        public static void loadMicrocontroller(String filename)
        {
            ProtocolVariable current_property = 0;
            filename = Path.Combine(ConfigPath, filename);
            XmlReader xreader = XmlReader.Create(filename);
            while (xreader.Read())
            {
                if (xreader.NodeType == XmlNodeType.Element)
                {
                    if (xreader.Name == "port_name")
                    {
                        current_property = ProtocolVariable.PORT_NAME;
                    }
                    else if (xreader.Name == "baud_rate")
                    {
                        current_property = ProtocolVariable.BAUD_RATE;
                    }
                }
                else if (xreader.NodeType == XmlNodeType.Text)
                {
                    switch (current_property)
                    {
                        case ProtocolVariable.PORT_NAME:
                            PortName = xreader.Value;
                            break;
                        case ProtocolVariable.BAUD_RATE:
                            BaudRate = System.Convert.ToInt16(xreader.Value);
                            break;
                        default:
                            XmlException xml_e = new XmlException("Defined Error: In Capsule.Settings.Microcontroller.loadMicrocontroller(): Unable to find node type " +
                                 System.Convert.ToString(current_property));
                            throw xml_e;
                    }
                }
            }
            xreader.Close();
        }

        /// <summary>
        /// Loads stimulation protocol information from XML.
        /// </summary>
        /// <param name="filename"></param>
        public static void loadProtocol(String filename)
        {
            filename = Path.Combine(ConfigPath, filename); // add the XML path
            XmlReader xreader = XmlReader.Create(filename);
            ProtocolVariable current_attribute = 0;

            while (xreader.Read())
            { // the xreader proceeds one node at a time.
                if (xreader.NodeType == XmlNodeType.Element)
                {
                    if (xreader.Name == "autonomous_stimulation")
                    {
                        current_attribute = ProtocolVariable.AUTONOMOUS_STIMULATION;
                    }
                    else if (xreader.Name == "frequency")
                    {
                        current_attribute = ProtocolVariable.FREQUENCY;
                    }
                    else if (xreader.Name == "nominal_amplitude")
                    {
                        current_attribute = ProtocolVariable.NOMINAL_AMPLITUDE;
                    }
                    else if (xreader.Name == "maximum_amplitude")
                    {
                        current_attribute = ProtocolVariable.MAXIMUM_AMPLITUDE;
                    }
                    else if (xreader.Name == "amplitude_ramp_time")
                    {
                        current_attribute = ProtocolVariable.AMPLITUDE_RAMP_TIME;
                    }

                }
                else if (xreader.NodeType == XmlNodeType.Text)
                {
                    switch (current_attribute)
                    {
                        case ProtocolVariable.AUTONOMOUS_STIMULATION:
                            {
                                if (xreader.Value == "true")
                                {
                                    // autonomous_stimulation = true;
                                }
                                else if (xreader.Value == "false")
                                {
                                    // autonomous_stimulation = false;
                                }
                                else
                                {
                                    throw new System.Exception("Defined Error: In Capsule.Settings.Protocol.loadProtocol(): Unable to parse value" +
                                        xreader.Value + "in case AUTONOMOUS_STIMULATION ");
                                }
                                break;
                            }

                        case ProtocolVariable.FREQUENCY:
                            {
                                GlobalFrequency = System.Convert.ToInt16(xreader.Value);
                                break;
                            }


                        case ProtocolVariable.MAXIMUM_AMPLITUDE:
                            {
                                GlobalMaximumAmplitude = System.Convert.ToInt16(xreader.Value);
                                break;
                            }

                        case ProtocolVariable.AMPLITUDE_RAMP_TIME:
                            {
                                GlobalRampTime = System.Convert.ToDouble(xreader.Value);
                                break;
                            }


                    }
                }
                else if (xreader.NodeType == XmlNodeType.EndElement)
                { // we have reached the end of a node.
                    if (xreader.Name == "Node")
                    {

                    }
                }
            }
            xreader.Close();
        }

        /// <summary>
        /// Loads a set of settings from a file location with the same name as the settings filename,
        /// but with the folder extension _settings afterwards.
        /// </summary>
        /// <param name="filename">The relative filename from the Config directory</param>
        /// <returns>Returns true if settings successfully loaded, false otherwise</returns>
        public static bool loadSettings(String filename)
        {
            try
            {
                filename = Path.Combine(ConfigPath, filename);
                filename += ".xml";
                XmlReader xreader = XmlReader.Create(filename);
                SettingsFiles file_type = SettingsFiles.PROTOCOL_FILE; // arbitrary choice

                while (xreader.Read())
                {
                    if (xreader.NodeType == XmlNodeType.Element)
                    {
                        if (xreader.Name == "defaults")
                        {
                            // do nothing, we are at the root node
                        }
                        else if (xreader.Name == "protocol_file")
                        {
                            file_type = SettingsFiles.PROTOCOL_FILE;
                        }
                        else if (xreader.Name == "electrode_array_file")
                        {
                            file_type = SettingsFiles.ELECTRODE_ARRAY_FILE;
                        }
                        else if (xreader.Name == "microcontroller_file")
                        {
                            file_type = SettingsFiles.MICROCONTROLLER_FILE;
                        }
                        else if (xreader.Name == "load_cell_file")
                        {
                            file_type = SettingsFiles.LOAD_CELL_FILE;
                        }
                        else if (xreader.Name == "data_directory")
                        {
                            file_type = SettingsFiles.DATA_DIRECTORY;
                        }
                        else
                        {
                            throw new XmlException("Defined Error: Base Settings file appears to have an improper node." + Convert.ToString(file_type));
                        }
                    }
                    else if (xreader.NodeType == XmlNodeType.Text)
                    {
                        switch (file_type)
                        {
                            case SettingsFiles.PROTOCOL_FILE:
                                loadProtocol(Path.Combine(ConfigPath, xreader.Value));
                                break;
                            case SettingsFiles.ELECTRODE_ARRAY_FILE:
                                loadArray(Path.Combine(ConfigPath, xreader.Value));
                                break;
                            case SettingsFiles.MICROCONTROLLER_FILE:
                                loadMicrocontroller(Path.Combine(ConfigPath, xreader.Value));
                                break;
                            case SettingsFiles.LOAD_CELL_FILE:
                                loadLoadCell(Path.Combine(ConfigPath, xreader.Value));
                                break;
                            default:
                                XmlException xml_ex = new XmlException("Defined Error: Unable to identify node type in settings file.");
                                throw xml_ex;
                        }

                    }
                }
                xreader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error loading the settings. Please check the default XML files." + ex.Message,
                     "Load Settings Failure", MessageBoxButtons.OK,
                   MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loads the default xml settings from the default_settings folder.
        /// </summary>
        /// <returns></returns>
        public static bool loadDefaultSettings()
        {
            return loadSettings("default");
        }
        
        enum SettingsFiles
        {
            LOAD_CELL_FILE,
            MICROCONTROLLER_FILE,
            PROTOCOL_FILE,
            ELECTRODE_ARRAY_FILE,
            DATA_DIRECTORY
        };

        enum ProtocolVariable
        {
            AUTONOMOUS_STIMULATION,
            FREQUENCY,
            NOMINAL_AMPLITUDE,
            MAXIMUM_AMPLITUDE,
            AMPLITUDE_RAMP_TIME,

            VOLTAGE,
            RESISTANCE,

            PORT_NAME,
            BAUD_RATE,

            NODE,
            NODE_ID,
            LOAD_CELL
        };
        
        enum Property
        {
            X_COORDINATE,
            Y_COORDINATE,
            SHAPE,
            WIDTH,
            HEIGHT,
            INDEX,
            STATUS,
        };

        /// <summary>
        /// The status of a node to send to the arduino, can be on or off.
        /// </summary>
        public enum NodeStatus
        {
            /// <summary>
            /// Active Status for a node. Causes the node to begin to fire.
            /// </summary>
            ACTIVE,

            /// <summary>
            /// Inactive status for a node. Causes the node to stop firing
            /// </summary>
            DORMANT
        }


        /// <summary>
        /// The global frequency for all of our nodes, only enforced on startup
        /// </summary>
        public static int GlobalFrequency { set; get; }
        /// <summary>
        /// The global maximum amplitude for all our nodes, not currently used.
        /// </summary>
        public static int GlobalMaximumAmplitude { set; get; }
        /// <summary>
        /// The global ramp time for all our nodes, not currently used.
        /// </summary>
        public static double GlobalRampTime { set; get; }

        /// <summary>
        /// The default baud rate for any serial devices, typically 19200.
        /// </summary>
        public static int BaudRate { set; get; }
        /// <summary>
        /// The default port name for a serial device. Not currently used.
        /// Now we load serial port names dynamically as they are available.
        /// </summary>
        public static String PortName { set; get; }
        /// <summary>
        /// The running voltage to our load cell. Can be set to 5 or 9V.
        /// Used after initialization.
        /// </summary>
        public static uint LoadCellVoltage { set; get; }
        /// <summary>
        /// The resistance across the part of our load cell. Set to 5, 10, or 20 ohms.
        /// Used after initialization.
        /// </summary>
        public static uint LoadCellResistance { set; get; }

        /// <summary>
        /// Node ids loaded in from the settings file. Not used after initialization.
        /// </summary>
        public static List<uint> NodeIDs { set; get; } = new List<uint>();
        /// <summary>
        /// The x coordinates for the nodes on our UI. Not used after initialization.
        /// </summary>
        public static List<int> NodeXCoors { set; get; } = new List<int>();
        /// <summary>
        /// The y coordinates for the nodes on our UI. Not used after initialization.
        /// </summary>
        public static List<int> NodeYCoors { set; get; } = new List<int>();
        /// <summary>
        /// The widths of each of our nodes on our UI. Not used after initialization.
        /// </summary>
        public static List<int> Widths { set; get; } = new List<int>();
        /// <summary>
        /// The heights of our of the nodes on our UI. Not used after initialization.
        /// </summary>
        public static List<int> Heights { set; get; } = new List<int>();
        
        /// <summary>
        /// The base path for all files and data related to this program.
        /// Used after initialization.
        /// </summary>
        public static String BasePath { set; get; } = Application.StartupPath;
        /// <summary>
        /// The path for all configuration files. Used after initialization.
        /// </summary>
        public static String ConfigPath { set; get; } = BasePath + "\\Config\\";
        /// <summary>
        /// The path for all stored data files. Used after initialization. 
        /// </summary>
        public static String DataPath { set; get; } = BasePath + "\\Data\\";
        /// <summary>
        /// The path for all arduino-related files. Used after initialization.
        /// </summary>
        public static String ArduinoPath { set; get; } = BasePath + "\\Arduino\\";
        /// <summary>
        /// The path for all avrdude-related files. Used after initialization.
        /// </summary>
        public static String AvrdudePath { set; get; } = ArduinoPath + "\\avrdude\\";

        /// <summary>
        /// All our experimental settings. 
        /// All of these are used after initialization and can be affected by the UI.
        /// </summary>
        public static class Experimental
        {
            /// <summary>
            /// The amount of time between node switches. Default value 15.
            /// </summary>
            public static int Interval { get; set; } = 15; // how often we rotate between nodes
            /// <summary>
            /// The number of steps the system can take per second (amplitude changes). Default value 20.
            /// </summary>
            public static int StepsPerSecond { get; set; } = 20; // our steps per second
            /// <summary>
            /// The experimental duration. Default value 120 seconds.
            /// </summary>
            public static int Duration { get; set; } = 120; // how long the experiment lasts total
            /// <summary>
            /// The experimental ramp time. Not related to GlobalRampTime. Default value 2 seconds.
            /// </summary>
            public static double RampTime { get; set; } = 2.0; // how long is our switching time?
            /// <summary>
            /// The current type of our experiment. Single channel/multi channel
            /// </summary>
            public static ExperimentType CurrentType { set; get; }

            /// <summary>
            /// The type of experiment we are conducting. Single or multi channel.
            /// </summary>
            public enum ExperimentType
            {
                /// <summary>
                /// Stimulation only sent over a single channel. No switching involved.
                /// </summary>
                SingleChannel,
                /// <summary>
                /// Stimulation sent over two or more channels. Dynamic switching involved.
                /// </summary>
                MultiChannel
            };
            static double down_delay = 0.6;
            /// <summary>
            /// How long we wait after beginning to ramp up a new node to ramp down the old one
            /// as a fraction of the total ramp time.
            /// </summary>
            public static double DownDelay
            {
                get
                {
                    return down_delay;
                }
                set
                {
                    if (value > 1.0 || value < 0)
                    {
                        throw new ArgumentOutOfRangeException("Defined Error, ActiveIntersection: Cannot set " +
                            "our value to greater than 1 or less than zero.");
                    }
                    else
                    {
                        down_delay = value;
                    }
                }
            }
        }
    }
}
