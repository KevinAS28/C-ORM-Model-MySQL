using System;
using System.Collections.Generic;
using System.Text;

namespace DBModel
{
    public class ModelConfiguration
    {
        public static string DatabaseName = "sekolah";
        public static string Host = "127.0.0.1";
        public static string Username = "root";
        public static string Password = "";

        static public string DefaultDatabase { get { return String.Format("server = '{0}'; uid = '{1}'; database='{2}'; pwd='{3}';", Host, Username, DatabaseName, Password); } }
    }
}