///////////////////////////////////////////////////////////////////////
/// File    : LasItem.cs
/// Desc    : Las Item.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet
{
    /// <summary>
    /// Las Item
    /// </summary>
    public class LasItem
    {
		public enum Type { BYTE = 0, SHORT, INT, LONG, FLOAT, DOUBLE, POINT10, GPSTIME11, RGB12, WAVEPACKET13, POINT14, RGBNIR14 }
		public Type type;

		public ushort size;
		public ushort version;

		public bool is_type(LasItem.Type t)
		{
			if (t != type) return false;
			switch (t)
			{
				case Type.POINT10:
					if (size != 20) return false;
					break;
				case Type.POINT14:
					if (size != 30) return false;
					break;
				case Type.GPSTIME11:
					if (size != 8) return false;
					break;
				case Type.RGB12:
					if (size != 6) return false;
					break;
				case Type.WAVEPACKET13:
					if (size != 29) return false;
					break;
				case Type.BYTE:
					if (size < 1) return false;
					break;
				default: return false;
			}
			return true;
		}

		public string get_name()
		{
			switch (type)
			{
				case Type.POINT10: return "POINT10";
				case Type.POINT14: return "POINT14";
				case Type.GPSTIME11: return "GPSTIME11";
				case Type.RGB12: return "RGB12";
				case Type.WAVEPACKET13: return "WAVEPACKET13";
				case Type.BYTE: return "BYTE";
				default: break;
			}
			return null;
		}
	}
}

