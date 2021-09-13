///////////////////////////////////////////////////////////////////////
/// File    : LasItemReader.cs
/// Desc    : Las item reader abstract class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using System;
using System.IO;

namespace LasLibNet.Implement
{
	class LasRawItemReader_RGB12 : LasRawItemReader
	{
		public LasRawItemReader_RGB12() { }

		public override void Read(LasPoint item)
		{
			byte[] buf=new byte[6];
			if(instream.Read(buf, 0, 6)!=6) throw new EndOfStreamException();
			item.rgb[0]=BitConverter.ToUInt16(buf, 0);
			item.rgb[1]=BitConverter.ToUInt16(buf, 2);
			item.rgb[2]=BitConverter.ToUInt16(buf, 4);
		}
	}
}
