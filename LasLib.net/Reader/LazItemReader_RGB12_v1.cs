///////////////////////////////////////////////////////////////////////
/// File    : LasItemReader.cs
/// Desc    : Las item reader abstract class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using LasLibNet.Model;
using LasLibNet.Utils;
using System.Diagnostics;

namespace LasLibNet.Implement
{
	class LazItemReader_RGB12_v1 : LazItemReader
	{
		public LazItemReader_RGB12_v1(ArithmeticDecoder dec)
		{
			// set decoder
			Debug.Assert(dec!=null);
			this.dec=dec;

			// create models and integer compressors
			m_byte_used=dec.createSymbolModel(64);
			ic_rgb=new IntegerCompressor(dec, 8, 6);
		}

		public override bool Init(LasPoint item)
		{
			// init state

			// init models and integer compressors
			dec.initSymbolModel(m_byte_used);
			ic_rgb.initDecompressor();

			// init last item
			r=item.red;
			g=item.green;
			b=item.blue;

			return true;
		}

		public override void Read(LasPoint item)
		{
			uint sym=dec.decodeSymbol(m_byte_used);

			ushort r1=item.red,g1=item.green,b1=item.blue;

			if((sym&(1<<0))!=0) r1=(ushort)ic_rgb.decompress(r&255, 0);
			else r1=(ushort)(r&0xFF);

			if((sym&(1<<1))!=0) r1|=(ushort)(((ushort)ic_rgb.decompress(r>>8, 1))<<8);
			else r1|=(ushort)(r&0xFF00);

			if((sym&(1<<2))!=0) g1=(ushort)ic_rgb.decompress(g&255, 2);
			else g1=(ushort)(g&0xFF);

			if((sym&(1<<3))!=0) g1|=(ushort)(((ushort)ic_rgb.decompress(g>>8, 3))<<8);
			else g1|=(ushort)(g&0xFF00);

			if((sym&(1<<4))!=0) b1=(ushort)ic_rgb.decompress(b&255, 4);
			else b1=(ushort)(b&0xFF);

			if((sym&(1<<5))!=0) b1|=(ushort)(((ushort)ic_rgb.decompress(b>>8, 5))<<8);
			else b1|=(ushort)(b&0xFF00);

			r=r1;
			g=g1;
			b=b1;
		}

		ArithmeticDecoder dec;
		ushort r, g, b;

		ArithmeticModel m_byte_used;
		IntegerCompressor ic_rgb;
	}
}
