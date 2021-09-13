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
	class LasCompressedItemReader_RGB12_v2 : LasCompressedItemReader
	{
		public LasCompressedItemReader_RGB12_v2(ArithmeticDecoder dec)
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
			Buffer.BlockCopy(item.rgb, 0, last_item, 0, 6);
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
				item.rgb[0]=(ushort)MyDefs.U8_FOLD(corr+(last_item[0]&255));
			}
			else
			{
				item.rgb[0]=(ushort)(last_item[0]&0xFF);
			}

			if((sym&(1<<1))!=0)
			{
				corr=(int)dec.decodeSymbol(m_rgb_diff_1);
				item.rgb[0]|=(ushort)((MyDefs.U8_FOLD(corr+(last_item[0]>>8)))<<8);
			}
			else
			{
				item.rgb[0]|=(ushort)(last_item[0]&0xFF00);
			}

			if((sym&(1<<6))!=0)
			{
				diff=(item.rgb[0]&0x00FF)-(last_item[0]&0x00FF);
				if((sym&(1<<2))!=0)
				{
					corr=(int)dec.decodeSymbol(m_rgb_diff_2);
					item.rgb[1]=(ushort)MyDefs.U8_FOLD(corr+MyDefs.U8_CLAMP(diff+(last_item[1]&255)));
				}
				else
				{
					item.rgb[1]=(ushort)(last_item[1]&0xFF);
				}

				if((sym&(1<<4))!=0)
				{
					corr=(int)dec.decodeSymbol(m_rgb_diff_4);
					diff=(diff+((item.rgb[1]&0x00FF)-(last_item[1]&0x00FF)))/2;
					item.rgb[2]=(ushort)MyDefs.U8_FOLD(corr+MyDefs.U8_CLAMP(diff+(last_item[2]&255)));
				}
				else
				{
					item.rgb[2]=(ushort)(last_item[2]&0xFF);
				}

				diff=(item.rgb[0]>>8)-(last_item[0]>>8);
				if((sym&(1<<3))!=0)
				{
					corr=(int)dec.decodeSymbol(m_rgb_diff_3);
					item.rgb[1]|=(ushort)((MyDefs.U8_FOLD(corr+MyDefs.U8_CLAMP(diff+(last_item[1]>>8))))<<8);
				}
				else
				{
					item.rgb[1]|=(ushort)(last_item[1]&0xFF00);
				}

				if((sym&(1<<5))!=0)
				{
					corr=(int)dec.decodeSymbol(m_rgb_diff_5);
					diff=(diff+((item.rgb[1]>>8)-(last_item[1]>>8)))/2;
					item.rgb[2]|=(ushort)((MyDefs.U8_FOLD(corr+MyDefs.U8_CLAMP(diff+(last_item[2]>>8))))<<8);
				}
				else
				{
					item.rgb[2]|=(ushort)(last_item[2]&0xFF00);
				}
			}
			else
			{
				item.rgb[1]=item.rgb[0];
				item.rgb[2]=item.rgb[0];
			}

			last_item[0]=item.rgb[0];
			last_item[1]=item.rgb[1];
			last_item[2]=item.rgb[2];
		}

		ArithmeticDecoder dec;
		ushort[] last_item=new ushort[3];

		ArithmeticModel m_byte_used;
		ArithmeticModel m_rgb_diff_0;
		ArithmeticModel m_rgb_diff_1;
		ArithmeticModel m_rgb_diff_2;
		ArithmeticModel m_rgb_diff_3;
		ArithmeticModel m_rgb_diff_4;
		ArithmeticModel m_rgb_diff_5;
	}
}
