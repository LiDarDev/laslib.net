
//////////////////////////////////////////////////////////////////////
/// File    : IFileWriter.cs
/// Desc    : File writer interface
/// Author  : Li G.Q.
/// Date    : 2021/9/22/
//////////////////////////////////////////////////////////////////////

using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Abstract
{
    /// <summary>
    /// File writer interface
    /// </summary>
    public interface IFileWriter
    {   
        /// <summary>
        /// File Header info.
        /// </summary>
        LasHeader Header { get; set; }

        /// <summary>
        /// Point count to be written.
        /// </summary>
        long PointCount { get; }

        /// <summary>
        /// Error message
        /// </summary>
        string Error { get; }

        /// <summary>
        /// Set file header
        /// </summary>
        /// <param name="header"></param>
        void SetHeader(LasHeader header);

        /// <summary>
        /// Create and open a out stream for the filename.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        bool OpenWriter(string filename);

        /// <summary>
        /// Close the out stream and the point writer.
        /// </summary>
        /// <returns></returns>
        bool CloseWriter();

        /// <summary>
        /// Create a writer for cloud point.
        /// </summary>
        /// <returns></returns>
        bool CreatePointWriter();

        /// <summary>
        /// Write the file header.
        /// </summary>
        /// <returns></returns>
        bool WriteHeader();

        /// <summary>
        /// Write the file header.
        /// </summary>
        /// <param name="header">The distination header</param>
        /// <returns></returns>
        bool WriteHeader(LasHeader header);

        /// <summary>
        /// Write the file header.
        /// </summary>
        /// <param name="header">The file byte array.</param>
        /// <returns></returns>
        bool WriteHeader(byte[] header);

        /// <summary>
        /// Write a point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        bool WritePoint(LasPoint point);
    }
}
