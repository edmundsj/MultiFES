using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CSharpProject
{
    static class Debug
    {

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

        // adds a statement to the debug text list
        public static void addStatement(Comms.Command sta)
        {
            statements.Add(sta);
        }

        // takes of the first debug statement on the list and returns it
        public static String chopStatement()
        {
            if(statements.Count > 0)
            {
                Comms.Command temp = statements[0];
                statements.RemoveAt(0);
                return parseCommand(temp);
            }
            return "";
        }

        public static ulong extractBitData(ulong data, ulong start_bit, uint end_bit)
        { // this extracts all the information in between the designated bits, in the form [start_bit, end_bit]
            ulong desired_data = 0;
            ulong mask = 1; // starts off as ...0001
                                    /*
                                    Serial.print("Starting at bit ");
                                    Serial.println(start_bit);
                                    */
            mask = mask * Convert.ToUInt64(Math.Pow(2, start_bit));
            for (ulong i = start_bit; i <= end_bit; i++)
            {
                desired_data += ((mask & data) / Convert.ToUInt64(Math.Pow(2, start_bit)));
                mask = mask << 1;
            }
            return desired_data;
        }

        public static uint extractBitData(uint data, uint start_bit, uint end_bit)
        { // this extracts all the information in between the designated bits, in the form [start_bit, end_bit]
            uint desired_data = 0;
            uint mask = 1; // starts off as ...0001
                            /*
                            Serial.print("Starting at bit ");
                            Serial.println(start_bit);
                            */
            mask = mask * Convert.ToUInt32(Math.Pow(2, start_bit));
            for (uint i = start_bit; i <= end_bit; i++)
            {
                desired_data += ((mask & data) / Convert.ToUInt32(Math.Pow(2, start_bit)));
                mask = mask << 1;
            }
            return desired_data;
        }

        public static String parseCommand(Comms.Command s)
        {
            Comms.Command.CommandType t = s.type;
            int node_id = s.NodeID;
            int val = s.Value;
            return Convert.ToString(s.Bytes[0] + s.Bytes[1]*256) + ": " + Convert.ToString(t) + ", " + Convert.ToString(node_id) + "," + val.ToString();
        }
        public static bool Enabled { get; set; }

        static List<Comms.Command> statements = new List<Comms.Command>();
    }
}
