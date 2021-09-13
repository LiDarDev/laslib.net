///////////////////////////////////////////////////////////////////////
/// File    : LasItemReader.cs
/// Desc    : Las item reader abstract class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet;
using System.IO;

namespace LasLibNet
{
	class LasRawItemReader_BYTE : LasRawItemReader
	{
		public LasRawItemReader_BYTE(uint number) { this.number=number; }

		public override void Read(LasPoint item)
		{
			if(instream.Read(item.extra_bytes, 0, (int)number)!=(int)number) throw new EndOfStreamException();
		}

		uint number=0;
	}
}
