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
    public class LasPoint_1_2_Format0_Reader : LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_2_Format0_Reader() { }

        public LasPoint_1_2_Format0_Reader(Stream instream)
        {
            this.instream = instream;
        }

        LasPoint_1_2_Format0 laspoint;

        public unsafe bool Read()
        {
            byte[] buffer = new byte[20];
            if (instream.Read(buffer, 0, 20) != 20) throw new EndOfStreamException();

            laspoint = new LasPoint_1_2_Format0();

            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format0* p = (point_1_2_format0*)pBuffer;
                laspoint.X = p->x;
                laspoint.Y = p->y;
                laspoint.Z = p->z;
                laspoint.intensity = p->intensity;
                laspoint.flags = p->flags;
                laspoint.classification = p->classification;
                laspoint.scan_angle_rank = p->scan_angle_rank;
                laspoint.user_data = p->user_data;
                laspoint.point_source_ID = p->point_source_ID;
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
            return 20;
        }
    }

    public class LasPoint_1_2_Format1_Reader : LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_2_Format1_Reader() { }

        public LasPoint_1_2_Format1_Reader(Stream instream)
        {
            this.instream = instream;
        }

        public unsafe bool Read()
        {
            byte[] buffer = new byte[28];
            if (instream.Read(buffer, 0, 28) != 28) throw new EndOfStreamException();

            LasPoint_1_2_Format1 laspoint = new LasPoint_1_2_Format1();

            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format1* p = (point_1_2_format1*)pBuffer;
                laspoint.X = p->x;
                laspoint.Y = p->y;
                laspoint.Z = p->z;
                laspoint.intensity = p->intensity;
                laspoint.flags = p->flags;
                laspoint.classification = p->classification;
                laspoint.scan_angle_rank = p->scan_angle_rank;
                laspoint.user_data = p->user_data;
                laspoint.point_source_ID = p->point_source_ID;
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

    public class LasPoint_1_2_Format2_Reader : LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_2_Format2_Reader() { }

        public LasPoint_1_2_Format2_Reader(Stream instream)
        {
            this.instream = instream;
        }

        public unsafe bool Read()
        {
            byte[] buffer = new byte[26];
            if (instream.Read(buffer, 0, 26) != 26) throw new EndOfStreamException();

            LasPoint_1_2_Format2 laspoint = new LasPoint_1_2_Format2();

            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format2* p = (point_1_2_format2*)pBuffer;
                laspoint.X = p->x;
                laspoint.Y = p->y;
                laspoint.Z = p->z;
                laspoint.intensity = p->intensity;
                laspoint.flags = p->flags;
                laspoint.classification = p->classification;
                laspoint.scan_angle_rank = p->scan_angle_rank;
                laspoint.user_data = p->user_data;
                laspoint.point_source_ID = p->point_source_ID;
                laspoint.Red = p->red;
                laspoint.Green = p->green;
                laspoint.Blue = p->blue;
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
            return 26;
        }

    }

    public class LasPoint_1_2_Format3_Reader : LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_2_Format3_Reader() { }

        public LasPoint_1_2_Format3_Reader(Stream instream)
        {
            this.instream = instream;
        }

        public unsafe bool Read()
        {
            byte[] buffer = new byte[34];
            if (instream.Read(buffer, 0, 34) != 34) throw new EndOfStreamException();

            LasPoint_1_2_Format3 laspoint = new LasPoint_1_2_Format3();

            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format3* p = (point_1_2_format3*)pBuffer;
                laspoint.X = p->x;
                laspoint.Y = p->y;
                laspoint.Z = p->z;
                laspoint.intensity = p->intensity;
                laspoint.flags = p->flags;
                laspoint.classification = p->classification;
                laspoint.scan_angle_rank = p->scan_angle_rank;
                laspoint.user_data = p->user_data;
                laspoint.point_source_ID = p->point_source_ID;
                laspoint.gps_time = p->gps_time;
                laspoint.Red = p->red;
                laspoint.Green = p->green;
                laspoint.Blue = p->blue; 
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
            return 44;
        }

    }
}
