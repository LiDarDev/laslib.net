
/////////////////////////////////////////////////////////////////////
/// File    : LasWriter.cs
/// Desc    : Las file writer.
/// Author  : Li G.Q.
/// Date    : 2021/9/18/
/////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Writer
{
    /// <summary>
    /// Las File writer
    /// </summary>
    public class LasWriter:FileWriter,IFileWriter
    {        
        public LasWriter()
        {
            
        }

        /// <summary>
        /// Constructure function
        /// </summary>
        /// <param name="outs">out stream</param>
        /// <param name="header">the las header</param>
        public LasWriter( LasHeader header)
        {           
            this.header = header;
        }
	}

}
