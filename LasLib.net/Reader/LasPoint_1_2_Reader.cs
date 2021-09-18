using LasLibNet.Abstract;
using LasLibNet.Model;
using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.Diagnostics;
=======
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
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

<<<<<<< HEAD
        public unsafe LasPoint Read()
        {
            byte[] buffer = new byte[20];
            if (instream.Read(buffer, 0, 20) != 20)
            {
                error = (new EndOfStreamException()).Message
                    ; return null;
            }

            point = new LasPoint();
=======
        LasPoint_1_2_Format0 laspoint;

        public unsafe bool Read()
        {
            byte[] buffer = new byte[20];
            if (instream.Read(buffer, 0, 20) != 20) throw new EndOfStreamException();

            laspoint = new LasPoint_1_2_Format0();
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09

            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format0* p = (point_1_2_format0*)pBuffer;
<<<<<<< HEAD
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
            return this.point;
=======
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
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
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

<<<<<<< HEAD
        public unsafe LasPoint Read()
        {
            byte[] buffer = new byte[28];
            if (instream.Read(buffer, 0, 28) != 28)
            {
                error = (new EndOfStreamException()).Message
                    ; return null;
            }

            point = new LasPoint();
=======
        public unsafe bool Read()
        {
            byte[] buffer = new byte[28];
            if (instream.Read(buffer, 0, 28) != 28) throw new EndOfStreamException();

            LasPoint_1_2_Format1 laspoint = new LasPoint_1_2_Format1();
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09

            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format1* p = (point_1_2_format1*)pBuffer;
<<<<<<< HEAD
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

            return this.point;
=======
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

    public class LasPoint_1_2_Format2_Reader : LasPointReaderBase, ILasPointReader
    {
        public LasPoint_1_2_Format2_Reader() { }

        public LasPoint_1_2_Format2_Reader(Stream instream)
        {
            this.instream = instream;
        }

<<<<<<< HEAD
        public unsafe LasPoint Read()
        {
            byte[] buffer = new byte[26];
            if (instream.Read(buffer, 0, 26) != 26)
            {
                error = (new EndOfStreamException()).Message;
                return null;
            }

            point = new LasPoint();
=======
        public unsafe bool Read()
        {
            byte[] buffer = new byte[26];
            if (instream.Read(buffer, 0, 26) != 26) throw new EndOfStreamException();

            LasPoint_1_2_Format2 laspoint = new LasPoint_1_2_Format2();
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09

            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format2* p = (point_1_2_format2*)pBuffer;
<<<<<<< HEAD
                point.X = p->x;
                point.Y = p->y;
                point.Z = p->z;
                point.intensity = p->intensity;
                point.flags = p->flags;
                point.classification = p->classification;
                point.scan_angle_rank = p->scan_angle_rank;
                point.user_data = p->user_data;
                point.point_source_ID = p->point_source_ID;
                point.red = p->red;
                point.green = p->green;
                point.blue = p->blue;
            }

            return this.point;
=======
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
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
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

<<<<<<< HEAD
        public unsafe LasPoint Read()
        {
            byte[] buffer = new byte[34];
            if (instream.Read(buffer, 0, 34) != 34)
            {
                error = (new EndOfStreamException()).Message;
                return null;
            }

            //#if DEBUG
            //            Debug.WriteLine(" Buffer: " + BitConverter.ToString(buffer));
            //#endif 

            point = new LasPoint();
=======
        public unsafe bool Read()
        {
            byte[] buffer = new byte[34];
            if (instream.Read(buffer, 0, 34) != 34) throw new EndOfStreamException();

            LasPoint_1_2_Format3 laspoint = new LasPoint_1_2_Format3();
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09

            fixed (byte* pBuffer = buffer)
            {
                point_1_2_format3* p = (point_1_2_format3*)pBuffer;
<<<<<<< HEAD
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
                point.red = p->red;
                point.green = p->green;
                point.blue = p->blue;
            }

            return this.point;
        }

        [Obsolete]
        /// <summary>
        /// Use C# traditional reading method. This function is only used to test.
        /// </summary>
        /// <returns></returns>
        public LasPoint Read2()
        {
            byte[] buffer = new byte[34];
            //if (instream.Read(buffer, 0, 34) != 34) { error = (new EndOfStreamException()).Message); return null; }

            if (instream.Length - instream.Position < 34)
            {
                error = (new EndOfStreamException()).Message;
                return null;
            }

            point = new LasPoint();
            instream.Read(buffer, 0, 4);
            point.X = BitConverter.ToInt32(buffer, 0);
            instream.Read(buffer, 0, 4);                                //8
            point.Y = BitConverter.ToInt32(buffer, 0);
            instream.Read(buffer, 0, 4);                                //12
            point.Z = BitConverter.ToInt32(buffer, 0);
            instream.Read(buffer, 0, 2);                                //14
            point.intensity = BitConverter.ToUInt16(buffer, 0);
            instream.Read(buffer, 0, 1);                                //15
            point.flags = buffer[0];
            instream.Read(buffer, 0, 1);                                //16                                         
            point.classification = buffer[0];
            instream.Read(buffer, 0, 1);                                //17
            point.scan_angle_rank = (sbyte)buffer[0];
            instream.Read(buffer, 0, 1);                                //18
            point.user_data = buffer[0];
            instream.Read(buffer, 0, 2);                                //20
            point.point_source_ID = BitConverter.ToUInt16(buffer, 0);
            instream.Read(buffer, 0, 8);                                //28
            point.gps_time = BitConverter.ToDouble(buffer, 0);
            instream.Read(buffer, 0, 2);                                //30
            point.red = BitConverter.ToUInt16(buffer, 0);
            instream.Read(buffer, 0, 2);                                //32
            point.green = BitConverter.ToUInt16(buffer, 0);
            instream.Read(buffer, 0, 2);                                //34
            point.blue = BitConverter.ToUInt16(buffer, 0);


            return point;
=======
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
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
        }

        /// <summary>
        /// Get the point size
        /// </summary>
        /// <returns></returns>
        public int GetPointSize()
        {
<<<<<<< HEAD
            return 34;
=======
            return 44;
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
        }

    }
}
