using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace LasLibNet.Test
{
    public class CSVHelper
    {       

        /// <summary>
        /// Read a csv and return datatable.
        /// </summary>
        /// <param name="path"> CSV file path</param>
        /// <returns></returns>
        public static DataTable ReadData(string path)
        {
            DataTable csvData = new DataTable();

            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();

                    foreach (string column in colFields)
                    {
                        DataColumn col = new DataColumn(column);
                        col.AllowDBNull = true;
                        csvData.Columns.Add(col);
                    }

                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return csvData;
        }

        /// <summary>
        /// Save all data in a datagridview control to a csv.
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Export(DataGridView dgv,string path)
        {
            try
            {
                StreamWriter csvFileWriter = new StreamWriter(path, false);

                var sb = new StringBuilder();
                var headers = dgv.Columns.Cast<DataGridViewColumn>();
                sb.AppendLine(string.Join(",", headers.Select(column => "\"" + column.HeaderText + "\"").ToArray()));
                csvFileWriter.WriteLine(sb);

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    var cells = row.Cells.Cast<DataGridViewCell>();
                    csvFileWriter.WriteLine(string.Join(",", cells.Select(cell => "\"" + cell.Value + "\"").ToArray()));
                }

                csvFileWriter.Flush();
                csvFileWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
