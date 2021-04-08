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
        public string FName;
        public string FName1;
        public List<string> mf003;

        public Form1()
        {
            InitializeComponent();



        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String connectionString = "Server=192.168.2.157;Port=5432;Username=postgresql;Password=sa1512;Database=pg_misc;";
            NpgsqlConnection npgSqlConnection= new NpgsqlConnection(connectionString);
            
            String connectionString_1 = "Server=192.168.2.155;Port=5432;Username=fuser;Password=6PJyRMLH#Sf@tQLL9Sc@; Database =foms;";
            NpgsqlConnection npgSqlConnection_1= new NpgsqlConnection(connectionString_1);



            try
            { npgSqlConnection.Open(); 
             npgSqlConnection_1.Open(); }
            catch
            {
                this.Text += ".... Ошибка подключения к базе!";
            }
            finally
            {
                this.Text += ".... Соединение установлено!";

            }

            string query_1= "select code from foms.f003 where substring(code,1,2)='15' and coalesce(dateend,'2200-01-01')>'2021-01-01'";
            
            NpgsqlCommand cmd_1 = new NpgsqlCommand(query_1, npgSqlConnection_1);
            NpgsqlDataReader f003 = cmd_1.ExecuteReader();
            mf003 = new List<string>();
            if (f003.HasRows)
            {
                while (f003.Read())
                {
                    
                    mf003.Add(f003[0].ToString());
                }
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


        public string Logger(string input ,  ListBox lb)
        {
            lb.Items.Add(input);
            return  input + "\r\n";
            
        }



        private  void Goer(ListBox listBox)
        {
            
            string Logg;
            foreach (FileInfo findedFile in di_pack.GetFiles("L*.xml"))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(findedFile.FullName);


                XmlElement xRoot = xDoc.DocumentElement;
 
                foreach (XmlNode xnode_1 in xRoot.SelectNodes("ZGLV"))
                {
                    bool succ = false;
                    FileInfo st=null;
                    FName1 = xnode_1["FILENAME1"].InnerText;


                    try   ///ищем HM файл.
                    {
                        st = di_pack.GetFiles(FName1 + ".xml")[0];
                        Logg =Logger ("Файл "+FName1+" со случаями для файла " + findedFile.Name + " найден!", listBox);
                        succ = true;
                    }
                    catch
                    {
                        Logg = Logger("Файл" + FName1 + "  со случаями для файла " + FName1 + " не найден!", listBox);
                    }

                    if (succ)  //Загружаем XML если нашли НМ файл
                    {
                        XmlDocument xDoc_HM = new XmlDocument();
                        xDoc_HM.Load(st.FullName);
                        XmlElement xRoot_HM = xDoc_HM.DocumentElement;

                        foreach (XmlNode xnode_1_HM in xRoot_HM.SelectNodes("SCHET"))
                        {
                        
                              
                               

                                  if (mf003.FindIndex(s => s== xnode_1_HM["CODE_MO"].InnerText) <1)

                                   {
                                        Logg = Logger("001F.00.0030 ОШИБКА!", listBox);
                                   }
                                  
                                
                             

                            



                        }
                        }












                    foreach (XmlNode xnode_2 in xnode_1)
                    {

                     //  listBox.Items.Add(xnode_2.Name + xnode_2.InnerText);
                     
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


                    //MessageBox.Show(Logg);
                }
                
                
            }

        }
    }
}
