
///////////////////////////////////////////////////////////////////////
/// File    : LasReader.cs
/// Desc    : Reader class to read the las file.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using LasLibNet.Model;
using LasLibNet.Reader;
using LasLibNet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Reader
{
    /// <summary>
    /// LasReader reads the las file.
    /// </summary>
    public class LasReader: FileReader, IFileReader
    {
        /// <summary>
        /// Point reader.
        /// </summary>
        protected ILasPointReader reader;

        /// <summary>
        /// The file version.
        /// </summary>
        /// <param name="version_major"></param>
        /// <param name="version_minor"></param>
        /// <param name="version_revision"></param>
        /// <param name="version_build"></param>
        /// <returns></returns>
        public void GetVersion(out byte version_major, out byte version_minor, out ushort version_revision, out uint version_build)
        {
            version_major = LasFile.VERSION_MAJOR;
            version_minor = LasFile.VERSION_MINOR;
            version_revision = LasFile.VERSION_REVISION;
            version_build = LasFile.VERSION_BUILD_DATE;
        }


        /// <summary>
        /// Create point reader by the version and point data format.
        /// </summary>
        public bool CreatePointReader()
        {
            this.reader = base.CreateRawPointReader();
            return reader!=null;
        }


        public bool SeekPoint(long index)
        {
            try
            { 
                if (index>this.npoints)
                {
                    error = string.Format("Seeking from index {0} to index {1} for file with {2} points failed", p_sequence, index, npoints);
                    return false;
                }
                // seek to the point
                streamin.Seek(this.header.offset_to_point_data+index*this.reader.GetPointSize()
                    , SeekOrigin.Begin);
               
                p_sequence = index;
            }
            catch
            {
                error = "Internal error in seek_point";
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        /// Read a next point, and set the data to the variable point.
        /// </summary>
        /// <returns></returns>
        public LasPoint ReadPoint()
        {
            if (reader == null)
            {
                error = "Reader wasn't opened. Please open the reader before you read a point.";
                return null;
            }

            try
            {
                // read the point
                if (reader.Read()==null)
                {
                    error = string.Format("Reading point with index {0} of {1} total points failed", p_sequence, npoints);
                    return null;
                }

                p_sequence++;
            }
            catch
            {
                error = "Internal error in read_point";
                return null;
            }

            error = null;
            return reader.GetPoint();
        }
               

    }
}
