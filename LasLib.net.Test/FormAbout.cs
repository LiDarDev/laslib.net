using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LasLibNet.Test
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = @" This is free software, written by Li G.Q. from Central southern university."
                + "The author's email is Ligq168@csu.edu.cn. You can redistribute and/or modify it under the"
                +" terms of the GNU Lesser General Licence as published by the Free Software"
                +" Foundation. See the COPYING file for more information.\n\r"
                +" This software is distributed WITHOUT ANY WARRANTY and without even the"
                +" implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/LiDarDev/laslib.net");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
