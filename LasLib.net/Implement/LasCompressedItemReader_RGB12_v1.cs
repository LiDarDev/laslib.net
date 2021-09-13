///////////////////////////////////////////////////////////////////////
/// File    : LasItemReader.cs
/// Desc    : Las item reader abstract class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using LasLibNet.Utils;
using System.Diagnostics;

namespace LasLibNet.Implement
{
	class LasCompressedItemReader_RGB12_v1 : LasCompressedItemReader
	{
		public LasCompressedItemReader_RGB12_v1(ArithmeticDecoder dec)
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
			r=item.rgb[0];
			g=item.rgb[1];
			b=item.rgb[2];

			return true;
		}

		public override void Read(LasPoint item)
		{
			uint sym=dec.decodeSymbol(m_byte_used);

			ushort[] item16=item.rgb;

			if((sym&(1<<0))!=0) item16[0]=(ushort)ic_rgb.decompress(r&255, 0);
			else item16[0]=(ushort)(r&0xFF);

			if((sym&(1<<1))!=0) item16[0]|=(ushort)(((ushort)ic_rgb.decompress(r>>8, 1))<<8);
			else item16[0]|=(ushort)(r&0xFF00);

			if((sym&(1<<2))!=0) item16[1]=(ushort)ic_rgb.decompress(g&255, 2);
			else item16[1]=(ushort)(g&0xFF);

			if((sym&(1<<3))!=0) item16[1]|=(ushort)(((ushort)ic_rgb.decompress(g>>8, 3))<<8);
			else item16[1]|=(ushort)(g&0xFF00);

			if((sym&(1<<4))!=0) item16[2]=(ushort)ic_rgb.decompress(b&255, 4);
			else item16[2]=(ushort)(b&0xFF);

			if((sym&(1<<5))!=0) item16[2]|=(ushort)(((ushort)ic_rgb.decompress(b>>8, 5))<<8);
			else item16[2]|=(ushort)(b&0xFF00);

			r=item16[0];
			g=item16[1];
			b=item16[2];
		}

		ArithmeticDecoder dec;
		ushort r, g, b;

		ArithmeticModel m_byte_used;
		IntegerCompressor ic_rgb;
	}
}
