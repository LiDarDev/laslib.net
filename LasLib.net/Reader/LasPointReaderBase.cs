
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
        protected Object point;

        public Object GetPoint()
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

        //byte[] buffer = new byte[17];

        ///// <summary>
        ///// 读取点基础数据
        ///// </summary>
        ///// <returns></returns>
        //protected unsafe LasPointBase Read()
        //{
            
        //    if (instream.Read(buffer, 0, 17) != 17) throw new EndOfStreamException();

        //    fixed (byte* pBuffer = buffer)
        //    {
        //        point_base* p = (point_base*)pBuffer;
        //        this.pointBase.X = p->x;
        //        this.pointBase.Y = p->y;
        //        this.pointBase.Z = p->z;
        //        this.pointBase.intensity = p->intensity;
        //        this.pointBase.flags = p->flags;
        //        this.pointBase.classification = p->classification;
        //        this.pointBase.scan_angle_rank = p->scan_angle_rank;
        //    }

        //    return pointBase;
        //}
    }
}
