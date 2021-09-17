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

namespace LasLibNet
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
            str = "\treserved = " + reserved.ToString() + "\n"
               + "\tuser_id  = " + BitConverter.ToString(user_id) + "\n"
               + "\trecord_id = " + record_id.ToString() + "\n"
               + "\trecord_length_after_header = " + this.record_length_after_header.ToString() + "\n"
               + "\tdescription = " + BitConverter.ToString(description) + "\n"
               + "\tdata = " + BitConverter.ToString(data);
            return str;
        }
    }
}
