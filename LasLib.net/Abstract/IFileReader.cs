

///////////////////////////////////////////////////////////////////////
/// File    : IFileReader.cs
/// Desc    : Las point data reader/parser interface.
/// Author  : Li G.Q.
/// Date    : 2021/9/17/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Abstract
{
    public interface IFileReader
    {
        #region Interface properties
        
        LasHeader Header { get;  }

        long PointIndex { get; }

        long PointsNumber {get; }

        LasPoint Point{get;}

        string Error { get; }

        #endregion

        #region Interface functions
        bool OpenReader(Stream streamin);

        bool OpenReader(string file_name);

        bool CloseReader();

        bool CreatePointReader();

        bool SeekPoint(long index);

        LasPoint ReadPoint();

        /// <summary>
        /// All header info include the public block and extended block.
        /// </summary>
        /// <returns></returns>
        byte[] GetExtendHeader();

        #endregion
    }
}
