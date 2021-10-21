
///////////////////////////////////////////////////////////////////////
/// File    : LasHeader.cs
/// Desc    : Las file this.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////
///
using LasLibNet.Model;
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
    public class LasHeader: ICloneable
    {
        #region private arrtribues
        private ushort _file_source_ID;
        private ushort _global_encoding;
        private uint _project_ID_GUID_data_1;
        private ushort _project_ID_GUID_data_2;
        private ushort _project_ID_GUID_data_3;
        private byte[] _project_ID_GUID_data_4 = new byte[8];
        private byte _version_major;
        private byte _version_minor;
        private byte[] _system_identifier = new byte[32];
        private byte[] _generating_software = new byte[32];
        private ushort _file_creation_day;
        private ushort _file_creation_year;
        private ushort _header_size;
        private uint _offset_to_point_data;
        private uint _number_of_variable_length_records;
        private byte _point_data_format;
        private ushort _point_data_record_length;
        private uint _number_of_point_records;
        private uint[] _number_of_points_by_return = new uint[5];
        private double _x_scale_factor;
        private double _y_scale_factor;
        private double _z_scale_factor;
        private double _x_offset;
        private double _y_offset;
        private double _z_offset;
        private double _max_x;
        private double _min_x;
        private double _max_y;
        private double _min_y;
        private double _max_z;
        private double _min_z;

        // LAS 1.3 and higher only
        private ulong _start_of_waveform_data_packet_record;

        // LAS 1.4 and higher only
        private ulong _start_of_first_extended_variable_length_record;
        private uint _number_of_extended_variable_length_records;
        private ulong _extended_number_of_point_records;
        private ulong[] _extended_number_of_points_by_return = new ulong[15];

        // optional
        private uint _user_data_in_header_size;
        private byte[] _user_data_in_header;

        // optional VLRs
        private List<LasVLR> _vlrs = new List<LasVLR>();

        // optional
        private uint _user_data_after_header_size;
        private byte[] _user_data_after_header;
        #endregion

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

            set => header = value;
        }

        public ushort file_source_ID { get => _file_source_ID; set => _file_source_ID = value; }
        public ushort global_encoding { get => _global_encoding; set => _global_encoding = value; }
        public uint project_ID_GUID_data_1 { get => _project_ID_GUID_data_1; set => _project_ID_GUID_data_1 = value; }
        public ushort project_ID_GUID_data_2 { get => _project_ID_GUID_data_2; set => _project_ID_GUID_data_2 = value; }
        public ushort project_ID_GUID_data_3 { get => _project_ID_GUID_data_3; set => _project_ID_GUID_data_3 = value; }

        public byte[] project_ID_GUID_data_4 { get => _project_ID_GUID_data_4; set => _project_ID_GUID_data_4 = value; }

        public byte version_major { get => _version_major; set => _version_major = value; }
        public byte version_minor { get => _version_minor; set => _version_minor = value; }

        public byte[] system_identifier {get=> _system_identifier;set=>_system_identifier=value;}

        public byte[] generating_software { get => _generating_software; set => _generating_software = value; }

        public ushort file_creation_day { get => _file_creation_day; set => _file_creation_day = value; }
        public ushort file_creation_year { get => _file_creation_year; set => _file_creation_year = value; }
        public ushort header_size { get => _header_size; set => _header_size = value; }
        public uint offset_to_point_data { get => _offset_to_point_data; set => _offset_to_point_data = value; }
        public uint number_of_variable_length_records { get => _number_of_variable_length_records; set => _number_of_variable_length_records = value; }
        public byte point_data_format { get => _point_data_format; set => _point_data_format = value; }
        public ushort point_data_record_length { get => _point_data_record_length; set => _point_data_record_length = value; }
        public uint number_of_point_records { get => _number_of_point_records; set => _number_of_point_records = value; }

        public uint[] number_of_points_by_return { get => _number_of_points_by_return; set=> _number_of_points_by_return=value; }

        public double x_scale_factor { get => _x_scale_factor; set => _x_scale_factor = value; }
        public double y_scale_factor { get => _y_scale_factor; set => _y_scale_factor = value; }
        public double z_scale_factor { get => _z_scale_factor; set => _z_scale_factor = value; }
        public double x_offset { get => _x_offset; set => _x_offset = value; }
        public double y_offset { get => _y_offset; set => _y_offset = value; }
        public double z_offset { get => _z_offset; set => _z_offset = value; }
        public double max_x { get => _max_x; set => _max_x = value; }
        public double min_x { get => _min_x; set => _min_x = value; }
        public double max_y { get => _max_y; set => _max_y = value; }
        public double min_y { get => _min_y; set => _min_y = value; }
        public double max_z { get => _max_z; set => _max_z = value; }
        public double min_z { get => _min_z; set => _min_z = value; }
        public ulong start_of_waveform_data_packet_record { get => _start_of_waveform_data_packet_record; set => _start_of_waveform_data_packet_record = value; }
        public ulong start_of_first_extended_variable_length_record { get => _start_of_first_extended_variable_length_record; set => _start_of_first_extended_variable_length_record = value; }
        public uint number_of_extended_variable_length_records { get => _number_of_extended_variable_length_records; set => _number_of_extended_variable_length_records = value; }
        public ulong extended_number_of_point_records { get => _extended_number_of_point_records; set => _extended_number_of_point_records = value; }

        public ulong[] extended_number_of_points_by_return => _extended_number_of_points_by_return;

        public uint user_data_in_header_size { get => _user_data_in_header_size; set => _user_data_in_header_size = value; }
        public byte[] user_data_in_header { get => _user_data_in_header; set => _user_data_in_header = value; }
        public List<LasVLR> vlrs { get => _vlrs; set => _vlrs = value; }
        public uint user_data_after_header_size { get => _user_data_after_header_size; set => _user_data_after_header_size = value; }
        public byte[] user_data_after_header { get => _user_data_after_header; set => _user_data_after_header = value; }

        /// <summary>
        /// Initialize the setting for R1.2 and format3.
        /// </summary>
        public void Init()
        {
            this.file_source_ID = 0;
            this.global_encoding = 0;
            this.project_ID_GUID_data_1 = 0;
            this.project_ID_GUID_data_2 = 0;
            this.project_ID_GUID_data_3 = 0;
            this.project_ID_GUID_data_4 = new byte[8];
            this.version_major = 01;
            this.version_minor = 02; ;
            this.system_identifier = Encoding.UTF8.GetBytes("CSU LasLibNet R1.0, 20210918    ");
            this.generating_software = Encoding.UTF8.GetBytes("CSU LasLibNet R1.0, 20210918    ");
            this.file_creation_day = (ushort)DateTime.Now.DayOfYear;
            this.file_creation_year = (ushort)DateTime.Now.Year;
            this.header_size = 227;
            this.offset_to_point_data = 227;
            this.number_of_variable_length_records = 0;
            this.point_data_format = 03;
            this.point_data_record_length = 34;
            this.number_of_point_records = 0;
            uint[] uints = { 0, 0, 0, 0, 0 };
            this.number_of_points_by_return = uints;
            this.x_scale_factor = 0.01000;
            this.y_scale_factor = 0.01000;
            this.z_scale_factor = 0.01000;
            this.x_offset = 0;
            this.y_offset = 0;
            this.z_offset = 0;
            this.max_x = 0;
            this.min_x = 0;
            this.max_y = 0;
            this.min_y = 0;
            this.max_z = 0;
            this.min_z = 0;
            this.start_of_waveform_data_packet_record = 0;
            this.start_of_first_extended_variable_length_record = 0;
            this.number_of_extended_variable_length_records = 0;
            this.user_data_in_header_size = 0;
            this.vlrs = null;
            this.user_data_after_header_size = 0;
        }

        /// <summary>
        /// Clone this header.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var header = (LasHeader)MemberwiseClone();
            return header;
        }

    }
}
