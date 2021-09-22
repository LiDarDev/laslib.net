//===============================================================================
//
//  FILE	: LazReader.cs
//  DESC	: Laz File reader
//  COPYRIGHT:
//    This is free software; you can redistribute and/or modify it under the
//    terms of the GNU Lesser General Licence as published by the Free Software
//    Foundation. See the COPYING file for more information.//
//    This software is distributed WITHOUT ANY WARRANTY and without even the
//    implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using LasLibNet.Abstract;
using LasLibNet.Model;
using LasLibNet.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LasLibNet.Reader
{
	/// <summary>
	/// Laz file reader
	/// </summary>
	public class LazReader :FileReader, IFileReader
	{
		/// <summary>
		/// Raw point reader
		/// </summary>
		ILasPointReader lasPointReader;
		/// <summary>
		/// Compressed point reader
		/// </summary>
		LazPointReader reader;

		LazFile lazFile = null;

		public override bool OpenReader(string file_name)
		{
			bool result=base.OpenReader(file_name);
			if (result == false) return false;

			result = this.read_extra_header();
			if (result == false) return false;

			return true;
		}


		/// <summary>
		/// Read extra header for a laz file.
		/// </summary>
		/// <returns></returns>
		bool read_extra_header()
		{
			byte[] buffer = new byte[32];
			

			try
			{
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
							if (userid == "laszip encoded")
							{
								lazFile = new LazFile();

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
								lazFile.compressor = BitConverter.ToUInt16(buffer, 0);

								if (streamin.Read(buffer, 0, 2) != 2)
								{
									error = "Reading coder";
									return false;
								}
								lazFile.coder = BitConverter.ToUInt16(buffer, 0);

								if (streamin.Read(buffer, 0, 1) != 1)
								{
									error = "Reading version_major failed";
									return false;
								}
								lazFile.version_major = buffer[0];

								if (streamin.Read(buffer, 0, 1) != 1)
								{
									error = "Reading version_minor failed";
									return false;
								}
								lazFile.version_minor = buffer[0];

								if (streamin.Read(buffer, 0, 2) != 2)
								{
									error = "Reading version_revision failed";
									return false;
								}
								lazFile.version_revision = BitConverter.ToUInt16(buffer, 0);

								if (streamin.Read(buffer, 0, 4) != 4)
								{
									error = "Reading options failed";
									return false;
								}
								lazFile.options = BitConverter.ToUInt32(buffer, 0);

								if (streamin.Read(buffer, 0, 4) != 4)
								{
									error = "Reading chunk_size failed";
									return false;
								}
								lazFile.chunk_size = BitConverter.ToUInt32(buffer, 0);

								if (streamin.Read(buffer, 0, 8) != 8)
								{
									error = "Reading number_of_special_evlrs failed";
									return false;
								}
								lazFile.number_of_special_evlrs = BitConverter.ToInt64(buffer, 0);

								if (streamin.Read(buffer, 0, 8) != 8)
								{
									error = "Reading offset_to_special_evlrs failed";
									return false;
								}
								lazFile.offset_to_special_evlrs = BitConverter.ToInt64(buffer, 0);

								if (streamin.Read(buffer, 0, 2) != 2)
								{
									error = "Reading num_items failed";
									return false;
								}
								lazFile.num_items = BitConverter.ToUInt16(buffer, 0);

								lazFile.items = new LazItem[lazFile.num_items];
								for (int j = 0; j < lazFile.num_items; j++)
								{
									lazFile.items[j] = new LazItem();

									if (streamin.Read(buffer, 0, 2) != 2)
									{
										error = string.Format("Reading type of item {0} failed", j);
										return false;
									}
									lazFile.items[j].type = (LazItem.Type)BitConverter.ToUInt16(buffer, 0);

									if (streamin.Read(buffer, 0, 2) != 2)
									{
										error = string.Format("Reading size of item {0} failed", j);
										return false;
									}
									lazFile.items[j].size = BitConverter.ToUInt16(buffer, 0);

									if (streamin.Read(buffer, 0, 2) != 2)
									{
										error = string.Format("Reading version of item {0} failed", j);
										return false;
									}
									lazFile.items[j].version = BitConverter.ToUInt16(buffer, 0);
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
						if (userid == "laszip encoded")
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

				// remove extra bits in point data type
				if ((header.point_data_format & 128) != 0 || (header.point_data_format & 64) != 0)
				{
					if (lazFile == null)
					{
						error = "this file was compressed with an experimental version of LazZip. contact 'martin.isenburg@rapidlasso.com' for assistance";
						return false;
					}
					header.point_data_format &= 127;
				}

				// check if file is compressed
				if (lazFile != null)
				{
					// yes. check the compressor state
					is_compressed = true;
					if (!lazFile.check())
					{
						error = string.Format("{0} upgrade to the latest release of LAStools (with LazZip) or contact 'martin.isenburg@rapidlasso.com' for assistance"
							, lazFile.Error);
						return false;
					}
				}
				else
				{
					// no. setup an un-compressed read
					is_compressed = false;
					lazFile = new LazFile();
					if (!lazFile.setup(header.point_data_format, header.point_data_record_length, LazFile.COMPRESSOR_NONE))
					{
						error = string.Format("Invalid combination of point_data_format {0} and point_data_record_length {1}", header.point_data_format, header.point_data_record_length);
						return false;
					}
				}

				// create point's item pointers
				for (int i = 0; i < lazFile.num_items; i++)
				{
					switch (lazFile.items[i].type)
					{
						case LazItem.Type.POINT14:
						case LazItem.Type.POINT10:
						case LazItem.Type.GPSTIME11:
						case LazItem.Type.RGBNIR14:
						case LazItem.Type.RGB12:
						case LazItem.Type.WAVEPACKET13:
							break;
						case LazItem.Type.BYTE:
							point.num_extra_bytes = lazFile.items[i].size;
							point.extra_bytes = new byte[point.num_extra_bytes];
							break;
						default:
							error = string.Format("Unknown LazItem type {0}", lazFile.items[i].type);
							return false;
					}
				}

			}
			catch
			{
				error = "Internal error in read_extra_header()";
				return false;
			}

			error = null;
			return true;
		}

		/// <summary>
		/// Create point reader.
		/// </summary>
		/// <returns></returns>
		public bool CreatePointReader()
		{
			this.lasPointReader = base.CreateRawPointReader();

			// create the point reader
			reader = new LazPointReader(this.lasPointReader);
			if (!reader.setup(lazFile))
			{
				error = "setup of LazPointReader failed";
				return false;
			}

			if (!reader.init(streamin))
			{
				error = "init of LazPointReader failed";
				return false;
			}

			lazFile = null;

			return true;
		}

		bool check_for_integer_overflow()
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
					error = string.Format("Quantization sign flip for min_x from {0} to {1}. Set scale factor for x coarser than {2}", header.min_x, dequant_min_x, header.x_scale_factor);
					return false;
				}
				if ((header.max_x > 0) != (dequant_max_x > 0))
				{
					error = string.Format("Quantization sign flip for max_x from {0} to {1}. Set scale factor for x coarser than {2}", header.max_x, dequant_max_x, header.x_scale_factor);
					return false;
				}
				if ((header.min_y > 0) != (dequant_min_y > 0))
				{
					error = string.Format("Quantization sign flip for min_y from {0} to {1}. Set scale factor for y coarser than {2}", header.min_y, dequant_min_y, header.y_scale_factor);
					return false;
				}
				if ((header.max_y > 0) != (dequant_max_y > 0))
				{
					error = string.Format("Quantization sign flip for max_y from {0} to {1}. Set scale factor for y coarser than {2}", header.max_y, dequant_max_y, header.y_scale_factor);
					return false;
				}
				if ((header.min_z > 0) != (dequant_min_z > 0))
				{
					error = string.Format("Quantization sign flip for min_z from {0} to {1}. Set scale factor for z coarser than {2}", header.min_z, dequant_min_z, header.z_scale_factor);
					return false;
				}
				if ((header.max_z > 0) != (dequant_max_z > 0))
				{
					error = string.Format("quantization sign flip for max_z from {0} to {1}. Set scale factor for z coarser than {2}", header.max_z, dequant_max_z, header.z_scale_factor);
					return false;
				}
			}
			catch
			{
				error = "Internal error in auto_offset()";
				return false;
			}

			error = null;
			return true;
		}

		bool auto_offset()
		{
			try
			{
				if (reader != null)
				{
					error = "cannot auto offset after reader was opened";
					return false;
				}

				// check scale factor
				double x_scale_factor = header.x_scale_factor;
				double y_scale_factor = header.y_scale_factor;
				double z_scale_factor = header.z_scale_factor;

				if ((x_scale_factor <= 0) || double.IsInfinity(x_scale_factor))
				{
					error = string.Format("Invalid x scale_factor {0} in header", header.x_scale_factor);
					return false;
				}

				if ((y_scale_factor <= 0) || double.IsInfinity(y_scale_factor))
				{
					error = string.Format("Invalid y scale_factor {0} in header", header.y_scale_factor);
					return false;
				}

				if ((z_scale_factor <= 0) || double.IsInfinity(z_scale_factor))
				{
					error = string.Format("Invalid z scale_factor {0} in header", header.z_scale_factor);
					return false;
				}

				double center_bb_x = (header.min_x + header.max_x) / 2;
				double center_bb_y = (header.min_y + header.max_y) / 2;
				double center_bb_z = (header.min_z + header.max_z) / 2;

				if (double.IsInfinity(center_bb_x))
				{
					error = string.Format("Invalid x coordinate at center of bounding box (min: {0} max: {1})", header.min_x, header.max_x);
					return false;
				}

				if (double.IsInfinity(center_bb_y))
				{
					error = string.Format("Invalid y coordinate at center of bounding box (min: {0} max: {1})", header.min_y, header.max_y);
					return false;
				}

				if (double.IsInfinity(center_bb_z))
				{
					error = string.Format("Invalid z coordinate at center of bounding box (min: {0} max: {1})", header.min_z, header.max_z);
					return false;
				}

				double x_offset = header.x_offset;
				double y_offset = header.y_offset;
				double z_offset = header.z_offset;

				header.x_offset = (MyDefs.I64_FLOOR(center_bb_x / x_scale_factor / 10000000)) * 10000000 * x_scale_factor;
				header.y_offset = (MyDefs.I64_FLOOR(center_bb_y / y_scale_factor / 10000000)) * 10000000 * y_scale_factor;
				header.z_offset = (MyDefs.I64_FLOOR(center_bb_z / z_scale_factor / 10000000)) * 10000000 * z_scale_factor;

				if (check_for_integer_overflow() ==false)
				{
					header.x_offset = x_offset;
					header.y_offset = y_offset;
					header.z_offset = z_offset;
					return false;
				}
			}
			catch
			{
				error = "Internal error in auto_offset()";
				return false;
			}

			error = null;
			return true;
		}

		public bool SetPoint(LasPoint point)
		{
			if (point == null)
			{
				error = "The point_struct pointer is zero";
				return false;
			}

			if (reader != null)
			{
				error = "Cannot set point for reader";
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
				//Array.Copy(point.rgb, this.point.rgb, 4);
				this.point.red = point.red;
				this.point.green = point.green;
				this.point.blue = point.blue;
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
							error = string.Format("The target point has {0} extra bytes but source point has {1}", this.point.num_extra_bytes, point.num_extra_bytes);
							return false;
						}
					}
					else
					{
						error = "The target point has extra bytes but source point does not";
						return false;
					}
				}
				else
				{
					if (point.extra_bytes != null)
					{
						error = "The source point has extra bytes but target point does not";
						return false;
					}
				}
			}
			catch
			{
				error = "Internal error in set_point()";
				return false;
			}

			error = null;
			return true;
		}
		
		public unsafe bool SetGeokeys(ushort number, LasGeokey[] key_entries)
		{
			if (number == 0)
			{
				error = "The number of key_entries is zero";
				return false;
			}

			if (key_entries == null)
			{
				error = "The key_entries pointer is zero";
				return false;
			}

			if (reader != null)
			{
				error = "Cannot set geokeys after reader was opened";
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
				byte[] v = Encoding.ASCII.GetBytes(string.Format("CSU LasLibNet {0}.{1} r{2} ({3})", LazFile.VERSION_MAJOR, LazFile.VERSION_MINOR, LazFile.VERSION_REVISION, LazFile.VERSION_BUILD_DATE));
				Array.Copy(v, vlr.description, Math.Min(v.Length, 31));

				vlr.data = buffer;

				// add the VLR
				if (AddVLR(vlr) ==false)
				{
					error = string.Format("The invalide setting {0} geokeys", number);
					return false;
				}
			}
			catch
			{
				error = "Internal error in set_geokey_entries()";
				return false;
			}

			error = null;
			return true;
		}

		public bool SetGeodoubleParams(ushort number, double[] geodouble_params)
		{
			if (number == 0)
			{
				error = "The number of geodouble_params is zero";
				return false;
			}

			if (geodouble_params == null)
			{
				error = "The geodouble_params pointer is zero";
				return false;
			}

			if (reader != null)
			{
				error = "Cannot set geodouble_params after reader was opened";
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
				byte[] v = Encoding.ASCII.GetBytes(string.Format("CSU LasLibNet {0}.{1} r{2} ({3})", LazFile.VERSION_MAJOR, LazFile.VERSION_MINOR, LazFile.VERSION_REVISION, LazFile.VERSION_BUILD_DATE));
				Array.Copy(v, vlr.description, Math.Min(v.Length, 31));

				byte[] buffer = new byte[number * 8];
				Buffer.BlockCopy(geodouble_params, 0, buffer, 0, number * 8);
				vlr.data = buffer;

				// add the VLR
				if (AddVLR(vlr) == false)
				{
					error = string.Format("Invalide setting {0} geodouble_params", number);
					return false;
				}
			}
			catch
			{
				error = "Internal error in set_geodouble_params()";
				return false;
			}

			error = null;
			return true;
		}

		public bool SetGeoasciiParams(ushort number, byte[] geoascii_params)
		{
			if (number == 0)
			{
				error = "The number of geoascii_params is zero";
				return false;
			}

			if (geoascii_params == null)
			{
				error = "The geoascii_params pointer is zero";
				return false;
			}

			if (reader != null)
			{
				error = "Cannot set geoascii_params after reader was opened";
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
				byte[] v = Encoding.ASCII.GetBytes(string.Format("CSU LasLibNet {0}.{1} r{2} ({3})", LazFile.VERSION_MAJOR, LazFile.VERSION_MINOR, LazFile.VERSION_REVISION, LazFile.VERSION_BUILD_DATE));
				Array.Copy(v, vlr.description, Math.Min(v.Length, 31));

				vlr.data = geoascii_params;

				// add the VLR
				if (AddVLR(vlr) ==false)
				{
					error = string.Format("Invalide setting {0} geoascii_params", number);
					return false;
				}
			}
			catch
			{
				error = "Internal error in set_geoascii_params()";
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
				error = "The lasVLR_struct vlr pointer is zero";
				return false;
			}

			if ((vlr.record_length_after_header > 0) && (vlr.data == null))
			{
				error = string.Format("VLR has record_length_after_header of {0} but VLR data pointer is zero", vlr.record_length_after_header);
				return false;
			}

			if (reader != null)
			{
				error = "Cannot add vlr after reader was opened";
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
				error = "Internal error in AddVLR";
				return false;
			}

			error = null;
			return true;
		}

		bool CheckHeaderAndSetup(LasHeader header
			, out LazFile laszip
			, ref LasPoint point
			, out uint laszip_vrl_payload_size)
		{
			bool compress = true;
			laszip = null;
			laszip_vrl_payload_size = 0;
			
			#region check header and prepare point

			uint vlrs_size = 0;

			if (header.version_major != 1)
			{
				error = string.Format("Unknown LAS version {0}.{1}", header.version_major, header.version_minor);
				return false;
			}

			if (compress && (header.point_data_format > 5))
			{
				error = string.Format("Compressor does not yet support point data format {1}", header.point_data_format);
				return false;
			}

			if (header.number_of_variable_length_records != 0)
			{
				if (header.vlrs == null)
				{
					error = string.Format("The number_of_variable_length_records is {0} but vlrs pointer is zero", header.number_of_variable_length_records);
					return false;
				}

				for (int i = 0; i < header.number_of_variable_length_records; i++)
				{
					vlrs_size += 54;
					if (header.vlrs[i].record_length_after_header != 0)
					{
						if (header.vlrs == null)
						{
							error = string.Format("The vlrs[{0}].record_length_after_header is {1} but vlrs[{0}].data pointer is zero", i, header.vlrs[i].record_length_after_header);
							return false;
						}
						vlrs_size += header.vlrs[i].record_length_after_header;
					}
				}
			}

			if ((vlrs_size + header.header_size + header.user_data_after_header_size) != header.offset_to_point_data)
			{
				error = string.Format("The header_size ({0}) plus vlrs_size ({1}) plus user_data_after_header_size ({2}) does not equal offset_to_point_data ({3})", header.header_size, vlrs_size, header.user_data_after_header_size, header.offset_to_point_data);
				return false;
			}

			try
			{
				laszip = new LazFile();
			}
			catch
			{
				error = "Could not alloc LazZip";
				return false;
			}

			if (!laszip.setup(header.point_data_format, header.point_data_record_length, LazFile.COMPRESSOR_NONE))
			{
				error = string.Format("Invalid combination of point_data_format {0} and point_data_record_length {1}", header.point_data_format, header.point_data_record_length);
				return false;
			}

			#region create point's item pointers
			for (uint i = 0; i < laszip.num_items; i++)
			{
				switch (laszip.items[i].type)
				{
					case LazItem.Type.POINT14:
					case LazItem.Type.POINT10:
					case LazItem.Type.GPSTIME11:
					case LazItem.Type.RGBNIR14:
					case LazItem.Type.RGB12:
					case LazItem.Type.WAVEPACKET13: break;
					case LazItem.Type.BYTE:
						point.num_extra_bytes = laszip.items[i].size;
						point.extra_bytes = new byte[point.num_extra_bytes];
						break;
					default:
						error = string.Format("Unknown LazItem type {0}", laszip.items[i].type);
						return false;
				}
			}
			#endregion

			if (compress)
			{
				if (!laszip.setup(header.point_data_format, header.point_data_record_length, LazFile.COMPRESSOR_DEFAULT))
				{
					error = string.Format("Cannot compress point_data_format {0} with point_data_record_length {1}", header.point_data_format, header.point_data_record_length);
					return false;
				}
				laszip.request_version(2);
				laszip_vrl_payload_size = 34u + 6u * laszip.num_items;
			}
			else
			{
				laszip.request_version(0);
			}
			#endregion

			return true;
		}


		public bool SeekPoint(long index)
		{
			try
			{
				// seek to the point
				if (!reader.seek((uint)p_sequence, (uint)index))
				{
					error = string.Format("seeking from index {0} to index {1} for file with {2} points", p_sequence, index, npoints);
					return false;
				}
				p_sequence = index;
			}
			catch
			{
				error = "internal error in laszip_seek_point";
				return false;
			}

			error = null;
			return true;
		}

		public LasPoint ReadPoint()
		{
			if (reader == null)
			{
				error = "Reading points before reader was opened";
				return null;
			}

			try
			{
				// read the point
				if ((point=reader.Read())==null)
				{
					error = string.Format("Reading point with index {0} of {1} total points failed: {2}"
						, p_sequence, npoints,reader.Error);
					return null;
				}

				p_sequence++;
			}
			catch
			{
				error = "Internal error in laszip_read_point";
				return null;
			}

			error = null;
			return point;
		}

		public override bool CloseReader()
		{
			if (reader == null)
			{
				error = "closing reader before it was opened";
				return false;
			}

			try
			{
				if (!reader.done())
				{
					error = "done of LazPointReader failed";
					return false;
				}

				reader = null;
				if (!leaveStreamInOpen) streamin.Close();
				streamin = null;
			}
			catch
			{
				error = "internal error in laszip_close_reader";
				return false;
			}

			error = null;
			return true;
		}

	}
}
