using LasLibNet.Abstract;
using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Reader
{
<<<<<<< HEAD
    public class LasPoint_1_0_Format0_Reader : LasPointReaderBase, ILasPointReader
=======
    public class LasPoint_1_0_Format0_Reader:LasPointReaderBase, ILasPointReader
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
    {
        public LasPoint_1_0_Format0_Reader() { }

        public LasPoint_1_0_Format0_Reader(Stream instream)
        {
            this.instream = instream;
        }

<<<<<<< HEAD
        /// <summary>
        /// Current read point.
        /// </summary>
        public LasPoint Point { get => point; set => point = value; }
=======
        LasPoint_1_0_Format0 laspoint;

        /// <summary>
        /// Current read point.
        /// </summary>
        public LasPoint_1_0_Format0 Point { get => laspoint; set => laspoint = value; }
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09

        /// <summary>
        /// Read a point
        /// </summary>
        /// <returns></returns>
<<<<<<< HEAD
        public unsafe LasPoint Read()
        {
            byte[] buffer = new byte[20];
            if (instream.Read(buffer, 0, 20) != 20)
            {
                error = (new EndOfStreamException()).Message;
                return null;
            }

            try
            {
                point = new LasPoint();
=======
        public unsafe bool Read()
        {
            byte[] buffer = new byte[20];
            if (instream.Read(buffer, 0, 20) != 20) throw new EndOfStreamException();

            try
            {
                laspoint = new LasPoint_1_0_Format0();
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09

                fixed (byte* pBuffer = buffer)
                {
                    point_1_0_format0* p = (point_1_0_format0*)pBuffer;
<<<<<<< HEAD
                    point.X = p->x;
                    point.Y = p->y;
                    point.Z = p->z;
                    point.intensity = p->intensity;
                    point.flags = p->flags;
                    point.classification = p->classification;
                    point.scan_angle_rank = p->scan_angle_rank;
                    point.file_marker = p->file_marker;
                    point.user_bit_field = p->user_bit_field;
                }

                return point;
=======
                    laspoint.X = p->x;
                    laspoint.Y = p->y;
                    laspoint.Z = p->z;
                    laspoint.intensity = p->intensity;
                    laspoint.flags = p->flags;
                    laspoint.classification = p->classification;
                    laspoint.scan_angle_rank = p->scan_angle_rank;
                    laspoint.file_marker = p->file_marker;
                    laspoint.user_bit_field = p->user_bit_field;
                }
                this.point = laspoint;
                return true;
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Get the point size
        /// </summary>
        /// <returns></returns>
        public int GetPointSize()
        {
            return 20;
        }

    }

    public class LasPoint_1_0_Format1_Reader : LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_0_Format1_Reader() { }

        public LasPoint_1_0_Format1_Reader(Stream instream)
        {
            this.instream = instream;
        }
<<<<<<< HEAD
        

        public unsafe LasPoint Read()
        {
            byte[] buffer = new byte[28];
            if (instream.Read(buffer, 0, 28) != 28)
            {
                error = (new EndOfStreamException()).Message;
                return null;
            }

            point = new LasPoint();
=======

        LasPoint_1_0_Format1 laspoint;

        public LasPoint_1_0_Format1 Point { get => laspoint; set => laspoint = value; }

        public unsafe bool Read()
        {
            byte[] buffer = new byte[28];
            if (instream.Read(buffer, 0, 28) != 28) throw new EndOfStreamException();

            laspoint = new LasPoint_1_0_Format1();
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09

            fixed (byte* pBuffer = buffer)
            {
                point_1_0_format1* p = (point_1_0_format1*)pBuffer;
<<<<<<< HEAD
                point.X = p->x;
                point.Y = p->y;
                point.Z = p->z;
                point.intensity = p->intensity;
                point.flags = p->flags;
                point.classification = p->classification;
                point.scan_angle_rank = p->scan_angle_rank;
                point.file_marker = p->file_marker;
                point.user_bit_field = p->user_bit_field;
                point.gps_time = p->gps_time;
            }

            return point;
=======
                laspoint.X = p->x;
                laspoint.Y = p->y;
                laspoint.Z = p->z;
                laspoint.intensity = p->intensity;
                laspoint.flags = p->flags;
                laspoint.classification = p->classification;
                laspoint.scan_angle_rank = p->scan_angle_rank;
                laspoint.file_marker = p->file_marker;
                laspoint.user_bit_field = p->user_bit_field;
                laspoint.gps_time = p->gps_time;
            }
            this.point = laspoint;
            return true;
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
        }

        /// <summary>
        /// Get the point size
        /// </summary>
        /// <returns></returns>
        public int GetPointSize()
        {
            return 28;
        }
    }
}
