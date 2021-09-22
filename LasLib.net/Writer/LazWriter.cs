using LasLibNet.Abstract;
using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Writer
{

    public class LazWriter : FileWriter, IFileWriter
    {
        public LazWriter()
        {

        }

        /// <summary>
        /// Constructure function
        /// </summary>        
        /// <param name="header">the las header</param>
        public LazWriter(LasHeader header)
        {
            this.header = header;
        }

        public bool CreatePointWriter()
        {
            throw new NotImplementedException();
        }

        bool IFileWriter.CloseWriter()
        {
            throw new NotImplementedException();
        }


        bool IFileWriter.OpenWriter(string filename)
        {
            throw new NotImplementedException();
        }

        void IFileWriter.SetHeader(LasHeader header)
        {
            throw new NotImplementedException();
        }

        bool IFileWriter.WriteHeader()
        {
            throw new NotImplementedException();
        }

        bool IFileWriter.WriteHeader(LasHeader header)
        {
            throw new NotImplementedException();
        }

        bool IFileWriter.WriteHeader(byte[] header)
        {
            throw new NotImplementedException();
        }

        bool IFileWriter.WritePoint(LasPoint point)
        {
            throw new NotImplementedException();
        }
    }
}
