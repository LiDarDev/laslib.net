///////////////////////////////////////////////////////////////////////
/// File    : LasItemReader.cs
/// Desc    : Las item reader abstract class.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

namespace LasLibNet.Abstract
{
	public abstract class LasItemReader
	{
		public abstract void Read(LasPoint item);
	}

	abstract class LasCompressedItemReader : LasItemReader
	{
		public abstract bool Init(LasPoint item);
	}
}
