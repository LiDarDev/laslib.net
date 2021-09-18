
///////////////////////////////////////////////////////////////////////
/// File    : LasPointReaderBase.cs
/// Desc    : Las point reader baseclass.
/// Author  : Li G.Q.
/// Date    : 2021/9/17/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Reader
{
    public class LasPointReaderBase
    {
        /// <summary>
        /// 输入流
        /// </summary>
        protected Stream instream = null;

        /// <summary>
        /// Current point
        /// </summary>
        protected LasPoint point;

        public LasPoint GetPoint()
        {
            return this.point;
        }


        public LasPointReaderBase() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="instream">输入流</param>
        public LasPointReaderBase(Stream instream)
        {
            this.instream = instream;
        }

        protected string error;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get => error; }
    }
}
