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
using System.Xml;


namespace AutoMEK
{

    public partial class Form1 : Form
    {

        public DirectoryInfo di_pack = new DirectoryInfo(@".\Incoming");
        public DirectoryInfo di_packed = new DirectoryInfo(@".\Outgoing\");
        class ZGLV
        {
            public string VERSION,  FILENAME, SD_Z, TEST, VER_PO;

            private string _DATA;

           
            
            public string DATA
            { get
                {
                    return DATA.ToString();
                }
                set
                {
                    try
                    {
                        DATA = value;
                    }
                    catch
                    {
                        DATA = "!!!!!";
                        //Здесь должно добавляться в протокол ошибок
                    }
                   
                    
                }
            
            }
            




            /* public ZGLV(string version, string data, string filename,string sd_z, string test, string ver_po)
             {
                 VERSION = version;
                 DATA= data; 
                 FILENAME= filename; 
                 SD_Z = sd_z;  
                 TEST = test;
                 VER_PO = ver_po;
             }
            */
            public override string ToString()
            {
                return FILENAME;
            }

            public void SetField(string name, string val)
            {
                var field = typeof(ZGLV).GetField(name);
                field.SetValue(this, val);
            }
        }


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



            if (!di_pack.Exists)
            { di_pack.Create(); }

            if (!di_packed.Exists)
            { di_packed.Create(); }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Goer(listBox1);
        }

        private async void Goer(ListBox listBox)
        {
            int i = 0;
            string str=null;
            foreach (FileInfo findedFile in di_pack.GetFiles())
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(findedFile.FullName);

                XmlElement xRoot = xDoc.DocumentElement;
 

        
              


                foreach (XmlNode xnode_1 in xRoot.SelectNodes("ZGLV"))
                {
                    ZGLV zerg = new ZGLV();

                    foreach (XmlNode xnode_2 in xnode_1)
                    {
                       // listBox.Items.Add(xnode_2.Name + xnode_2.InnerText);
                        zerg.SetField(xnode_2.Name.ToUpper(), xnode_2.InnerText);
                      //    str=(xnode_1.Name + " -=- " + xnode_2.Name);
                        //listBox.Items.Add(str);
                       
                    }

                    /*XmlNode attr = xnode.Attributes.GetNamedItem("Version");
                    if (attr != null)
                    { listBox1.Items.Add(attr.Value); } else 
                    {
                       
                        
                        listBox1.Items.Add(i.ToString());
                        i++;
                    };*/

                      listBox.Items.Add(zerg.FILENAME);
                      listBox.Items.Add(zerg.DATA);

                }
                
                
            }

        }
    }
}
