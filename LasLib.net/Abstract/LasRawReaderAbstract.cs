///////////////////////////////////////////////////////////////////////
/// File    : LasRawReader.cs
/// Desc    : Las item raw data reader/parser.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using System.IO;

namespace LasLibNet
{
	abstract class LasRawItemReader : LasItemReader
	{
		public bool Init(Stream instream)
		{
			if(instream==null) return false;
			this.instream=instream;
			return true;
		}

		protected Stream instream=null;
	}
}
