using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LasLibNet;

namespace LasLibNet.Utils
{
    public static class Tools
    {
        private static string modelName;
        /// <summary>
        /// Enumerate all properties within a class. 
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="model">Object</param>
        public static void DisplayClassProperties<T>(T model)
        {
            Type t = model.GetType();
            modelName=t.ToString();

            PropertyInfo[] PropertyList = t.GetProperties();
            foreach (PropertyInfo item in PropertyList)
            {
                string name = item.Name;
                object value = item.GetValue(model, null);

                DisplayValue(name, value);
            }
        }
        /// <summary>
        /// Display the variable name and its value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void DisplayValue(string name, object value)
        {
            if (value == null) return;
            Type t = value.GetType();
            if (t.Equals(typeof(byte)))
                Debug.WriteLine(modelName+".{0} = {1}", name, ((byte)value).ToString("x2"));
            else if (t.Equals(typeof(byte[])))
                Debug.WriteLine(modelName+".{0} = {1}", name, BitConverter.ToString((byte[])value));
            else if (t.Equals(typeof(UInt32[])))
            {
                Debug.WriteLine(modelName+".{0} = {1}", name, array_string((UInt32[])value));
            }
            else if (t.Equals(typeof(UInt64[])))
            {
                Debug.WriteLine(modelName+".{0} = {1}", name, array_string((UInt64[])value));
            }
            else if (t.Equals(typeof(UInt16[])))
            {
                Debug.WriteLine(modelName + ".{0} = {1}", name, array_string((UInt16[])value));
            }
            else if (t.Equals(typeof(int)) || t.Equals(typeof(long))
                || t.Equals(typeof(short)) || t.Equals(typeof(uint)) || t.Equals(typeof(UInt32))
                || t.Equals(typeof(ushort)) || t.Equals(typeof(ulong)))
                Debug.WriteLine(modelName+".{0} = {1}", name, value);
            else if (t.Equals(typeof(double)) || t.Equals(typeof(Single)))
                Debug.WriteLine(modelName+".{0} = {1}", name, ((double)value).ToString("f6"));
            else if (t.Equals(typeof(List<LasVLR>)))
                Debug.WriteLine(modelName+".{0} = {1}", name, list_string((List<LasVLR>)value));
            else if (t.Equals(typeof(string)))
                Debug.WriteLine(modelName+".{0} = \"{1}\"", name, value);
            else
                Console.WriteLine(modelName+".{0} = {1}", name, value.ToString());
        }

        private static string array_string(UInt16[] arr)
        {
            string str = "";
            foreach (UInt16 b in arr)
                str += (str == "" ? "" : ",") + b.ToString();
            return "{" + str + "}";
        }
        private static string array_string(UInt32[] arr)
        {
            string str = "";
            foreach (UInt32 b in arr)
                str += (str == "" ? "" : ",") + b.ToString();
            return "{" + str + "}";
        }
        private static string array_string(UInt64[] arr)
        {
            string str = "";
            foreach (UInt64 b in arr)
                str += (str == "" ? "" : ",") + b.ToString();
            return "{" + str + "}";
        }

        private static string list_string(List<LasVLR> vlrs)
        {
            string str = "";
            foreach (LasVLR vlr in vlrs)
            {
                str += (str == "" ? "" : ",") + "{" + vlr.ToString() + "}";
            }
            return "[" + str + "]";
        }
    }
}
