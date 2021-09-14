///////////////////////////////////////////////////////////////////////
/// File    : LasItemReader.cs
/// Desc    : Las item reader abstract class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet;
using LasLibNet.Abstract;
using System;
using System.IO;

namespace LasLibNet.Implement
{
	class LasRawItemReader_GPSTIME11 : LasRawItemReader
	{
		public LasRawItemReader_GPSTIME11() { }

		public override void Read(LasPoint item)
		{
			if(instream.Read(buffer, 0, 8)!=8) throw new EndOfStreamException();

			item.gps_time=BitConverter.ToDouble(buffer, 0);
		}

		byte[] buffer=new byte[8];
	}
}
