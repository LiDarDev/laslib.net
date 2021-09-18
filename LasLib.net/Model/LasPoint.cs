
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

namespace LasLibNet.Model
{
    /// <summary>
    /// Las Point
    /// </summary>
    public class LasPoint : LasPointBase
    {
        #region V1.0_Format0
        protected byte _file_marker;
        public byte file_marker { get => _file_marker; set => _file_marker = value; }

        protected ushort _user_bit_field;
        public ushort user_bit_field
        {
            get => _user_bit_field;
            set => _user_bit_field = value;
        }
        #endregion

        #region V1.1_Format0
        protected byte _user_data;
        public byte user_data { get => _user_data; set => _user_data = value; }

        protected ushort _point_source_ID;
        public ushort point_source_ID { get => _point_source_ID; set => _point_source_ID = value; }
        #endregion

        #region V1.2_Format1, V1.2_Format3, V1.3_Format1,V1.3_Format3
        private double _gps_time;
        public double gps_time { get => _gps_time; set => _gps_time = value; }
        #endregion

        #region #region V1.2_Format2, V1.2_Format3, V1.3_Format2,V1.3_Format3
        private ushort _red, _green, _blue;

        public ushort red { get => _red; set => _red = value; }
        public ushort green { get => _green; set => _green = value; }
        public ushort blue { get => _blue; set => _blue = value; }
        #endregion

        #region Extend fields.
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

        #endregion

        #region Public methods

        public bool IsSame(LasPoint p)
        {
            if (x != p.x) return false;
            if (y != p.y) return false;
            if (z != p.z) return false;

            if (_intensity != p.intensity) return false;
            if (_flags != p.flags) return false;
            if (_classification != p.classification) return false;
            if (_scan_angle_rank != p.scan_angle_rank) return false;
            if (_user_data != p.user_data) return false;

            if (_point_source_ID != p.point_source_ID) return false;
            if (_gps_time != p.gps_time) return false;
            if (_red != p.red) return false;
            if (_green != p.green) return false;
            if (_blue != p.blue) return false;

            return true;
        }

        #region To Point R1.0
        /// <summary>
        /// Get the point of Las R1.0 and format 0. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_0_Format0 ToPoint_1_0_Format0()
        {
            LasPoint_1_0_Format0 p = new LasPoint_1_0_Format0();
            p.X = this.x;
            p.Y = this.y;
            p.Z = this.z;
            p.intensity = this._intensity;
            p.flags = _flags;
            p.classification = _classification;
            p.scan_angle_rank = _scan_angle_rank;
            p.file_marker = _file_marker;
            p.user_bit_field = _user_bit_field;
            return p;
        }

        /// <summary>
        /// Get the point of Las R1.0 and format 1. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_0_Format1 ToPoint_1_0_Format1()
        {
            LasPoint_1_0_Format1 p = new LasPoint_1_0_Format1();
            p.X = this.x;
            p.Y = this.y;
            p.Z = this.z;
            p.intensity = this._intensity;
            p.flags = _flags;
            p.classification = _classification;
            p.scan_angle_rank = _scan_angle_rank;
            p.file_marker = _file_marker;
            p.user_bit_field = _user_bit_field;
            p.gps_time = _gps_time;
            return p;
        }
        #endregion

        #region To Point R1.1
        /// <summary>
        /// Get the point of Las R1.1 and format 0. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_1_Format0 ToPoint_1_1_Format0()
        {
            LasPoint_1_1_Format0 p = new LasPoint_1_1_Format0();
            p.X = this.x;
            p.Y = this.y;
            p.Z = this.z;
            p.intensity = this._intensity;
            p.flags = _flags;
            p.classification = _classification;
            p.scan_angle_rank = _scan_angle_rank;
            p.user_data = _user_data;
            p.point_source_ID = this._point_source_ID;
            return p;
        }

        /// <summary>
        /// Get the point of Las R1.1 and format 1. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_1_Format1 ToPoint_1_1_Format1()
        {
            LasPoint_1_1_Format1 p = new LasPoint_1_1_Format1();
            p.X = this.x;
            p.Y = this.y;
            p.Z = this.z;
            p.intensity = this._intensity;
            p.flags = _flags;
            p.classification = _classification;
            p.scan_angle_rank = _scan_angle_rank;
            p.user_data = _user_data;
            p.point_source_ID = this._point_source_ID;
            p.gps_time = _gps_time;
            return p;
        }
        #endregion

        #region To Point R1.2

        /// <summary>
        /// Get the point of Las R1.2 and format 0. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_2_Format0 ToPoint_1_2_Format0()
        {
            LasPoint_1_2_Format0 p = (LasPoint_1_2_Format0)this.ToPoint_1_1_Format0();
            return p;
        }

        /// <summary>
        /// Get the point of Las R1.2 and format 1. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_2_Format1 ToPoint_1_2_Format1()
        {
            LasPoint_1_2_Format1 p = (LasPoint_1_2_Format1)this.ToPoint_1_1_Format1();
            return p;
        }

        /// <summary>
        /// Get the point of Las R1.2 and format 2. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_2_Format2 ToPoint_1_2_Format2()
        {
            LasPoint_1_2_Format2 p = new LasPoint_1_2_Format2();
            p.X = this.x;
            p.Y = this.y;
            p.Z = this.z;
            p.intensity = this._intensity;
            p.flags = _flags;
            p.classification = _classification;
            p.scan_angle_rank = _scan_angle_rank;
            p.user_data = _user_data;
            p.point_source_ID = this._point_source_ID;
            p.Red = this._red;
            p.Green = this._green;
            p.Blue = this._blue;
            return p;
        }

        /// <summary>
        /// Get the point of Las R1.2 and format 3. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_2_Format3 ToPoint_1_2_Format3()
        {
            LasPoint_1_2_Format3 p = new LasPoint_1_2_Format3();
            p.X = this.x;
            p.Y = this.y;
            p.Z = this.z;
            p.intensity = this._intensity;
            p.flags = _flags;
            p.classification = _classification;
            p.scan_angle_rank = _scan_angle_rank;
            p.user_data = _user_data;
            p.point_source_ID = this._point_source_ID;
            p.Red = this._red;
            p.Green = this._green;
            p.Blue = this._blue;
            p.gps_time = _gps_time;
            return p;
        }
        #endregion

        #region To Point R1.3
        /// <summary>
        /// Get the point of Las R1.3 and format 0. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_3_Format0 ToPoint_1_3_Format0()
        {
            LasPoint_1_3_Format0 p = (LasPoint_1_3_Format0)this.ToPoint_1_2_Format0();
            return p;
        }

        /// <summary>
        /// Get the point of Las R1.3 and format 1. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_3_Format1 ToPoint_1_3_Format1()
        {
            LasPoint_1_3_Format1 p = (LasPoint_1_3_Format1)this.ToPoint_1_3_Format1();
            return p;
        }

        /// <summary>
        /// Get the point of Las R1.3 and format 2. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_3_Format2 ToPoint_1_3_Format2()
        {
            LasPoint_1_3_Format2 p = (LasPoint_1_3_Format2)this.ToPoint_1_2_Format2();

            return p;
        }

        /// <summary>
        /// Get the point of Las R1.3 and format 3. 
        /// </summary>
        /// <returns></returns>
        public LasPoint_1_3_Format3 ToPoint_1_3_Format3()
        {
            LasPoint_1_3_Format3 p = (LasPoint_1_3_Format3)this.ToPoint_1_2_Format3();
            return p;
        }
        #endregion


        #region From Point R1.0
        /// <summary>
        /// Get the las point from the Las R1.0 and format 0 point. 
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_0_Format0(LasPoint_1_0_Format0 p)
        {
            this.x = p.X;
            this.y = p.Y;
            this.z = p.Z;
            this._intensity = p.intensity;
            _flags = p.flags;
            _classification = p.classification;
            _scan_angle_rank = p.scan_angle_rank;
            _file_marker = p.file_marker;
            _user_bit_field = p.user_bit_field;
        }

        /// <summary>
        ///  Get the las point from the Las R1.0 and format 1 point.
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_0_Format1(LasPoint_1_0_Format1 p)
        {
            this.x = p.X;
            this.y = p.Y;
            this.z = p.Z;
            this._intensity = p.intensity;
            _flags = p.flags;
            _classification = p.classification;
            _scan_angle_rank = p.scan_angle_rank;
            _file_marker = p.file_marker;
            _user_bit_field = p.user_bit_field;
            _gps_time = p.gps_time;

        }
        #endregion

        #region From Point R1.1
        /// <summary>
        ///  Get the las point from the Las R1.1 and format 0 point.
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_1_Format0(LasPoint_1_1_Format0 p)
        {
            this.x = p.X;
            this.y = p.Y;
            this.z = p.Z;
            this._intensity = p.intensity;
            _flags = p.flags;
            _classification = p.classification;
            _scan_angle_rank = p.scan_angle_rank;
            _user_data = p.user_data;
            this._point_source_ID = p.point_source_ID;
        }

        /// <summary>
        ///  Get the las point from the Las R1.1 and format 1 point. 
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_1_Format1(LasPoint_1_1_Format1 p)
        {
            this.x = p.X;
            this.y = p.Y;
            this.z = p.Z;
            this._intensity = p.intensity;
            _flags = p.flags;
            _classification = p.classification;
            _scan_angle_rank = p.scan_angle_rank;
            _user_data = p.user_data;
            this._point_source_ID = p.point_source_ID;
            _gps_time = p.gps_time;
        }
        #endregion

        #region From Point R1.2

        /// <summary>
        ///  Get the las point from the Las R1.2 and format 0 point.
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_2_Format0(LasPoint_1_2_Format0 p)
        {
            LasPoint_1_1_Format0 p1 = (LasPoint_1_1_Format0)p;
            this.FromPoint_1_1_Format0(p1);
        }

        /// <summary>
        ///  Get the las point from the Las R1.2 and format 1 point.
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_2_Format1(LasPoint_1_2_Format1 p)
        {
            LasPoint_1_1_Format1 p1 = (LasPoint_1_1_Format1)p;
            this.FromPoint_1_1_Format1(p1);
        }

        /// <summary>
        ///  Get the las point from the Las R1.2 and format 2 point.
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_2_Format2(LasPoint_1_2_Format2 p)
        {
            this.x = p.X;
            this.y = p.Y;
            this.z = p.Z;
            this._intensity = p.intensity;
            _flags = p.flags;
            _classification = p.classification;
            _scan_angle_rank = p.scan_angle_rank;
            _user_data = p.user_data;
            this._point_source_ID = p.point_source_ID;
            this._red = p.Red;
            this._green = p.Green;
            this._blue = p.Blue;
        }

        /// <summary>
        ///  Get the las point from the R1.2 and format 3 point.
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_2_Format3(LasPoint_1_2_Format3 p)
        {
            this.x = p.X;
            this.y = p.Y;
            this.z = p.Z;
            this._intensity = p.intensity;
            _flags = p.flags;
            _classification = p.classification;
            _scan_angle_rank = p.scan_angle_rank;
            _user_data = p.user_data;
            this._point_source_ID = p.point_source_ID;
            this._red = p.Red;
            this._green = p.Green;
            this._blue = p.Blue;
            _gps_time = p.gps_time;
        }
        #endregion

        #region From Point R1.3
        /// <summary>
        /// Get the las point from the R1.3 and format 0 point. 
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_3_Format0(LasPoint_1_3_Format0 p)
        {
            LasPoint_1_2_Format0 p1 = (LasPoint_1_2_Format0)p;
            this.FromPoint_1_2_Format0(p1);
        }

        /// <summary>
        /// Get the las point from the R1.3 and format 1 point.
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_3_Format1(LasPoint_1_3_Format1 p)
        {
            LasPoint_1_2_Format1 p1 = (LasPoint_1_2_Format1)p;
            this.FromPoint_1_2_Format1(p1);
        }

        /// <summary>
        /// Get the las point from the R1.3 and format 2 point.
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_3_Format2(LasPoint_1_3_Format2 p)
        {
            LasPoint_1_2_Format2 p1 = (LasPoint_1_2_Format2)p;
            this.FromPoint_1_2_Format2(p1);
        }

        /// <summary>
        /// Get the las point from the R1.3 and format 3 point.
        /// </summary>
        /// <returns></returns>
        public void FromPoint_1_3_Format3(LasPoint_1_3_Format3 p)
        {
            LasPoint_1_2_Format3 p1 = (LasPoint_1_2_Format3)p;
            this.FromPoint_1_2_Format3(p1);

        }
        #endregion


        #region Get Buffer Pointer of the R1.0 point
        /// <summary>
        /// Get the buffer of Las R1.0 and format 0. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_0_Format0()
        {
            byte[] buffer = new byte[PointSize.POINT_1_0_FORMAT0];
            fixed (byte* pBuffer = buffer)
            {
                point_1_0_format0* p10 = (point_1_0_format0*)pBuffer;
                p10->x = x;
                p10->y = y;
                p10->z = z;
                p10->intensity = _intensity;
                p10->flags = _flags;
                p10->classification = _classification;
                p10->scan_angle_rank = _scan_angle_rank;
                p10->file_marker = _file_marker;
                p10->user_bit_field = _user_bit_field;
            }

            return buffer;

        }

        /// <summary>
        /// Get the point buffer of Las R1.0 and format 1. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_0_Format1()
        {
            byte[] buffer = new byte[PointSize.POINT_1_0_FORMAT1];
            fixed (byte* pBuffer = buffer)
            {
                point_1_0_format1* p = (point_1_0_format1*)pBuffer;
                p->x = x;
                p->y = y;
                p->z = z;
                p->intensity = _intensity;
                p->flags = _flags;
                p->classification = _classification;
                p->scan_angle_rank = _scan_angle_rank;
                p->file_marker = _file_marker;
                p->user_bit_field = _user_bit_field;
                p->gps_time = _gps_time;
            }

            return buffer;

        }
        #endregion

        #region Get Buffer of the  R1.1 Point
        /// <summary>
        /// Get the point buffer of Las R1.1 and format 0. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_1_Format0()
        {
            byte[] buffer = new byte[PointSize.POINT_1_1_FORMAT0];
            fixed (byte* pBuffer = buffer)
            {
                point_1_1_format0* p = (point_1_1_format0*)pBuffer;
                p->x = x;
                p->y = y;
                p->z = z;
                p->intensity = _intensity;
                p->flags = _flags;
                p->classification = _classification;
                p->scan_angle_rank = _scan_angle_rank;
                p->user_data = _user_data;
                p->point_source_ID = _point_source_ID;
            }

            return buffer;
        }

        /// <summary>
        /// Get buffer for the point of Las R1.1 and format 1. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_1_Format1()
        {
            byte[] buffer = new byte[PointSize.POINT_1_1_FORMAT1];
            fixed (byte* pBuffer = buffer)
            {
                point_1_1_format1* p = (point_1_1_format1*)pBuffer;
                p->x = x;
                p->y = y;
                p->z = z;
                p->intensity = _intensity;
                p->flags = _flags;
                p->classification = _classification;
                p->scan_angle_rank = _scan_angle_rank;
                p->user_data = _user_data;
                p->point_source_ID = _point_source_ID;
                p->gps_time = _gps_time;
            }

            return buffer;

        }
        #endregion

        #region Get buffer for the Point of R1.2

        /// <summary>
        /// Get the buffer for the point of Las R1.2 and format 0. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_2_Format0()
        {
            byte[] buffer = new byte[PointSize.POINT_1_2_FORMAT0];
            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format0* p = (point_1_2_format0*)pBuffer;
                p->x = x;
                p->y = y;
                p->z = z;
                p->intensity = _intensity;
                p->flags = _flags;
                p->classification = _classification;
                p->scan_angle_rank = _scan_angle_rank;
                p->user_data = _user_data;
                p->point_source_ID = _point_source_ID;
            }

            return buffer;
        }

        /// <summary>
        /// Get Buffer for the point of Las R1.2 and format 1. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_2_Format1()
        {
            byte[] buffer = new byte[PointSize.POINT_1_2_FORMAT1];
            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format1* p = (point_1_2_format1*)pBuffer;
                p->x = x;
                p->y = y;
                p->z = z;
                p->intensity = _intensity;
                p->flags = _flags;
                p->classification = _classification;
                p->scan_angle_rank = _scan_angle_rank;
                p->user_data = _user_data;
                p->point_source_ID = _point_source_ID;
                p->gps_time = _gps_time;
            }

            return buffer;
        }

        /// <summary>
        /// Get buffer for the point of Las R1.2 and format 2. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_2_Format2()
        {
            byte[] buffer = new byte[PointSize.POINT_1_2_FORMAT2];
            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format2* p = (point_1_2_format2*)pBuffer;
                p->x = x;
                p->y = y;
                p->z = z;
                p->intensity = _intensity;
                p->flags = _flags;
                p->classification = _classification;
                p->scan_angle_rank = _scan_angle_rank;
                p->user_data = _user_data;
                p->point_source_ID = _point_source_ID;
                p->red = _red;
                p->green = _green;
                p->blue = _blue;
            }

            return buffer;

        }

        /// <summary>
        /// Get buffer for the point of Las R1.2 and format 3. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_2_Format3()
        {

            byte[] buffer = new byte[PointSize.POINT_1_2_FORMAT3];
            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format3* p = (point_1_2_format3*)pBuffer;
                p->x = x;
                p->y = y;
                p->z = z;
                p->intensity = _intensity;
                p->flags = _flags;
                p->classification = _classification;
                p->scan_angle_rank = _scan_angle_rank;
                p->user_data = _user_data;
                p->point_source_ID = _point_source_ID;
                p->red = _red;
                p->green = _green;
                p->blue = _blue;
                p->gps_time = _gps_time;
            }

            return buffer;

        }
        #endregion

        #region Get buffer for the Point R1.3
        /// <summary>
        /// Get buffer for the point of Las R1.3 and format 0. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_3_Format0()
        {
            return this.GetBuffer_Point_1_2_Format0();
        }

        /// <summary>
        /// Get buffer for the point of Las R1.3 and format 1. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_3_Format1()
        {
            return this.GetBuffer_Point_1_2_Format1();
        }

        /// <summary>
        /// Get buffer for the point of Las R1.3 and format 2. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_3_Format2()
        {
            return this.GetBuffer_Point_1_2_Format2();
        }

        /// <summary>
        /// Get buffer for the point of Las R1.3 and format 3. 
        /// </summary>
        /// <returns></returns>
        public unsafe byte[] GetBuffer_Point_1_3_Format3()
        {
            return this.GetBuffer_Point_1_2_Format3();
        }
        #endregion

        #endregion
    }
}
