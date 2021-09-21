

namespace LasLibNet.Utils
{
	static class AC
	{
		// this header byte needs to change in case incompatible change happen
		internal const int HEADER_BYTE=2;
		internal const int BUFFER_SIZE=1024;

		internal const uint MinLength=0x01000000u; // threshold for renormalization
		internal const uint MaxLength=0xFFFFFFFFu; // maximum AC interval length
	}

	static class DM
	{
		// Maximum values for general models
		internal const int LengthShift=15; // length bits discarded before mult.
		internal const uint MaxCount=1u<<LengthShift; // for adaptive models
	}

	static class BM
	{
		// Maximum values for binary models
		internal const int LengthShift=13; // length bits discarded before mult.
		internal const uint MaxCount=1u<<LengthShift; // for adaptive models
	}
}
