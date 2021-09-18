

///////////////////////////////////////////////////////////////////////
/// File    : LasPoint_1_1.cs
/// Desc    : Las point model for r1.1
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
    /// LAS1.1 Point model for format 0. 
    /// </summary>
    public class LasPoint_1_1_Format0:LasPointBase
    {
        protected byte _user_data;
        public byte user_data { get => _user_data; set => _user_data = value; }

        protected ushort _point_source_ID;
        public ushort point_source_ID { get => _point_source_ID; set => _point_source_ID = value; }

    }

    /// <summary>
    /// LAS1.1 Point model for format 1. 
    /// </summary>
    public class LasPoint_1_1_Format1 : LasPoint_1_1_Format0
    {
        protected double _gps_time;
        public double gps_time { get => _gps_time; set => _gps_time = value; }
    }
}
