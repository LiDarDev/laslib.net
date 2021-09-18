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
    public class LasPoint_1_0_Format0_Reader:LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_0_Format0_Reader() { }

        public LasPoint_1_0_Format0_Reader(Stream instream)
        {
            this.instream = instream;
        }

        LasPoint_1_0_Format0 laspoint;

        /// <summary>
        /// Current read point.
        /// </summary>
        public LasPoint_1_0_Format0 Point { get => laspoint; set => laspoint = value; }

        /// <summary>
        /// Read a point
        /// </summary>
        /// <returns></returns>
        public unsafe bool Read()
        {
            byte[] buffer = new byte[20];
            if (instream.Read(buffer, 0, 20) != 20) throw new EndOfStreamException();

            try
            {
                laspoint = new LasPoint_1_0_Format0();

                fixed (byte* pBuffer = buffer)
                {
                    point_1_0_format0* p = (point_1_0_format0*)pBuffer;
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

        LasPoint_1_0_Format1 laspoint;

        public LasPoint_1_0_Format1 Point { get => laspoint; set => laspoint = value; }

        public unsafe bool Read()
        {
            byte[] buffer = new byte[28];
            if (instream.Read(buffer, 0, 28) != 28) throw new EndOfStreamException();

            laspoint = new LasPoint_1_0_Format1();

            fixed (byte* pBuffer = buffer)
            {
                point_1_0_format1* p = (point_1_0_format1*)pBuffer;
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
