using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Model
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_base
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_0_format0
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte file_marker;
        public ushort user_bit_field;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_0_format1
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte file_marker;
        public ushort user_bit_field;
        public double gps_time;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_1_format0
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;      
        public ushort point_source_ID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_1_format1
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;
        public double gps_time;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_2_format0
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_2_format1
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;
        public double gps_time;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_2_format2
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;
        public ushort blue;
        public ushort red;
        public ushort green;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_2_format3
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;
        public double gps_time;
        public ushort blue;
        public ushort red;
        public ushort green;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_3_format0
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_3_format1
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;
        public double gps_time;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_3_format3
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;
        public ushort blue;
        public ushort red;
        public ushort green;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct point_1_3_format4
    {
        public int x;
        public int y;
        public int z;
        public ushort intensity;
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;
        public double gps_time;
        public ushort blue;
        public ushort red;
        public ushort green;
    }
}
