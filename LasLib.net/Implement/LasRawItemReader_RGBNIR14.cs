///////////////////////////////////////////////////////////////////////
/// File    : LasRawItemReader.cs
/// Desc    : Las Raw item reader implement class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet;
using LasLibNet.Abstract;
using System;
using System.IO;

namespace LasLibNet.Implement
{
	class LasRawItemReader_RGBNIR14 : LasRawItemReader
	{
		public LasRawItemReader_RGBNIR14() { }

        public override void Read(LasPoint item)
		{
			byte[] buf=new byte[8];
			if(instream.Read(buf, 0, 8)!=8) throw new EndOfStreamException();
			item.rgb[0]=BitConverter.ToUInt16(buf, 0);
			item.rgb[1]=BitConverter.ToUInt16(buf, 2);
			item.rgb[2]=BitConverter.ToUInt16(buf, 4);
			item.rgb[3]=BitConverter.ToUInt16(buf, 6);
		}
	}
}
