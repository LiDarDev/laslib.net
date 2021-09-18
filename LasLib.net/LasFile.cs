///////////////////////////////////////////////////////////////////////
/// File    : LasFile.cs
/// Desc    : Las File info.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

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
		const int CODER_TOTAL_NUMBER_OF = 1;

		const int CHUNK_SIZE_DEFAULT = 50000;

		// pack to and unpack from VLR
		public byte[] bytes;
	
		// in case a function returns false this string describes the problem
		public string get_error() { return error_string; }

		// stored in LasFile VLR data section
		public ushort compressor;
		public ushort coder;
		public byte version_major;
		public byte version_minor;
		public ushort version_revision;
		public uint options;
		public uint chunk_size;
		public long number_of_special_evlrs; // must be -1 if unused
		public long offset_to_special_evlrs; // must be -1 if unused
		public ushort num_items;

		public LasFile()
		{
			compressor = COMPRESSOR_DEFAULT;
			coder = CODER_ARITHMETIC;
			version_major = VERSION_MAJOR;
			version_minor = VERSION_MINOR;
			version_revision = VERSION_REVISION;
			options = 0;
			num_items = 0;
			chunk_size = CHUNK_SIZE_DEFAULT;
			number_of_special_evlrs = -1;
			offset_to_special_evlrs = -1;
			error_string = null;
			
			bytes = null;
		}

		bool return_error(string error)
		{
			error_string = string.Format("{0} (LasFile v{1}.{2}r{3})", error, VERSION_MAJOR, VERSION_MINOR, VERSION_REVISION);
			return false;
		}

		string error_string;
	}
}
