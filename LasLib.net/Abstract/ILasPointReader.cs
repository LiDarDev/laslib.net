
///////////////////////////////////////////////////////////////////////
/// File    : ILasPointReader.cs
/// Desc    : Las point data reader/parser interface.
/// Author  : Li G.Q.
/// Date    : 2021/9/17/
///////////////////////////////////////////////////////////////////////


<<<<<<< HEAD
using LasLibNet.Model;
=======
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09
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
<<<<<<< HEAD
        LasPoint Read();
=======
        bool Read();
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09

        /// <summary>
        /// Get current point
        /// </summary>
        /// <returns></returns>
<<<<<<< HEAD
        LasPoint GetPoint();
=======
        Object GetPoint();
>>>>>>> 9ef5d2225b60abb57dc40befb324256f99f13d09

        /// <summary>
        /// The data size of a point.
        /// </summary>
        /// <returns></returns>
        int GetPointSize();
    }
}
