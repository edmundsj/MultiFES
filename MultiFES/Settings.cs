using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace CSharpProject
{
    public static class Settings
    {
        // loads our array settings from XML into the settings class
        public static void loadArray(String filename)
        {
            filename = Path.Combine(CSV_PATH, filename); // add the CSV path
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
        // loads our load cell data into the settings class
        public static void loadLoadCell(String filename)
        {
            ProtocolVariable current_property = 0;
            filename = Path.Combine(XML_PATH, filename);
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

        // loads our microcontroller settings from XML into the settings class
        public static void loadMicrocontroller(String filename)
        {
            ProtocolVariable current_property = 0;
            filename = Path.Combine(XML_PATH, filename);
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

        // loads our stimulation protocol paramaters from XML into the settings class
        public static void loadProtocol(String filename)
        {
            filename = Path.Combine(XML_PATH, filename); // add the XML path
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

        // load our complete set of settigs from XML.
        public static bool loadSettings(String filename)
        {
            try
            {
                filename = Path.Combine(XML_PATH, filename);
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
                                loadProtocol(Path.Combine(XML_PATH, xreader.Value));
                                break;
                            case SettingsFiles.ELECTRODE_ARRAY_FILE:
                                loadArray(Path.Combine(XML_PATH, xreader.Value));
                                break;
                            case SettingsFiles.MICROCONTROLLER_FILE:
                                loadMicrocontroller(Path.Combine(XML_PATH, xreader.Value));
                                break;
                            case SettingsFiles.LOAD_CELL_FILE:
                                loadLoadCell(Path.Combine(XML_PATH, xreader.Value));
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

        // load our default settings.
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

        public enum Property
        {
            X_COORDINATE,
            Y_COORDINATE,
            SHAPE,
            WIDTH,
            HEIGHT,
            INDEX,
            STATUS,
        };

        public enum NodeStatus
        {
            ACTIVE,
            DORMANT
        }


        // static bool autonomous_stimulation;
        public static int GlobalFrequency { set; get; }
        public static int GlobalMaximumAmplitude { set; get; }
        public static double GlobalRampTime { set; get; }

        public static int BaudRate { set; get; }
        public static String PortName { set; get; }
        public static uint LoadCellVoltage { set; get; }
        public static uint LoadCellResistance { set; get; }

        public static List<uint> NodeIDs { set; get; } = new List<uint>();
        public static List<int> NodeXCoors { set; get; } = new List<int>();
        public static List<int> NodeYCoors { set; get; } = new List<int>();
        public static List<int> Widths { set; get; } = new List<int>();
        public static List<int> Heights { set; get; } = new List<int>();
        
        public static String BASE_PATH { set; get; } = Application.StartupPath;
        public static String XML_PATH { set; get; } = BASE_PATH + "\\Config\\";
        public static String CSV_PATH { set; get; } = BASE_PATH + "\\Data\\";
        public static String ARDUINO_PATH { set; get; } = BASE_PATH + "\\Arduino\\";
        public static String AVRDUDE_PATH { set; get; } = ARDUINO_PATH + "\\avrdude\\";

        public static class Experimental
        {
            public static int Interval { get; set; } = 15; // how often we rotate between nodes
            public static int StepsPerSecond { get; set; } = 20; // our steps per second
            public static int Duration { get; set; } = 120; // how long the experiment lasts total
            public static bool SaveAfter { get; set; } // do we want to automatically save afterwards?
            public static double RampTime { get; set; } = 2.0; // how long is our switching time?
            public static ExperimentType CurrentType { set; get; }

            public enum ExperimentType
            {
                SingleChannel,
                MultiChannel
            };
            static double down_delay = 0.6;
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
            static int maximum_steps = 20; // default value
            public static int MaximumSteps
            {
                get
                {
                    return maximum_steps;
                }
                set
                {
                    if (value > 0 && value < 255) // is a reasonable value 
                        maximum_steps = value;
                }
            }
        }
    }
}
