using LasLibNet;
using LasLibNet.Model;
using LasLibNet.Test;
using LasLibNet.Utils;
using LasLibNet.Writer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace LasLib.net.Test
{
    public partial class FormMain : Form
    {
        string lasFile = "";

        LasReader lasReader = new LasReader();
        LasHeader lasHeader;
        LasHeader newHeader;  // New las file header.

        bool isCompressed = true;

        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open a las file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {

            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.tslFile.Text = this.openFileDialog.FileName;
                this.lasFile = this.tslFile.Text;
                if (lasReader.OpenReader(this.lasFile) == false)
                {
                    MessageBox.Show(lasReader.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    lasReader.CloseReader();
                    return;
                }
                this.toolStripButton2.Enabled = true;
                this.toolStripButton3.Enabled = true;
                this.toolStripButton4.Enabled = true;
                this.tsbSaveAs.Enabled = true;                

                //Display the header info.
                this.toolStripButton2_Click(null, null);
            }
        }

        /// <summary>
        /// Display info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            lasHeader = this.lasReader.Header;
            this.dgvInfo.Rows.Clear();

            #region 创建数据行

            Tools.DisplayClassProperties<LasHeader>(lasHeader, this.dgvInfo);

            //this.dgvInfo.Rows.Add("Version", lasHeader.version_major + "." + lasHeader.version_minor);
            //this.dgvInfo.Rows.Add("Header Size", lasHeader.header_size.ToString());
            //this.dgvInfo.Rows.Add("Point Count", lasHeader.number_of_point_records.ToString());
            //this.dgvInfo.Rows.Add("Point data format", lasHeader.point_data_format.ToString());
            //this.dgvInfo.Rows.Add("X Max", lasHeader.max_x.ToString("f4"));
            //this.dgvInfo.Rows.Add("X Min", lasHeader.min_x.ToString("f4"));
            //this.dgvInfo.Rows.Add("Y Max", lasHeader.max_y.ToString("f4"));
            //this.dgvInfo.Rows.Add("Y Min", lasHeader.min_y.ToString("f4"));
            #endregion

            Tools.DisplayClassProperties<LasHeader>(lasHeader);
        }

        /// <summary>
        /// Display all points
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            this.tslMain.Text = " Reading data...";
            this.statusStrip1.Refresh();
            this.Cursor = Cursors.WaitCursor;

            #region Create DataTable
            DataTable dt = new DataTable();
            DataColumn colId = new DataColumn("id", typeof(string));
            dt.Columns.Add(colId);
            DataColumn colX = new DataColumn("X", typeof(string));
            dt.Columns.Add(colX);
            DataColumn colY = new DataColumn("Y", typeof(string));
            dt.Columns.Add(colY);
            DataColumn colZ = new DataColumn("Z", typeof(string));
            dt.Columns.Add(colZ);
            DataColumn colI = new DataColumn("I", typeof(string));
            dt.Columns.Add(colI);
            DataColumn colR = new DataColumn("R", typeof(string));
            dt.Columns.Add(colR);
            DataColumn colG = new DataColumn("G", typeof(string));
            dt.Columns.Add(colG);
            DataColumn colB = new DataColumn("B", typeof(string));
            dt.Columns.Add(colB);
            #endregion

            #region Add Data Row
            // Go to the first point
            lasReader.SeekPoint(0);
            if (this.dgvData.DataSource != null)
            {
                DataTable dt1 = (DataTable)this.dgvData.DataSource;
                if (dt1 != null)
                    dt1.Clear();
            }
            // Loop through number of points indicated
            for (int pointIndex = 0; pointIndex < this.lasHeader.number_of_point_records; pointIndex++)
            {
                // Read the point
                LasPoint p = lasReader.ReadPoint();
                if (p == null)
                {
                    MessageBox.Show(lasReader.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                
                DataRow row = dt.NewRow();
                row[colId] = (pointIndex+1).ToString();
                row[colX] = p.GeoX.ToString("f2");
                row[colY] = p.GeoY.ToString("f2");
                row[colZ] = p.GeoZ.ToString("f2");
                row[colI] = p.intensity.ToString();
                row[colR] = p.red.ToString();
                row[colG] = p.green.ToString();
                row[colB] = p.blue.ToString();
                dt.Rows.Add(row);

            }
            #endregion

            this.dgvData.DataSource = dt;
            this.Cursor = Cursors.Default;
            this.tslMain.Text = "Data read successfully!";
            this.tsbExportCSV.Enabled = true;

        }

        /// <summary>
        /// Close the reader
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.lasReader.CloseReader();
            //this.dgvData.Rows.Clear();
            DataTable dt = (DataTable)this.dgvData.DataSource;
            if (dt != null)
                dt.Clear();
            this.dgvInfo.Rows.Clear();
            this.tslFile.Text = "No file opened!";

            this.toolStripButton2.Enabled = false;
            this.toolStripButton3.Enabled = false;
            this.toolStripButton4.Enabled = false;
            this.tsbSaveAs.Enabled = false;
            this.tsbExportCSV.Enabled = false;
        }

        /// <summary>
        /// Create a new las file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbCreateLas_Click(object sender, EventArgs e)
        {
            if (this.dgvData.Rows.Count < 1)
            {
                MessageBox.Show("Please open a csv firstly !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                // Search the x/y/z max and min value.
                double max_x = double.MinValue, max_y = double.MinValue, max_z = double.MinValue,
                       min_x = double.MaxValue, min_y = double.MaxValue, min_z = double.MaxValue;

                DataTable dt = (DataTable)this.dgvData.DataSource;
                double d;
                foreach (DataRow row in dt.Rows)
                {
                    d = double.Parse(row["X"].ToString());
                    if (d > max_x) max_x = d;
                    if (d < min_x) min_x = d;

                    d = double.Parse(row["Y"].ToString());
                    if (d > max_y) max_y = d;
                    if (d < max_y) min_y = d;

                    d = double.Parse(row["Z"].ToString());
                    if (d > max_z) max_z = d;
                    if (d < max_z) min_z = d;
                }

                LasHeader header = this.CreateHeader();
                header.number_of_point_records = (uint)dt.Rows.Count;
                header.max_x = max_x;
                header.min_x = min_x;
                header.max_y = max_y;
                header.min_y = min_y;
                header.max_z = max_z;
                header.min_z = min_z;

                this.dgvInfo.Rows.Clear();
                Tools.DisplayClassProperties<LasHeader>(header, this.dgvInfo);

                this.newHeader = header;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: "+ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            

        }

        private LasHeader CreateHeader()
        {
            LasHeader header = new LasHeader();

            header.file_source_ID = 0;
            header.global_encoding = 0;
            header.project_ID_GUID_data_1 = 0;
            header.project_ID_GUID_data_2 = 0;
            header.project_ID_GUID_data_3 = 0;
            header.project_ID_GUID_data_4 = new byte[8];
            header.version_major = 01;
            header.version_minor = 02; ;
            header.system_identifier = Encoding.UTF8.GetBytes("CSU LasLibNet R1.0, 20210918    ") ;
            header.generating_software = Encoding.UTF8.GetBytes("CSU LasLibNet R1.0, 20210918    ");
            header.file_creation_day = (ushort)DateTime.Now.DayOfYear;
            header.file_creation_year = (ushort)DateTime.Now.Year;
            header.header_size = 227;
            header.offset_to_point_data = 227;
            header.number_of_variable_length_records = 0;
            header.point_data_format = 03;
            header.point_data_record_length = 34;
            header.number_of_point_records = 0;
            uint[] uints = { 0, 0, 0, 0, 0 };
            header.number_of_points_by_return =uints ;
            header.x_scale_factor = 0.01000;
            header.y_scale_factor = 0.01000;
            header.z_scale_factor = 0.01000;
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
            header.user_data_in_header_size = 0;
            header.vlrs = null;
            header.user_data_after_header_size = 0;

            return header;
        }

        /// <summary>
        /// Clone to another las file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbSaveAs_Click(object sender, EventArgs e)
        {

            if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("You HAVE TO choose a las file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string FileName = this.saveFileDialog.FileName;

            LasWriter lazWriter = new LasWriter(this.lasHeader);

            bool result = lazWriter.OpenWriter(FileName);
            if (!result)
            {
                MessageBox.Show(lazWriter.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("  #Open Writer failed : " + lazWriter.Error);
                lazWriter.CloseWriter();
                return;
            }
            byte[] header_info = lasReader.GetExtendHeader();
            if (header_info == null)
            {
                MessageBox.Show(lazWriter.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
                lazWriter.CloseWriter();
                return;
            }

            if (lazWriter.WriteHeader(header_info) == false)
            {
                MessageBox.Show(lazWriter.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lazWriter.CloseWriter();
                return;
            }

            // Loop through number of points indicated
            for (int pointIndex = 0; pointIndex < this.lasHeader.number_of_point_records; pointIndex++)
            {
                // Read the point
                LasPoint p = lasReader.ReadPoint();
                if (p == null)
                {
                    MessageBox.Show(lasReader.Error, "Read Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine(" #Read point failed : " + lasReader.Error);
                    break;
                }
               

                result = lazWriter.WritePoint(p);
                if (!result)
                {
                    MessageBox.Show(lazWriter.Error, "Write failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine("  #Write point failed : " + lazWriter.Error);
                    break;
                }
            }

            lazWriter.CloseWriter();

            if (!result)
            {
                MessageBox.Show("An error occurred while writing to the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
                MessageBox.Show("Save the data succesfully!", "Info.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        
        }

        private void tsbOpenCSV_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                try
                {
                    dialog.Filter = "csv files (*.csv)|*.csv";
                    dialog.Multiselect = false;
                    dialog.InitialDirectory = ".";
                    dialog.Title = "Select file (only in csv format)";
                    
                    this.tslMain.Text = " Reading csv ...";
                    this.Cursor = Cursors.WaitCursor;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        dtCSV = CSVHelper.ReadData(dialog.FileName);
                        this.dgvData.DataSource = dtCSV;
                    }

                    this.tslMain.Text = "Read CSV data successfully.";
                    this.Cursor = Cursors.Default;
                }
                catch (Exception ex)
                { 
                    
                }
            }
        }

        private void tsbExportCSV_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                try
                {
                    dialog.Filter = "csv files (*.csv)|*.csv";
                    dialog.InitialDirectory = ".";
                    dialog.Title = "Select file (only in csv format)";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        this.tslMain.Text = " Exporting csv ...";
                        this.Cursor = Cursors.WaitCursor;

                        CSVHelper.Export(this.dgvData, dialog.FileName);
                        MessageBox.Show(" Export data to CSV Successfully.", "Info.", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.tslMain.Text = "Export data to CSV file successfully.";
                        this.Cursor = Cursors.Default;
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        DataTable dtCSV;

        /// <summary>
        /// Save a new las.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton5_Click(object sender, EventArgs e)
        {

            if (this.saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("You hav't choose a las file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string lasFile = this.saveFileDialog.FileName;

            // Todo, Set header from the dgvinfo. 
            LasWriter lasWriter = new LasWriter(newHeader);
            if (!lasWriter.OpenWriter(lasFile))
            {
                MessageBox.Show(lasWriter.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lasWriter.CloseWriter();
                return;
            }

            if (!lasWriter.WriteHeader())
            {
                MessageBox.Show(lasWriter.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lasWriter.CloseWriter();
                return;
            }

            LasPoint p = new LasPoint();
            LasHeader.Instance = newHeader;  // Set new header to LasHeader, which can be seemed as a globe.

            // Write point
            foreach (DataRow row in dtCSV.Rows)
            {
                p.GeoX = double.Parse(row["X"].ToString());
                p.GeoY = double.Parse(row["Y"].ToString());
                p.GeoZ = double.Parse(row["Z"].ToString());
                p.intensity = ushort.Parse(row["I"].ToString());
                p.red = ushort.Parse(row["R"].ToString());
                p.green = ushort.Parse(row["G"].ToString());
                p.blue = ushort.Parse(row["B"].ToString());

                lasWriter.WritePoint(p);
            }

            lasWriter.CloseWriter();
            MessageBox.Show("Create a las file successfully.");

        }
    }
}
