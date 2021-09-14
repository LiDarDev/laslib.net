
///////////////////////////////////////////////////////////////////////
/// File    : LasHeader.cs
/// Desc    : Las file header.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet
{
    /// <summary>
    /// Las file header
    /// </summary>
    public class LasHeader
    {
        public ushort file_source_ID;
        public ushort global_encoding;
        public uint project_ID_GUID_data_1;
        public ushort project_ID_GUID_data_2;
        public ushort project_ID_GUID_data_3;
        public readonly byte[] project_ID_GUID_data_4 = new byte[8];
        public byte version_major;
        public byte version_minor;
        public readonly byte[] system_identifier = new byte[32];
        public readonly byte[] generating_software = new byte[32];
        public ushort file_creation_day;
        public ushort file_creation_year;
        public ushort header_size;
        public uint offset_to_point_data;
        public uint number_of_variable_length_records;
        public byte point_data_format;
        public ushort point_data_record_length;
        public uint number_of_point_records;
        public readonly uint[] number_of_points_by_return = new uint[5];
        public double x_scale_factor;
        public double y_scale_factor;
        public double z_scale_factor;
        public double x_offset;
        public double y_offset;
        public double z_offset;
        public double max_x;
        public double min_x;
        public double max_y;
        public double min_y;
        public double max_z;
        public double min_z;

        // LAS 1.3 and higher only
        public ulong start_of_waveform_data_packet_record;

        // LAS 1.4 and higher only
        public ulong start_of_first_extended_variable_length_record;
        public uint number_of_extended_variable_length_records;
        public ulong extended_number_of_point_records;
        public readonly ulong[] extended_number_of_points_by_return = new ulong[15];

        // optional
        public uint user_data_in_header_size;
        public byte[] user_data_in_header;

        // optional VLRs
        public List<LasVLR> vlrs = new List<LasVLR>();

        // optional
        public uint user_data_after_header_size;
        public byte[] user_data_after_header;

        static LasHeader header;
        /// <summary>
        /// LasHeader instance.
        /// </summary>
        public static LasHeader Instance
        {
            get
            {
                if (header == null)
                    header = new LasHeader();
                return header;
            }
        }
    }
}
