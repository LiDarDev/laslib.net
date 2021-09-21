///////////////////////////////////////////////////////////////////////
/// File    : LasFile.cs
/// Desc    : Las File info.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet
{
    /// <summary>
    /// Las File Info.
    /// </summary>
    public class LasFile
    {
		public const int VERSION_MAJOR = 2;
		public const int VERSION_MINOR = 2;
		public const int VERSION_REVISION = 0;
		public const int VERSION_BUILD_DATE = 20210918;

		public const int COMPRESSOR_NONE = 0;
		public const int COMPRESSOR_POINTWISE = 1;
		public const int COMPRESSOR_POINTWISE_CHUNKED = 2;
		public const int COMPRESSOR_TOTAL_NUMBER_OF = 3;

		public const int COMPRESSOR_CHUNKED = COMPRESSOR_POINTWISE_CHUNKED;
		public const int COMPRESSOR_NOT_CHUNKED = COMPRESSOR_POINTWISE;

		public const int COMPRESSOR_DEFAULT = COMPRESSOR_CHUNKED;

		public const int CODER_ARITHMETIC = 0;
		public const int CODER_TOTAL_NUMBER_OF = 1;

		public const int CHUNK_SIZE_DEFAULT = 50000;

		protected string error;

		// pack to and unpack from VLR
		public byte[] bytes;
	
		// in case a function returns false this string describes the problem
		public string Error { get=> error; }
				

		public LasFile()
		{			
			error = null;			
			bytes = null;
		}

		


	}
}
