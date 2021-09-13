///////////////////////////////////////////////////////////////////////
/// File    : LasItemReader.cs
/// Desc    : Las item reader abstract class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using LasLibNet.Utils;
using System;
using System.Diagnostics;

namespace LasLibNet.Implement
{
	class LasCompressedItemReader_BYTE_v2 : LasCompressedItemReader
	{
		public LasCompressedItemReader_BYTE_v2(ArithmeticDecoder dec, uint number)
		{
			// set decoder
			Debug.Assert(dec!=null);
			this.dec=dec;
			Debug.Assert(number>0);
			this.number=number;

			// create models and integer compressors
			m_byte=new ArithmeticModel[number];
			for(uint i=0; i<number; i++)
			{
				m_byte[i]=dec.createSymbolModel(256);
			}

			// create last item
			last_item=new byte[number];
		}

		public override bool Init(LasPoint item)
		{
			// init state

			// init models and integer compressors
			for(uint i=0; i<number; i++)
			{
				dec.initSymbolModel(m_byte[i]);
			}

			// init last item
			Buffer.BlockCopy(item.extra_bytes, 0, last_item, 0, (int)number);

			return true;
		}

		public override void Read(LasPoint item)
		{
			for(uint i=0; i<number; i++)
			{
				int value=(int)(last_item[i]+dec.decodeSymbol(m_byte[i]));
				item.extra_bytes[i]=(byte)MyDefs.U8_FOLD(value);
			}

			Buffer.BlockCopy(item.extra_bytes, 0, last_item, 0, (int)number);
		}

		ArithmeticDecoder dec;
		uint number;
		byte[] last_item;

		ArithmeticModel[] m_byte;
	}
}
