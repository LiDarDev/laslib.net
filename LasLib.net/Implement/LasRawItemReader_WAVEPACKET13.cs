///////////////////////////////////////////////////////////////////////
/// File    : LasRawItemReader.cs
/// Desc    : Las raw item reader implement class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet;
using LasLibNet.Abstract;
using System.IO;

namespace LasLibNet.Implement
{
	class LasRawItemReader_WAVEPACKET13 : LasRawItemReader
	{
		public LasRawItemReader_WAVEPACKET13() { }

		public override void Read(LasPoint item)
		{
			if(instream.Read(item.wave_packet, 0, 29)!=29) throw new EndOfStreamException();
		}
	}
}
