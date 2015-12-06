using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CSharpProject
{
    /// <summary>
    /// This class handles all debugging-related things. It is intended to be used in tandem
    /// with the UI, rather than acting as an auxiliary set of code.
    /// </summary>
    static class Debug
    {
        /// <summary>
        /// Checks to see if the debug Buffer is zero
        /// </summary>
        public static bool IsEmpty
        {
            get
            {
                if (statements.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Adds a statement to our statement buffer to be handled later.
        /// </summary>
        /// <param name="sta">The command to add</param>
        public static void addStatement(Comms.Command sta)
        {
            statements.Add(parseCommand(sta));
        }

        /// <summary>
        /// Adds a statement to our statement buffer to be handled later.
        /// </summary>
        /// <param name="sta">The raw data</param>
        public static void addStatement(byte[] arr)
        {
            statements.Add(parseCommand(arr));
        }

        // takes of the first debug statement on the list and returns it
        public static String PopStatement()
        {
            if(statements.Count > 0)
            {
                String temp = statements[0];
                statements.RemoveAt(0);
                return temp;
            }
            return "";
        }

        /// <summary>
        /// This extracts bit data from an unsigned integer, and returns it as an integer where the 1's place
        /// is shifted to the start bit.
        /// </summary>
        /// <param name="data">The raw data to be parsed</param>
        /// <param name="start_bit">The zero-indexed start bit</param>
        /// <param name="end_bit">The zero-indexed end bit</param>
        /// <returns></returns>
        public static ulong extractBitData(ulong data, ulong start_bit, uint end_bit)
        { // this extracts all the information in between the designated bits, in the form [start_bit, end_bit]
            ulong desired_data = 0;
            ulong mask = 1; // starts off as ...0001
            mask = mask * Convert.ToUInt64(Math.Pow(2, start_bit));
            for (ulong i = start_bit; i <= end_bit; i++)
            {
                desired_data += ((mask & data) / Convert.ToUInt64(Math.Pow(2, start_bit)));
                mask = mask << 1;
            }
            return desired_data;
        }

        /// <summary>
        /// This extracts bit data from an unsigned integer, and returns it as an integer where the 1's place
        /// is shifted to the start bit.
        /// </summary>
        /// <param name="data">The raw data to be parsed</param>
        /// <param name="start_bit">The zero-indexed start bit</param>
        /// <param name="end_bit">The zero-indexed end bit</param>
        /// <returns></returns>
        public static uint extractBitData(uint data, uint start_bit, uint end_bit)
        { // this extracts all the information in between the designated bits, in the form [start_bit, end_bit]
            uint desired_data = 0;
            uint mask = 1; // starts off as ...0001
            mask = mask * Convert.ToUInt32(Math.Pow(2, start_bit));
            for (uint i = start_bit; i <= end_bit; i++)
            {
                desired_data += ((mask & data) / Convert.ToUInt32(Math.Pow(2, start_bit)));
                mask = mask << 1;
            }
            return desired_data;
        }

        /// <summary>
        /// This parses a command into a debug statement to be printed to the screen 
        /// or written to file.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>Returns a string containing command info.</returns>
        public static String parseCommand(Comms.Command s)
        {
            Comms.Command.CommandType t = s.type;
            int node_id = s.NodeID;
            int val = s.Value;
            return Convert.ToString(s.Bytes[0] + s.Bytes[1]*256) + ": " + Convert.ToString(t) + ", " + Convert.ToString(node_id) + "," + val.ToString();
        }

        /// <summary>
        /// This parses a byte array into a debug statement to be printed to the screen 
        /// or written to file, and treats it as a Command.
        /// </summary>
        /// <param name="byte_arr">The raw byte array data</param>
        /// <returns>Returns a string containing command info.</returns>
        public static String parseCommand(byte[] byte_arr)
        {
            String return_str = "";
            for(int i = 0; i < byte_arr.Length; i++)
            {
                return_str += byte_arr[i].ToString();
            }
            if (byte_arr.Length == 2)
            {
                return_str += ": " + Convert.ToString((Comms.Command.CommandType)extractBitData(byte_arr[0], 0, 1)) + ", "; // the raw data sent
                return_str += Convert.ToString(extractBitData(byte_arr[0], 2, 5)) + ", ";
                uint value_data = extractBitData(byte_arr[0], 6, 7) + 4 * extractBitData(byte_arr[1], 0, 5);
                return_str += Convert.ToString(extractBitData(value_data, 0, 7));
            }
            else if(byte_arr.Length == 1) // the only single-byte data we send are abort signals.
            {
                return_str += Convert.ToString(byte_arr[0]) + ":";
                return_str += Convert.ToString((Comms.Command.CommandType)byte_arr[0]);
            }
            else
            {
                return_str = "INVALID COMMAND SENT.";
            }
            return return_str;
        }

        /// <summary>
        /// Whether debugging is enabled or not (whether it prints to screen or not)
        /// </summary>
        public static bool Enabled { get; set; }

        static List<String> statements = new List<String>();
    }
}
