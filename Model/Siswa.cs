using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DBModel
{
    class Siswa : Model
    {
        public string Table = "siswa";
        public string PrimaryKey = "no_induk";
        public string Database = ModelConfiguration.DefaultDatabase;
        public List<string> Columns = new List<string>() { "no_induk", "nama", "jk", "alamat", "kelas" };

        public Siswa(object[] data) : base(data)
        {

        }
        public Siswa() { }

        public override string table { get { return this.Table; } set { this.Table = value; } }
        public override string pk { get { return this.PrimaryKey; } set { this.PrimaryKey = value; } }
        public override string database { get { return this.Database; } set { this.Database = value; } }
        public override List<string> columns { get { return this.Columns; } set { this.Columns = value; } }
    }

}
