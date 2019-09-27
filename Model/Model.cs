//DATABASE MODEL V 0.1  MADE BY KEVIN AS

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DBModel
{
    public abstract class Model
    {

        abstract public string database { get; set; }

        public object[] data { get; set; }

        public MySqlConnection dbConnection { get; set; }

        abstract public string table { get; set; } //table name

        abstract public string pk { get; set; } //primary key

        public bool init = false;

        public MySqlCommand cmd;

        abstract public List<string> columns { get; set; }

        public static string testt = "kevin";

        public string this[string key]
        {
            get
            {
                int index = this.columns.IndexOf(key);
                return Convert.ToString(this.data[index]);
            }
            set
            {
                int index = this.columns.IndexOf(key);
                this.data[index] = value;
            }

        }


        public List<string> getColumns()
        {
            string get_column_names_query = String.Format(@"SELECT `COLUMN_NAME` 
                                              FROM `INFORMATION_SCHEMA`.`COLUMNS` 
                                              WHERE `TABLE_SCHEMA`='sekolah' 
                                              AND `TABLE_NAME`='{0}';", this.table);
            Console.WriteLine(get_column_names_query);
            List<string> col = new List<string>();
            foreach (object o in this.Query(get_column_names_query)[0])
            {
                col.Add(o.ToString());
            }
            return col;
        }

        public List<object[]> Query(string query)
        {
            dbConnection = new MySqlConnection(database);
            dbConnection.Open();

            List<object[]> data = new List<object[]>();
            cmd = new MySqlCommand(query, dbConnection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);
                data.Add(values);
            }
            dbConnection.Close();
            return data;
        }

        public DataTable DtQuery(string query)
        {
            dbConnection = new MySqlConnection(database);
            dbConnection.Open();

            cmd = new MySqlCommand(query, dbConnection);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            cmd.ExecuteNonQuery();
            DataTable dt = new DataTable();
            adp.Fill(dt);
            dbConnection.Close();
            return dt;
        }

        public MySqlDataAdapter DAQuery(string query)
        {
            dbConnection = new MySqlConnection(database);
            dbConnection.Open();

            cmd = new MySqlCommand(query, dbConnection);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            return adp;
        }

        public MySqlDataAdapter DAAll()
        {
            string query = String.Format("select * from {0}", this.table);
            return this.DAQuery(query);
        }

        public void QueryNo(string query)
        {
            dbConnection = new MySqlConnection(database);
            dbConnection.Open(); ;
            cmd = new MySqlCommand(query, dbConnection);
            cmd.ExecuteNonQuery();
        }

        public Model(object[] d)
        {
            this.data = d;
        }
        public Model(Dictionary<string, string> data)
        {

        }
        public Model() { }


        public Model Find(string val)
        {
            string query = String.Format("select * from {0} where `{1}` = '{2}';", table, pk, val);
            Console.WriteLine(query);
            object[] d = this.Query(query)[0];
            Model ToRet = this;
            ToRet.data = d;
            return ToRet;
        }



        public List<object[]> All()
        {
            List<object[]> data = new List<object[]>();

            foreach (object[] dat in this.Query(String.Format("select * from {0};", table)))
            {
                data.Add(dat);
            }
            return data;
        }

        public DataTable DtAll()
        {
            return DtQuery(String.Format("select * from {0};", table));
        }




        public List<object[]> Where(Dictionary<string, string> args)
        {
            List<object[]> data = new List<object[]>();
            string selector = "where ";
            List<string> keys = new List<string>(args.Keys.AsEnumerable());
            List<string> values = new List<string>(args.Values.AsEnumerable());
            for (int i = 0; i < args.Count - 1; i++)
            {
                selector += String.Format("`{0}`='{1}', ", keys[i], values[i]);
            }
            selector += String.Format("`{0}`='{1}'", keys[args.Count - 1], values[args.Count - 1]);
            string query = String.Format("select * from {0} " + selector, table);
            //Console.WriteLine(query);
            foreach (object[] dat in this.Query(query))
            {
                data.Add(dat);
            }
            return data;
        }

        public void delete(string pk)
        {
            this.QueryNo(String.Format("delete from `{0}` where `{1}`='{2}'", table, this.pk, pk));
        }

        public bool delete()
        {
            if (this.Query(String.Format("select * from `{0}` where `{1}`='{2};'", table, pk, this[pk])).Count == 1)
            {
                this.delete((string)this[pk]);
                return true;
            }
            return false;
        }



        public bool save()
        {
            //if true, then its exist and updated. otherwise false

            //search the data first, if exist then update it.
            List<object[]> dat = this.Query(String.Format("select * from {0} where `{1}` = '{2}';", table, pk, this[pk]));
            if (dat.Count == 0)
            {
                string columnsStr = "";
                string dataStr = "";
                for (int i = 0; i < columns.Count - 1; i++)
                {
                    columnsStr += String.Format("`{0}`, ", this.columns[i]);
                }
                columnsStr += String.Format("`{0}`", this.columns[columns.Count - 1]);


                for (int i = 0; i < columns.Count() - 1; i++)
                {
                    dataStr += String.Format("'{0}', ", this[columns[i]]);
                }

                dataStr += String.Format("'{0}' ", this[columns[columns.Count() - 1]]);

                string query = String.Format("insert into {0}({1}) values({2})", table, columnsStr, dataStr);
                this.QueryNo(query);
                Console.WriteLine(query);
                return true;

            }
            else
            {
                string query = "update {0} set ";
                for (int i = 0; i < columns.Count - 1; i++)
                {
                    query += String.Format("`{0}`='{1}', ", columns[i], this[columns[i]]);
                }
                int ii = columns.Count - 1;
                query += String.Format("`{0}`='{1}' ", columns[ii], this[columns[ii]]);
                query += String.Format(" where `{0}` = '{1}'", pk, this[pk]);
                query = String.Format(query, table);
                Console.WriteLine(query);
                this.QueryNo(query);
                return false;
            }
        }

    }
}
