///////////////////////////////////////////////////////////////////////
/// File    : Las_vlr.cs
/// Desc    : Las vlr.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Model
{
    /// <summary>
    /// LasVLR
    /// </summary>
    public class LasVLR
    {
        public ushort reserved;
        public byte[] user_id = new byte[16];
        public ushort record_id;
        public ushort record_length_after_header;
        public byte[] description = new byte[32];
        public byte[] data;

        public override string ToString()
        {
            string str = "";
            str = "reserved = " + reserved.ToString() + ","
               + "user_id  = " + Encoding.UTF8.GetString(user_id) + ","
               + "record_id = " + record_id.ToString() + ","
               + "record_length_after_header = " + this.record_length_after_header.ToString() + ","
               + "description = " + Encoding.UTF8.GetString(description) + ","
               + "data = " + BitConverter.ToString(data);
            return str;
        }
    }
}
