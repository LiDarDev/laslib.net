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
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            lasHeader = this.lasReader.header;

            #region 创建数据表
            DataTable dt = new DataTable();

            DataColumn colKey = new DataColumn( "colKey", typeof(string));
            colKey.Caption = "  Key";
            dt.Columns.Add(colKey);
            DataColumn colVal = new DataColumn("colVal", typeof(string));
            colVal.Caption = "   Value ";
            dt.Columns.Add(colVal);
            #endregion

            #region 创建数据行
            //新建行
            DataRow row = dt.NewRow();
            row[colKey] = "Version";
            row[colVal] = lasHeader.version_major+"."+lasHeader.version_minor;
            //添加行
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[colKey] = "Header Size";
            row[colVal] = lasHeader.header_size.ToString();
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "Point Count";
            row[1] = lasHeader.number_of_point_records.ToString();
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "X Max";
            row[1] = lasHeader.max_x.ToString("F4");
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "X Min";
            row[1] = lasHeader.min_x.ToString("F4");
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "Y Max";
            row[1] = lasHeader.max_y.ToString("F4");
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "Y Min";
            row[1] = lasHeader.min_y.ToString("F4");
            dt.Rows.Add(row);
            #endregion

            this.dgvInfo.DataSource = dt;
        }


        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            #region Create DataTable
            DataTable dt = new DataTable();
            DataColumn colId = new DataColumn("colId", typeof(string));
            dt.Columns.Add(colId);
            DataColumn colX = new DataColumn("colX", typeof(string));
            dt.Columns.Add(colX);
            DataColumn colY = new DataColumn("colY", typeof(string));
            dt.Columns.Add(colY);
            DataColumn colZ = new DataColumn("colZ", typeof(string));
            dt.Columns.Add(colZ);
            DataColumn colI = new DataColumn("colI", typeof(string));
            dt.Columns.Add(colI);
            DataColumn colR = new DataColumn("R", typeof(string));
            dt.Columns.Add(colR);
            DataColumn colB = new DataColumn("colB", typeof(string));
            dt.Columns.Add(colB);
            DataColumn colG = new DataColumn("colG", typeof(string));
            dt.Columns.Add(colG);
            #endregion

            #region Add Data Row
            // Loop through number of points indicated
            for (int pointIndex = 0; pointIndex < this.lasHeader.number_of_point_records; pointIndex++)
            {
                // Read the point
                lasReader.read_point();
                LasPoint p = lasReader.GetPointPointer();
                DataRow row = dt.NewRow();
                row[colX] = p.X.ToString("f4");
                row[colY] = p.Y.ToString("f4");
                row[colZ] = p.Z.ToString("f4");
                dt.Rows.Add(row);
            }
            #endregion

            this.dgvData.DataSource = dt;
        }
    }
}
