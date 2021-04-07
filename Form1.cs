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
                        Logg =Logger ("Файл со случаями для файла " + FName1 + " найден!", listBox);
                        succ = true;
                    }
                    catch
                    {
                        Logg = Logger("Файл со случаями для файла " + FName1 + " не найден!", listBox);
                    }

                    if (succ)  //Загружаем XML если нашли НМ файл
                    {
                        XmlDocument xDoc_HM = new XmlDocument();
                        xDoc_HM.Load(st.FullName);
                        XmlElement xRoot_HM = xDoc_HM.DocumentElement;
                        foreach (XmlNode xnode_1_HM in xRoot_HM.SelectNodes("ZGLV"))
                        {
                            foreach (XmlNode xnode_2_HM in xnode_1_HM)
                            {

                                listBox.Items.Add(xnode_2_HM.Name + xnode_2_HM.InnerText);

                                //    str=(xnode_1.Name + " -=- " + xnode_2.Name);
                                //listBox.Items.Add(str);

                            }



                        }
                        }












                    foreach (XmlNode xnode_2 in xnode_1)
                    {

                       listBox.Items.Add(xnode_2.Name + xnode_2.InnerText);
                     
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


                    MessageBox.Show(Logg);
                }
                
                
            }

        }
    }
}
