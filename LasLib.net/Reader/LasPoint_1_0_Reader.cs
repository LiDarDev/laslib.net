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
    public class LasPoint_1_0_Format0_Reader : LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_0_Format0_Reader() { }

        public LasPoint_1_0_Format0_Reader(Stream instream)
        {
            this.instream = instream;
        }

        /// <summary>
        /// Current read point.
        /// </summary>
        public LasPoint Point { get => point; set => point = value; }

        /// <summary>
        /// Read a point
        /// </summary>
        /// <returns></returns>
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
                point = new LasPoint(LasHeader.Instance);

                fixed (byte* pBuffer = buffer)
                {
                    point_1_0_format0* p = (point_1_0_format0*)pBuffer;
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
        

        public unsafe LasPoint Read()
        {
            byte[] buffer = new byte[28];
            if (instream.Read(buffer, 0, 28) != 28)
            {
                error = (new EndOfStreamException()).Message;
                return null;
            }

            point = new LasPoint();

            fixed (byte* pBuffer = buffer)
            {
                point_1_0_format1* p = (point_1_0_format1*)pBuffer;
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
