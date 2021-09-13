///////////////////////////////////////////////////////////////////////
/// File    : LasItemReader.cs
/// Desc    : Las item reader abstract class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using System.IO;
using System.Runtime.InteropServices;

namespace LasLibNet.Implement
{
	class LasRawItemReader_POINT10 : LasRawItemReader
	{
		[StructLayout(LayoutKind.Sequential, Pack=1)]
		struct LAStempReadPoint10
		{
			public int x;
			public int y;
			public int z;
			public ushort intensity;
			public byte flags;
			public byte classification;
			public sbyte scan_angle_rank;
			public byte user_data;
			public ushort point_source_ID;
		}

		public LasRawItemReader_POINT10() { }
		public unsafe override void Read(LasPoint item)
		{
			if(instream.Read(buffer, 0, 20)!=20) throw new EndOfStreamException();

			fixed(byte* pBuffer=buffer)
			{
				LAStempReadPoint10* p10=(LAStempReadPoint10*)pBuffer;
				item.X=p10->x;
				item.Y=p10->y;
				item.Z=p10->z;
				item.intensity=p10->intensity;
				item.flags=p10->flags;
				item.classification=p10->classification;
				item.scan_angle_rank=p10->scan_angle_rank;
				item.user_data=p10->user_data;
				item.point_source_ID=p10->point_source_ID;
			}
		}

		byte[] buffer=new byte[20];
	}
}
