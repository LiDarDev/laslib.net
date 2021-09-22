
//////////////////////////////////////////////////////////////////////
/// File    : FileWriter.cs
/// Desc    : File writer base class
/// Author  : Li G.Q.
/// Date    : 2021/9/22/
//////////////////////////////////////////////////////////////////////


using LasLibNet.Abstract;
using LasLibNet.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LasLibNet.Writer
{
    /// <summary>
    /// File Writer
    /// </summary>
    public class FileWriter
    {
        protected Stream outstream;

		/// <summary>
		/// Raw point writer
		/// </summary>
		protected ILasPointWriter lasPointWriter;

		protected LasHeader header;
        /// <summary>
        /// Las File Header
        /// </summary>
        public LasHeader Header
        {
            get => header;
            set => header = value;
        }

        /// <summary>
        /// The written point count.
        /// </summary>
        protected long p_count = 0;
        /// <summary>
        /// The written point count.
        /// </summary>
        public long PointCount
        {
            get => this.p_count;
        }

        protected string error;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error
        {
            get => error;
        }

        /// <summary>
        /// Open the file writer
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool OpenWriter(string filename)
        {
            if (filename == null || filename.Length == 0)
            {
                error = "The filename is empty";
                return false;
            }

            try
            {
                outstream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
				return true;
            }
            catch (Exception e)
            {
                error = string.Format("Cannot open file '{0}' : {1}", filename, e.Message);
                return false;
            }
        }


		/// <summary>
		/// Set header.
		/// </summary>
		/// <param name="header"></param>
		public void SetHeader(LasHeader header)
		{
			this.header = header;
		}

		/// <summary>
		/// Close the out stream.
		/// </summary>
		/// <returns></returns>
		public virtual bool CloseWriter()
		{
			try
			{
				lasPointWriter = null;
				outstream.Close();
				outstream.Dispose();
				outstream = null;
			}
			catch
			{
				error = "Internal error in las file writer close";
				return false;
			}

			error = null;
			return true;
		}


		/// <summary>
		/// Write the header info into the file.
		/// </summary>
		/// <returns></returns>
		public bool WriteHeader()
		{
			#region write the header variable after variable
			try
			{
				outstream.WriteByte((byte)'L');
				outstream.WriteByte((byte)'A');
				outstream.WriteByte((byte)'S');
				outstream.WriteByte((byte)'F');
			}
			catch
			{
				error = "Write header.file_signature failed";
				return false;
			}

			try { outstream.Write(BitConverter.GetBytes(header.file_source_ID), 0, 2); }
			catch { error = "Write header.file_source_ID failed"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.global_encoding), 0, 2); }
			catch { error = "Write header.global_encoding failed"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.project_ID_GUID_data_1), 0, 4); }
			catch { error = "Write header.project_ID_GUID_data_1"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.project_ID_GUID_data_2), 0, 2); }
			catch { error = "Write header.project_ID_GUID_data_2"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.project_ID_GUID_data_3), 0, 2); }
			catch { error = "Write header.project_ID_GUID_data_3"; return false; }

			try { outstream.Write(header.project_ID_GUID_data_4, 0, 8); }
			catch { error = "Write header.project_ID_GUID_data_4"; return false; }

			try { outstream.WriteByte(header.version_major); }
			catch { error = "Write header.version_major"; return false; }

			try { outstream.WriteByte(header.version_minor); }
			catch { error = "Write header.version_minor"; return false; }

			try { outstream.Write(header.system_identifier, 0, 32); }
			catch { error = "Write header.system_identifier"; return false; }

			if (header.generating_software == null || header.generating_software.Length != 32)
			{
				byte[] generatingSoftware = Encoding.ASCII.GetBytes(string.Format("LasLibNet {0}.{1} r{2} ({3})"
					, LasFile.VERSION_MAJOR, LasFile.VERSION_MINOR
					, LasFile.VERSION_REVISION, LasFile.VERSION_BUILD_DATE));
				Array.Copy(generatingSoftware, header.generating_software, Math.Min(generatingSoftware.Length, 32));
			}

			try { outstream.Write(header.generating_software, 0, 32); }
			catch { error = "Write header.generating_software"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.file_creation_day), 0, 2); }
			catch { error = "Write header.file_creation_day"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.file_creation_year), 0, 2); }
			catch { error = "Write header.file_creation_year"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.header_size), 0, 2); }
			catch { error = "Write header.header_size"; return false; }


			try { outstream.Write(BitConverter.GetBytes(header.offset_to_point_data), 0, 4); }
			catch { error = "Write header.offset_to_point_data"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.number_of_variable_length_records), 0, 4); }
			catch { error = "Write header.number_of_variable_length_records"; return false; }


			try { outstream.WriteByte(header.point_data_format); }
			catch { error = "Write header.point_data_format"; return false; }


			try { outstream.Write(BitConverter.GetBytes(header.point_data_record_length), 0, 2); }
			catch { error = "Write header.point_data_record_length"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.number_of_point_records), 0, 4); }
			catch { error = "Write header.number_of_point_records"; return false; }

			for (uint i = 0; i < 5; i++)
			{
				try { outstream.Write(BitConverter.GetBytes(header.number_of_points_by_return[i]), 0, 4); }
				catch { error = string.Format("Write header.number_of_points_by_return {0}", i); return false; }
			}

			try { outstream.Write(BitConverter.GetBytes(header.x_scale_factor), 0, 8); }
			catch { error = "Write header.x_scale_factor"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.y_scale_factor), 0, 8); }
			catch { error = "Write header.y_scale_factor"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.z_scale_factor), 0, 8); }
			catch { error = "Write header.z_scale_factor"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.x_offset), 0, 8); }
			catch { error = "Write header.x_offset"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.y_offset), 0, 8); }
			catch { error = "Write header.y_offset"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.z_offset), 0, 8); }
			catch { error = "Write header.z_offset"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.max_x), 0, 8); }
			catch { error = "Write header.max_x"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.min_x), 0, 8); }
			catch { error = "Write header.min_x"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.max_y), 0, 8); }
			catch { error = "Write header.max_y"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.min_y), 0, 8); }
			catch { error = "Write header.min_y"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.max_z), 0, 8); }
			catch { error = "Write header.max_z"; return false; }

			try { outstream.Write(BitConverter.GetBytes(header.min_z), 0, 8); }
			catch { error = "Write header.min_z"; return false; }

			#region special handling for LAS 1.3+
			if (header.version_major == 1 && header.version_minor >= 3)
			{
				if (header.header_size < 235)
				{
					error = string.Format("for LAS 1.{0} header_size should at least be 235 but it is only {1}", header.version_minor, header.header_size);
					return false;
				}

				try { outstream.Write(BitConverter.GetBytes(header.start_of_waveform_data_packet_record), 0, 8); }
				catch { error = "Write header.start_of_waveform_data_packet_record"; return false; }

				header.user_data_in_header_size = header.header_size - 235u;
			}
			else header.user_data_in_header_size = header.header_size - 227u;
			#endregion

			#region special handling for LAS 1.4+
			if (header.version_major == 1 && header.version_minor >= 4)
			{
				if (header.header_size < 375)
				{
					error = string.Format("for LAS 1.{0} header_size should at least be 375 but it is only {1}", header.version_minor, header.header_size);
					return false;
				}

				try { outstream.Write(BitConverter.GetBytes(header.start_of_first_extended_variable_length_record), 0, 8); }
				catch { error = "Write header.start_of_first_extended_variable_length_record"; return false; }

				try { outstream.Write(BitConverter.GetBytes(header.number_of_extended_variable_length_records), 0, 4); }
				catch { error = "Write header.number_of_extended_variable_length_records"; return false; }

				try { outstream.Write(BitConverter.GetBytes(header.extended_number_of_point_records), 0, 8); }
				catch { error = "Write header.extended_number_of_point_records"; return false; }

				for (uint i = 0; i < 15; i++)
				{
					try { outstream.Write(BitConverter.GetBytes(header.extended_number_of_points_by_return[i]), 0, 8); }
					catch { error = string.Format("Write header.extended_number_of_points_by_return[{0}]", i); return false; }
				}

				header.user_data_in_header_size = header.header_size - 375u;
			}
			#endregion

			#region write any number of user-defined bytes that might have been added to the header
			if (header.user_data_in_header_size != 0)
			{
				try { outstream.Write(header.user_data_in_header, 0, (int)header.user_data_in_header_size); }
				catch { error = string.Format("Write {0} bytes of data into header.user_data_in_header", header.user_data_in_header_size); return false; }
			}
			#endregion

			#region write variable length records into the header
			if (header.number_of_variable_length_records != 0)
			{
				for (int i = 0; i < header.number_of_variable_length_records; i++)
				{
					// write variable length records variable after variable (to avoid alignment issues)
					try { outstream.Write(BitConverter.GetBytes(header.vlrs[i].reserved), 0, 2); }
					catch { error = string.Format("Write header.vlrs[{0}].reserved", i); return false; }

					try { outstream.Write(header.vlrs[i].user_id, 0, 16); }
					catch { error = string.Format("Write header.vlrs[{0}].user_id", i); return false; }

					try { outstream.Write(BitConverter.GetBytes(header.vlrs[i].record_id), 0, 2); }
					catch { error = string.Format("Write header.vlrs[{0}].record_id", i); return false; }

					try { outstream.Write(BitConverter.GetBytes(header.vlrs[i].record_length_after_header), 0, 2); }
					catch { error = string.Format("Write header.vlrs[{0}].record_length_after_header", i); return false; }

					try { outstream.Write(header.vlrs[i].description, 0, 32); }
					catch { error = string.Format("Write header.vlrs[{0}].description", i); return false; }

					// write data following the header of the variable length record
					if (header.vlrs[i].record_length_after_header != 0)
					{
						try { outstream.Write(header.vlrs[i].data, 0, header.vlrs[i].record_length_after_header); }
						catch { error = string.Format("Write {0} bytes of data into header.vlrs[{1}].data", header.vlrs[i].record_length_after_header, i); return false; }
					}
				}
			}

			#endregion

			#region write any number of user-defined bytes that might have been added after the header
			if (header.user_data_after_header_size != 0)
			{
				try { outstream.Write(header.user_data_after_header, 0, (int)header.user_data_after_header_size); }
				catch { error = string.Format("Write {0} bytes of data into header.user_data_after_header", header.user_data_after_header_size); return false; }
			}
			#endregion

			#endregion

			error = null;
			return true;
		}

		/// <summary>
		/// Write the header info into the file.
		/// </summary>
		/// <returns></returns>
		public bool WriteHeader(LasHeader header)
		{
			this.header = header;
			return this.WriteHeader();
		}

		/// <summary>
		/// Write the header byte[].
		/// </summary>
		/// <param name="header"></param>
		/// <returns></returns>
		public bool WriteHeader(byte[] header)
		{
			try
			{
				this.outstream.Write(header, 0, header.Length);
				return true;
			}
			catch (Exception e)
			{
				error = string.Format("Write the header failed : {0}. ", e.Message);
				return false;
			}
		}


		/// <summary>
		/// Create raw point writer by the version and point data format.
		/// </summary>
		public virtual bool CreatePointWriter()
		{
			if (this.header.version_major == 1 && this.header.version_minor == 0)
			{
				if (this.header.point_data_format == 0)
					this.lasPointWriter = new LasPoint_1_0_Format0_Writer(outstream);
				else
					this.lasPointWriter = new LasPoint_1_0_Format1_Writer(outstream);
			}
			else if (this.header.version_major == 1 && this.header.version_minor == 1)
			{
				if (this.header.point_data_format == 0)
					this.lasPointWriter = new LasPoint_1_1_Format0_Writer(outstream);
				else if (this.header.point_data_format == 1)
					this.lasPointWriter = new LasPoint_1_1_Format1_Writer(outstream);
				else
				{
					error = string.Format("The version {0}.{1} and format {2} is not supported"
				   , header.version_major, header.version_minor, header.point_data_format);
					return false;
				}
			}
			else if (this.header.version_major == 1 && this.header.version_minor == 2)
			{
				if (this.header.point_data_format == 0)
					this.lasPointWriter = new LasPoint_1_2_Format0_Writer(outstream);
				else if (this.header.point_data_format == 1)
					this.lasPointWriter = new LasPoint_1_2_Format1_Writer(outstream);
				else if (this.header.point_data_format == 2)
					this.lasPointWriter = new LasPoint_1_2_Format2_Writer(outstream);
				else if (this.header.point_data_format == 3)
					this.lasPointWriter = new LasPoint_1_2_Format3_Writer(outstream);
				else
				{
					error = string.Format("The version {0}.{1} and format {2} is not supported"
				   , header.version_major, header.version_minor, header.point_data_format);
					return false;
				}
			}
			else
			{
				error = string.Format("The version {0}.{1} is not supported"
					, header.version_major, header.version_minor);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Write a raw point
		/// </summary>
		/// <returns></returns>
		public virtual bool WritePoint(LasPoint point)
		{
			if (lasPointWriter == null)
			{
				error = "Writing points before writer was opened";
				return false;
			}

			try
			{
				// write the point
				if (!lasPointWriter.Write(point))
				{
					error = string.Format("Writing point failed");
					return false;
				}

				p_count++;
			}
			catch
			{
				error = "internal error in laszip_write_point";
				return false;
			}

			return true;
		}
	}
}
