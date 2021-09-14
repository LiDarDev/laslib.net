
///////////////////////////////////////////////////////////////////////
/// File    : LasWriter.cs
/// Desc    : Writer class to write the las file.
/// Author  : Li G.Q.
/// Date    : 2021/9/14/
///////////////////////////////////////////////////////////////////////

using LasLibNet.Model;
using LasLibNet.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet
{
    /// <summary>
    /// LasReader reads the las file.
    /// </summary>
    public class LasWriter
    {
        private LasHeader header = new LasHeader();
        /// <summary>
        /// Las File Header Infor.
        /// </summary>
        public LasHeader Header { get => header; }
        long p_count;
        public long PointCount { get => this.p_count; }
        long npoints;
        public long NPoints { get => this.npoints; }

        private LasPoint point = new LasPoint();
        /// <summary>
        /// Current point
        /// </summary>
        public LasPoint Point { get => this.point; }

        Stream streamout;
        bool leaveStreamOutOpen;
        LasPointWriter writer;

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
        /// Clearn the field variables.
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            try
            {
                if (writer != null)
                {
                    error = "Cannot init while writer is opened.";
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

                p_count = 0;
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

                streamout = null;
                writer = null;

                error = null;
                warning = null;

                // create default header
                byte[] generatingSoftware = Encoding.ASCII.GetBytes(string.Format("LasLibNet {0}.{1} r{2} ({3})"
                    , LasFile.VERSION_MAJOR
                    , LasFile.VERSION_MINOR
                    , LasFile.VERSION_REVISION
                    , LasFile.VERSION_BUILD_DATE));
                Array.Copy(generatingSoftware, header.generating_software, Math.Min(generatingSoftware.Length, 32));
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

        /// <summary>
        /// Set header
        /// </summary>
        /// <param name="header"></param>
        public void SetHeader(LasHeader header)
        {
            this.header = header;
        }

        public bool OpenWriter(Stream streamout, bool compress, bool leaveOpen = false)
        {
            if (!streamout.CanWrite)
            {
                error = "can not write output stream";
                return false;
            }

            if (writer != null)
            {
                error = "writer is already open";
                return false;
            }

            try
            {
                LasFile laszip;
                uint laszip_vrl_payload_size;

                bool err = CheckHeaderAndSetup(header, compress, out laszip, ref point, out laszip_vrl_payload_size, out error);
                if (err == false) return err;

                this.streamout = streamout;
                leaveStreamOutOpen = leaveOpen;

                return OpenWriterStream(compress, laszip, laszip_vrl_payload_size);
            }
            catch
            {
                error = string.Format("internal error in laszip_open_writer (Stream)");
                return false;
            }
        }

        public bool OpenWriter(string file_name, bool compress)
        {
            if (file_name == null || file_name.Length == 0)
            {
                error = "string file_name pointer is zero";
                return false;
            }

            if (writer != null)
            {
                error = "writer is already open";
                return false;
            }

            try
            {
                LasFile laszip;
                uint laszip_vrl_payload_size;

                bool err = CheckHeaderAndSetup(header, compress, out laszip, ref point, out laszip_vrl_payload_size, out error);
                if (err ==false) return err;

                #region open the file
                try
                {
                    streamout = new FileStream(file_name, FileMode.Create, FileAccess.Write, FileShare.Read);
                    leaveStreamOutOpen = false;
                }
                catch
                {
                    error = string.Format("cannot open file '{0}'", file_name);
                    return false;
                }
                #endregion

                return OpenWriterStream(compress, laszip, laszip_vrl_payload_size);
            }
            catch
            {
                error = string.Format("Internal error in OpenWriter '{0}'", file_name);
                return false;
            }
        }

        bool OpenWriterStream(bool compress, LasFile laszip, uint laszip_vrl_payload_size)
        {
            #region write the header variable after variable
            try
            {
                streamout.WriteByte((byte)'L');
                streamout.WriteByte((byte)'A');
                streamout.WriteByte((byte)'S');
                streamout.WriteByte((byte)'F');
            }
            catch
            {
                error = "Writing header.file_signature failed.";
                return false;
            }

            try { streamout.Write(BitConverter.GetBytes(header.file_source_ID), 0, 2); }
            catch { error = "Writing header.file_source_ID failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.global_encoding), 0, 2); }
            catch { error = "Writing header.global_encoding failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.project_ID_GUID_data_1), 0, 4); }
            catch { error = "Writing header.project_ID_GUID_data_1 failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.project_ID_GUID_data_2), 0, 2); }
            catch { error = "Writing header.project_ID_GUID_data_2 failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.project_ID_GUID_data_3), 0, 2); }
            catch { error = "Writing header.project_ID_GUID_data_3 failed"; return false; }

            try { streamout.Write(header.project_ID_GUID_data_4, 0, 8); }
            catch { error = "Writing header.project_ID_GUID_data_4 failed"; return false; }

            try { streamout.WriteByte(header.version_major); }
            catch { error = "Writing header.version_major failed"; return false; }

            try { streamout.WriteByte(header.version_minor); }
            catch { error = "Writing header.version_minor failed"; return false; }

            try { streamout.Write(header.system_identifier, 0, 32); }
            catch { error = "Writing header.system_identifier failed"; return false; }

            if (header.generating_software == null || header.generating_software.Length != 32)
            {
                byte[] generatingSoftware = Encoding.ASCII.GetBytes(string.Format("LasLibNet {0}.{1} r{2} ({3})"
                    , LasFile.VERSION_MAJOR
                    , LasFile.VERSION_MINOR
                    , LasFile.VERSION_REVISION
                    , LasFile.VERSION_BUILD_DATE));
                Array.Copy(generatingSoftware
                    , header.generating_software
                    , Math.Min(generatingSoftware.Length, 32));
            }

            try { streamout.Write(header.generating_software, 0, 32); }
            catch { error = "Writing header.generating_software failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.file_creation_day), 0, 2); }
            catch { error = "Writing header.file_creation_day failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.file_creation_year), 0, 2); }
            catch { error = "Writing header.file_creation_year failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.header_size), 0, 2); }
            catch { error = "Writing header.header_size failed"; return false; }

            if (compress) header.offset_to_point_data += (54 + laszip_vrl_payload_size);

            try { streamout.Write(BitConverter.GetBytes(header.offset_to_point_data), 0, 4); }
            catch { error = "Writing header.offset_to_point_data failed"; return false; }

            if (compress)
            {
                header.offset_to_point_data -= (54 + laszip_vrl_payload_size);
                header.number_of_variable_length_records += 1;
            }

            try { streamout.Write(BitConverter.GetBytes(header.number_of_variable_length_records), 0, 4); }
            catch { error = "Writing header.number_of_variable_length_records failed"; return false; }

            if (compress)
            {
                header.number_of_variable_length_records -= 1;
                //header.point_data_format |= 128;
            }

            try { streamout.WriteByte(header.point_data_format); }
            catch { error = "Writing header.point_data_format failed"; return false; }

            if (compress) header.point_data_format &= 127;

            try { streamout.Write(BitConverter.GetBytes(header.point_data_record_length), 0, 2); }
            catch { error = "Writing header.point_data_record_length failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.number_of_point_records), 0, 4); }
            catch { error = "Writing header.number_of_point_records failed"; return false; }

            for (uint i = 0; i < 5; i++)
            {
                try { streamout.Write(BitConverter.GetBytes(header.number_of_points_by_return[i]), 0, 4); }
                catch { error = string.Format("Writing header.number_of_points_by_return {0} failed", i); return false; }
            }

            try { streamout.Write(BitConverter.GetBytes(header.x_scale_factor), 0, 8); }
            catch { error = "Writing header.x_scale_factor failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.y_scale_factor), 0, 8); }
            catch { error = "Writing header.y_scale_factor failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.z_scale_factor), 0, 8); }
            catch { error = "Writing header.z_scale_factor failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.x_offset), 0, 8); }
            catch { error = "Writing header.x_offset failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.y_offset), 0, 8); }
            catch { error = "Writing header.y_offset failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.z_offset), 0, 8); }
            catch { error = "Writing header.z_offset failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.max_x), 0, 8); }
            catch { error = "Writing header.max_x failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.min_x), 0, 8); }
            catch { error = "Writing header.min_x failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.max_y), 0, 8); }
            catch { error = "Writing header.max_y failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.min_y), 0, 8); }
            catch { error = "Writing header.min_y failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.max_z), 0, 8); }
            catch { error = "Writing header.max_z failed"; return false; }

            try { streamout.Write(BitConverter.GetBytes(header.min_z), 0, 8); }
            catch { error = "Writing header.min_z failed"; return false; }

            #region special handling for LAS 1.3+
            if (header.version_major == 1 && header.version_minor >= 3)
            {
                if (header.header_size < 235)
                {
                    error = string.Format("For LAS 1.{0} header_size should at least be 235 but it is only {1}"
                        , header.version_minor, header.header_size);
                    return false;
                }

                try { streamout.Write(BitConverter.GetBytes(header.start_of_waveform_data_packet_record), 0, 8); }
                catch { error = "Writing header.start_of_waveform_data_packet_record failed"; return false; }

                header.user_data_in_header_size = header.header_size - 235u;
            }
            else header.user_data_in_header_size = header.header_size - 227u;
            #endregion

            #region special handling for LAS 1.4+
            if (header.version_major == 1 && header.version_minor >= 4)
            {
                if (header.header_size < 375)
                {
                    error = string.Format("For LAS 1.{0} header_size should at least be 375 but it is only {1}"
                        , header.version_minor, header.header_size);
                    return false;
                }

                try { streamout.Write(BitConverter.GetBytes(header.start_of_first_extended_variable_length_record), 0, 8); }
                catch { error = "Writing header.start_of_first_extended_variable_length_record failed"; return false; }

                try { streamout.Write(BitConverter.GetBytes(header.number_of_extended_variable_length_records), 0, 4); }
                catch { error = "Writing header.number_of_extended_variable_length_records failed"; return false; }

                try { streamout.Write(BitConverter.GetBytes(header.extended_number_of_point_records), 0, 8); }
                catch { error = "Writing header.extended_number_of_point_records failed"; return false; }

                for (uint i = 0; i < 15; i++)
                {
                    try { streamout.Write(BitConverter.GetBytes(header.extended_number_of_points_by_return[i]), 0, 8); }
                    catch { error = string.Format("Writing header.extended_number_of_points_by_return[{0}] failed", i); return false; }
                }

                header.user_data_in_header_size = header.header_size - 375u;
            }
            #endregion

            #region write any number of user-defined bytes that might have been added to the header
            if (header.user_data_in_header_size != 0)
            {
                try { streamout.Write(header.user_data_in_header, 0, (int)header.user_data_in_header_size); }
                catch { error = string.Format("Writing {0} bytes of data into header.user_data_in_header failed", header.user_data_in_header_size); return false; }
            }
            #endregion

            #region write variable length records into the header
            if (header.number_of_variable_length_records != 0)
            {
                for (int i = 0; i < header.number_of_variable_length_records; i++)
                {
                    // write variable length records variable after variable (to avoid alignment issues)
                    try { streamout.Write(BitConverter.GetBytes(header.vlrs[i].reserved), 0, 2); }
                    catch { error = string.Format("Writing header.vlrs[{0}].reserved failed", i); return false; }

                    try { streamout.Write(header.vlrs[i].user_id, 0, 16); }
                    catch { error = string.Format("Writing header.vlrs[{0}].user_id failed", i); return false; }

                    try { streamout.Write(BitConverter.GetBytes(header.vlrs[i].record_id), 0, 2); }
                    catch { error = string.Format("Writing header.vlrs[{0}].record_id failed", i); return false; }

                    try { streamout.Write(BitConverter.GetBytes(header.vlrs[i].record_length_after_header), 0, 2); }
                    catch { error = string.Format("Writing header.vlrs[{0}].record_length_after_header failed", i); return false; }

                    try { streamout.Write(header.vlrs[i].description, 0, 32); }
                    catch { error = string.Format("Writing header.vlrs[{0}].description failed", i); return false; }

                    // write data following the header of the variable length record
                    if (header.vlrs[i].record_length_after_header != 0)
                    {
                        try { streamout.Write(header.vlrs[i].data, 0, header.vlrs[i].record_length_after_header); }
                        catch { error = string.Format("Writing {0} bytes of data into header.vlrs[{1}].data failed", header.vlrs[i].record_length_after_header, i); return false; }
                    }
                }
            }

            if (compress)
            {
                #region write the LasFile VLR header
                uint i = header.number_of_variable_length_records;

                ushort reserved = 0xAABB;
                try { streamout.Write(BitConverter.GetBytes(reserved), 0, 2); }
                catch { error = string.Format("Writing header.vlrs[{0}].reserved failed", i); return false; }

                byte[] user_id1 = Encoding.ASCII.GetBytes("laszip encoded");
                byte[] user_id = new byte[16];
                Array.Copy(user_id1, user_id, Math.Min(16, user_id1.Length));
                try { streamout.Write(user_id, 0, 16); }
                catch { error = string.Format("Writing header.vlrs[{0}].user_id failed", i); return false; }

                ushort record_id = 22204;
                try { streamout.Write(BitConverter.GetBytes(record_id), 0, 2); }
                catch { error = string.Format("Writing header.vlrs[{0}].record_id failed", i); return false; }

                ushort record_length_after_header = (ushort)laszip_vrl_payload_size;
                try { streamout.Write(BitConverter.GetBytes(record_length_after_header), 0, 2); }
                catch { error = string.Format("Writing header.vlrs[{0}].record_length_after_header failed", i); return false; }

                // description field must be a null-terminate string, so we don't copy more than 31 characters
                byte[] description1 = Encoding.ASCII.GetBytes(string.Format("LasLibNet {0}.{1} r{2} ({3})"
                    , LasFile.VERSION_MAJOR
                    , LasFile.VERSION_MINOR
                    , LasFile.VERSION_REVISION
                    , LasFile.VERSION_BUILD_DATE));

                byte[] description = new byte[32];
                Array.Copy(description1, description, Math.Min(31, description1.Length));

                try { streamout.Write(description, 0, 32); }
                catch { error = string.Format("Writing header.vlrs[{0}].description failed", i); return false; }

                // write the LasFile VLR payload

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

                try { streamout.Write(BitConverter.GetBytes(laszip.compressor), 0, 2); }
                catch { error = string.Format("Writing compressor {0} failed", laszip.compressor); return false; }

                try { streamout.Write(BitConverter.GetBytes(laszip.coder), 0, 2); }
                catch { error = string.Format("Writing coder {0} failed", laszip.coder); return false; }

                try { streamout.WriteByte(laszip.version_major); }
                catch { error = string.Format("Writing version_major {0} failed", laszip.version_major); return false; }

                try { streamout.WriteByte(laszip.version_minor); }
                catch { error = string.Format("Writing version_minor {0} failed", laszip.version_minor); return false; }

                try { streamout.Write(BitConverter.GetBytes(laszip.version_revision), 0, 2); }
                catch { error = string.Format("Writing version_revision {0} failed", laszip.version_revision); return false; }

                try { streamout.Write(BitConverter.GetBytes(laszip.options), 0, 4); }
                catch { error = string.Format("Writing options {0} failed", laszip.options); return false; }

                try { streamout.Write(BitConverter.GetBytes(laszip.chunk_size), 0, 4); }
                catch { error = string.Format("Writing chunk_size {0} failed", laszip.chunk_size); return false; }

                try { streamout.Write(BitConverter.GetBytes(laszip.number_of_special_evlrs), 0, 8); }
                catch { error = string.Format("Writing number_of_special_evlrs {0} failed", laszip.number_of_special_evlrs); return false; }

                try { streamout.Write(BitConverter.GetBytes(laszip.offset_to_special_evlrs), 0, 8); }
                catch { error = string.Format("Writing offset_to_special_evlrs {0} failed", laszip.offset_to_special_evlrs); return false; }

                try { streamout.Write(BitConverter.GetBytes(laszip.num_items), 0, 2); }
                catch { error = string.Format("Writing num_items {0} failed", laszip.num_items); return false; }

                for (uint j = 0; j < laszip.num_items; j++)
                {
                    ushort type = (ushort)laszip.items[j].type;
                    try { streamout.Write(BitConverter.GetBytes(type), 0, 2); }
                    catch { error = string.Format("Writing type {0} of item {1} failed", laszip.items[j].type, j); return false; }

                    try { streamout.Write(BitConverter.GetBytes(laszip.items[j].size), 0, 2); }
                    catch { error = string.Format("Writing size {0} of item {1} failed", laszip.items[j].size, j); return false; }

                    try { streamout.Write(BitConverter.GetBytes(laszip.items[j].version), 0, 2); }
                    catch { error = string.Format("Writing version {0} of item {1} failed", laszip.items[j].version, j); return false; }
                }
                #endregion
            }
            #endregion

            #region write any number of user-defined bytes that might have been added after the header
            if (header.user_data_after_header_size != 0)
            {
                try { streamout.Write(header.user_data_after_header, 0, (int)header.user_data_after_header_size); }
                catch { error = string.Format("Writing {0} bytes of data into header.user_data_after_header failed", header.user_data_after_header_size); return false; }
            }
            #endregion

            #endregion

            #region create the point writer
            try { writer = new LasPointWriter(); }
            catch { error = "Could not alloc LasPointWriter"; return false; }

            if (!writer.setup(laszip.num_items, laszip.items, laszip))
            {
                error = "Setup of LasPointWriter failed";
                return false;
            }

            if (!writer.init(streamout))
            {
                error = "Init of LasPointWriter failed";
                return false;
            }
            #endregion

            // set the point number and point count
            npoints = header.number_of_point_records;
            p_count = 0;

            error = null;
            return true;
        }

        public bool WritePoint()
        {
            if (writer == null)
            {
                error = "Writing points before writer was opened";
                return false;
            }

            try
            {
                // write the point
                if (!writer.write(point))
                {
                    error = string.Format("Writing point with index {0} of {1} total points", p_count, npoints);
                    return false;
                }

                p_count++;
            }
            catch
            {
                error = "Internal error in laszip_write_point";
                return false;
            }

            error = null;
            return true;
        }

        public bool CloseWriter()
        {
            if (writer == null)
            {
                error = "closing writer before it was opened";
                return false;
            }

            try
            {
                if (!writer.done())
                {
                    error = "done of LasPointWriter failed";
                    return false;
                }

                writer = null;
                if (!leaveStreamOutOpen) streamout.Close();
                streamout = null;
            }
            catch
            {
                error = "internal error in laszip_writer_close";
                return false;
            }

            error = null;
            return true;
        }

        bool CheckIntegerOverflow()
        {
            try
            {
                // quantize and dequantize the bounding box with current scale_factor and offset
                int quant_min_x = MyDefs.I32_QUANTIZE((header.min_x - header.x_offset) / header.x_scale_factor);
                int quant_max_x = MyDefs.I32_QUANTIZE((header.max_x - header.x_offset) / header.x_scale_factor);
                int quant_min_y = MyDefs.I32_QUANTIZE((header.min_y - header.y_offset) / header.y_scale_factor);
                int quant_max_y = MyDefs.I32_QUANTIZE((header.max_y - header.y_offset) / header.y_scale_factor);
                int quant_min_z = MyDefs.I32_QUANTIZE((header.min_z - header.z_offset) / header.z_scale_factor);
                int quant_max_z = MyDefs.I32_QUANTIZE((header.max_z - header.z_offset) / header.z_scale_factor);

                double dequant_min_x = header.x_scale_factor * quant_min_x + header.x_offset;
                double dequant_max_x = header.x_scale_factor * quant_max_x + header.x_offset;
                double dequant_min_y = header.y_scale_factor * quant_min_y + header.y_offset;
                double dequant_max_y = header.y_scale_factor * quant_max_y + header.y_offset;
                double dequant_min_z = header.z_scale_factor * quant_min_z + header.z_offset;
                double dequant_max_z = header.z_scale_factor * quant_max_z + header.z_offset;

                // make sure that there is not sign flip (a 32-bit integer overflow) for the bounding box
                if ((header.min_x > 0) != (dequant_min_x > 0))
                {
                    error = string.Format("quantization sign flip for min_x from {0} to {1}. set scale factor for x coarser than {2}"
                        , header.min_x
                        , dequant_min_x
                        , header.x_scale_factor);
                    return false;
                }
                if ((header.max_x > 0) != (dequant_max_x > 0))
                {
                    error = string.Format("quantization sign flip for max_x from {0} to {1}. set scale factor for x coarser than {2}"
                        , header.max_x
                        , dequant_max_x
                        , header.x_scale_factor);
                    return false;
                }
                if ((header.min_y > 0) != (dequant_min_y > 0))
                {
                    error = string.Format("quantization sign flip for min_y from {0} to {1}. set scale factor for y coarser than {2}"
                        , header.min_y
                        , dequant_min_y
                        , header.y_scale_factor);
                    return false;
                }
                if ((header.max_y > 0) != (dequant_max_y > 0))
                {
                    error = string.Format("quantization sign flip for max_y from {0} to {1}. set scale factor for y coarser than {2}"
                        , header.max_y
                        , dequant_max_y
                        , header.y_scale_factor);
                    return false;
                }
                if ((header.min_z > 0) != (dequant_min_z > 0))
                {
                    error = string.Format("quantization sign flip for min_z from {0} to {1}. set scale factor for z coarser than {2}"
                        , header.min_z
                        , dequant_min_z
                        , header.z_scale_factor);
                    return false;
                }
                if ((header.max_z > 0) != (dequant_max_z > 0))
                {
                    error = string.Format("quantization sign flip for max_z from {0} to {1}. set scale factor for z coarser than {2}"
                        , header.max_z
                        , dequant_max_z
                        , header.z_scale_factor);
                    return false;
                }
            }
            catch
            {
                error = "internal error in laszip_auto_offset";
                return false;
            }

            error = null;
            return true;
        }

        bool auto_ffset()
        {
            try
            {
                // check scale factor
                double x_scale_factor = header.x_scale_factor;
                double y_scale_factor = header.y_scale_factor;
                double z_scale_factor = header.z_scale_factor;

                if ((x_scale_factor <= 0) || double.IsInfinity(x_scale_factor))
                {
                    error = string.Format("invalid x scale_factor {0} in header", header.x_scale_factor);
                    return false;
                }

                if ((y_scale_factor <= 0) || double.IsInfinity(y_scale_factor))
                {
                    error = string.Format("invalid y scale_factor {0} in header", header.y_scale_factor);
                    return false;
                }

                if ((z_scale_factor <= 0) || double.IsInfinity(z_scale_factor))
                {
                    error = string.Format("invalid z scale_factor {0} in header", header.z_scale_factor);
                    return false;
                }

                double center_bb_x = (header.min_x + header.max_x) / 2;
                double center_bb_y = (header.min_y + header.max_y) / 2;
                double center_bb_z = (header.min_z + header.max_z) / 2;

                if (double.IsInfinity(center_bb_x))
                {
                    error = string.Format("invalid x coordinate at center of bounding box (min: {0} max: {1})", header.min_x, header.max_x);
                    return false;
                }

                if (double.IsInfinity(center_bb_y))
                {
                    error = string.Format("invalid y coordinate at center of bounding box (min: {0} max: {1})", header.min_y, header.max_y);
                    return false;
                }

                if (double.IsInfinity(center_bb_z))
                {
                    error = string.Format("invalid z coordinate at center of bounding box (min: {0} max: {1})", header.min_z, header.max_z);
                    return false;
                }

                double x_offset = header.x_offset;
                double y_offset = header.y_offset;
                double z_offset = header.z_offset;

                header.x_offset = (MyDefs.I64_FLOOR(center_bb_x / x_scale_factor / 10000000)) * 10000000 * x_scale_factor;
                header.y_offset = (MyDefs.I64_FLOOR(center_bb_y / y_scale_factor / 10000000)) * 10000000 * y_scale_factor;
                header.z_offset = (MyDefs.I64_FLOOR(center_bb_z / z_scale_factor / 10000000)) * 10000000 * z_scale_factor;

                if (this.CheckIntegerOverflow() ==false)
                {
                    header.x_offset = x_offset;
                    header.y_offset = y_offset;
                    header.z_offset = z_offset;
                    return false;
                }
            }
            catch
            {
                error = "internal error in laszip_auto_offset";
                return false;
            }

            error = null;
            return true;
        }

        public bool SetPoint(LasPoint point)
        {
            if (point == null)
            {
                error = "laspoint_struct pointer is zero";
                return false;
            }

            try
            {
                this.point.classification = point.classification;
                this.point.edge_of_flight_line = point.edge_of_flight_line;
                this.point.extended_classification = point.extended_classification;
                this.point.extended_classification_flags = point.extended_classification_flags;
                this.point.extended_number_of_returns_of_given_pulse = point.extended_number_of_returns_of_given_pulse;
                this.point.extended_point_type = point.extended_point_type;
                this.point.extended_return_number = point.extended_return_number;
                this.point.extended_scan_angle = point.extended_scan_angle;
                this.point.extended_scanner_channel = point.extended_scanner_channel;
                this.point.gps_time = point.gps_time;
                this.point.intensity = point.intensity;
                this.point.num_extra_bytes = point.num_extra_bytes;
                this.point.number_of_returns_of_given_pulse = point.number_of_returns_of_given_pulse;
                this.point.point_source_ID = point.point_source_ID;
                this.point.return_number = point.return_number;
                Array.Copy(point.rgb, this.point.rgb, 4);
                this.point.scan_angle_rank = point.scan_angle_rank;
                this.point.scan_direction_flag = point.scan_direction_flag;
                this.point.user_data = point.user_data;
                this.point.X = point.X;
                this.point.Y = point.Y;
                this.point.Z = point.Z;
                Array.Copy(point.wave_packet, this.point.wave_packet, 29);

                if (this.point.extra_bytes != null)
                {
                    if (point.extra_bytes != null)
                    {
                        if (this.point.num_extra_bytes == point.num_extra_bytes)
                        {
                            Array.Copy(point.extra_bytes, this.point.extra_bytes, point.num_extra_bytes);
                        }
                        else
                        {
                            error = string.Format("target point has {0} extra bytes but source point has {1}", this.point.num_extra_bytes, point.num_extra_bytes);
                            return false;
                        }
                    }
                    else
                    {
                        error = "target point has extra bytes but source point does not";
                        return false;
                    }
                }
                else
                {
                    if (point.extra_bytes != null)
                    {
                        error = "source point has extra bytes but target point does not";
                        return false;
                    }
                }
            }
            catch
            {
                error = "internal error in laszip_set_point";
                return false;
            }

            error = null;
            return true;
        }

        public bool SetCoordinates(double[] coordinates)
        {
            if (coordinates == null)
            {
                error = "laszip_F64 coordinates pointer is zero";
                return false;
            }

            try
            {
                // set the coordinates
                point.X = MyDefs.I32_QUANTIZE((coordinates[0] - header.x_offset) / header.x_scale_factor);
                point.Y = MyDefs.I32_QUANTIZE((coordinates[1] - header.y_offset) / header.y_scale_factor);
                point.Z = MyDefs.I32_QUANTIZE((coordinates[2] - header.z_offset) / header.z_scale_factor);
            }
            catch
            {
                error = "internal error in laszip_set_coordinates";
                return false;
            }

            error = null;
            return true;
        }

        public unsafe bool SetGeokeys(ushort number, LasGeokey[] key_entries)
        {
            if (number == 0)
            {
                error = "number of key_entries is zero";
                return false;
            }

            if (key_entries == null)
            {
                error = "key_entries pointer is zero";
                return false;
            }

            try
            {
                // create the geokey directory
                byte[] buffer = new byte[sizeof(LasGeokey) * (number + 1)];

                fixed (byte* pBuffer = buffer)
                {
                    LasGeokey* key_entries_plus_one = (LasGeokey*)pBuffer;

                    key_entries_plus_one[0].key_id = 1;            // aka key_directory_version
                    key_entries_plus_one[0].tiff_tag_location = 1; // aka key_revision
                    key_entries_plus_one[0].count = 0;             // aka minor_revision
                    key_entries_plus_one[0].value_offset = number; // aka number_of_keys
                    for (int i = 0; i < number; i++) key_entries_plus_one[i + 1] = key_entries[i];
                }

                // fill a VLR
                LasVLR vlr = new LasVLR();
                vlr.reserved = 0xAABB;
                byte[] user_id = Encoding.ASCII.GetBytes("LASF_Projection");
                Array.Copy(user_id, vlr.user_id, Math.Min(user_id.Length, 16));
                vlr.record_id = 34735;
                vlr.record_length_after_header = (ushort)(8 + number * 8);

                // description field must be a null-terminate string, so we don't copy more than 31 characters
                byte[] v = Encoding.ASCII.GetBytes(string.Format("lasFile.net DLL {0}.{1} r{2} ({3})", LasFile.VERSION_MAJOR, LasFile.VERSION_MINOR, LasFile.VERSION_REVISION, LasFile.VERSION_BUILD_DATE));
                Array.Copy(v, vlr.description, Math.Min(v.Length, 31));

                vlr.data = buffer;

                // add the VLR
                if (!AddVLR(vlr))
                {
                    error = string.Format("setting {0} geokeys", number);
                    return false;
                }
            }
            catch
            {
                error = "internal error in laszip_set_geokey_entries";
                return false;
            }

            error = null;
            return true;
        }

        public bool SetGeodoubleParams(ushort number, double[] geodouble_params)
        {
            if (number == 0)
            {
                error = "number of geodouble_params is zero";
                return false;
            }

            if (geodouble_params == null)
            {
                error = "geodouble_params pointer is zero";
                return false;
            }

            try
            {
                // fill a VLR
                LasVLR vlr = new LasVLR();
                vlr.reserved = 0xAABB;
                byte[] user_id = Encoding.ASCII.GetBytes("LASF_Projection");
                Array.Copy(user_id, vlr.user_id, Math.Min(user_id.Length, 16));
                vlr.record_id = 34736;
                vlr.record_length_after_header = (ushort)(number * 8);

                // description field must be a null-terminate string, so we don't copy more than 31 characters
                byte[] v = Encoding.ASCII.GetBytes(string.Format("lasFile.net DLL {0}.{1} r{2} ({3})", LasFile.VERSION_MAJOR, LasFile.VERSION_MINOR, LasFile.VERSION_REVISION, LasFile.VERSION_BUILD_DATE));
                Array.Copy(v, vlr.description, Math.Min(v.Length, 31));

                byte[] buffer = new byte[number * 8];
                Buffer.BlockCopy(geodouble_params, 0, buffer, 0, number * 8);
                vlr.data = buffer;

                // add the VLR
                if (!AddVLR(vlr) )
                {
                    error = string.Format("setting {0} geodouble_params", number);
                    return false;
                }
            }
            catch
            {
                error = "internal error in set_geodouble_params";
                return false;
            }

            error = null;
            return true;
        }

        public bool SetGeoasciiParams(ushort number, byte[] geoascii_params)
        {
            if (number == 0)
            {
                error = "number of geoascii_params is zero";
                return false;
            }

            if (geoascii_params == null)
            {
                error = "geoascii_params pointer is zero";
                return false;
            }

            try
            {
                // fill a VLR
                LasVLR vlr = new LasVLR();
                vlr.reserved = 0xAABB;
                byte[] user_id = Encoding.ASCII.GetBytes("LASF_Projection");
                Array.Copy(user_id, vlr.user_id, Math.Min(user_id.Length, 16));
                vlr.record_id = 34737;
                vlr.record_length_after_header = number;

                // description field must be a null-terminate string, so we don't copy more than 31 characters
                byte[] v = Encoding.ASCII.GetBytes(string.Format("lasFile.net DLL {0}.{1} r{2} ({3})", LasFile.VERSION_MAJOR, LasFile.VERSION_MINOR, LasFile.VERSION_REVISION, LasFile.VERSION_BUILD_DATE));
                Array.Copy(v, vlr.description, Math.Min(v.Length, 31));

                vlr.data = geoascii_params;

                // add the VLR
                if (!AddVLR(vlr) )
                {
                    error = string.Format("setting {0} geoascii_params", number);
                    return false;
                }
            }
            catch
            {
                error = "internal error in set_geoascii_params";
                return false;
            }

            error = null;
            return true;
        }

        static bool ArrayCompare(byte[] a, byte[] b)
        {
            int len = Math.Min(a.Length, b.Length);
            int i = 0;
            for (; i < len; i++)
            {
                if (a[i] != b[i]) return false;
                if (a[i] == 0) break;
            }

            if (i < len - 1) return true;
            return a.Length == b.Length;
        }

        public bool AddVLR(LasVLR vlr)
        {
            if (vlr == null)
            {
                error = "vlr_struct vlr pointer is zero";
                return false;
            }

            if ((vlr.record_length_after_header > 0) && (vlr.data == null))
            {
                error = string.Format("VLR has record_length_after_header of {0} but VLR data pointer is zero", vlr.record_length_after_header);
                return false;
            }


            try
            {
                if (header.vlrs.Count > 0)
                {
                    // overwrite existing VLR ?
                    for (int i = (int)header.number_of_variable_length_records - 1; i >= 0; i--)
                    {
                        if (header.vlrs[i].record_id == vlr.record_id && !ArrayCompare(header.vlrs[i].user_id, vlr.user_id))
                        {
                            if (header.vlrs[i].record_length_after_header != 0)
                                header.offset_to_point_data -= header.vlrs[i].record_length_after_header;

                            header.vlrs.RemoveAt(i);
                        }
                    }
                }

                header.vlrs.Add(vlr);
                header.number_of_variable_length_records = (uint)header.vlrs.Count;
                header.offset_to_point_data += 54;

                // copy the VLR
                header.offset_to_point_data += vlr.record_length_after_header;
            }
            catch
            {
                error = "internal error in AddVLR";
                return false;
            }

            error = null;
            return true;
        }

        static bool CheckHeaderAndSetup(LasHeader header
            , bool compress
            , out LasFile lasFile
            , ref LasPoint point
            , out uint vrl_payload_size
            , out string error)
        {
            lasFile = null;
            vrl_payload_size = 0;
            error = null;

            #region check header and prepare point

            uint vlrs_size = 0;

            if (header.version_major != 1)
            {
                error = string.Format("Unknown LAS version {0}.{1}", header.version_major, header.version_minor);
                return false;
            }

            if (compress && (header.point_data_format > 5))
            {
                error = string.Format("Compressor does not yet support point data format {0}", header.point_data_format);
                return false;
            }

            if (header.number_of_variable_length_records != 0)
            {
                if (header.vlrs == null)
                {
                    error = string.Format("Number_of_variable_length_records is {0} but vlrs pointer is zero", header.number_of_variable_length_records);
                    return false;
                }

                for (int i = 0; i < header.number_of_variable_length_records; i++)
                {
                    vlrs_size += 54;
                    if (header.vlrs[i].record_length_after_header != 0)
                    {
                        if (header.vlrs == null)
                        {
                            error = string.Format("Vlrs[{0}].record_length_after_header is {1} but vlrs[{0}].data pointer is zero", i, header.vlrs[i].record_length_after_header);
                            return false;
                        }
                        vlrs_size += header.vlrs[i].record_length_after_header;
                    }
                }
            }

            if ((vlrs_size + header.header_size + header.user_data_after_header_size) != header.offset_to_point_data)
            {
                error = string.Format("Header_size ({0}) + vlrs_size ({1}) + user_data_after_header_size ({2}) does not equal offset_to_point_data ({3})", header.header_size, vlrs_size, header.user_data_after_header_size, header.offset_to_point_data);
                return false;
            }

            try
            {
                lasFile = new LasFile();
            }
            catch
            {
                error = "Could not alloc LASFILE";
                return false;
            }

            if (!lasFile.setup(header.point_data_format, header.point_data_record_length, LasFile.COMPRESSOR_NONE))
            {
                error = string.Format("Invalid combination of point_data_format {0} and point_data_record_length {1}"
                    , header.point_data_format, header.point_data_record_length);
                return false;
            }

            #region create point's item pointers
            for (uint i = 0; i < lasFile.num_items; i++)
            {
                switch (lasFile.items[i].type)
                {
                    case LasItem.Type.POINT14:
                    case LasItem.Type.POINT10:
                    case LasItem.Type.GPSTIME11:
                    case LasItem.Type.RGBNIR14:
                    case LasItem.Type.RGB12:
                    case LasItem.Type.WAVEPACKET13: break;
                    case LasItem.Type.BYTE:
                        point.num_extra_bytes = lasFile.items[i].size;
                        point.extra_bytes = new byte[point.num_extra_bytes];
                        break;
                    default:
                        error = string.Format("unknown LasItem type {0}", lasFile.items[i].type);
                        return false;
                }
            }
            #endregion

            if (compress)
            {
                if (!lasFile.setup(header.point_data_format, header.point_data_record_length, LasFile.COMPRESSOR_DEFAULT))
                {
                    error = string.Format("Cannot compress point_data_format {0} with point_data_record_length {1}"
                        , header.point_data_format
                        , header.point_data_record_length);
                    return false;
                }
                lasFile.request_version(2);
                vrl_payload_size = 34u + 6u * lasFile.num_items;
            }
            else
            {
                lasFile.request_version(0);
            }
            #endregion

            return true;
        }

    }
}
