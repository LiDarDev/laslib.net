
/////////////////////////////////////////////////////////////////////
/// File    : LasPoint_1_0_Writer.cs
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
    /// Writer for the point of R1.0 and format 0.
    /// </summary>
    public class LasPoint_1_0_Format0_Writer : LasPointWriterBase, ILasPointWriter
    {
        public LasPoint_1_0_Format0_Writer(Stream streamout)
        {
            this.outstream = streamout;
        }

        public bool Write(LasPoint point)
        {
            byte[] buffer = point.GetBuffer_Point_1_0_Format0();
            try
            {
                outstream.Write(buffer, 0, PointSize.POINT_1_0_FORMAT0);
            }
            catch(Exception ex)
            {
                this.error = ex.Message;
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Writer for the point of R1.0 and format 1.
    /// </summary>
    public class LasPoint_1_0_Format1_Writer : LasPointWriterBase, ILasPointWriter
    {
        public LasPoint_1_0_Format1_Writer(Stream streamout)
        {
            this.outstream = streamout;
        }

        public bool Write(LasPoint point)
        {
            byte[] buffer = point.GetBuffer_Point_1_0_Format1();
            try
            {
                outstream.Write(buffer, 0, PointSize.POINT_1_0_FORMAT1);
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
