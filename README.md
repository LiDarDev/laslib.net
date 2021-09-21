# LasLibNet
* The LasLibNet is a library coded ONLY by C#, and used to read or write the las/laz file.  The author is a teacher and a developer in GIS department of **Central Southen University**, Hunan province, China. If you any question, you can send email to Ligq168@csu.edu.cn.
* LasLibNet库是用C#编写的、用于读取或写入las/laz文件的.net 库， 作者是**中南大学地理信息系**的老师，如果你有任何问题，可以发邮件到Ligq168@csu.edu.cn, 或者加QQ41733233.

## Usage
### 1. How to read a las file? 
    // Create a las reader instance
    LasReader lasReader = new LasReader();
    // Declare a las header variable to get the header of a las file
    LasHeader lasHeader;
    
    // Open a las file
    if(lasReader.OpenReader(@"d:\sample_data\sample.las"))
    {
      lasHeader = lasReader.Header;
      // Toto about the header or point data.
    }  
   
### 2. How to traverse all point data? 

    // Create a point reader
    lasReader.CreatePointReader();
            
    // Loop through number of points indicated
     for (int pointIndex = 0; pointIndex < lasHeader.number_of_point_records; pointIndex++)
      {
          // Read the point
          LasPoint p = lasReader.ReadPoint();
          if (p == null)
          {
              MessageBox.Show(lasReader.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
              break;
          }
          // Todo about the las point.
     }
     
### 3. How to get different VERSION and FORMAT point data?
    If you want know about the las file format, you can access https://en.wikipedia.org/wiki/LAS_file_format.
    In the LasLibNet lib, some las point MODEL have been defined. For example:
    
    Model.LasPoint_1_2_Format3 point_1_2_3 = p.ToPoint_1_2_Format3();  // The p is the variable defined in pre step.
    
    Yet, if you want to convert the special version and format point MODEL into LasPoint MODEL. You can:
    LasPoint p = new LasPoint(); 
    p.FromPoint_1_2_Format3(point_1_2_3);  // The point_1_2_3 is the a variable of the model LasPoint_1_2_Format3.
    
### 4. How to create a new las file?
    - Create and set the las header setting. For example:
        LasHeader header = LasHeader.Instance;
        header.Init();  //Set all parameters to the configuration of R1.2 and format3 Las Point.
      
    - Read all point data, and calculat the max and min value for x/y/z, and set these values to header variable. 
      Get number of points, and set the number_of_point_records. For example:
         header.number_of_point_records = points_count;
         header.max_x = max_x;
         header.min_x = min_x;
         header.max_y = max_y;
         header.min_y = min_y;
         header.max_z = max_z;
         header.min_z = min_z;
             
    - Create LasWriter instance, open the writer, and write the header. Codes are following:
            LasWriter lasWriter = new LasWriter(newHeader);
            if (!lasWriter.OpenWriter(lasFile))   //the lasFile is the las file path wanted to be create.
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
            
    - Write point data. For example:
           foreach (DataRow row in dtCSV.Rows)
           {
             LasPoint p = new LasPoint();
             p.GeoX = double.Parse(row["X"].ToString());
             p.GeoY = double.Parse(row["Y"].ToString());
             p.GeoZ = double.Parse(row["Z"].ToString());
             p.intensity = ushort.Parse(row["I"].ToString());
             p.red = ushort.Parse(row["R"].ToString());
             p.green = ushort.Parse(row["G"].ToString());
             p.blue = ushort.Parse(row["B"].ToString());
           }
           
    - Close writer.
           lasWriter.CloseWriter();
   
## Example
   ![image](https://github.com/LiDarDev/laslib.net/blob/main/Images/GUI.jpg)
