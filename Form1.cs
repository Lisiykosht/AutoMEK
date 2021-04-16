using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Npgsql;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;
using System.Xml;



namespace AutoMEK
{

    public partial class Form1 : Form
    {

        public DirectoryInfo di_pack = new DirectoryInfo(@".\Incoming");
        public DirectoryInfo di_packed = new DirectoryInfo(@".\Outgoing\");
        public string FName;
        public string FName1;
        public int ThreadCount;
         List<Tuple<string , DateTime , DateTime >> mf003;
         List<Tuple<int,double,DateTime , DateTime >> mkslp;
         List<Tuple<string ,  DateTime >> mmkb;
        public int N_ZAP;
        public Form1()
        {
            InitializeComponent();



        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /*String connectionString = "Server=192.168.2.157;Port=5432;Username=postgresql;Password=sa1512;Database=pg_misc;";
            NpgsqlConnection npgSqlConnection= new NpgsqlConnection(connectionString);
            */
            String connectionString_1 = "Server=192.168.2.155;Port=5432;Username=fuser;Password=6PJyRMLH#Sf@tQLL9Sc@; Database =foms;";
            NpgsqlConnection npgSqlConnection_1= new NpgsqlConnection(connectionString_1);



            try
            {// npgSqlConnection.Open(); 
             npgSqlConnection_1.Open(); }
            catch
            {
                this.Text += ".... Ошибка подключения к базе!";
            }
            finally
            {
                this.Text += ".... Соединение установлено!";

            }

            string query_1= "select code, datebeg, coalesce(dateend,'2200-01-01') dateend from foms.f003 where substring(code,1,2)='15' and coalesce(dateend,'2200-01-01')>'2021-01-01'";
            string query_2= "select code,  coalesce(date_end,'2200-01-01') date_end from foms.mkb where deleted=false";
            string query_3= "select idsl,zkoef,datebeg,  coalesce(datebeg,'2021-01-01') date_end from public.k_kslp";
            
            NpgsqlCommand cmd_1 = new NpgsqlCommand(query_1, npgSqlConnection_1);
            NpgsqlCommand cmd_2 = new NpgsqlCommand(query_2, npgSqlConnection_1);
            NpgsqlCommand cmd_3 = new NpgsqlCommand(query_3, npgSqlConnection_1);

            NpgsqlDataReader f003 = cmd_1.ExecuteReader();
            
           

            mf003 = new List<Tuple<string, DateTime, DateTime>>();
            mkslp = new List<Tuple<int, double, DateTime, DateTime>>();
            mmkb = new List<Tuple<string, DateTime>>();
            

            if (f003.HasRows)
            {
                while (f003.Read())
                {
                    mf003.Add(Tuple.Create( f003[0].ToString()  , Convert.ToDateTime(f003[1].ToString()) , Convert.ToDateTime(f003[2].ToString()) ));
                }
            }
            f003.Close();


            NpgsqlDataReader mkb = cmd_2.ExecuteReader();

            if (mkb.HasRows)
            {
                while (mkb.Read())
                {
                  mmkb.Add(Tuple.Create( mkb[0].ToString()  ,mkb.GetDateTime(1)));
                }
            }
            mkb.Close();
            NpgsqlDataReader kslp = cmd_3.ExecuteReader();

            if (kslp.HasRows)
            {
                while (kslp.Read())
                {
                    mkslp.Add(Tuple.Create(kslp.GetInt32(0), kslp.GetDouble(1), kslp.GetDateTime(2),kslp.GetDateTime(3)));
                }
            }


            if (!di_pack.Exists)
            { di_pack.Create(); }

            if (!di_packed.Exists)
            { di_packed.Create(); }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread myThread = new Thread(new ParameterizedThreadStart(Goer));
            myThread.Start(listBox1);
        }


        public string Logger(string input ,  ListBox lb)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new Action(() => lb.Items.Add(DateTime.Now + " ---->" + input)));
                
                lb.Invoke(new Action(() => lb.TopIndex = Math.Max(lb.Items.Count - 1, 0)));

            }
            return   input + "\r\n";
            
        }



        public  void Goer(object olistBox)
        {
            ListBox listBox = (ListBox)olistBox;
            ThreadCount++;
            string Logg;
            foreach (FileInfo findedFile in di_pack.GetFiles("L*.xml"))
            {
                XDocument xDoc =  XDocument.Load(findedFile.FullName);



                XElement xRoot = xDoc.Element("PERS_LIST");

              //  MessageBox.Show(xRoot.ToString());
                   
               foreach (XElement xnode_1 in xRoot.Elements("ZGLV"))
                {
                    bool succ = false;
                    FileInfo st=null;
                    FName1 = xnode_1.Element("FILENAME1").Value;


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
                        XDocument xDoc_HM =  XDocument.Load(st.FullName, LoadOptions.SetLineInfo);
                        XElement xRoot_HM = xDoc_HM.Element("ZL_LIST");
                      
                        
                        foreach (XElement xnode_1_HM in xRoot_HM.Elements("SCHET"))
                        {
                             if (mf003.FindIndex(s => s.Item1== xnode_1_HM.Element("CODE_MO").Value &&  s.Item2 < Convert.ToDateTime(xnode_1_HM.Element("DSCHET").Value) &&  s.Item3 > Convert.ToDateTime(xnode_1_HM.Element("DSCHET").Value)) <1)
                              {
                                            Logg = Logger("001F.00.0030  -  [CODE_MO] Организация "+ xnode_1_HM.Element("CODE_MO").Value + " не найдена в справочнике F003  ", listBox);
                              }
                            
                             //Logg = Logger(mf003[0].Item1+" - "+mf003[0].Item2 + " - " + mf003[0].Item3 , listBox);
                            //listBox.Items.Add(xnode_1_HM["CODE_MO"].InnerText + " " + Convert.ToDateTime(xnode_1_HM["DSCHET"].InnerText));
                            
                        }
                        int xnode_1_HM_ZAP_row = 0;

                        foreach (XElement xnode_1_HM_ZAP in xRoot_HM.Elements("ZAP"))
                        {
                            N_ZAP = 0;
                            if (xnode_1_HM_ZAP.Element("N_ZAP") != null)
                            {
                                if (!Int32.TryParse(xnode_1_HM_ZAP.Element("N_ZAP").Value, out N_ZAP) | xnode_1_HM_ZAP.Element("N_ZAP").Value.Length > 9)
                                {
                                    Logg = Logger("004F.00.0190 - [N_ZAP] Поле N_ZAP  содержит недопустимое значение ["+ xnode_1_HM_ZAP.Element("N_ZAP").Value + "] в ZAP №  " + xnode_1_HM_ZAP_row, listBox);
                                }

                            }
                            else 
                            {
                                 
                                Logg = Logger("003F.00.2110 - [N_ZAP] Пропущено обязательное поле N_ZAP в ZAP № " + xnode_1_HM_ZAP_row +" строка ("+ ((IXmlLineInfo)xnode_1_HM_ZAP).LineNumber + ")", listBox);
                            };

                            foreach (XElement xnode_1_HM_SLUCH in xnode_1_HM_ZAP.Elements("SLUCH"))
                            {
                                if (xnode_1_HM_SLUCH.Element("DS1").Value != null)
                                {
                                    
                               if (mmkb.FindIndex(s => s.Item1 == xnode_1_HM_SLUCH.Element("DS1").Value && s.Item2 >= Convert.ToDateTime(xnode_1_HM_SLUCH.Element("DATE_2").Value) ) < 1)
                               {
                                        Logg = Logger("005F.00.0040  - N_ZAP " + N_ZAP +"  [DS1] Диагноз ["+ xnode_1_HM_SLUCH.Element("DS1").Value + "] не найден в справочнике MKB-10", listBox);
                               }
                                    if (xnode_1_HM_SLUCH.Element("DS2") != null)
                                    {
                                        if ((xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS2").Value))// || (xnode_1_HM_SLUCH.Element("DS3") != null) && (xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS3").Value))
                                                                             Logg = Logger("006F.00.0430  - N_ZAP " + N_ZAP + " [DS1 -- DS2] Диагноз " + xnode_1_HM_SLUCH.Element("DS1").Value + " не должен равняться DS2", listBox);

                                        if (mmkb.FindIndex(s => s.Item1 == xnode_1_HM_SLUCH.Element("DS2").Value && s.Item2 >= Convert.ToDateTime(xnode_1_HM_SLUCH.Element("DATE_2").Value)) < 1)
                                            Logg = Logger("005F.00.0050 - N_ZAP " + N_ZAP + " [DS2] Диагноз [" + xnode_1_HM_SLUCH.Element("DS2").Value + "] не найден в справочнике MKB-10", listBox);

                                        if ((xnode_1_HM_SLUCH.Element("DS3") != null) && (xnode_1_HM_SLUCH.Element("DS2").Value == xnode_1_HM_SLUCH.Element("DS3").Value))// ||  (xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS3").Value))
                                            Logg = Logger("006F.00.0440  - N_ZAP " + N_ZAP + " [DS2 -- DS3] Диагноз " + xnode_1_HM_SLUCH.Element("DS2").Value + " не должен равняться DS3", listBox);


                                    }

                                        


                                    if (xnode_1_HM_SLUCH.Element("DS3") != null) 
                                    {
                                        if ((xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS3").Value))// || (xnode_1_HM_SLUCH.Element("DS3") != null) && (xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS3").Value))
                                            Logg = Logger("006F.00.0430  - N_ZAP " + N_ZAP + " [DS1 -- DS3] Диагноз " + xnode_1_HM_SLUCH.Element("DS1").Value + " не должен равняться DS3", listBox);

                                        if (mmkb.FindIndex(s => s.Item1 == xnode_1_HM_SLUCH.Element("DS3").Value && s.Item2 >= Convert.ToDateTime(xnode_1_HM_SLUCH.Element("DATE_2").Value)) < 1)
                                            Logg = Logger("005F.00.0060 - N_ZAP " + N_ZAP + " [DS3] Диагноз [" + xnode_1_HM_SLUCH.Element("DS3").Value + "] не найден в справочнике MKB-10", listBox);

                                       
                                    }


                                    if (xnode_1_HM_SLUCH.Element("KSG_KPG")!= null && xnode_1_HM_SLUCH.Element("KSG_KPG").Element("IT_SL") != null)
                                    {
                                        if(double.TryParse(xnode_1_HM_SLUCH.Element("KSG_KPG").Element("IT_SL").Value, out double II)|| xnode_1_HM_SLUCH.Element("KSG_KPG").Element("IT_SL").Value.Length != 7)
                                            Logg = Logger("003F.00.1150 - N_ZAP " + N_ZAP + " [IT_SL] элемент должен соответствовать маске 9.99999", listBox);

                                        /*
                                        */

                                        if (double.TryParse(xnode_1_HM_SLUCH.Element("KSG_KPG").Element("IT_SL").Value, out double Ix) && Ix == 1.0f)
                                            Logg = Logger("006F.00.1350 - N_ZAP " + N_ZAP + " [IT_SL] элемент не может принимать значение 1 в 2021 году!", listBox);

                                        if (xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF") == null)
                                        {
                                            Logg = Logger("003F.00.1150 - N_ZAP " + N_ZAP + " [IT_SL] элемент должен отсутствовать при отсутствии поля SL_KOEF", listBox);
                                            Logg = Logger("003F.00.1160 - N_ZAP " + N_ZAP + " [SL_KOEF] элемент должен присутствовать при наличии поля IT_SL", listBox);
                                        }
                                        else
                                        {
                                            if (xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL") == null)
                                            {
                                                Logg = Logger("003F.00.2770 - N_ZAP " + N_ZAP + " [SL_KOEF/IDSL] элемент должен присутствовать при наличии поля SL_KOEF", listBox);
                                            }
                                            else
                                            {
                                                //MessageBox.Show(Int32.TryParse(xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value, out int x) + "    " + xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value.Length);
                                                
                                                if (!Int32.TryParse(xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value, out int xx) || xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value.Length > 4)
                                                { Logg = Logger("004F.00.1180 - N_ZAP " + N_ZAP + " [SL_KOEF/IDSL] элемент должен соответствовать маске 9999", listBox); }
                                                else {

                                                    if (mkslp.FindIndex(s => s.Item1 == xx && s.Item2 !=null&& s.Item3 < Convert.ToDateTime(xnode_1_HM_SLUCH.Element("DATE_2").Value) && s.Item4 >= Convert.ToDateTime(xnode_1_HM_SLUCH.Element("DATE_2").Value)) < 1)
                                                        Logg = Logger("005F.00.0160 - N_ZAP " + N_ZAP + " [SL_KOEF/IDSL] КСЛП  [" + xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value + "] не найден в справочнике КСЛП", listBox);
                                                }


                                            }

                                            if (xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("Z_SL") == null)
                                            {
                                                Logg = Logger("003F.00.2780 - N_ZAP " + N_ZAP + " [SL_KOEF/Z_SL] элемент должен присутствовать при наличии поля SL_KOEF", listBox);
                                            }
                                            else
                                            {

                                            }
                                        }




                                        

                                    }
                                    else
                                    {
                                        if (xnode_1_HM_SLUCH.Element("KSG_KPG") != null && xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF") != null)
                                        {
                                            Logg = Logger("003F.00.1140 - N_ZAP " + N_ZAP + " [IT_SL] элемент должен присутствовать при наличии поля SL_KOEF", listBox);
                                            Logg = Logger("003F.00.1170 - N_ZAP " + N_ZAP + " [SL_KOEF] элемент должен отсутствовать при отсутствии поля IT_SL", listBox);
                                        }
                                    }

                                    // if (xnode_1_HM_SLUCH["DS2"] != null) Logg = Logger("006F.00.0430  -  [DS2] есть  = "+ xnode_1_HM_SLUCH["DS2"].InnerText, listBox); ;

                                }
                                else
                                {
                                    Logg = Logger("003F.00.2451  - [DS1] Отсутствует обязательное поле DS1 ! ", listBox);
                                }
                               
                               /* 
                               */


                            }





                            xnode_1_HM_ZAP_row++;
                        }
                    
                        //xDoc_HM.Save(st.FullName);
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
                //xDoc.Save(findedFile.FullName);


            }
            Logg = Logger("Обработка файла " + FName1 + " завершена!", listBox);

            long totalMemory = GC.GetTotalMemory(false);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            ThreadCount--;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text= ThreadCount.ToString();

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
