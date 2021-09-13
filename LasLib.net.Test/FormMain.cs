using LasLibNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LasLib.net.Test
{
    public partial class FormMain : Form
    {
        string lasFile = "";

        LasReader lasReader = new LasReader();
        LasHeader lasHeader;

        bool isCompressed = true;

        public FormMain()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.tslFile.Text = this.openFileDialog.FileName;
                this.lasFile = this.tslFile.Text;
                lasReader.OpenReader(this.lasFile, ref this.isCompressed);

                this.toolStripButton2_Click(null, null);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            lasHeader = this.lasReader.header;
            
            #region 创建数据行
                     
            this.dgvInfo.Rows.Add("Version",lasHeader.version_major+"."+lasHeader.version_minor);
            this.dgvInfo.Rows.Add("Header Size",lasHeader.header_size.ToString());
            this.dgvInfo.Rows.Add("Point Count",lasHeader.number_of_point_records.ToString());
            this.dgvInfo.Rows.Add("X Max",lasHeader.max_x.ToString("f4"));
            this.dgvInfo.Rows.Add("X Min", lasHeader.min_x.ToString("f4"));
            this.dgvInfo.Rows.Add("Y Max",lasHeader.max_y.ToString("f4"));
            this.dgvInfo.Rows.Add("Y Min",lasHeader.min_y.ToString("f4"));
            #endregion

        }


        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            this.tsProgressbar.Value = 0;
            this.tsProgressbar.Maximum = (int)this.lasHeader.number_of_point_records;

            #region Add Data Row
            // Loop through number of points indicated
            for (int pointIndex = 0; pointIndex < this.lasHeader.number_of_point_records; pointIndex++)
            {
                // Read the point
                lasReader.read_point();
                LasPoint p = lasReader.GetPointPointer();
               
                this.dgvData.Rows.Add(pointIndex.ToString()
                    ,p.X.ToString("f4")
                    ,p.Y.ToString("f4")
                    ,p.Z.ToString("f4")
                    ,p.intensity.ToString("f4")
                    ,p.rgb[0].ToString()
                    ,p.rgb[1].ToString()
                    ,p.rgb[2].ToString());

                this.tsProgressbar.Value++;
                //this.statusStrip1.Refresh();
            }
            #endregion

            this.tsProgressbar.Value = this.tsProgressbar.Maximum;

        }
    }
}
