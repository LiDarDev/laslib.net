
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
        protected LasHeader lasHeader=LasHeader.Instance;
        /// <summary>
        /// 头部信息
        /// </summary>
        public LasHeader Header { get => lasHeader; set => lasHeader = value; }
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
                    return lasHeader.x_scale_factor * this.x + lasHeader.x_offset;
            }
            set
            {               
                    this.x = MyDefs.I32_QUANTIZE((value - lasHeader.x_offset) / lasHeader.x_scale_factor);
            }
        }

        /// <summary>
        /// The Y of geographic coordinate.
        /// </summary>
        public double GeoY
        {
            get
            {
                
                    return lasHeader.y_scale_factor * this.y + lasHeader.y_offset;
            }
            set
            {
               
                    this.y = MyDefs.I32_QUANTIZE((value - lasHeader.y_offset) / lasHeader.y_scale_factor);
            }
        }


        /// <summary>
        /// The Y of geographic coordinate.
        /// </summary>
        public double GeoZ
        {
            get
            {              
                    return lasHeader.z_scale_factor * this.z + lasHeader.z_offset;  
            }
            set
            {
                this.z = MyDefs.I32_QUANTIZE((value - lasHeader.z_offset) / lasHeader.z_scale_factor);
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

    }
}
