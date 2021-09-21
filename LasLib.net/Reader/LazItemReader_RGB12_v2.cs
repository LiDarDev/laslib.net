///////////////////////////////////////////////////////////////////////
/// File    : LasItemReader.cs
/// Desc    : Las item reader abstract class.
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
	class LazItemReader_RGB12_v2 : LazItemReader
	{
		public LazItemReader_RGB12_v2(ArithmeticDecoder dec)
		{
			// set decoder
			Debug.Assert(dec!=null);
			this.dec=dec;

			// create models and integer compressors
			m_byte_used=dec.createSymbolModel(128);
			m_rgb_diff_0=dec.createSymbolModel(256);
			m_rgb_diff_1=dec.createSymbolModel(256);
			m_rgb_diff_2=dec.createSymbolModel(256);
			m_rgb_diff_3=dec.createSymbolModel(256);
			m_rgb_diff_4=dec.createSymbolModel(256);
			m_rgb_diff_5=dec.createSymbolModel(256);
		}

		public override bool Init(LasPoint item)
		{
			// init state

			// init models and integer compressors
			dec.initSymbolModel(m_byte_used);
			dec.initSymbolModel(m_rgb_diff_0);
			dec.initSymbolModel(m_rgb_diff_1);
			dec.initSymbolModel(m_rgb_diff_2);
			dec.initSymbolModel(m_rgb_diff_3);
			dec.initSymbolModel(m_rgb_diff_4);
			dec.initSymbolModel(m_rgb_diff_5);

			// init last item			
			red = item.red;
			green = item.green;
			blue = item.blue;

			return true;
		}

		public override void Read(LasPoint item)
		{
			int corr;
			int diff=0;

			uint sym=dec.decodeSymbol(m_byte_used);
			if((sym&(1<<0))!=0)
			{
				corr=(int)dec.decodeSymbol(m_rgb_diff_0);
				item.red=(ushort)MyDefs.U8_FOLD(corr+(red&255));
			}
			else
			{
				item.red=(ushort)(red&0xFF);
			}

			if((sym&(1<<1))!=0)
			{
				corr=(int)dec.decodeSymbol(m_rgb_diff_1);
				item.red|=(ushort)((MyDefs.U8_FOLD(corr+(red>>8)))<<8);
			}
			else
			{
				item.red|=(ushort)(red&0xFF00);
			}

			if((sym&(1<<6))!=0)
			{
				diff=(item.red&0x00FF)-(red&0x00FF);
				if((sym&(1<<2))!=0)
				{
					corr=(int)dec.decodeSymbol(m_rgb_diff_2);
					item.green=(ushort)MyDefs.U8_FOLD(corr+MyDefs.U8_CLAMP(diff+(green&255)));
				}
				else
				{
					item.green=(ushort)(green&0xFF);
				}

				if((sym&(1<<4))!=0)
				{
					corr=(int)dec.decodeSymbol(m_rgb_diff_4);
					diff=(diff+((item.green&0x00FF)-(green&0x00FF)))/2;
					item.blue=(ushort)MyDefs.U8_FOLD(corr+MyDefs.U8_CLAMP(diff+(blue&255)));
				}
				else
				{
					item.blue=(ushort)(blue&0xFF);
				}

				diff=(item.red>>8)-(red>>8);
				if((sym&(1<<3))!=0)
				{
					corr=(int)dec.decodeSymbol(m_rgb_diff_3);
					item.green|=(ushort)((MyDefs.U8_FOLD(corr+MyDefs.U8_CLAMP(diff+(green>>8))))<<8);
				}
				else
				{
					item.green|=(ushort)(green&0xFF00);
				}

				if((sym&(1<<5))!=0)
				{
					corr=(int)dec.decodeSymbol(m_rgb_diff_5);
					diff=(diff+((item.green>>8)-(green>>8)))/2;
					item.blue|=(ushort)((MyDefs.U8_FOLD(corr+MyDefs.U8_CLAMP(diff+(blue>>8))))<<8);
				}
				else
				{
					item.blue|=(ushort)(blue&0xFF00);
				}
			}
			else
			{
				item.green=item.red;
				item.blue=item.red;
			}

			red=item.red;
			green=item.green;
			blue=item.blue;
		}

		ArithmeticDecoder dec;
		ushort red,green,blue;

		ArithmeticModel m_byte_used;
		ArithmeticModel m_rgb_diff_0;
		ArithmeticModel m_rgb_diff_1;
		ArithmeticModel m_rgb_diff_2;
		ArithmeticModel m_rgb_diff_3;
		ArithmeticModel m_rgb_diff_4;
		ArithmeticModel m_rgb_diff_5;
	}
}
