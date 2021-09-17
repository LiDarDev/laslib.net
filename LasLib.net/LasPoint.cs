
///////////////////////////////////////////////////////////////////////
/// File    : LasPoint.cs
/// Desc    : Las point class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////


using LasLibNet.Utils;
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
        /// <summary>
        /// The coordinate of las file.
        /// </summary>
        private int x, y, z;
        private ushort _intensity;

        /// <summary>
        /// X
        /// </summary>
        public int X { get => x; set => x = value; }
        /// <summary>
        /// Y
        /// </summary>
        public int Y { get => y; set => y = value; }
        /// <summary>
        /// Z
        /// </summary>
        public int Z { get => z; set => z = value; }
        public ushort intensity { get => _intensity; set => _intensity = value; }

        /// <summary>
        /// The X of geographic coordinate.
        /// </summary>
        public double GeoX
        {
            get
            {
                return LasHeader.Instance.x_scale_factor * this.x + LasHeader.Instance.x_offset;
            }
            set
            {
                this.x = MyDefs.I32_QUANTIZE((value - LasHeader.Instance.x_offset) / LasHeader.Instance.x_scale_factor);
            }
        }

        /// <summary>
        /// The Y of geographic coordinate.
        /// </summary>
        public double GeoY
        {
            get
            {
                return LasHeader.Instance.y_scale_factor * this.y + LasHeader.Instance.y_offset;
            }
            set
            {
                this.y = MyDefs.I32_QUANTIZE((value - LasHeader.Instance.y_offset) / LasHeader.Instance.y_scale_factor);
            }
        }


        /// <summary>
        /// The Y of geographic coordinate.
        /// </summary>
        public double GeoZ
        {
            get
            {
                return LasHeader.Instance.z_scale_factor * this.z + LasHeader.Instance.z_offset;
            }
            set
            {
                this.z = MyDefs.I32_QUANTIZE((value - LasHeader.Instance.z_offset) / LasHeader.Instance.z_scale_factor);
            }
        }


        //public byte return_number : 3;
        public byte return_number
        {
            get { return (byte)(_flags & 7); }
            set { _flags = (byte)((_flags & 0xF8) | (value & 7)); }
        }
        //public byte number_of_returns_of_given_pulse : 3;
        public byte number_of_returns_of_given_pulse
        {
            get { return (byte)((_flags >> 3) & 7); }
            set { _flags = (byte)((_flags & 0xC7) | ((value & 7) << 3)); }
        }
        //public byte scan_direction_flag : 1;
        public byte scan_direction_flag
        {
            get { return (byte)((_flags >> 6) & 1); }
            set { _flags = (byte)((_flags & 0xBF) | ((value & 1) << 6)); }
        }
        //public byte edge_of_flight_line : 1;
        public byte edge_of_flight_line
        {
            get { return (byte)((_flags >> 7) & 1); }
            set { _flags = (byte)((_flags & 0x7F) | ((value & 1) << 7)); }
        }
        private byte _flags;
        public byte flags { get => _flags; set => _flags = value; }

        private byte _classification;
        public byte classification { get => _classification; set => _classification = value; }

        private sbyte _scan_angle_rank;
        public sbyte scan_angle_rank { get => _scan_angle_rank; set => _scan_angle_rank = value; }

        private byte _user_data;
        public byte user_data { get => _user_data; set => _user_data = value; }

        private ushort _point_source_ID;
        public ushort point_source_ID { get => _point_source_ID; set => _point_source_ID = value; }

        private double _gps_time;
        public double gps_time { get => _gps_time; set => _gps_time = value; }

        private ushort[] _rgb = new ushort[4];
        public ushort[] rgb { get => _rgb; set => _rgb = value; }

        private byte[] _wave_packet = new byte[29];

        public byte[] wave_packet { get => _wave_packet; set => _wave_packet = value; }

        // LAS 1.4 only
        //public byte extended_point_type : 2;
        public byte extended_point_type
        {
            get { return (byte)(_extended_flags & 3); }
            set { _extended_flags = (byte)((_extended_flags & 0xFC) | (value & 3)); }
        }
        //public byte extended_scanner_channel : 2;
        public byte extended_scanner_channel
        {
            get { return (byte)((_extended_flags >> 2) & 3); }
            set { _extended_flags = (byte)((_extended_flags & 0xF3) | ((value & 3) << 2)); }
        }
        //public byte extended_classification_flags : 4;
        public byte extended_classification_flags
        {
            get { return (byte)((_extended_flags >> 4) & 0xF); }
            set { _extended_flags = (byte)((_extended_flags & 0xF) | ((value & 0xF) << 4)); }
        }

        private byte _extended_flags;
        private byte _extended_classification;
        public byte extended_flags { get => _extended_flags; set => _extended_flags = value; }
        public byte extended_classification { get => _extended_classification; set => _extended_classification = value; }

        //public byte extended_return_number : 4;
        public byte extended_return_number
        {
            get { return (byte)(_extended_returns & 0xF); }
            set { _extended_returns = (byte)((_extended_returns & 0xF0) | (value & 0xF)); }
        }
        //public byte extended_number_of_returns_of_given_pulse : 4;
        public byte extended_number_of_returns_of_given_pulse
        {
            get { return (byte)((_extended_returns >> 4) & 0xF); }
            set { _extended_returns = (byte)((_extended_returns & 0xF) | ((value & 0xF) << 4)); }
        }



        private byte _extended_returns;
        public byte extended_returns { get => _extended_returns; set => _extended_returns = value; }

        private short _extended_scan_angle;
        public short extended_scan_angle { get => _extended_scan_angle; set => _extended_scan_angle = value; }

        private int _num_extra_bytes;
        public int num_extra_bytes { get => _num_extra_bytes; set => _num_extra_bytes = value; }
        private byte[] _extra_bytes;
        public byte[] extra_bytes { get => _extra_bytes; set => _extra_bytes = value; }
     
     

        public bool IsSame(LasPoint p)
        {
            if (x != p.x) return false;
            if (y != p.y) return false;
            if (z != p.z) return false;

            if (_intensity != p._intensity) return false;
            if (_flags != p._flags) return false;
            if (_classification != p._classification) return false;
            if (_scan_angle_rank != p._scan_angle_rank) return false;
            if (_user_data != p._user_data) return false;

            if (_point_source_ID != p._point_source_ID) return false;
            if (_gps_time != p._gps_time) return false;
            if (_rgb[0] != p._rgb[0]) return false;
            if (_rgb[1] != p._rgb[1]) return false;
            if (_rgb[2] != p._rgb[2]) return false;
            if (_rgb[3] != p._rgb[3]) return false;
            for (int i = 0; i < 29; i++)
                if (_wave_packet[i] != p._wave_packet[i]) return false;

            if (_extended_flags != p._extended_flags) return false;
            if (_extended_classification != p._extended_classification) return false;
            if (_extended_returns != p._extended_returns) return false;
            if (_extended_scan_angle != p._extended_scan_angle) return false;

            if (_num_extra_bytes != p._num_extra_bytes) return false;
            for (int i = 0; i < _num_extra_bytes; i++)
                if (_extra_bytes[i] != p._extra_bytes[i]) return false;

            return true;
        }
    }
}
