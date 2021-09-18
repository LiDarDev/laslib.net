using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Writer
{
    /// <summary>
    /// Las point writer base class
    /// </summary>
    public class LasPointWriterBase
    {
        protected Stream outstream;

        protected string error;
        public string Error { get => error; }

        public LasPointWriterBase() { }

    }
}
