
///////////////////////////////////////////////////////////////////////////
/// File    : ReaderBase.cs
/// Desc    : Reader base class
/// Author  : Li G.Q.
/// Date    : 2021/9/20/
/// Remark  : If any question, you can send email to : Ligq168@csu.edu.cn.
///////////////////////////////////////////////////////////////////////////

using LasLibNet.Abstract;
using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Reader
{
    /// <summary>
    /// Reader base class
    /// </summary>
    public class FileReader
    {
        #region Fields and Properties
        protected LasHeader header =  LasHeader.Instance;
        /// <summary>
        /// Las File Header Infor.
        /// </summary>
        public LasHeader Header { get => header; }
        /// <summary>
        /// The sequence of the current points. 
        /// </summary>
        protected long p_sequence;
        /// <summary>
        /// The sequence of the current points. 
        /// </summary>
        public long PointIndex { get => this.p_sequence; }
        /// <summary>
        /// The total points number of this opend las file. 
        /// </summary>
        protected long npoints;
        /// <summary>
        /// The points number of this opend las file. 
        /// </summary>
        public long PointsNumber { get => this.npoints; }

        protected LasPoint point = new LasPoint(LasHeader.Instance);
        /// <summary>
        /// Current point
        /// </summary>
        public LasPoint Point { get => this.point; }

        protected Stream streamin;
        protected bool leaveStreamInOpen;

        protected string error;
        /// <summary>
        /// Return the error message.
        /// </summary>
        public string Error { get => this.error; }
        
        protected string warning;
        /// <summary>
        /// Return the warning message.
        /// </summary>
        public string Warning { get => this.warning; }

        /// <summary>
        /// ExtHeader include PUBLIC HEADER BLOCK and VLRs. That is all data before the data.
        /// </summary>
        protected byte[] ext_header;
        /// <summary>
        /// ExtHeader include PUBLIC HEADER BLOCK and VLRs. That is all data before the data.
        /// </summary>
        public byte[] ExtHeader
        {
            get => this.ext_header;
        }

        protected bool is_compressed=false;
        public bool IsCompressed {
            get => is_compressed;
        }


        #endregion

        #region Open las file reader
        public bool OpenReader(Stream streamin)
        {
            if (!streamin.CanRead)
            {
                error = "Can not read input stream";
                return false;
            }

            if (streamin.Length <= 0)
            {
                error = "The input stream is empty : nothing to read";
                return false;
            }

            this.streamin = streamin;

            return open_reader_stream();
        }
        /// <summary>
        /// Open in stream for a laz file
        /// </summary>
        /// <param name="file_name"></param>
        /// <returns></returns>
        public virtual bool OpenReader(string file_name)
        {
            if (file_name == null || file_name.Length == 0)
            {
                error = "The file_name pointer is zero";
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

            return open_reader_stream();
        }

        /// <summary>
        /// open stream.
        /// </summary>
        /// <returns></returns>
        protected bool open_reader_stream()
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

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.file_source_ID failed";
                    return false;
                }
                header.file_source_ID = BitConverter.ToUInt16(buffer, 0);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.global_encoding failed";
                    return false;
                }
                header.global_encoding = BitConverter.ToUInt16(buffer, 0);

                if (streamin.Read(buffer, 0, 4) != 4)
                {
                    error = "Reading header.project_ID_GUID_data_1 failed";
                    return false;
                }
                header.project_ID_GUID_data_1 = BitConverter.ToUInt32(buffer, 0);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.project_ID_GUID_data_2 failed";
                    return false;
                }
                header.project_ID_GUID_data_2 = BitConverter.ToUInt16(buffer, 0);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.project_ID_GUID_data_3 failed";
                    return false;
                }
                header.project_ID_GUID_data_3 = BitConverter.ToUInt16(buffer, 0);

                if (streamin.Read(header.project_ID_GUID_data_4, 0, 8) != 8)
                {
                    error = "Reading header.project_ID_GUID_data_4 failed";
                    return false;
                }

                if (streamin.Read(buffer, 0, 1) != 1)
                {
                    error = "Reading header.version_major failed";
                    return false;
                }
                header.version_major = buffer[0];

                if (streamin.Read(buffer, 0, 1) != 1)
                {
                    error = "Reading header.version_minor failed";
                    return false;
                }
                header.version_minor = buffer[0];

                if (streamin.Read(header.system_identifier, 0, 32) != 32)
                {
                    error = "Reading header.system_identifier failed";
                    return false;
                }

                if (streamin.Read(header.generating_software, 0, 32) != 32)
                {
                    error = "Reading header.generating_software failed";
                    return false;
                }

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.file_creation_day failed";
                    return false;
                }
                header.file_creation_day = BitConverter.ToUInt16(buffer, 0);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.file_creation_year failed";
                    return false;
                }
                header.file_creation_year = BitConverter.ToUInt16(buffer, 0);

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.header_size failed";
                    return false;
                }
                header.header_size = BitConverter.ToUInt16(buffer, 0);

                if (streamin.Read(buffer, 0, 4) != 4)
                {
                    error = "Reading header.offset_to_point_data failed";
                    return false;
                }
                header.offset_to_point_data = BitConverter.ToUInt32(buffer, 0);

                if (streamin.Read(buffer, 0, 4) != 4)
                {
                    error = "Reading header.number_of_variable_length_records failed";
                    return false;
                }
                header.number_of_variable_length_records = BitConverter.ToUInt32(buffer, 0);

                if (streamin.Read(buffer, 0, 1) != 1)
                {
                    error = "Reading header.point_data_format failed";
                    return false;
                }
                header.point_data_format = buffer[0];

#if DEBUG
                Debug.WriteLine(" # point_data_format = "+header.point_data_format.ToString());
#endif

                if (streamin.Read(buffer, 0, 2) != 2)
                {
                    error = "Reading header.point_data_record_length failed";
                    return false;
                }
                header.point_data_record_length = BitConverter.ToUInt16(buffer, 0);

                if (streamin.Read(buffer, 0, 4) != 4)
                {
                    error = "Reading header.number_of_point_records failed";
                    return false;
                }
                header.number_of_point_records = BitConverter.ToUInt32(buffer, 0);

                for (int i = 0; i < 5; i++)
                {
                    if (streamin.Read(buffer, 0, 4) != 4)
                    {
                        error = string.Format("Reading header.number_of_points_by_return {0} failed", i);
                        return false;
                    }
                    header.number_of_points_by_return[i] = BitConverter.ToUInt32(buffer, 0);
                }

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.x_scale_factor failed";
                    return false;
                }
                header.x_scale_factor = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.y_scale_factor failed";
                    return false;
                }
                header.y_scale_factor = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.z_scale_factor failed";
                    return false;
                }
                header.z_scale_factor = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.x_offset failed";
                    return false;
                }
                header.x_offset = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.y_offset failed";
                    return false;
                }
                header.y_offset = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.z_offset failed";
                    return false;
                }
                header.z_offset = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.max_x failed";
                    return false;
                }
                header.max_x = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.min_x failed failed";
                    return false;
                }
                header.min_x = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.max_y failed failed";
                    return false;
                }
                header.max_y = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.min_y failed failed";
                    return false;
                }
                header.min_y = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.max_z failed failed";
                    return false;
                }
                header.max_z = BitConverter.ToDouble(buffer, 0);

                if (streamin.Read(buffer, 0, 8) != 8)
                {
                    error = "Reading header.min_z failed failed";
                    return false;
                }
                header.min_z = BitConverter.ToDouble(buffer, 0);  //227
                ////////////Finish PUBLIC HEADER BLOCK reading.///////////////////////////////////////////////////////////

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
               
                if (header.number_of_variable_length_records != 0)
                {
                    header.vlrs = new List<LasVLR>();

                    for (int i = 0; i < header.number_of_variable_length_records; i++)
                    {
                        header.vlrs.Add(new LasVLR());

                        // make sure there are enough bytes left to read a variable length record before the point block starts
                        if (((int)header.offset_to_point_data - vlrs_size - header.header_size) < 54)
                        {
                            warning = string.Format("only {0} bytes until point block after reading {1} of {2} vlrs. skipping remaining vlrs ...", (int)header.offset_to_point_data - vlrs_size - header.header_size, i, header.number_of_variable_length_records);
                            header.number_of_variable_length_records = (uint)i;
                            break;
                        }

                        // read variable length records variable after variable (to avoid alignment issues)
                        if (streamin.Read(buffer, 0, 2) != 2)
                        {
                            error = string.Format("reading header.vlrs[{0}].reserved", i);
                            return false;
                        }
                        header.vlrs[i].reserved = BitConverter.ToUInt16(buffer, 0);

                        if (streamin.Read(header.vlrs[i].user_id, 0, 16) != 16)
                        {
                            error = string.Format("reading header.vlrs[{0}].user_id", i);
                            return false;
                        }

                        if (streamin.Read(buffer, 0, 2) != 2)
                        {
                            error = string.Format("reading header.vlrs[{0}].record_id", i);
                            return false;
                        }
                        header.vlrs[i].record_id = BitConverter.ToUInt16(buffer, 0);

                        if (streamin.Read(buffer, 0, 2) != 2)
                        {
                            error = string.Format("reading header.vlrs[{0}].record_length_after_header", i);
                            return false;
                        }
                        header.vlrs[i].record_length_after_header = BitConverter.ToUInt16(buffer, 0);

                        if (streamin.Read(header.vlrs[i].description, 0, 32) != 32)
                        {
                            error = string.Format("reading header.vlrs[{0}].description", i);
                            return false;
                        }

                        // keep track on the number of bytes we have read so far
                        vlrs_size += 54;

                        // check variable length record contents
                        if (header.vlrs[i].reserved != 0xAABB)
                        {
                            warning = string.Format("wrong header.vlrs[{0}].reserved: {1} != 0xAABB", i, header.vlrs[i].reserved);
                        }

                        // make sure there are enough bytes left to read the data of the variable length record before the point block starts
                        if (((int)header.offset_to_point_data - vlrs_size - header.header_size) < header.vlrs[i].record_length_after_header)
                        {
                            warning = string.Format("only {0} bytes until point block when trying to read {1} bytes into header.vlrs[{2}].data", (int)header.offset_to_point_data - vlrs_size - header.header_size, header.vlrs[i].record_length_after_header, i);
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
                            if (userid == "laszip encoded")
                            {
                                LazFile laszip = new LazFile();

                                // read the LASzip VLR payload

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
                                    error = "reading compressor";
                                    return false;
                                }
                                laszip.compressor = BitConverter.ToUInt16(buffer, 0);

                                if (streamin.Read(buffer, 0, 2) != 2)
                                {
                                    error = "reading coder";
                                    return false;
                                }
                                laszip.coder = BitConverter.ToUInt16(buffer, 0);

                                if (streamin.Read(buffer, 0, 1) != 1)
                                {
                                    error = "reading version_major";
                                    return false;
                                }
                                laszip.version_major = buffer[0];

                                if (streamin.Read(buffer, 0, 1) != 1)
                                {
                                    error = "reading version_minor";
                                    return false;
                                }
                                laszip.version_minor = buffer[0];

                                if (streamin.Read(buffer, 0, 2) != 2)
                                {
                                    error = "reading version_revision";
                                    return false;
                                }
                                laszip.version_revision = BitConverter.ToUInt16(buffer, 0);

                                if (streamin.Read(buffer, 0, 4) != 4)
                                {
                                    error = "reading options";
                                    return false;
                                }
                                laszip.options = BitConverter.ToUInt32(buffer, 0);

                                if (streamin.Read(buffer, 0, 4) != 4)
                                {
                                    error = "reading chunk_size";
                                    return false;
                                }
                                laszip.chunk_size = BitConverter.ToUInt32(buffer, 0);

                                if (streamin.Read(buffer, 0, 8) != 8)
                                {
                                    error = "reading number_of_special_evlrs";
                                    return false;
                                }
                                laszip.number_of_special_evlrs = BitConverter.ToInt64(buffer, 0);

                                if (streamin.Read(buffer, 0, 8) != 8)
                                {
                                    error = "reading offset_to_special_evlrs";
                                    return false;
                                }
                                laszip.offset_to_special_evlrs = BitConverter.ToInt64(buffer, 0);

                                if (streamin.Read(buffer, 0, 2) != 2)
                                {
                                    error = "reading num_items";
                                    return false;
                                }
                                laszip.num_items = BitConverter.ToUInt16(buffer, 0);

                                laszip.items = new LazItem[laszip.num_items];
                                for (int j = 0; j < laszip.num_items; j++)
                                {
                                    laszip.items[j] = new LazItem();

                                    if (streamin.Read(buffer, 0, 2) != 2)
                                    {
                                        error = string.Format("reading type of item {0}", j);
                                        return false;
                                    }
                                    laszip.items[j].type = (LazItem.Type)BitConverter.ToUInt16(buffer, 0);

                                    if (streamin.Read(buffer, 0, 2) != 2)
                                    {
                                        error = string.Format("reading size of item {0}", j);
                                        return false;
                                    }
                                    laszip.items[j].size = BitConverter.ToUInt16(buffer, 0);

                                    if (streamin.Read(buffer, 0, 2) != 2)
                                    {
                                        error = string.Format("reading version of item {0}", j);
                                        return false;
                                    }
                                    laszip.items[j].version = BitConverter.ToUInt16(buffer, 0);
                                }
                            }
                            else
                            {
                                header.vlrs[i].data = new byte[header.vlrs[i].record_length_after_header];
                                if (streamin.Read(header.vlrs[i].data, 0, header.vlrs[i].record_length_after_header) != header.vlrs[i].record_length_after_header)
                                {
                                    error = string.Format("reading {0} bytes of data into header.vlrs[{1}].data", header.vlrs[i].record_length_after_header, i);
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

                        // special handling for LASzip VLR
                        if (userid == "laszip encoded")
                        {
                            // we take our the VLR for LASzip away
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
                        error = string.Format("reading {0} bytes of data into header.user_data_after_header", header.user_data_after_header_size);
                        return false;
                    }
                }


                // If read laz file, then continue read extra header info.
                //if (this.GetType().IsAssignableFrom(typeof(LazReader)))
                //    this.read_extra_header();

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


        #region Close the reader & Clean
        /// <summary>
        /// Close the las file reader.
        /// </summary>
        /// <returns></returns>
        public virtual bool CloseReader()
        {
           
            try
            {
               // reader = null;
                if (!leaveStreamInOpen) streamin.Close();
                streamin = null;

                //Clean();
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
                point.red = 0;
                point.green = 0;
                point.blue = 0;
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
                //reader = null;

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

        /// <summary>
        /// Get all header info include PUBLIC HEADER BLOCK and VLS, USER EXTEND DATA.
        /// </summary>
        /// <returns></returns>
        public byte[] GetExtendHeader()
        {
            try
            {
                //Remember the current position.
                long cur_position = this.streamin.Position;
                this.ext_header = new byte[this.header.offset_to_point_data];
                streamin.Position = 0;
                streamin.Read(this.ext_header, 0, (int)this.header.offset_to_point_data);
                //Reset the position to last position
                streamin.Position = cur_position;
                return this.ext_header;
            }
            catch
            {
                error = "Internal error in READ HEAD INFO";
                return null;
            }

        }

        /// <summary>
        /// Create raw point reader by the version and point data format.
        /// </summary>
        protected ILasPointReader CreateRawPointReader()
        {
            ILasPointReader reader;
            if (this.header.version_major == 1 && this.header.version_minor == 0)
            {
                if (this.header.point_data_format == 0)
                    reader = new LasPoint_1_0_Format0_Reader(streamin);
                else
                    reader = new LasPoint_1_0_Format1_Reader(streamin);
            }
            else if (this.header.version_major == 1 && this.header.version_minor == 1)
            {
                if (this.header.point_data_format == 0)
                    reader = new LasPoint_1_1_Format0_Reader(streamin);
                else if (this.header.point_data_format == 1)
                    reader = new LasPoint_1_1_Format1_Reader(streamin);
                else
                {
                    error = string.Format("The version {0}.{1} and format {2} is not supported"
                   , header.version_major, header.version_minor, header.point_data_format);
                    return null;
                }
            }
            else if (this.header.version_major == 1 && this.header.version_minor == 2)
            {
                if (this.header.point_data_format == 0)
                    reader = new LasPoint_1_2_Format0_Reader(streamin);
                else if (this.header.point_data_format == 1)
                    reader = new LasPoint_1_2_Format1_Reader(streamin);
                else if (this.header.point_data_format == 2)
                    reader = new LasPoint_1_2_Format2_Reader(streamin);
                else if (this.header.point_data_format == 3)
                    reader = new LasPoint_1_2_Format3_Reader(streamin);
                else
                {
                    error = string.Format("The version {0}.{1} and format {2} is not supported"
                   , header.version_major, header.version_minor, header.point_data_format);
                    return null;
                }
            }
            else
            {
                error = string.Format("The version {0}.{1} is not supported"
               , header.version_major, header.version_minor);
                return null;
            }

            return reader;
        }
    }
}
