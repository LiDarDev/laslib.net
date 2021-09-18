
///////////////////////////////////////////////////////////////////////
/// File    : LasPointBase.cs
/// Desc    : Las point baseclass.
/// Author  : Li G.Q.
/// Date    : 2021/9/17/
///////////////////////////////////////////////////////////////////////


using LasLibNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Model
{
    /// <summary>
    /// Las Point
    /// </summary>
    public class LasPointBase
    {
        /// <summary>
        /// The coordinate of las file.
        /// </summary>
        protected int x, y, z;
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

        #region Geographics coordinates
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
        #endregion


        protected ushort _intensity;
        public ushort intensity { get => _intensity; set => _intensity = value; }

        #region return_number,number_of_returns,scan_dirction_flag,edge_of_flit_line
        /// <summary>
        /// The flag byte includes return_number(3bits),
        /// number_of_returns(3bits), scan_dirction_flag(1bit),
        /// edge_of_flit_line(1bit). 
        /// </summary>
        protected byte _flags;
        public byte flags { get => _flags; set => _flags = value; }
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
        #endregion

        protected byte _classification;
        public byte classification { get => _classification; set => _classification = value; }

        protected sbyte _scan_angle_rank;
        public sbyte scan_angle_rank { get => _scan_angle_rank; set => _scan_angle_rank = value; }

<<<<<<< HEAD
=======

        /// <summary>
        /// Check if two points are same.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsSame(LasPointBase p)
        {
            if (x != p.X) return false;
            if (y != p.Y) return false;
            if (z != p.Z) return false;

            if (_intensity != p.intensity) return false;
            if (_flags != p.flags) return false;
            if (_classification != p.classification) return false;
            if (_scan_angle_rank != p.scan_angle_rank) return false;

            return true;
        }

        /// <summary>
        /// Clone the p's attributes to this class instance.
        /// </summary>
        /// <param name="p"></param>
        public void Clone(LasPointBase p)
        {
            x = p.X; 
            y = p.Y; 
            z = p.Z;
            _intensity = p.intensity;
            _flags = p.flags;
            _classification = p.classification;
            _scan_angle_rank = p.scan_angle_rank;
        }
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
    }
}
