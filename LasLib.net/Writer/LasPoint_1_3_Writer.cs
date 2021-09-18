
/////////////////////////////////////////////////////////////////////
/// File    : LasPoint_1_3_Writer.cs
/// Desc    : Las point of R1.0 writer.
/// Author  : Li G.Q.
/// Date    : 2021/9/18/
/////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using LasLibNet.Model;
using LasLibNet.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Writer
{
    /// <summary>
    /// Writer for the point of R1.2 and format 0.
    /// </summary>
    public class LasPoint_1_3_Format0_Writer : LasPointWriterBase, ILasPointWriter
    {
        public LasPoint_1_3_Format0_Writer(Stream streamout)
        {
            this.outstream = streamout;
        }

        public bool Write(LasPoint point)
        {
            byte[] buffer = point.GetBuffer_Point_1_3_Format0();
            try
            {
                outstream.Write(buffer, 0, PointSize.POINT_1_3_FORMAT0);
            }
            catch (Exception ex)
            {
                this.error = ex.Message;
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Writer for the point of R1.2 and format 0.
    /// </summary>
    public class LasPoint_1_3_Format1_Writer : LasPointWriterBase, ILasPointWriter
    {
        public LasPoint_1_3_Format1_Writer(Stream streamout)
        {
            this.outstream = streamout;
        }

        public bool Write(LasPoint point)
        {
            byte[] buffer = point.GetBuffer_Point_1_3_Format1();
            try
            {
                outstream.Write(buffer, 0, PointSize.POINT_1_3_FORMAT1);
            }
            catch (Exception ex)
            {
                this.error = ex.Message;
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Writer for the point of R1.2 and format 2.
    /// </summary>
    public class LasPoint_1_3_Format2_Writer : LasPointWriterBase, ILasPointWriter
    {
        public LasPoint_1_3_Format2_Writer(Stream streamout)
        {
            this.outstream = streamout;
        }

        public bool Write(LasPoint point)
        {
            byte[] buffer = point.GetBuffer_Point_1_3_Format2();
            try
            {
                outstream.Write(buffer, 0, PointSize.POINT_1_3_FORMAT2);
            }
            catch (Exception ex)
            {
                this.error = ex.Message;
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Writer for the point of R1.2 and format 3.
    /// </summary>
    public class LasPoint_1_3_Format3_Writer : LasPointWriterBase, ILasPointWriter
    {
        public LasPoint_1_3_Format3_Writer(Stream streamout)
        {
            this.outstream = streamout;
        }

        public bool Write(LasPoint point)
        {
            byte[] buffer = point.GetBuffer_Point_1_3_Format3();
            try
            {
                outstream.Write(buffer, 0, PointSize.POINT_1_3_FORMAT3);
            }
            catch (Exception ex)
            {
                this.error = ex.Message;
                return false;
            }

            return true;
        }
    }
}
