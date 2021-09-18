
///////////////////////////////////////////////////////////////////////
/// File    : LasPoint_1_2.cs
/// Desc    : Las point model for r1.2
/// Author  : Li G.Q.
/// Date    : 2021/9/17/
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Model
{
    /// <summary>
    /// LAS1.2 Point model for format 0. 
    /// </summary>
    public class LasPoint_1_2_Format0:LasPoint_1_1_Format0
    {

    }

    /// <summary>
    /// LAS1.2 Point model for format 1. 
    /// </summary>
    public class LasPoint_1_2_Format1: LasPoint_1_1_Format1
    {

    }

    /// <summary>
    /// LAS1.2 Point model for format 2. 
    /// </summary>
    public class LasPoint_1_2_Format2: LasPoint_1_1_Format0
    {
        protected ushort blue;
        protected ushort red;
        protected ushort green;

        public ushort Red { get => red; set => red = value; }
        public ushort Green { get => green; set => green = value; }
        public ushort Blue { get => blue; set => blue = value; }
    }

    /// <summary>
    /// LAS1.2 Point model for format 3. 
    /// </summary>
    public class LasPoint_1_2_Format3 : LasPoint_1_2_Format1
    {
        protected ushort blue;
        protected ushort red;
        protected ushort green;

        public ushort Red { get => red; set => red = value; }
        public ushort Green { get => green; set => green = value; }
        public ushort Blue { get => blue; set => blue = value; }
    }

}
