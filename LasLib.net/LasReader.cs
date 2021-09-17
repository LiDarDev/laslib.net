
///////////////////////////////////////////////////////////////////////
/// File    : LasReader.cs
/// Desc    : Reader class to read the las file.
/// Author  : Li G.Q.
/// Date    : 2021/9/13/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using LasLibNet.Model;
using LasLibNet.Reader;
using LasLibNet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet
{
    /// <summary>
    /// LasReader reads the las file.
    /// </summary>
    public class LasReader
    {
        #region Fields and Properties
        private LasHeader header = LasHeader.Instance;
        /// <summary>
        /// Las File Header Infor.
        /// </summary>
        public LasHeader Header { get => header; }
        /// <summary>
        /// The sequence of the current points. 
        /// </summary>
        long p_sequence;
        /// <summary>
        /// The sequence of the current points. 
        /// </summary>
        public long PointIndex { get => this.p_sequence; }
        /// <summary>
        /// The total points number of this opend las file. 
        /// </summary>
        long npoints;
        /// <summary>
        /// The points number of this opend las file. 
        /// </summary>
        public long NPoints { get => this.npoints; }

        private LasPoint point = new LasPoint();
        /// <summary>
        /// Current point
        /// </summary>
        public LasPoint Point { get => this.point; }

        Stream streamin;
        bool leaveStreamInOpen;
        /// <summary>
        /// Point reader.
        /// </summary>
        ILasPointReader reader;


        string error;
        /// <summary>
        /// Return the error message.
        /// </summary>
        public string Error { get => this.error; }
        string warning;
        /// <summary>
        /// Return the warning message.
        /// </summary>
        public string Warning { get => this.warning; }

        /// <summary>
        /// ExtHeader include PUBLIC HEADER BLOCK and VLRs. That is all data before the data.
        /// </summary>
        private byte[] ext_header;
        public byte[] ExtHeader
        {
            get => this.ext_header;
        }

        /// <summary>
        /// Temp buffer to storage the header buffer.
        /// </summary>
        private byte[] buffer_temp = new byte[2000];

        #endregion

        /// <summary>
        /// The file version.
        /// </summary>
        /// <param name="version_major"></param>
        /// <param name="version_minor"></param>
        /// <param name="version_revision"></param>
        /// <param name="version_build"></param>
        /// <returns></returns>
        public void GetVersion(out byte version_major, out byte version_minor, out ushort version_revision, out uint version_build)
        {
            version_major = LasFile.VERSION_MAJOR;
            version_minor = LasFile.VERSION_MINOR;
            version_revision = LasFile.VERSION_REVISION;
            version_build = LasFile.VERSION_BUILD_DATE;
        }


        #region Open las file reader
        public bool OpenReader(Stream streamin, ref bool is_compressed, bool leaveOpen = false)
        {
            if (!streamin.CanRead)
            {
                error = "can not read input stream";
                return false;
            }

            if (streamin.Length <= 0)
            {
                error = "input stream is empty : nothing to read";
                return false;
            }

            if (reader != null)
            {
                error = "Reader is already open";
                return false;
            }

            this.streamin = streamin;
            leaveStreamInOpen = leaveOpen;

            return open_reader_stream(ref is_compressed);
        }

        public bool OpenReader(string file_name, ref bool is_compressed)
        {
            if (file_name == null || file_name.Length == 0)
            {
                error = "file_name pointer is zero";
                return false;
            }

            if (reader != null)
            {
                error = "Reader is already open";
                return false;
            }

            // open the file
            try
            {
                streamin = File.OpenRead(file_name);
                leaveStreamInOpen = false;
            }
            catch
            {
                error = string.Format("Cannot open file '{0}'", file_name);
                return false;
            }

            return open_reader_stream(ref is_compressed);
        }

        bool open_reader_stream(ref bool is_compressed)
        {
            try
            {
                byte[] buffer = new byte[32];

                #region read the header variable after variable
                if (streamin.Read(buffer, 0, 4) != 4)
                {
                    error = "Reading header.file_signature failed";
                    return false;
                }

                if (buffer[0] != 'L' && buffer[1] != 'A' && buffer[2] != 'S' && buffer[3] != 'F')
                {
                    error = "Wrong file_signature is not a LAS/LAZ file.";
                    return false;
                }
                //Array.Copy(buffer, 0, buffer_temp, 0, 4);


                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.file_source_ID failed";
                    return false;
                }
                header.file_source_ID = BitConverter.ToUInt16(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 4, 2);


                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.global_encoding failed";
                    return false;
                }
                header.global_encoding = BitConverter.ToUInt16(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 6, 2);

                if (streamin.Read(buffer, 0, 4) != 4)
                {
                    error = "Reading header.project_ID_GUID_data_1 failed";
                    return false;
                }
                header.project_ID_GUID_data_1 = BitConverter.ToUInt32(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 8, 4);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.project_ID_GUID_data_2 failed";
                    return false;
                }
                header.project_ID_GUID_data_2 = BitConverter.ToUInt16(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 12, 2);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.project_ID_GUID_data_3 failed";
                    return false;
                }
                header.project_ID_GUID_data_3 = BitConverter.ToUInt16(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 14, 2);


                if (streamin.Read(header.project_ID_GUID_data_4, 0, 8) != 8)
                {
                    error = "Reading header.project_ID_GUID_data_4 failed";
                    return false;
                }
                //Array.Copy(header.project_ID_GUID_data_4, 0, buffer_temp, 16, 8);

                if (streamin.Read(buffer, 0, 1) != 1)
                {
                    error = "Reading header.version_major failed";
                    return false;
                }
                header.version_major = buffer[0];
                //Array.Copy(buffer, 0, buffer_temp, 24, 1);

                if (streamin.Read(buffer, 0, 1) != 1)
                {
                    error = "Reading header.version_minor failed";
                    return false;
                }
                header.version_minor = buffer[0];
                //Array.Copy(buffer, 0, buffer_temp, 25, 1);

                if (streamin.Read(header.system_identifier, 0, 32) != 32)
                {
                    error = "Reading header.system_identifier failed";
                    return false;
                }
                //Array.Copy(header.system_identifier, 0, buffer_temp, 26, 32);

                if (streamin.Read(header.generating_software, 0, 32) != 32)
                {
                    error = "Reading header.generating_software failed";
                    return false;
                }
                //Array.Copy(header.generating_software, 0, buffer_temp, 58, 32);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.file_creation_day failed";
                    return false;
                }
                header.file_creation_day = BitConverter.ToUInt16(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 90, 2);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.file_creation_year failed";
                    return false;
                }
                header.file_creation_year = BitConverter.ToUInt16(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 92, 2);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.header_size failed";
                    return false;
                }
                header.header_size = BitConverter.ToUInt16(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 94, 2);

                if (streamin.Read(buffer, 0, 4) != 4)
                {
                    error = "Reading header.offset_to_point_data failed";
                    return false;
                }
                header.offset_to_point_data = BitConverter.ToUInt32(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 96, 4);

                if (streamin.Read(buffer, 0, 4) != 4)
                {
                    error = "Reading header.number_of_variable_length_records failed";
                    return false;
                }
                header.number_of_variable_length_records = BitConverter.ToUInt32(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 100, 4);

                if (streamin.Read(buffer, 0, 1) != 1)
                {
                    error = "Reading header.point_data_format failed";
                    return false;
                }
                header.point_data_format = buffer[0];
                //Array.Copy(buffer, 0, buffer_temp, 104, 1);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.point_data_record_length failed";
                    return false;
                }
                header.point_data_record_length = BitConverter.ToUInt16(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 105, 2);

                if (streamin.Read(buffer, 0, 4) != 4)
                {
                    error = "Reading header.number_of_point_records failed";
                    return false;
                }
                header.number_of_point_records = BitConverter.ToUInt32(buffer, 0);
                //Array.Copy(buffer, 0, buffer_temp, 107, 4);

                for (int i = 0; i < 5; i++)
                {
                    if (streamin.Read(buffer, 0, 4) != 4)
                    {
                        error = string.Format("Reading header.number_of_points_by_return {0} failed", i);
                        return false;
                    }
                    header.number_of_points_by_return[i] = BitConverter.ToUInt32(buffer, 0);
                    //Array.Copy(buffer, 0, buffer_temp,111+i*4, 4);
                }

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.x_scale_factor failed";
                    return false;
                }
                header.x_scale_factor = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 131, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.y_scale_factor failed";
                    return false;
                }
                header.y_scale_factor = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 139, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.z_scale_factor failed";
                    return false;
                }
                header.z_scale_factor = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 147, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.x_offset failed";
                    return false;
                }
                header.x_offset = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 155, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.y_offset failed";
                    return false;
                }
                header.y_offset = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 163, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.z_offset failed";
                    return false;
                }
                header.z_offset = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 171, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.max_x failed";
                    return false;
                }
                header.max_x = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 179, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.min_x failed failed";
                    return false;
                }
                header.min_x = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 187, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.max_y failed failed";
                    return false;
                }
                header.max_y = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 195, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.min_y failed failed";
                    return false;
                }
                header.min_y = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 203, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.max_z failed failed";
                    return false;
                }
                header.max_z = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 211, 8);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.min_z failed failed";
                    return false;
                }
                header.min_z = BitConverter.ToDouble(buffer, 0);
                ////Array.Copy(buffer, 0, buffer_temp, 219, 8);

                // special handling for LAS 1.3
                if ((header.version_major == 1) && (header.version_minor >= 3))
                {
                    if (header.header_size < 235)
                    {
                        error = string.Format("For LAS 1.{0} header_size should at least be 235 but it is only {1}"
                            , header.version_minor, header.header_size);
                        return false;
                    }
                    else
                    {
                        if (streamin.Read(buffer, 0, 8) != 8)
                        {
                            error = "Reading header.start_of_waveform_data_packet_record failed";
                            return false;
                        }
                        header.start_of_waveform_data_packet_record = BitConverter.ToUInt64(buffer, 0);
                        header.user_data_in_header_size = (uint)header.header_size - 235;
                        // //Array.Copy(buffer, 0, buffer_temp, 227, 8);
                    }
                }
                else
                {
                    header.user_data_in_header_size = (uint)header.header_size - 227;
                }

                // special handling for LAS 1.4
                if ((header.version_major == 1) && (header.version_minor >= 4))
                {
                    if (header.header_size < 375)
                    {
                        error = string.Format("Ror LAS 1.{0} header_size should at least be 375 but it is only {1}"
                            , header.version_minor, header.header_size);
                        return false;
                    }
                    else
                    {
                        if (streamin.Read(buffer, 0, 8) != 8)
                        {
                            error = "Reading header.start_of_first_extended_variable_length_record failed";
                            return false;
                        }
                        header.start_of_first_extended_variable_length_record = BitConverter.ToUInt64(buffer, 0);
                        // //Array.Copy(buffer, 0, buffer_temp, 227, 8);

                        if (streamin.Read(buffer, 0, 4) != 4)
                        {
                            error = "Reading header.number_of_extended_variable_length_records failed";
                            return false;
                        }
                        header.number_of_extended_variable_length_records = BitConverter.ToUInt32(buffer, 0);
                        ////Array.Copy(buffer, 0, buffer_temp, 235, 4);

                        if (streamin.Read(buffer, 0, 8) != 8)
                        {
                            error = "Reading header.extended_number_of_point_records failed";
                            return false;
                        }
                        header.extended_number_of_point_records = BitConverter.ToUInt64(buffer, 0);
                        ////Array.Copy(buffer, 0, buffer_temp, 239, 8);

                        for (int i = 0; i < 15; i++)
                        {
                            if (streamin.Read(buffer, 0, 8) != 8)
                            {
                                error = string.Format("Reading header.extended_number_of_points_by_return[{0}] failed", i);
                                return false;
                            }
                            header.extended_number_of_points_by_return[i] = BitConverter.ToUInt64(buffer, 0);
                            ////Array.Copy(buffer, 0, buffer_temp, 247+i*8, 8);
                        }
                        header.user_data_in_header_size = (uint)header.header_size - 375;
                    }
                }

                // load any number of user-defined bytes that might have been added to the header
                if (header.user_data_in_header_size > 0)
                {
                    header.user_data_in_header = new byte[header.user_data_in_header_size];

                    if (streamin.Read(header.user_data_in_header, 0, (int)header.user_data_in_header_size) != header.user_data_in_header_size)
                    {
                        error = string.Format("Reading {0} bytes of data into header.user_data_in_header failed", header.user_data_in_header_size);
                        return false;
                    }

                }
                #endregion

                #region read variable length records into the header
                uint vlrs_size = 0;
                LasFile lasFile = null;

                if (header.number_of_variable_length_records != 0)
                {
                    header.vlrs = new List<LasVLR>();

                    for (int i = 0; i < header.number_of_variable_length_records; i++)
                    {
                        header.vlrs.Add(new LasVLR());

                        // make sure there are enough bytes left to read a variable length record before the point block starts
                        if (((int)header.offset_to_point_data - vlrs_size - header.header_size) < 54)
                        {
                            warning = string.Format("Only {0} bytes until point block after reading {1} of {2} vlrs. skipping remaining vlrs ...", (int)header.offset_to_point_data - vlrs_size - header.header_size, i, header.number_of_variable_length_records);
                            header.number_of_variable_length_records = (uint)i;
                            break;
                        }

                        // read variable length records variable after variable (to avoid alignment issues)
                        if (streamin.Read(buffer, 0, 2) != 2)
                        {
                            error = string.Format("Reading header.vlrs[{0}].reserved  failed", i);
                            return false;
                        }
                        header.vlrs[i].reserved = BitConverter.ToUInt16(buffer, 0);

                        if (streamin.Read(header.vlrs[i].user_id, 0, 16) != 16)
                        {
                            error = string.Format("Reading header.vlrs[{0}].user_id failed", i);
                            return false;
                        }

                        if (streamin.Read(buffer, 0, 2) != 2)
                        {
                            error = string.Format("Reading header.vlrs[{0}].record_id failed", i);
                            return false;
                        }
                        header.vlrs[i].record_id = BitConverter.ToUInt16(buffer, 0);

                        if (streamin.Read(buffer, 0, 2) != 2)
                        {
                            error = string.Format("Reading header.vlrs[{0}].record_length_after_header failed", i);
                            return false;
                        }
                        header.vlrs[i].record_length_after_header = BitConverter.ToUInt16(buffer, 0);

                        if (streamin.Read(header.vlrs[i].description, 0, 32) != 32)
                        {
                            error = string.Format("Reading header.vlrs[{0}].description failed", i);
                            return false;
                        }

                        // keep track on the number of bytes we have read so far
                        vlrs_size += 54;

                        // check variable length record contents
                        if (header.vlrs[i].reserved != 0xAABB)
                        {
                            warning = string.Format("Wrong header.vlrs[{0}].reserved: {1} != 0xAABB", i, header.vlrs[i].reserved);
                        }

                        // make sure there are enough bytes left to read the data of the variable length record before the point block starts
                        if (((int)header.offset_to_point_data - vlrs_size - header.header_size) < header.vlrs[i].record_length_after_header)
                        {
                            warning = string.Format("Only {0} bytes until point block when trying to read {1} bytes into header.vlrs[{2}].data"
                                , (int)header.offset_to_point_data - vlrs_size - header.header_size
                                , header.vlrs[i].record_length_after_header, i);
                            header.vlrs[i].record_length_after_header = (ushort)(header.offset_to_point_data - vlrs_size - header.header_size);
                        }

                        string userid = "";
                        for (int a = 0; a < header.vlrs[i].user_id.Length; a++)
                        {
                            if (header.vlrs[i].user_id[a] == 0) break;
                            userid += (char)header.vlrs[i].user_id[a];
                        }

                        // load data following the header of the variable length record
                        if (header.vlrs[i].record_length_after_header != 0)
                        {
                            if (userid == "LasFile encoded")
                            {
                                lasFile = new LasFile();

                                // read the LasFile VLR payload

                                //     U16  compressor                2 bytes 
                                //     U32  coder                     2 bytes 
                                //     U8   version_major             1 byte 
                                //     U8   version_minor             1 byte
                                //     U16  version_revision          2 bytes
                                //     U32  options                   4 bytes 
                                //     I32  chunk_size                4 bytes
                                //     I64  number_of_special_evlrs   8 bytes
                                //     I64  offset_to_special_evlrs   8 bytes
                                //     U16  num_items                 2 bytes
                                //        U16 type                2 bytes * num_items
                                //        U16 size                2 bytes * num_items
                                //        U16 version             2 bytes * num_items
                                // which totals 34+6*num_items

                                if (streamin.Read(buffer, 0, 2) != 2)
                                {
                                    error = "Reading compressor failed";
                                    return false;
                                }
                                lasFile.compressor = BitConverter.ToUInt16(buffer, 0);

                                if (streamin.Read(buffer, 0, 2) != 2)
                                {
                                    error = "Reading coder";
                                    return false;
                                }
                                lasFile.coder = BitConverter.ToUInt16(buffer, 0);

                                if (streamin.Read(buffer, 0, 1) != 1)
                                {
                                    error = "Reading version_major failed";
                                    return false;
                                }
                                lasFile.version_major = buffer[0];

                                if (streamin.Read(buffer, 0, 1) != 1)
                                {
                                    error = "Reading version_minor failed";
                                    return false;
                                }
                                lasFile.version_minor = buffer[0];

                                if (streamin.Read(buffer, 0, 2) != 2)
                                {
                                    error = "Reading version_revision failed";
                                    return false;
                                }
                                lasFile.version_revision = BitConverter.ToUInt16(buffer, 0);

                                if (streamin.Read(buffer, 0, 4) != 4)
                                {
                                    error = "Reading options failed";
                                    return false;
                                }
                                lasFile.options = BitConverter.ToUInt32(buffer, 0);

                                if (streamin.Read(buffer, 0, 4) != 4)
                                {
                                    error = "Reading chunk_size failed";
                                    return false;
                                }
                                lasFile.chunk_size = BitConverter.ToUInt32(buffer, 0);

                                if (streamin.Read(buffer, 0, 8) != 8)
                                {
                                    error = "Reading number_of_special_evlrs failed";
                                    return false;
                                }
                                lasFile.number_of_special_evlrs = BitConverter.ToInt64(buffer, 0);

                                if (streamin.Read(buffer, 0, 8) != 8)
                                {
                                    error = "Reading offset_to_special_evlrs failed";
                                    return false;
                                }
                                lasFile.offset_to_special_evlrs = BitConverter.ToInt64(buffer, 0);

                                if (streamin.Read(buffer, 0, 2) != 2)
                                {
                                    error = "Reading num_items failed";
                                    return false;
                                }
                                lasFile.num_items = BitConverter.ToUInt16(buffer, 0);

                                lasFile.items = new LasItem[lasFile.num_items];
                                for (int j = 0; j < lasFile.num_items; j++)
                                {
                                    lasFile.items[j] = new LasItem();

                                    if (streamin.Read(buffer, 0, 2) != 2)
                                    {
                                        error = string.Format("Reading type of item {0} failed", j);
                                        return false;
                                    }
                                    lasFile.items[j].type = (LasItem.Type)BitConverter.ToUInt16(buffer, 0);

                                    if (streamin.Read(buffer, 0, 2) != 2)
                                    {
                                        error = string.Format("Reading size of item {0} failed", j);
                                        return false;
                                    }
                                    lasFile.items[j].size = BitConverter.ToUInt16(buffer, 0);

                                    if (streamin.Read(buffer, 0, 2) != 2)
                                    {
                                        error = string.Format("Reading version of item {0} failed", j);
                                        return false;
                                    }
                                    lasFile.items[j].version = BitConverter.ToUInt16(buffer, 0);
                                }
                            }
                            else
                            {
                                header.vlrs[i].data = new byte[header.vlrs[i].record_length_after_header];
                                if (streamin.Read(header.vlrs[i].data, 0, header.vlrs[i].record_length_after_header) != header.vlrs[i].record_length_after_header)
                                {
                                    error = string.Format("Reading {0} bytes of data into header.vlrs[{1}].data failed"
                                        , header.vlrs[i].record_length_after_header, i);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            header.vlrs[i].data = null;
                        }

                        // keep track on the number of bytes we have read so far
                        vlrs_size += header.vlrs[i].record_length_after_header;

                        // special handling for LasFile VLR
                        if (userid == "LasFile encoded")
                        {
                            // we take our the VLR for LasFile away
                            header.offset_to_point_data -= (uint)(54 + header.vlrs[i].record_length_after_header);
                            vlrs_size -= (uint)(54 + header.vlrs[i].record_length_after_header);
                            header.vlrs.RemoveAt(i);
                            i--;
                            header.number_of_variable_length_records--;
                        }
                    }
                }
                #endregion

                // load any number of user-defined bytes that might have been added after the header
                header.user_data_after_header_size = header.offset_to_point_data - vlrs_size - header.header_size;
                if (header.user_data_after_header_size != 0)
                {
                    header.user_data_after_header = new byte[header.user_data_after_header_size];

                    if (streamin.Read(header.user_data_after_header, 0, (int)header.user_data_after_header_size) != header.user_data_after_header_size)
                    {
                        error = string.Format("Reading {0} bytes of data into header.user_data_after_header failed"
                            , header.user_data_after_header_size);
                        return false;
                    }
                }

                #region Read all header bytes before the data.
                this.ext_header = new byte[header.offset_to_point_data];
                streamin.Position = 0;
                streamin.Read(this.ext_header, 0, (int)header.offset_to_point_data);
                #endregion

                this.SetPointReader();

                // set the point number and point count
                npoints = header.number_of_point_records;
                p_sequence = 0;
            }
            catch
            {
                error = "Internal error in open_reader";
                return false;
            }

            error = null;
            return true;
        }

        #endregion

        /// <summary>
        /// Create point reader by the version and point data format.
        /// </summary>
        private void SetPointReader()
        {
            if (this.header.version_major == 1 && this.header.version_minor == 0)
            {
                if (this.header.point_data_format == 0)
                    this.reader = new LasPoint_1_0_Format0_Reader(streamin);
                else
                    this.reader = new LasPoint_1_0_Format1_Reader(streamin);
            }
            else if (this.header.version_major == 1 && this.header.version_minor == 1)
            {
                if (this.header.point_data_format == 0)
                    this.reader = new LasPoint_1_1_Format0_Reader(streamin);
                else if (this.header.point_data_format == 1)
                    this.reader = new LasPoint_1_1_Format1_Reader(streamin);
            }
            else if (this.header.version_major == 1 && this.header.version_minor == 2)
            {
                if (this.header.point_data_format == 0)
                    this.reader = new LasPoint_1_2_Format0_Reader(streamin);
                else if (this.header.point_data_format == 1)
                    this.reader = new LasPoint_1_2_Format1_Reader(streamin);
                else if (this.header.point_data_format == 2)
                    this.reader = new LasPoint_1_2_Format2_Reader(streamin);
                else if (this.header.point_data_format == 3)
                    this.reader = new LasPoint_1_2_Format3_Reader(streamin);
            }
        }

        #region Close the reader & Clean
        /// <summary>
        /// Close the las file reader.
        /// </summary>
        /// <returns></returns>
        public bool CloseReader()
        {
            if (reader == null)
            {
                error = "Closing reader before it was opened";
                return false;
            }

            try
            { 
                reader = null;
                if (!leaveStreamInOpen) streamin.Close();
                streamin = null;

                Clean();
            }
            catch
            {
                error = "internal error in close_reader";
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        /// Clearn the field variables.
        /// </summary>
        /// <returns></returns>
        public bool Clean()
        {
            try
            {
                if (reader != null)
                {
                    error = "cannot clean while reader is open.";
                    return false;
                }

                // zero everything
                header.file_source_ID = 0;
                header.global_encoding = 0;
                header.project_ID_GUID_data_1 = 0;
                header.project_ID_GUID_data_2 = 0;
                header.project_ID_GUID_data_3 = 0;
                Array.Clear(header.project_ID_GUID_data_4, 0, header.project_ID_GUID_data_4.Length);
                header.version_major = 0;
                header.version_minor = 0;
                Array.Clear(header.system_identifier, 0, header.system_identifier.Length);
                Array.Clear(header.generating_software, 0, header.generating_software.Length);
                header.file_creation_day = 0;
                header.file_creation_year = 0;
                header.header_size = 0;
                header.offset_to_point_data = 0;
                header.number_of_variable_length_records = 0;
                header.point_data_format = 0;
                header.point_data_record_length = 0;
                header.number_of_point_records = 0;
                Array.Clear(header.number_of_points_by_return, 0, header.number_of_points_by_return.Length);
                header.x_scale_factor = 0;
                header.y_scale_factor = 0;
                header.z_scale_factor = 0;
                header.x_offset = 0;
                header.y_offset = 0;
                header.z_offset = 0;
                header.max_x = 0;
                header.min_x = 0;
                header.max_y = 0;
                header.min_y = 0;
                header.max_z = 0;
                header.min_z = 0;
                header.start_of_waveform_data_packet_record = 0;
                header.start_of_first_extended_variable_length_record = 0;
                header.number_of_extended_variable_length_records = 0;
                header.extended_number_of_point_records = 0;
                Array.Clear(header.extended_number_of_points_by_return, 0, header.extended_number_of_points_by_return.Length);
                header.user_data_in_header_size = 0;
                header.user_data_in_header = null;
                header.vlrs = null;
                header.user_data_after_header_size = 0;
                header.user_data_after_header = null;

                p_sequence = 0;
                npoints = 0;

                point.X = 0;
                point.Y = 0;
                point.Z = 0;
                point.intensity = 0;
                point.return_number = 0;// : 3;
                point.number_of_returns_of_given_pulse = 0;// : 3;
                point.scan_direction_flag = 0;// : 1;
                point.edge_of_flight_line = 0;// : 1;
                point.classification = 0;
                point.scan_angle_rank = 0;
                point.user_data = 0;
                point.point_source_ID = 0;
                point.gps_time = 0;
                point.rgb = new ushort[4];
                point.wave_packet = new byte[29];
                point.extended_point_type = 0;// : 2;
                point.extended_scanner_channel = 0;// : 2;
                point.extended_classification_flags = 0;// : 4;
                point.extended_classification = 0;
                point.extended_return_number = 0;// : 4;
                point.extended_number_of_returns_of_given_pulse = 0;// : 4;
                point.extended_scan_angle = 0;
                point.num_extra_bytes = 0;
                point.extra_bytes = null;

                streamin = null;
                reader = null;

                error = null;
                warning = null;

                // create default header
                byte[] generatingSoftware = Encoding.ASCII.GetBytes(string.Format("LasLibNet {0}.{1} r{2} ({3})"
                    , LasFile.VERSION_MAJOR
                    , LasFile.VERSION_MINOR
                    , LasFile.VERSION_REVISION
                    , LasFile.VERSION_BUILD_DATE));
                //Array.Copy(generatingSoftware, header.generating_software, Math.Min(generatingSoftware.Length, 32));
                header.version_major = 1;
                header.version_minor = 2;
                header.header_size = 227;
                header.offset_to_point_data = 227;
                header.point_data_format = 1;
                header.point_data_record_length = 28;
                header.x_scale_factor = 0.01;
                header.y_scale_factor = 0.01;
                header.z_scale_factor = 0.01;
            }
            catch
            {
                error = "Internal error in LasReader.Clean";
                return false;
            }

            return true;
        }

        #endregion


        public bool SeekPoint(long index)
        {
            try
            { 
                if (index>this.npoints)
                {
                    error = string.Format("Seeking from index {0} to index {1} for file with {2} points failed", p_sequence, index, npoints);
                    return false;
                }
                // seek to the point
                streamin.Seek(this.header.offset_to_point_data+index*this.reader.GetPointSize()
                    , SeekOrigin.Begin);
               
                p_sequence = index;
            }
            catch
            {
                error = "Internal error in seek_point";
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        /// Read a next point, and set the data to the variable point.
        /// </summary>
        /// <returns></returns>
        public Object ReadPoint()
        {
            if (reader == null)
            {
                error = "Reader wasn't opened. Please open the reader before you read a point.";
                return null;
            }

            try
            {
                // read the point
                if (!reader.Read())
                {
                    error = string.Format("Reading point with index {0} of {1} total points failed", p_sequence, npoints);
                    return null;
                }

                p_sequence++;
            }
            catch
            {
                error = "Internal error in read_point";
                return null;
            }

            error = null;
            return reader.GetPoint();
        }

    }
}
