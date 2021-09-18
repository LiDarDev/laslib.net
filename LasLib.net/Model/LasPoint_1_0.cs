
///////////////////////////////////////////////////////////////////////
/// File    : LasPoint_1_0.cs
/// Desc    : Las point model for r1.0
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
    /// LAS1.0 Point model for format 0. 
    /// </summary>
    public class LasPoint_1_0_Format0 : LasPointBase
    {
        protected byte _file_marker;

        public byte file_marker { get => _file_marker; set => _file_marker = value; }

        protected ushort _user_bit_field;
        public ushort user_bit_field
        {
            get => _user_bit_field;
            set => _user_bit_field = value;
        }
    }

    /// <summary>
    /// LAS1.0 Point model for format 1. 
    /// </summary>
    public class LasPoint_1_0_Format1 : LasPoint_1_0_Format0
    {
        protected double _gps_time;
        public double gps_time { get => _gps_time; set => _gps_time = value; }
    }
}
