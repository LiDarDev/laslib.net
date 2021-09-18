
///////////////////////////////////////////////////////////////////////
/// File    : ILasPointReader.cs
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
    /// <summary>
    /// Las point data reader/parser interface.
    /// </summary>
    public interface ILasPointReader
    {
        /// <summary>
        /// Read a point
        /// </summary>
        /// <returns></returns>
        LasPoint Read();

        /// <summary>
        /// Get current point
        /// </summary>
        /// <returns></returns>
        LasPoint GetPoint();

        /// <summary>
        /// The data size of a point.
        /// </summary>
        /// <returns></returns>
        int GetPointSize();
    }
}
