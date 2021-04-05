using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;



namespace AutoMEK
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
            


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String connectionString = "Server=192.168.2.157;Port=5432;Username=postgresql;Password=sa1512;Database=pg_misc;";
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionString);
            try
            { npgSqlConnection.Open(); }
            catch
            {
                this.Text += ".... Ошибка подключения к базе!";
            }
            finally
            {
                this.Text += ".... Соединение установлено!";
               
            }

            
            DirectoryInfo di_pack = new DirectoryInfo(@".\Incoming");
            DirectoryInfo di_packed = new DirectoryInfo(@".\Outgoing\");
            if (!di_pack.Exists)
            { di_pack.Create(); }

            if (!di_packed.Exists)
            { di_packed.Create(); }
        }
    }
}
