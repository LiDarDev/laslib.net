///////////////////////////////////////////////////////////////////////
/// File    : LazItemReader_BYTE_v1.cs
/// Desc    : Las compressed item reader for BYTE_v1.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using LasLibNet.Model;
using LasLibNet.Utils;
using System;
using System.Diagnostics;

namespace LasLibNet.Implement
{
	class LazItemReader_BYTE_v1 : LazItemReader
	{
		public LazItemReader_BYTE_v1(ArithmeticDecoder dec, uint number)
		{
			// set decoder
			Debug.Assert(dec!=null);
			this.dec=dec;
			Debug.Assert(number!=0);
			this.number=number;

			// create models and integer compressors
			ic_byte=new IntegerCompressor(dec, 8, number);

			// create last item
			last_item=new byte[number];
		}

		public override bool Init(LasPoint item)
		{
			// init state

			// init models and integer compressors
			ic_byte.initDecompressor();

			// init last item
			Buffer.BlockCopy(item.extra_bytes, 0, last_item, 0, (int)number);

			return true;
		}

		public override void Read(LasPoint item)
		{
			for(uint i=0; i<number; i++)
			{
				last_item[i]=item.extra_bytes[i]=(byte)(ic_byte.decompress(last_item[i], i));
			}
		}

		ArithmeticDecoder dec;
		uint number;
		byte[] last_item;

		IntegerCompressor ic_byte;
	}
}
