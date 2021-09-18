
///////////////////////////////////////////////////////////////////////
/// File    : ILasPointWriter.cs
/// Desc    : Las point data writer interface.
/// Author  : Li G.Q.
/// Date    : 2021/9/17/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Abstract
{
    public interface ILasPointWriter
    {
        bool Write(LasPoint point);
    }
}
