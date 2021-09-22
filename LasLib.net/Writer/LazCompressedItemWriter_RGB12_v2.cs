//===============================================================================
//
//  FILE:  laswriteitemcompressed_rgb12_v2.cs
//
//  CONTENTS:
//
//    Implementation of LasCompressedItemWriter for RGB12 items (version 2).
//
//  PROGRAMMERS:
//
//    martin.isenburg@rapidlasso.com  -  http://rapidlasso.com
//
//  COPYRIGHT:
//
//    (c) 2005-2012, martin isenburg, rapidlasso - tools to catch reality
//    (c) of the C# port 2014 by Shinta <shintadono@googlemail.com>
//
//    This is free software; you can redistribute and/or modify it under the
//    terms of the GNU Lesser General Licence as published by the Free Software
//    Foundation. See the COPYING file for more information.
//
//    This software is distributed WITHOUT ANY WARRANTY and without even the
//    implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//
//  CHANGE HISTORY: omitted for easier Copy&Paste (pls see the original)
//
//===============================================================================

using LasLibNet.Abstract;
using LasLibNet.Model;
using LasLibNet.Utils;
using System;
using System.Diagnostics;

namespace LasLibNet.Implement
{
	class LasCompressedItemWriter_RGB12_v2 : LasCompressedItemWriter
	{
		public LasCompressedItemWriter_RGB12_v2(ArithmeticEncoder enc)
		{
			// set encoder
			Debug.Assert(enc!=null);
			this.enc=enc;

			// create models and integer compressors
			m_byte_used=enc.createSymbolModel(128);
			m_rgb_diff_0=enc.createSymbolModel(256);
			m_rgb_diff_1=enc.createSymbolModel(256);
			m_rgb_diff_2=enc.createSymbolModel(256);
			m_rgb_diff_3=enc.createSymbolModel(256);
			m_rgb_diff_4=enc.createSymbolModel(256);
			m_rgb_diff_5=enc.createSymbolModel(256);
		}

		public override bool init(LasPoint item)
		{
			// init state

			// init models and integer compressors
			enc.initSymbolModel(m_byte_used);
			enc.initSymbolModel(m_rgb_diff_0);
			enc.initSymbolModel(m_rgb_diff_1);
			enc.initSymbolModel(m_rgb_diff_2);
			enc.initSymbolModel(m_rgb_diff_3);
			enc.initSymbolModel(m_rgb_diff_4);
			enc.initSymbolModel(m_rgb_diff_5);

			// init last item
			//Buffer.BlockCopy(item.rgb, 0, last_item, 0, 6);
			red = item.red;
			green = item.green;
			blue = item.blue;

			return true;
		}

		public override bool write(LasPoint item)
		{
			int diff_l=0;
			int diff_h=0;

			uint sym=0;

			bool rl=(red&0x00FF)!=(item.red&0x00FF); if(rl) sym|=1;
			bool rh=(red&0xFF00)!=(item.red&0xFF00); if(rh) sym|=2;
			bool gl=(green&0x00FF)!=(item.green&0x00FF); if(gl) sym|=4;
			bool gh=(green&0xFF00)!=(item.green&0xFF00); if(gh) sym|=8;
			bool bl=(blue&0x00FF)!=(item.blue&0x00FF); if(bl) sym|=16;
			bool bh=(blue&0xFF00)!=(item.blue&0xFF00); if(bh) sym|=32;
			
			bool allColors=((item.red&0x00FF)!=(item.green&0x00FF))||((item.red&0x00FF)!=(item.blue&0x00FF))||
				((item.red&0xFF00)!=(item.green&0xFF00))||((item.red&0xFF00)!=(item.blue&0xFF00));
			if(allColors) sym|=64;

			enc.encodeSymbol(m_byte_used, sym);
			if(rl)
			{
				diff_l=((int)(item.red&255))-(red&255);
				enc.encodeSymbol(m_rgb_diff_0, (byte)MyDefs.U8_FOLD(diff_l));
			}
			if(rh)
			{
				diff_h=((int)(item.red>>8))-(red>>8);
				enc.encodeSymbol(m_rgb_diff_1, (byte)MyDefs.U8_FOLD(diff_h));
			}

			if(allColors)
			{
				if(gl)
				{
					int corr=((int)(item.green&255))-MyDefs.U8_CLAMP(diff_l+(green&255));
					enc.encodeSymbol(m_rgb_diff_2, (byte)MyDefs.U8_FOLD(corr));
				}
				if(bl)
				{
					diff_l=(diff_l+(item.green&255)-(green&255))/2;
					int corr=((int)(item.blue&255))-MyDefs.U8_CLAMP(diff_l+(blue&255));
					enc.encodeSymbol(m_rgb_diff_4, (byte)MyDefs.U8_FOLD(corr));
				}
				if(gh)
				{
					int corr=((int)(item.green>>8))-MyDefs.U8_CLAMP(diff_h+(green>>8));
					enc.encodeSymbol(m_rgb_diff_3, (byte)MyDefs.U8_FOLD(corr));
				}
				if(bh)
				{
					diff_h=(diff_h+(item.green>>8)-(green>>8))/2;
					int corr=((int)(item.blue>>8))-MyDefs.U8_CLAMP(diff_h+(blue>>8));
					enc.encodeSymbol(m_rgb_diff_5, (byte)MyDefs.U8_FOLD(corr));
				}
			}

			red=item.red;
			green=item.green;
			blue=item.blue;

			return true;
		}

		ArithmeticEncoder enc;
		// Last Point Color.
		ushort red, green, blue;

		ArithmeticModel m_byte_used;
		ArithmeticModel m_rgb_diff_0;
		ArithmeticModel m_rgb_diff_1;
		ArithmeticModel m_rgb_diff_2;
		ArithmeticModel m_rgb_diff_3;
		ArithmeticModel m_rgb_diff_4;
		ArithmeticModel m_rgb_diff_5;
	}
}
