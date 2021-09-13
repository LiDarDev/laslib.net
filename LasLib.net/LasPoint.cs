﻿
///////////////////////////////////////////////////////////////////////
/// File    : LasPoint.cs
/// Desc    : Las point class.
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
    /// Las Point
    /// </summary>
    public class LasPoint
    {
        public int X;
        public int Y;
        public int Z;
        public ushort intensity;
        //public byte return_number : 3;
        public byte return_number { get { return (byte)(flags & 7); } set { flags = (byte)((flags & 0xF8) | (value & 7)); } }
        //public byte number_of_returns_of_given_pulse : 3;
        public byte number_of_returns_of_given_pulse { get { return (byte)((flags >> 3) & 7); } set { flags = (byte)((flags & 0xC7) | ((value & 7) << 3)); } }
        //public byte scan_direction_flag : 1;
        public byte scan_direction_flag { get { return (byte)((flags >> 6) & 1); } set { flags = (byte)((flags & 0xBF) | ((value & 1) << 6)); } }
        //public byte edge_of_flight_line : 1;
        public byte edge_of_flight_line { get { return (byte)((flags >> 7) & 1); } set { flags = (byte)((flags & 0x7F) | ((value & 1) << 7)); } }
        public byte flags;
        public byte classification;
        public sbyte scan_angle_rank;
        public byte user_data;
        public ushort point_source_ID;

        public double gps_time;
        public ushort[] rgb = new ushort[4];
        public byte[] wave_packet = new byte[29];

        // LAS 1.4 only
        //public byte extended_point_type : 2;
        public byte extended_point_type { get { return (byte)(extended_flags & 3); } set { extended_flags = (byte)((extended_flags & 0xFC) | (value & 3)); } }
        //public byte extended_scanner_channel : 2;
        public byte extended_scanner_channel { get { return (byte)((extended_flags >> 2) & 3); } set { extended_flags = (byte)((extended_flags & 0xF3) | ((value & 3) << 2)); } }
        //public byte extended_classification_flags : 4;
        public byte extended_classification_flags { get { return (byte)((extended_flags >> 4) & 0xF); } set { extended_flags = (byte)((extended_flags & 0xF) | ((value & 0xF) << 4)); } }
        public byte extended_flags;
        public byte extended_classification;
        //public byte extended_return_number : 4;
        public byte extended_return_number { get { return (byte)(extended_returns & 0xF); } set { extended_returns = (byte)((extended_returns & 0xF0) | (value & 0xF)); } }
        //public byte extended_number_of_returns_of_given_pulse : 4;
        public byte extended_number_of_returns_of_given_pulse { get { return (byte)((extended_returns >> 4) & 0xF); } set { extended_returns = (byte)((extended_returns & 0xF) | ((value & 0xF) << 4)); } }
        public byte extended_returns;
        public short extended_scan_angle;

        public int num_extra_bytes;
        public byte[] extra_bytes;

        public bool IsSame(LasPoint p)
        {
            if (X != p.X) return false;
            if (Y != p.Y) return false;
            if (Z != p.Z) return false;

            if (intensity != p.intensity) return false;
            if (flags != p.flags) return false;
            if (classification != p.classification) return false;
            if (scan_angle_rank != p.scan_angle_rank) return false;
            if (user_data != p.user_data) return false;

            if (point_source_ID != p.point_source_ID) return false;
            if (gps_time != p.gps_time) return false;
            if (rgb[0] != p.rgb[0]) return false;
            if (rgb[1] != p.rgb[1]) return false;
            if (rgb[2] != p.rgb[2]) return false;
            if (rgb[3] != p.rgb[3]) return false;
            for (int i = 0; i < 29; i++)
                if (wave_packet[i] != p.wave_packet[i]) return false;

            if (extended_flags != p.extended_flags) return false;
            if (extended_classification != p.extended_classification) return false;
            if (extended_returns != p.extended_returns) return false;
            if (extended_scan_angle != p.extended_scan_angle) return false;

            if (num_extra_bytes != p.num_extra_bytes) return false;
            for (int i = 0; i < num_extra_bytes; i++)
                if (extra_bytes[i] != p.extra_bytes[i]) return false;

            return true;
        }
    }
}