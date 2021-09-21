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
using System.Runtime.InteropServices;

namespace LasLibNet.Implement
{
	class LazItemReader_WAVEPACKET13_v1 : LazItemReader
	{
		public LazItemReader_WAVEPACKET13_v1(ArithmeticDecoder dec)
		{
			// set decoder
			Debug.Assert(dec!=null);
			this.dec=dec;

			// create models and integer compressors
			m_packet_index=dec.createSymbolModel(256);
			m_offset_diff[0]=dec.createSymbolModel(4);
			m_offset_diff[1]=dec.createSymbolModel(4);
			m_offset_diff[2]=dec.createSymbolModel(4);
			m_offset_diff[3]=dec.createSymbolModel(4);
			ic_offset_diff=new IntegerCompressor(dec, 32);
			ic_packet_size=new IntegerCompressor(dec, 32);
			ic_return_point=new IntegerCompressor(dec, 32);
			ic_xyz=new IntegerCompressor(dec, 32, 3);
		}

		public unsafe override bool Init(LasPoint item)
		{
			// init state
			last_diff_32=0;
			sym_last_offset_diff=0;

			// init models and integer compressors
			dec.initSymbolModel(m_packet_index);
			dec.initSymbolModel(m_offset_diff[0]);
			dec.initSymbolModel(m_offset_diff[1]);
			dec.initSymbolModel(m_offset_diff[2]);
			dec.initSymbolModel(m_offset_diff[3]);
			ic_offset_diff.initDecompressor();
			ic_packet_size.initDecompressor();
			ic_return_point.initDecompressor();
			ic_xyz.initDecompressor();

			// init last item
			fixed(byte* pItem=item.wave_packet)
			{
				last_item=*(LasWavepacket13*)(pItem+1);
			}

			return true;
		}

		public unsafe override void Read(LasPoint item)
		{
			item.wave_packet[0]=(byte)(dec.decodeSymbol(m_packet_index));

			fixed(byte* pItem=item.wave_packet)
			{
				LasWavepacket13* wave=(LasWavepacket13*)(pItem+1);

				sym_last_offset_diff=dec.decodeSymbol(m_offset_diff[sym_last_offset_diff]);

				if(sym_last_offset_diff==0)
				{
					wave->offset=last_item.offset;
				}
				else if(sym_last_offset_diff==1)
				{
					wave->offset=last_item.offset+last_item.packet_size;
				}
				else if(sym_last_offset_diff==2)
				{
					last_diff_32=ic_offset_diff.decompress(last_diff_32);
					wave->offset=(ulong)((long)last_item.offset+last_diff_32);
				}
				else
				{
					wave->offset=dec.readInt64();
				}

				wave->packet_size=(uint)ic_packet_size.decompress((int)last_item.packet_size);
				wave->return_point.i32=ic_return_point.decompress(last_item.return_point.i32);
				wave->x.i32=ic_xyz.decompress(last_item.x.i32, 0);
				wave->y.i32=ic_xyz.decompress(last_item.y.i32, 1);
				wave->z.i32=ic_xyz.decompress(last_item.z.i32, 2);

				last_item=*wave;
			}
		}

		ArithmeticDecoder dec;
		LasWavepacket13 last_item;

		int last_diff_32;
		uint sym_last_offset_diff;
		ArithmeticModel m_packet_index;
		ArithmeticModel[] m_offset_diff=new ArithmeticModel[4];
		IntegerCompressor ic_offset_diff;
		IntegerCompressor ic_packet_size;
		IntegerCompressor ic_return_point;
		IntegerCompressor ic_xyz;
	}
}
