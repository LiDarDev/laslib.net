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
    public class LasPoint_1_1_Format0_Reader : LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_1_Format0_Reader() { }

        public LasPoint_1_1_Format0_Reader(Stream instream)
        {
            this.instream = instream;
        }

        public unsafe LasPoint Read()
        {
            byte[] buffer = new byte[20];
            if (instream.Read(buffer, 0, 20) != 20)
            {
                error = (new EndOfStreamException()).Message;
                return null;
            }

            point = new LasPoint();

            fixed (byte* pBuffer = buffer)
            {
                point_1_1_format0* p = (point_1_1_format0*)pBuffer;
                point.X = p->x;
                point.Y = p->y;
                point.Z = p->z;
                point.intensity = p->intensity;
                point.flags = p->flags;
                point.classification = p->classification;
                point.scan_angle_rank = p->scan_angle_rank;
                point.user_data = p->user_data;
                point.point_source_ID = p->point_source_ID;
            }

            return point;
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

    public class LasPoint_1_1_Format1_Reader : LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_1_Format1_Reader() { }

        public LasPoint_1_1_Format1_Reader(Stream instream)
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
                point_1_1_format1* p = (point_1_1_format1*)pBuffer;
                point.X = p->x;
                point.Y = p->y;
                point.Z = p->z;
                point.intensity = p->intensity;
                point.flags = p->flags;
                point.classification = p->classification;
                point.scan_angle_rank = p->scan_angle_rank;
                point.user_data = p->user_data;
                point.point_source_ID = p->point_source_ID;
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
