
///////////////////////////////////////////////////////////////////////
/// File    : LasPoint_1_3.cs
/// Desc    : Las point model for r1.2
/// Author  : Li G.Q.
/// Date    : 2021/9/17/
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Model
{
    /// <summary>
    /// LAS1.2 Point model for format 0. 
    /// </summary>
    public class LasPoint_1_3_Format0 : LasPoint_1_2_Format0
    {

    }

    /// <summary>
    /// LAS1.2 Point model for format 1. 
    /// </summary>
    public class LasPoint_1_3_Format1 : LasPoint_1_2_Format1
    {

    }

    /// <summary>
    /// LAS1.2 Point model for format 2. 
    /// </summary>
    public class LasPoint_1_3_Format2 : LasPoint_1_2_Format2
    {
      
    }

    /// <summary>
    /// LAS1.2 Point model for format 3. 
    /// </summary>
    public class LasPoint_1_3_Format3 : LasPoint_1_2_Format3
    {
       
    }

    /// <summary>
    /// LAS1.3 Point model for format 4. 
    /// </summary>
    public class LasPoint_1_3_Format4 : LasPoint_1_3_Format1
    {

        protected byte[] _wave_packet_descriptor_index = new byte[29];

        public byte[] wave_packet_descriptor_index
        {
            get => _wave_packet_descriptor_index;
            set => _wave_packet_descriptor_index = value;
        }

        protected ulong _byte_offset_to_waveform_data;  //8bytes
        public ulong byte_offset_to_waveform_data
        {
            get => _byte_offset_to_waveform_data;
            set => _byte_offset_to_waveform_data = value;
        }

        protected ulong _waveform_packet_size_in_bytes;
        public ulong waveform_packet_size_in_bytes
        {
            get => _waveform_packet_size_in_bytes;
            set => _waveform_packet_size_in_bytes = value;
        }


        protected float _return_Point_waveform_location;
        public float Return_Point_waveform_location { get => _return_Point_waveform_location; set => _return_Point_waveform_location = value; }

        protected float _xt, _yt, _zt;

        public float Xt { get => _xt; set => _xt = value; }
        public float Yt { get => _yt; set => _yt = value; }
        public float Zt { get => _zt; set => _zt = value; }


    }

}