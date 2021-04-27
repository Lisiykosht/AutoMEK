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

        private DirectoryInfo di_pack = new DirectoryInfo(@".\Incoming");
        private DirectoryInfo di_packed = new DirectoryInfo(@".\Outgoing\");
        private string fName;
        private string fName1;
        private int threadCount;
        List<Tuple<string , DateTime , DateTime >> mf003;
         List<Tuple<int,double,DateTime , DateTime >> mkslp;
        private List<Tuple<string, string, DateTime, DateTime>> mV024;
        private List<Tuple<string, string, DateTime, DateTime>> mV014;
        private List<Tuple<string, string, DateTime, DateTime>> mPERS;
        List<Tuple<string ,  DateTime >> mmkb;
        private int n_ZAP;
        private string ID_PAC;
        private string W;
        private DateTime dATE_1;
        private DateTime dATE_2;
        private DateTime DR;
        private DateTime DR_P;
        private string USL_OK;
        private string FOR_POM;

        public DirectoryInfo Di_pack { get => di_pack; set => di_pack = value; }
        public DirectoryInfo Di_packed { get => di_packed; set => di_packed = value; }
        public string FName { get => fName; set => fName = value; }
        public string FName1 { get => fName1; set => fName1 = value; }
        
        public int ThreadCount { get => threadCount; set => threadCount = value; }
        public List<Tuple<string, DateTime, DateTime>> Mf003 { get => mf003; set => mf003 = value; }
        public List<Tuple<int, double, DateTime, DateTime>> Mkslp { get => mkslp; set => mkslp = value; }
        public List<Tuple<string, string, DateTime, DateTime>> MV024 { get => mV024; set => mV024 = value; }
        public List<Tuple<string, string, DateTime, DateTime>> MV014 { get => mV014; set => mV014 = value; }
        public List<Tuple<string, string, DateTime, DateTime>> MPERS { get => mPERS; set => mPERS = value; }
        public List<Tuple<string, DateTime>> Mmkb { get => mmkb; set => mmkb = value; }
        public int N_ZAP { get => n_ZAP; set => n_ZAP = value; }
        public DateTime DATE_1 { get => dATE_1; set => dATE_1 = value; }
        public DateTime DATE_2 { get => dATE_2; set => dATE_2 = value; }
        

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
            string query_3= "select idsl,zkoef,datebeg,  coalesce(dateend,'2221-01-01') date_end from public.k_kslp";
            string query_4= "select iddkk,dkkname,datebeg,  coalesce(dateend,'2221-01-01') date_end from foms.v024";
            string query_5= "select idfrmmp,frmmpname,datebeg,  coalesce(dateend,'2221-01-01') date_end from foms.v014";
            
            NpgsqlCommand cmd_1 = new NpgsqlCommand(query_1, npgSqlConnection_1);
            NpgsqlCommand cmd_2 = new NpgsqlCommand(query_2, npgSqlConnection_1);
            NpgsqlCommand cmd_3 = new NpgsqlCommand(query_3, npgSqlConnection_1);
            NpgsqlCommand cmd_4 = new NpgsqlCommand(query_4, npgSqlConnection_1);
            NpgsqlCommand cmd_5 = new NpgsqlCommand(query_5, npgSqlConnection_1);

            NpgsqlDataReader f003 = cmd_1.ExecuteReader();
            
           

            Mf003 = new List<Tuple<string, DateTime, DateTime>>();
            Mkslp = new List<Tuple<int, double, DateTime, DateTime>>();
            Mmkb = new List<Tuple<string, DateTime>>();
            MV024 = new List<Tuple<string,string, DateTime, DateTime>>();
            MV014 = new List<Tuple<string,string, DateTime, DateTime>>();
            MPERS = new List<Tuple<string,string, DateTime, DateTime>>();
            

            if (f003.HasRows)
            {
                while (f003.Read())
                {
                    Mf003.Add(Tuple.Create( f003[0].ToString()  , Convert.ToDateTime(f003[1].ToString()) , Convert.ToDateTime(f003[2].ToString()) ));
                }
            }
            f003.Close();


            NpgsqlDataReader mkb = cmd_2.ExecuteReader();

            if (mkb.HasRows)
            {
                while (mkb.Read())
                {
                  Mmkb.Add(Tuple.Create( mkb[0].ToString()  ,mkb.GetDateTime(1)));
                }
            }
            mkb.Close();
            NpgsqlDataReader kslp = cmd_3.ExecuteReader();

            if (kslp.HasRows)
            {
                while (kslp.Read())
                {
                    Mkslp.Add(Tuple.Create(kslp.GetInt32(0), kslp.GetDouble(1), kslp.GetDateTime(2),kslp.GetDateTime(3)));
                }
            }
            kslp.Close();


            NpgsqlDataReader V024 = cmd_4.ExecuteReader();
            if (V024.HasRows)
            {
                while (kslp.Read())
                {
                    
                    MV024.Add(Tuple.Create(V024[0].ToString(), V024[1].ToString(), V024.GetDateTime(2), V024.GetDateTime(3)));
                }
            }
            V024.Close();

            NpgsqlDataReader V014 = cmd_5.ExecuteReader();
            if (V014.HasRows)
            {
                while (kslp.Read())
                {

                    MV014.Add(Tuple.Create(V014[0].ToString(), V014[1].ToString(), V014.GetDateTime(2), V014.GetDateTime(3)));
                }
            }
            V014.Close();

            if (!Di_pack.Exists)
            { Di_pack.Create(); }

            if (!Di_packed.Exists)
            { Di_packed.Create(); }

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

        private bool IsTrueText(string str)
        {
            char[] charStr = str.ToCharArray();
            int[] masObr = new int[] { 44, 130, 46, 58, 185, 32, 160, 95, 45, 150, 151, 47, 92, 124, 166, 40, 91, 123, 41, 93, 125, 39, 34, 96, 145, 147, 146, 148, 60, 139, 171, 62, 155, 187, 132 };
            bool flag = true;
            for (int i = 0; i < charStr.Length; i++)
            {
                if (!Char.IsLetterOrDigit(charStr[i]) && !masObr.Contains((int)charStr[i]))
                    return false;
            }
            return flag;
        }

        public  void Goer(object olistBox)
        {
            ListBox listBox = (ListBox)olistBox;
            ThreadCount++;
            string Logg;
            foreach (FileInfo findedFile in Di_pack.GetFiles("L*.xml"))
            {
                XDocument xDoc =  XDocument.Load(findedFile.FullName);



                XElement xRoot = xDoc.Element("PERS_LIST");

              //  MessageBox.Show(xRoot.ToString());
               foreach (XElement x_node_PERS in xRoot.Elements("PERS"))
                {


                    if (x_node_PERS.Element("ID_PAC") != null)
                    {
                        ID_PAC = x_node_PERS.Element("ID_PAC").Value;
                        if (ID_PAC.Length<37)
                        {
                            if (!IsTrueText(ID_PAC)) 
                                Logg = Logger("004F.00.1570  -  [PERS\\ID_PAC] Поле содержит недопустимые символы!", listBox); ;
                            

                        }
                        else
                        {
                            Logg = Logger("004F.00.1570  -  [PERS\\ID_PAC] Допустимая длина поля 36 превышена (" + ID_PAC.Length + ")или !", listBox);
                        }

                                            }
                    else
                    {
                        Logg = Logger("003F.00.3080  -  [PERS\\ID_PAC] Поле является обязательным! Cтрока (" + ((IXmlLineInfo)x_node_PERS).LineNumber + ")", listBox);
                    }






                    if (x_node_PERS.Element("DR")!=null)
                    {
                        if (DateTime.TryParse(x_node_PERS.Element("DR").Value, out DateTime dR))
                        { 
                            
                            DR = dR;
                            



                        }
                        else
                        {
                            Logg = Logger("004F.00.1610  - N_ZAP " + N_ZAP + "[NPR_DATE] Указана некорректная дата! NPR_DATE принят как " + DATE_1 + "!", listBox);
                        }



                    }
                    else
                    {
                        Logg = Logger("003F.00.3100  -  [PERS\\DR] Поле является обязательным!", listBox);
                    }

                    if (x_node_PERS.Element("W") != null)
                    {
                        W = x_node_PERS.Element("W").Value;
                    }
                    else
                    {
                        Logg = Logger("003F.00.3090  -  [PERS\\W] Поле является обязательным!", listBox);
                    }



                    MPERS.Add(Tuple.Create(ID_PAC, W, DR, DR_P));
                    
                }
                




               foreach (XElement xnode_1 in xRoot.Elements("ZGLV"))
                {
                    bool succ = false;
                    FileInfo st=null;
                    FName1 = xnode_1.Element("FILENAME1").Value;


                    try   ///ищем HM файл.
                    {
                        st = Di_pack.GetFiles(FName1 + ".xml")[0];
                        Logg =Logger ("Файл "+FName1+" со случаями для файла " + findedFile.Name + " найден!", listBox);
                        succ = true;
                    }
                    catch
                    {
                        Logg = Logger("Файл" + FName1 + "  со случаями для файла " + findedFile.Name + " не найден!", listBox);
                    }

                    if (succ)  //Загружаем XML если нашли НМ файл
                    {
                        XDocument xDoc_HM =  XDocument.Load(st.FullName, LoadOptions.SetLineInfo);
                        XElement xRoot_HM = xDoc_HM.Element("ZL_LIST");
                      
                        
                        foreach (XElement xnode_1_HM in xRoot_HM.Elements("SCHET"))
                        {
                            if (!DateTime.TryParse(xnode_1_HM.Element("DSCHET").Value, out DateTime DSCHET))
                            {
                                Logg = Logger("003F.00.2070  -  [DSCHET] Ошибка обработки файла  " + FName1+ "! Поле DSCHET является обязатльным для заполнения! DSCHET принят как "+DateTime.Now+"!", listBox);
                                

                            }
                            else
                            {
                                DSCHET = DateTime.Now;
                            }
                            {
                                if (Mf003.FindIndex(s => s.Item1 == xnode_1_HM.Element("CODE_MO").Value && s.Item2 < DSCHET && s.Item3 > Convert.ToDateTime(xnode_1_HM.Element("DSCHET").Value)) < 0)
                                        { 
                                            Logg = Logger("001F.00.0030  -  [CODE_MO] Организация " + xnode_1_HM.Element("CODE_MO").Value + " не найдена в справочнике F003  ", listBox);
                                        }
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
                                if (!Int32.TryParse(xnode_1_HM_ZAP.Element("N_ZAP").Value, out n_ZAP) | xnode_1_HM_ZAP.Element("N_ZAP").Value.Length > 9)
                                {
                                    Logg = Logger("004F.00.0190 - [N_ZAP] Поле N_ZAP  содержит недопустимое значение ["+ xnode_1_HM_ZAP.Element("N_ZAP").Value + "] в ZAP №  " + xnode_1_HM_ZAP_row + " строка (" + ((IXmlLineInfo)xnode_1_HM_ZAP).LineNumber + ")", listBox);
                                }

                            }
                            else 
                            {
                                 
                                Logg = Logger("003F.00.2110 - [N_ZAP] Пропущено обязательное поле N_ZAP в ZAP № " + xnode_1_HM_ZAP_row +" строка ("+ ((IXmlLineInfo)xnode_1_HM_ZAP).LineNumber + ")", listBox);
                            };


                            if (xnode_1_HM_ZAP.Element("PACIENT") != null)
                            {
                                if (xnode_1_HM_ZAP.Element("PACIENT").Element("ID_PAC") != null)
                                {
                                    
                                    ID_PAC = xnode_1_HM_ZAP.Element("PACIENT").Element("ID_PAC").Value;
                                    if (ID_PAC.Length < 37)
                                    {
                                        if (!IsTrueText(ID_PAC))
                                            Logg = Logger("004F.00.0210  -  [PACIENT\\ID_PAC] Поле содержит недопустимые символы!", listBox); ;


                                    }
                                    else
                                    {
                                        Logg = Logger("004F.00.0210  -  [PACIENT\\ID_PAC] Допустимая длина поля 36 превышена (" + ID_PAC.Length + ") !", listBox);
                                    }

                                }
                                else
                                {
                                    Logg = Logger("003F.00.2150  - N_ZAP " + N_ZAP + " [ID_PAC] Пропущено обязательный блок ID_PAC", listBox);
                                }

                                if (xnode_1_HM_ZAP.Element("PACIENT").Element("VPOLIS") == null)
                                    Logg = Logger("003F.00.2160  - N_ZAP " + N_ZAP + " [VPOLIS] Пропущено обязательное поле VPOLIS", listBox); 
                                
                                if (xnode_1_HM_ZAP.Element("PACIENT").Element("NPOLIS") == null)
                                    Logg = Logger("003F.00.2170  - N_ZAP " + N_ZAP + " [NPOLIS] Пропущено обязательное поле NPOLIS", listBox);

                                if (xnode_1_HM_ZAP.Element("PACIENT").Element("NOVOR") != null) 
                                {


                                }
                                else
                                {
                                    Logg = Logger("003F.00.2180  - N_ZAP " + N_ZAP + " [NOVOR] Пропущено обязательное поле NOVOR", listBox);
                                }
                            }
                            else
                            {
                                Logg = Logger("003F.00.2130  - N_ZAP " + N_ZAP + " [PACIENT] Пропущено обязательное блок PACIENT", listBox);
                            }





                            foreach (XElement xnode_1_HM_SLUCH in xnode_1_HM_ZAP.Elements("SLUCH"))
                            {

                                DATE_2 = Convert.ToDateTime(xnode_1_HM_SLUCH.Element("DATE_2").Value);
                                DATE_1 = Convert.ToDateTime(xnode_1_HM_SLUCH.Element("DATE_1").Value);
                                DR = MPERS[MPERS.FindIndex(s => s.Item1 == ID_PAC)].Item3;
                               // MessageBox.Show(MPERS[MPERS.FindIndex(s => s.Item1 == ID_PAC)].Item3 + " xxx " + DATE_1);
                                if (DR> DATE_1)
                                    Logg = Logger("006F.00.0680  - ID_PAC " + ID_PAC + "[PERS\\DR] Дата рождения меньше даты начала случая  ДР: " + DR + "--Д1: " + DATE_1 + "!", listBox);

                                if (xnode_1_HM_SLUCH.Element("DS1").Value != null)
                                {
                                  
                                    if (Mmkb.FindIndex(s => s.Item1 == xnode_1_HM_SLUCH.Element("DS1").Value && s.Item2 >= DATE_2 ) < 0)
                               {
                                        Logg = Logger("005F.00.0040  - N_ZAP " + N_ZAP +"  [DS1] Диагноз ["+ xnode_1_HM_SLUCH.Element("DS1").Value + "] не найден в справочнике MKB-10", listBox);
                               }
                                    if (xnode_1_HM_SLUCH.Element("DS2") != null)
                                    {
                                        if ((xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS2").Value))// || (xnode_1_HM_SLUCH.Element("DS3") != null) && (xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS3").Value))
                                                                             Logg = Logger("006F.00.0430  - N_ZAP " + N_ZAP + " [DS1 -- DS2] Диагноз " + xnode_1_HM_SLUCH.Element("DS1").Value + " не должен равняться DS2", listBox);

                                        if (Mmkb.FindIndex(s => s.Item1 == xnode_1_HM_SLUCH.Element("DS2").Value && s.Item2 >= DATE_2) < 0)
                                            Logg = Logger("005F.00.0050 - N_ZAP " + N_ZAP + " [DS2] Диагноз [" + xnode_1_HM_SLUCH.Element("DS2").Value + "] не найден в справочнике MKB-10", listBox);

                                        if ((xnode_1_HM_SLUCH.Element("DS3") != null) && (xnode_1_HM_SLUCH.Element("DS2").Value == xnode_1_HM_SLUCH.Element("DS3").Value))// ||  (xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS3").Value))
                                            Logg = Logger("006F.00.0440  - N_ZAP " + N_ZAP + " [DS2 -- DS3] Диагноз " + xnode_1_HM_SLUCH.Element("DS2").Value + " не должен равняться DS3", listBox);


                                    }

                                        


                                    if (xnode_1_HM_SLUCH.Element("DS3") != null) 
                                    {
                                        if ((xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS3").Value))// || (xnode_1_HM_SLUCH.Element("DS3") != null) && (xnode_1_HM_SLUCH.Element("DS1").Value == xnode_1_HM_SLUCH.Element("DS3").Value))
                                            Logg = Logger("006F.00.0430  - N_ZAP " + N_ZAP + " [DS1 -- DS3] Диагноз " + xnode_1_HM_SLUCH.Element("DS1").Value + " не должен равняться DS3", listBox);

                                        if (Mmkb.FindIndex(s => s.Item1 == xnode_1_HM_SLUCH.Element("DS3").Value && s.Item2 >= DATE_2) < 0)
                                            Logg = Logger("005F.00.0060 - N_ZAP " + N_ZAP + " [DS3] Диагноз [" + xnode_1_HM_SLUCH.Element("DS3").Value + "] не найден в справочнике MKB-10", listBox);

                                       
                                    }


                                    if (xnode_1_HM_SLUCH.Element("USL_OK") != null)
                                    {
                                        USL_OK = xnode_1_HM_SLUCH.Element("USL_OK").Value;

                                    }
                                    else
                                    {
                                        Logg = Logger("003F.00.2220  - N_ZAP " + N_ZAP + " [USL_OK] Является обязательным!", listBox);
                                    }

                                    if (xnode_1_HM_SLUCH.Element("FOR_POM") != null)
                                    {
                                        FOR_POM = xnode_1_HM_SLUCH.Element("FOR_POM").Value;

                                        if (mV014.FindIndex(s => s.Item1 == FOR_POM && s.Item3 < DATE_2 && s.Item4 >= DATE_2) < 0)
                                            Logg = Logger("001F.00.0180  - N_ZAP " + N_ZAP + " [FOR_POM] Значение FOR_POM=" + FOR_POM+" не найдено в справочнике V014!", listBox);


                                        switch (USL_OK)
                                        {
                                            case "2":
                                                if (!(FOR_POM=="2" || FOR_POM=="3"))
                                                    Logg = Logger("006F.00.1280  - N_ZAP " + N_ZAP + " [FOR_POM] Для USL_OK=2 поле FOR_POM должно равняться 2 или 3! FOR_POM="+FOR_POM, listBox);
                                                break;
                                            case "3":
                                                if (!(FOR_POM == "2" || FOR_POM == "3"))
                                                    Logg = Logger("006F.00.1280  - N_ZAP " + N_ZAP + " [FOR_POM] Для USL_OK=3 поле FOR_POM должно равняться 2 или 3! FOR_POM=" + FOR_POM, listBox);
                                              
                                                break;
                                            case "4":
                                                if (!(FOR_POM == "2" || FOR_POM == "1"))
                                                    Logg = Logger("006F.00.1280  - N_ZAP " + N_ZAP + " [FOR_POM] Для USL_OK=4 поле FOR_POM должно равняться 2 или 1! FOR_POM=" + FOR_POM, listBox);
                                                break;

                                        }
                                        
                                    }
                                    else
                                    {
                                        Logg = Logger("003F.00.2240 - N_ZAP " + N_ZAP + " [FOR_POM] Является обязательным!", listBox);
                                    }

                                    if (xnode_1_HM_SLUCH.Element("NPR_MO") != null)
                                    {
                                        if (xnode_1_HM_SLUCH.Element("NPR_DATE") != null)
                                        {
                                            if (Convert.ToDateTime( xnode_1_HM_SLUCH.Element("NPR_DATE").Value)<DR)
                                                Logg = Logger("006F.00.0311  - N_ZAP " + N_ZAP + " [NPR_DATE] Значение поля NPR_DATE "+ Convert.ToDateTime(xnode_1_HM_SLUCH.Element("NPR_DATE").Value) + "не должно быть меньше DR" + DR, listBox);
                                        }
                                        else
                                        {
                                            if (!DateTime.TryParse(xnode_1_HM_SLUCH.Element("NPR_DATE").Value, out DateTime NPR_DATE))
                                            {
                                                Logg = Logger("004F.00.0540  - N_ZAP " + N_ZAP + "[NPR_DATE] Указана некорректная дата! NPR_DATE принят как " + DATE_1 + "!", listBox);

                                                if (NPR_DATE>DATE_1)
                                                    Logg = Logger("006F.00.0310  - N_ZAP " + N_ZAP + "[NPR_DATE]  NPR_DATE "+NPR_DATE+" больше даты на чала случая("+DATE_1+")!", listBox);

                                            }
                                            if ((USL_OK == "2" & FOR_POM == "3") | USL_OK == "2")
                                                Logg = Logger("003F.00.0410  - N_ZAP " + N_ZAP + " [NPR_DATE] Поле должно присутствовать при (FOR_POM=3 и USL_OK=1) или USL_OK=2", listBox);

                                            Logg = Logger("003F.00.0402  - N_ZAP " + N_ZAP + " [NPR_DATE] Поле должно присутствовать при наличии поля NPR_MO", listBox);
                                        }
                                    }
                                    else
                                    {
                                        if ((USL_OK=="2" & FOR_POM == "3") | USL_OK == "2")
                                            Logg = Logger("003F.00.0400  - N_ZAP " + N_ZAP + " [NPR_MO] Поле должно присутствовать при (FOR_POM=3 и USL_OK=1) или USL_OK=2", listBox);
                                        
                                        if (xnode_1_HM_SLUCH.Element("NPR_DATE") != null)
                                        {
                                            Logg = Logger("003F.00.0401  - N_ZAP " + N_ZAP + " [NPR_MO] Поле должно присутствовать при наличии поля NPR_DATE", listBox);
                                        }
                                        else
                                        {

                                        }
                                    }


                                    if (xnode_1_HM_SLUCH.Element("NAPR") != null)
                                    {
                                        

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
                                                
                                                if (!Int32.TryParse(xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value, out int xx) || xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value.Length > 4)
                                                { Logg = Logger("004F.00.1180 - N_ZAP " + N_ZAP + " [SL_KOEF/IDSL] элемент должен соответствовать маске 9999", listBox); }
                                                else {

                                                    if (Mkslp.FindIndex(s => s.Item1 == xx && s.Item3 < DATE_2 && s.Item4 >= DATE_2) < 0)
                                                    {
                                                        Logg = Logger("005F.00.0160 - N_ZAP " + N_ZAP + " [SL_KOEF/IDSL] КСЛП  [" + xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value + "] не найден в справочнике КСЛП", listBox);
                                                        
                                                        

                                                    }
                                                    if (xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("Z_SL") == null)
                                                    {
                                                        Logg = Logger("003F.00.2780 - N_ZAP " + N_ZAP + " [SL_KOEF/Z_SL] элемент должен присутствовать при наличии поля SL_KOEF", listBox);
                                                    }
                                                    else
                                                    {
                                                        if (!double.TryParse(xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("Z_SL").Value.ToString().Replace(".",","), out double ikslp) )//|| xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("Z_SL").Value.Length < 4)
                                                        {
                                                            Logg = Logger("004F.00.1190 - N_ZAP " + N_ZAP + " [SL_KOEF/Z_SL] значение КСЛП  [" +ikslp+" )( "+ xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("Z_SL").Value + "] элемент должен соответствовать маске 9.99999", listBox);
                                                        }
                                                        else
                                                        {
                                                           
                                                            if (Mkslp.FindIndex(s => s.Item1 == Convert.ToInt32(xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value) && s.Item2 == ikslp && s.Item3 < DATE_2 && s.Item4 >= DATE_2) < 0)
                                                            {
                                                                
                                                                Logg = Logger("004F.00.1190 - N_ZAP " + N_ZAP + " [SL_KOEF/Z_SL] КСЛП  номер [" + xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("IDSL").Value +"--"+xnode_1_HM_SLUCH.Element("KSG_KPG").Element("SL_KOEF").Element("Z_SL").Value + "] не найден в справочнике КСЛП", listBox);
                                                            }
                                                            else
                                                            {

                                                            }
                                                        }
                                                            


                                                    }



                                                }

                                            

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
                                    Logg = Logger("003F.00.2451  - N_ZAP " + N_ZAP + " [DS1] Отсутствует обязательное поле DS1 ! ", listBox);
                                }

                                /* if (xnode_1_HM_SLUCH.Element("TARIF").Value != null)
                                   {
                                       if (xnode_1_HM_SLUCH.Element("TARIF").Value != null) 
                                   }
                                */



                                //Блок ONK_SL

                                if (xnode_1_HM_SLUCH.Element("ONK_SL") != null)
                                {
                                    if (xnode_1_HM_SLUCH.Element("ONK_SL").Element("ONK_USL") != null) 

                                    {
                                        foreach (XElement xnode_1_ONK_USL in xnode_1_HM_SLUCH.Element("ONK_SL").Elements("ONK_USL")) 
                                        {
                                            if (xnode_1_ONK_USL.Element("USL_TIP") != null)
                                            {
                                                int val = Int16.Parse(xnode_1_ONK_USL.Element("USL_TIP").Value);


                                                if (xnode_1_ONK_USL.Element("LEK_PR")!=null)
                                                {
                                                    if (!(val == 2 ^ val == 4))
                                                        Logg = Logger("003F.00.1500  - N_ZAP " + N_ZAP + " [LEK_PR] элемент должен отсутствовать при USL_TIP<>{2,4}! ", listBox);
                                                  
                                                    if (xnode_1_ONK_USL.Element("LEK_PR").Element("REGNUM")==null)
                                                        Logg = Logger("003F.00.2660  - N_ZAP " + N_ZAP + " [REGNUM] элемент должен присутствовать при наличии LEK_PR! ", listBox);

                                                    if (xnode_1_ONK_USL.Element("LEK_PR").Element("CODE_SH") == null)
                                                    {
                                                        Logg = Logger("003F.00.2670  - N_ZAP " + N_ZAP + " [CODE_SH] элемент должен присутствовать при наличии LEK_PR! ", listBox);
                                                    }
                                                    else
                                                    {
                                                        
                                                    }
                                                        

                                                    if (xnode_1_ONK_USL.Element("LEK_PR").Element("DATE_INJ") == null)
                                                    {
                                                        Logg = Logger("003F.00.2680  - N_ZAP " + N_ZAP + " [DATE_INJ] элемент должен присутствовать при наличии LEK_PR! ", listBox);
                                                    }
                                                    else
                                                    {
                                                        if (!DateTime.TryParse(xnode_1_ONK_USL.Element("LEK_PR").Element("DATE_INJ").Value,out DateTime DATE_INJ))
                                                            {
                                                            Logg = Logger("004F.00.1540  - N_ZAP " + N_ZAP + " [DATE_INJ] элемент должен содержать корректную дату! ", listBox);
                                                            }  
                                                        if (DATE_INJ< DATE_1)
                                                            Logg = Logger("006F.00.0610  - N_ZAP " + N_ZAP + " [DATE_INJ] Должен быть больше или равен DATE_1! ", listBox);
                                                        if (DATE_INJ> DATE_2)
                                                            Logg = Logger("006F.00.0620  - N_ZAP " + N_ZAP + " [DATE_INJ] Должен быть меньше или равен DATE_2! ", listBox);


                                                    }
                                                        



                                                }
                                                else
                                                {
                                                    if ((val == 2 ^ val == 4))
                                                        Logg = Logger("003F.00.1490  - N_ZAP " + N_ZAP + " [LEK_PR] элемент должен присутствовать при USL_TIP={2,4}! ", listBox);

                                                }


                                            }
                                            else
                                            {

                                            }
                                        }
                                        
                                    }
                                    
                                }

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
