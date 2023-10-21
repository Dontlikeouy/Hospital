using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;
using Hospital.Templates;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Windows.Markup;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;
using static Npgsql.PostgresTypes.PostgresCompositeType;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;
using Microsoft.SqlServer.Server;
using System.Globalization;
using System.Diagnostics;

namespace Hospital
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            MyTemplate.PopUps = PopUps;
            MyTemplate.PopUpsWarning = PopUpsWarning;
            ConnectToSql();




        }

        //initializations
        class MyUser
        {
            public string Id { get; set; }
            public string Surname { get; set; }
            public string Name { get; set; }
            public string MiddleName { get; set; }
            public string TypeUser { get; set; }
        }
        MyUser myuser;


        bool MyClaerCheckText(bool allRequired = false)
        {
            Regex regexClear = new Regex(@"\s|\+");
            Regex regexCheck = new Regex(@"\*");

            bool exit = false;
            foreach (var keyContent in MyTemplate.contentPopUps.Keys)
            {
                if (MyTemplate.contentPopUps[keyContent].textBox != null)
                {
                    MyTemplate.contentPopUps[keyContent].textBox.Text = regexClear.Replace(MyTemplate.contentPopUps[keyContent].textBox.Text, "");

                    if (MyTemplate.contentPopUps[keyContent].textBox.Text == "")
                    {
                        if (allRequired == true)
                        {
                            exit = true;
                        }
                        else if (regexCheck.IsMatch(keyContent) == true)
                        {
                            exit = true;
                        }


                    }
                }
            }
            return exit;
        }
        private void LoginUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyClaerCheckText() == true)
            {
                return;
            }
            var sql = $"SELECT \"IDUser\",\"Surname\",\"Name\", \"MiddleName\", \"TypeUser\" FROM \"Users\" WHERE \"State\"='active' AND \"Login\" = '{MyTemplate.contentPopUps["Логин"].textBox.Text}' AND \"Password\"='{MyTemplate.contentPopUps["Пароль"].textBox.Text}';";

            NpgsqlDataReader rdr = MyRequstSql_Return(sql);

            if (rdr.HasRows == true)
            {
                rdr.Read();

                myuser = new MyUser
                {
                    Id = rdr.GetValue(0).ToString(),
                    Surname = rdr.GetValue(1).ToString(),
                    Name = rdr.GetValue(2).ToString(),
                    MiddleName = rdr.GetValue(3).ToString(),
                    TypeUser = rdr.GetValue(4).ToString(),
                };
                rdr.Close();


                PopUps.Content = null;
                switch (myuser.TypeUser)
                {
                    case "patient":
                        {
                            CreatePatient();
                            break;
                        }
                    case "admin":
                        {
                            CreateAdmin();
                            break;
                        }
                    case "doctor":
                        {
                            CreateDoctor();
                            break;
                        }
                }
            }
            else
                rdr.Close();


        }

        //Восстановить пароль
        private void RestartPasswordUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyClaerCheckText(true) == true)
            {
                return;
            }

            if (long.TryParse((MyTemplate.contentPopUps["Снилс"].textBox.Text), out var number))
            {
                MyRequstSql_Execute($"UPDATE \"Users\" SET \"Password\"='{MyTemplate.contentPopUps["Новый пароль"].textBox.Text}' WHERE \"Snils\"='{MyTemplate.contentPopUps["Снилс"].textBox.Text}';");
                InitializationUser();
            }
        }
        private void CreateRestartPasswordUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyTemplate.contentPopUps = new Dictionary<string, MyTextBox>
            {
                {"Снилс", new MyTextBox()},
                {"Новый пароль", new MyTextBox()}
            };

            PopUps.Content = MyTemplate.PopUps_Input
            (
                "Восстановить пароль",
                new MyButton("Восстановить пароль", RestartPasswordUser_MouseDown),

                false,
                new MyButton("Авторизироваться", CreateLoginUser_MouseDown),
                new MyButton("Зарегистрироваться", CreateRegisterUser_MouseDown)
            );
        }
        //..Восстановить пароль
        private void RegisterUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyClaerCheckText() == true)
            {
                return;
            }

            if (long.TryParse((MyTemplate.contentPopUps["Снилс*"].textBox.Text), out var number)
                && long.TryParse((MyTemplate.contentPopUps["Номер телефона*"].textBox.Text), out var number1))
            {

                try
                {

                    MyRequstSql_Execute(MyRequstSql_InsertPopUps
                    (
                        "Users",
                        new List<string>
                        {
                            "Surname",
                            "Name",
                            "MiddleName",
                            "Snils",
                            "Phone",
                            "Email",
                            "Login",
                            "Password",
                            "State",
                            "TypeUser"
                        }
                    ));

                }
                catch
                {
                    return;
                }
                InitializationUser();
            }
        }

        private void CreateRegisterUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyTemplate.contentPopUps = new Dictionary<string, MyTextBox>
            {
                {"Фамилия*",new MyTextBox()},
                {"Имя*",new MyTextBox()},
                {"Отчество*",new MyTextBox()},
                {"Снилс*",new MyTextBox()},
                {"Номер телефона*",new MyTextBox()},
                {"Email",new MyTextBox(900)},
                {"Логин*",new MyTextBox()},
                {"Пароль*",new MyTextBox()},

            };
            PopUps.Content = MyTemplate.PopUps_Input
            (
                "Регистрация",
                new MyButton("Зарегистрироваться", RegisterUser_MouseDown),
                false,
                new MyButton("Забыл пароль ?", CreateRestartPasswordUser_MouseDown),
                new MyButton("Авторизироваться", CreateLoginUser_MouseDown)

            );
            MyTemplate.contentPopUps.Add("State", new MyTextBox("active"));
            MyTemplate.contentPopUps.Add("TypeUser", new MyTextBox("patient"));

        }
        private void CreateLoginUser_MouseDown(object sender, MouseButtonEventArgs e)
        {
            InitializationUser();
        }
        void InitializationUser()
        {
            MyTemplate.contentPopUps = new Dictionary<string, MyTextBox>
            {
                {"Логин",new MyTextBox()},
                {"Пароль",new MyTextBox()}
            };
            PopUps.Content = MyTemplate.PopUps_Input
            (
                "Авторизация",
                new MyButton("Авторизироваться", LoginUser_MouseDown),
                false,
                new MyButton("Забыл пароль ?", CreateRestartPasswordUser_MouseDown),
                new MyButton("Зарегистрироваться", CreateRegisterUser_MouseDown)


            );
        }
        //..initializations

        //RequestPostgreSql
        static string cs = "Host=localhost;Username=postgres;Password=postgres;Database=Hospital";
        static string pathFile = @"connect.txt";
        static NpgsqlConnection con;
        private void ReconectToSql_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConnectToSql();

        }
        void ConnectToSql()
        {
            if (File.Exists(pathFile))
            {
                cs = File.ReadAllText(pathFile);

            }
            else
            {

                using (File.Create(pathFile)) ;
                File.WriteAllText(pathFile, cs);
            }

            try
            {
                con = new NpgsqlConnection(cs);
                con.Open();

            }
            catch (Exception ex)
            {
                con.Close();
                PopUps.Content = MyTemplate.PopUps_Output
                (
                    "Ошибка",
                    $"Не удалось установить свзяь с сервером: {ex.Message}",
                    new MyButton("Попробовать ещё раз", ReconectToSql_MouseDown)

                );
                return;

            }
            InitializationUser();

        }
        void MyRequstSql_Execute(string request)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(request, con);
            cmd.ExecuteNonQuery();
        }

        NpgsqlDataReader MyRequstSql_Return(string request)
        {
            NpgsqlCommand cmd = new NpgsqlCommand(request, con);
            NpgsqlDataReader rdr = cmd.ExecuteReader();
            return rdr;
        }

        class MyTypeValue
        {
            public bool needSymbol { get; set; }
            public string myValue { get; set; }
            public MyTypeValue(string myValue, bool needSymbol = true)
            {
                this.needSymbol = needSymbol;
                this.myValue = myValue;
            }
        }

        string MyRequstSql_InsertPopUps(string Table, List<string> Columns, bool needClose = true)
        {
            var sql = $"INSERT INTO \"{Table}\"";
            var sqlValue = ") VALUES(";
            sql += "(";
            int i = 0;
            foreach (var contentPopUps in MyTemplate.contentPopUps)
            {
                if (contentPopUps.Value.skip == false)
                {
                    if (contentPopUps.Value.textBox != null)
                    {
                        if (contentPopUps.Value.textBox.Text != "")
                        {
                            sql += $"\"{Columns[i]}\"";

                            if (contentPopUps.Value.needSymbol)
                                sqlValue += $"\'{contentPopUps.Value.textBox.Text}\'";
                            else
                                sqlValue += $"{contentPopUps.Value.textBox.Text}";
                        }
                        else
                        {
                            i++;
                            continue;
                        }
                    }
                    else
                    {
                        sql += $"\"{Columns[i]}\"";

                        if (contentPopUps.Value.needSymbol)
                            sqlValue += $"\'{contentPopUps.Value.text}\'";
                        else
                            sqlValue += $"{contentPopUps.Value.text}";

                    }
                    if (i + 1 < Columns.Count)
                    {
                        sql += ", ";
                        sqlValue += ", ";

                    }
                    i++;
                }
            }
            if (needClose)
            {
                sqlValue += ");";
            }
            else
            {
                sqlValue += ")";

            }
            return sql + sqlValue;
        }
        //..RequestPostgreSql

        //Администратор


        //Зарегистрировать врача
        private void RegisterDoctor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyClaerCheckText() == true)
            {
                return;
            }

            if (long.TryParse((MyTemplate.contentPopUps["Снилс*"].textBox.Text), out var number)
                && long.TryParse((MyTemplate.contentPopUps["Номер телефона*"].textBox.Text), out var number1))
            {
                NpgsqlDataReader rdr;
                try
                {

                    rdr = MyRequstSql_Return(MyRequstSql_InsertPopUps
                    (
                        "Users",
                        new List<string>
                        {
                            "Surname",
                            "Name",
                            "MiddleName",
                            "Snils",
                            "Phone",
                            "Email",
                            "Login",
                            "Password",
                            "State",
                            "TypeUser"
                        }, false
                    ) + "RETURNING \"IDUser\" as \"ID\"" + ";");

                }
                catch
                {
                    return;
                }
                rdr.Read();
                object IDUser = rdr.GetValue(0);
                rdr.Close();

                rdr = MyRequstSql_Return($"INSERT INTO \"Post\"(\"Post\") VALUES ('{MyTemplate.contentPopUps["Должность*"].textBox.Text}') ON CONFLICT (\"Post\") DO UPDATE SET  \"Post\"=EXCLUDED.\"Post\" RETURNING \"IDPost\";");
                rdr.Read();
                object IDPost = rdr.GetValue(0);
                rdr.Close();

                rdr = MyRequstSql_Return($"INSERT INTO \"Specialization\"(\"Specialization\") VALUES ('{MyTemplate.contentPopUps["Специальность*"].textBox.Text}') ON CONFLICT (\"Specialization\") DO UPDATE SET \"Specialization\"=EXCLUDED.\"Specialization\" RETURNING \"IDSpecialization\";");
                rdr.Read();
                object IDSpecialization = rdr.GetValue(0);
                rdr.Close();
                MyRequstSql_Execute($"INSERT INTO \"Doctor\"(\"IDUser\", \"Specialization\", \"Post\")VALUES ({IDUser}, {IDSpecialization}, {IDPost});");

                PopUps.Content = null;

            }
        }
        private void CreateRegisterDoctor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyTemplate.contentPopUps = new Dictionary<string, MyTextBox>
            {
                {"Фамилия*",new MyTextBox()},
                {"Имя*",new MyTextBox()},
                {"Отчество*",new MyTextBox()},
                {"Снилс*",new MyTextBox()},
                {"Номер телефона*",new MyTextBox()},
                {"Email",new MyTextBox(900)},
                {"Специальность*",new MyTextBox(900,true)},
                {"Должность*",new MyTextBox(900,true)},
                {"Логин*",new MyTextBox()},
                {"Пароль*",new MyTextBox()},

            };
            PopUps.Content = MyTemplate.PopUps_Input
            (
                "Регистрация врача",
                new MyButton("Зарегистрироваться", RegisterDoctor_MouseDown),
                true

            );
            MyTemplate.contentPopUps.Add("State", new MyTextBox("active"));
            MyTemplate.contentPopUps.Add("TypeUser", new MyTextBox("doctor"));

        }
        //..Зарегистрировать врача

        void RestoreRemoveDoctor(string Text)
        {
            MyRequstSql_Execute($"UPDATE \"Users\" SET \"State\"='{Text}' WHERE \"IDUser\"={MyTemplate.contentMain[MyTemplate.MyTag][0]};");
            ((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][4]).Text = Text;
        }
        //Удалить врача
        private void RemoveDoctor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RestoreRemoveDoctor("delete");
        }
        //..Удалить врача
        //Возобновить врача
        private void RestoreDoctor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RestoreRemoveDoctor("active");
        }
        //..Возобновить врача

        //Сформировать расписание
        string format = "dd.MM.yyyy HH:mm";

        DateTime dateStart;
        DateTime dateEnd;
        Byte betweenTime;
        void WorkSchedule(string forced = "")
        {
            while (dateEnd - dateStart >= new TimeSpan(0, betweenTime, 0))
            {
                MyRequstSql_Execute($"INSERT INTO \"Chart\"(\"IDDoctor\", \"DateTime\")VALUES ({MyTemplate.contentMain[MyTemplate.MyTag][0]}, '{dateStart.ToString(format)}') {forced};");
                dateStart = dateStart.AddMinutes(betweenTime);
            }
        }
        private void CreateWarningWorkSchedule_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WorkSchedule("ON CONFLICT DO NOTHING");

            PopUps.Content = null;
        }
        private void WorkSchedule_MouseDown(object sender, MouseButtonEventArgs e)
        {


            if (DateTime.TryParseExact(MyTemplate.contentPopUps["Начальная дата и время (99.99.9999 99:99)"].textBox.Text, format, null, DateTimeStyles.None, out dateStart)
                && DateTime.TryParseExact(MyTemplate.contentPopUps["Конечная дата и время (99.99.9999 99:99)"].textBox.Text, format, null, DateTimeStyles.None, out dateEnd)
                && Byte.TryParse(MyTemplate.contentPopUps["Промежуток (20 минут)"].textBox.Text, out betweenTime))
            {
                //dateEnd = dateEnd.AddMinutes(betweenTime);
                if (dateEnd - dateStart >= new TimeSpan(0, betweenTime, 0))
                {
                    try
                    {
                        WorkSchedule();
                        PopUps.Content = null;
                    }
                    catch
                    {
                        PopUpsWarning.Content = MyTemplate.PopUps_Output("Опевещение", "Данный диапазон повторяется и поэтому не может быть записан. Записать принудительно?", new MyButton("Записать принудительно", CreateWarningWorkSchedule_MouseDown), true);
                    }
                    MyRequstSql_Execute("DELETE FROM \"Chart\" WHERE \"DateTime\" IN (SELECT \"InformationAboutReceptions\".\"DateTime\"\r\n\tFROM \"Chart\" JOIN \"InformationAboutReceptions\" ON \r\n\t\"Chart\".\"IDDoctor\"=\"InformationAboutReceptions\".\"IDDoctor\" \r\n\tAND \"Chart\".\"DateTime\"=\"InformationAboutReceptions\".\"DateTime\")");
                }

            }


        }
        private void CreateWorkSchedule_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyTemplate.contentPopUps = new Dictionary<string, MyTextBox>
            {
                {"Начальная дата и время (99.99.9999 99:99)",new MyTextBox(16)},
                {"Конечная дата и время (99.99.9999 99:99)",new MyTextBox(16)},
                {"Промежуток (20 минут)",new MyTextBox(2) }

            };
            PopUps.Content = MyTemplate.PopUps_Input
            (
                Title = "Cформировать расписание",
                new MyButton("Cформировать расписание", WorkSchedule_MouseDown),
                true
            );
        }

        //..Сформировать расписание


        //Посмотреть расписание врача
        private void CreateViewSchedule_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var rdr = MyRequstSql_Return($"SELECT CONCAT(\"Surname\",' ', \"Name\",' ', \"MiddleName\") as \"ФИО\" FROM \"Users\" WHERE \"IDUser\"={MyTemplate.contentMain[MyTemplate.MyTag][0]};");

            rdr.Read();
            var FIO = rdr.GetValue(0).ToString();
            rdr.Close();

            rdr = MyRequstSql_Return($"SELECT \"DateTime\" FROM \"Chart\"  WHERE \"IDDoctor\"={MyTemplate.contentMain[MyTemplate.MyTag][0]};");


            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                FIO
            );
            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Дата и время"
                },
                rdr
            );
        }
        //..Посмотреть расписание врача

        //Упрвление врачами

        void CreatMnagementDoctor()
        {
            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                $"{myuser.Surname} {myuser.Name} {myuser.MiddleName}"
            );
            var rdr = MyRequstSql_Return
            (
                "SELECT \"Doctor\".\"IDUser\",(CONCAT(\"Surname\",' ', \"Name\",' ', \"MiddleName\")) as \"ФИО\", \"Specialization\".\"Specialization\", \"Post\".\"Post\",\"State\" FROM \"Doctor\"JOIN \"Specialization\" ON \"IDSpecialization\"= \"Doctor\".\"Specialization\"JOIN \"Post\" ON \"IDPost\"= \"Doctor\".\"Post\"JOIN \"Users\" ON \"Users\".\"IDUser\"= \"Doctor\".\"IDUser\";"
            );

            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Врач",
                    "ФИО",
                    "Специальность",
                    "Должность",
                    "Статус"
                },
                rdr,
                new List<MyButton>
                {
                    new MyButton("Сформировать расписание",CreateWorkSchedule_MouseDown),
                    new MyButton("Посмотреть расписание врача",CreateViewSchedule_MouseDown),
                    new MyButton("Удалить врача",RemoveDoctor_MouseDown),
                    new MyButton("Возобновить врача",RestoreDoctor_MouseDown),
                },
                null,
                true
            );
        }
        private void CreatMnagementDoctor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreatMnagementDoctor();
        }
        //..Управление врачами

        void AdminPanel()
        {
            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                $"{myuser.Surname} {myuser.Name} {myuser.MiddleName}"
            );
            MainContent.Content = MyTemplate.CreateContent
            (
                new List<MyButton>
                {
                    new MyButton("Зарегистрировать врача",CreateRegisterDoctor_MouseDown),
                }
            );
        }
        private void AdminPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AdminPanel();
        }

        void CreateAdmin()
        {
            TabsHead.Content = MyTemplate.TabsHeadCreate
            (
                new List<MyTextBlock>
                {
                    new MyTextBlock("Панель администратора",AdminPanel_MouseDown,true),
                    new MyTextBlock("Упрвление врачами",CreatMnagementDoctor_MouseDown),
                }
             );

            AdminPanel();
        }
        //..Панель администратора



        //Пациент

        //Запись на приём к врачу
        void CreateViewSchedulePatient()
        {
            var rdr = MyRequstSql_Return
            (
                $"SELECT \"DateTime\" FROM \"Chart\" WHERE \"IDDoctor\"={IDDoctor} ;"
            );
            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Дата и время"
                },
                rdr,
                new List<MyButton>
                {
                    new MyButton("Записаться на приём",MakeAppointment_MouseDown)
                }
            );

        }
        object IDDoctor;
        private void CreateViewSchedulePatient_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                $"{((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][1]).Text}: {((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][2]).Text}",
                new SourceImage("Images/update1.ico", "Images/update2.ico", CreateViewSchedulePatient_MouseDown)
            );
            IDDoctor = MyTemplate.contentMain[MyTemplate.MyTag][0];
            CreateViewSchedulePatient();
        }
        private void MakeAppointment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var rdr = MyRequstSql_Return($"SELECT  Count(\"IDPatient\") FROM \"InformationAboutReceptions\" WHERE \"IDPatient\" = {myuser.Id} AND \"IDDoctor\" = {IDDoctor} AND \"Conclusion\" IS NULL;");
            rdr.Read();
            int countAll = rdr.GetInt16(0);
            rdr.Close();
            if (countAll == 0)
            {
                MyRequstSql_Execute($"INSERT INTO \"InformationAboutReceptions\"(\"IDPatient\", \"IDDoctor\", \"DateTime\")VALUES ({myuser.Id}, {IDDoctor},'{((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][0]).Text}');");


                MyRequstSql_Execute($"DELETE FROM \"Chart\" WHERE \"DateTime\"='{((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][0]).Text}';");
                PopUpsWarning.Content = MyTemplate.PopUps_Output
                (
                    "Уведомление",
                    "Вы успешно зарегистрировались на приём к врачу",
                    new MyButton("ОК", null)
                );
                CreateViewSchedulePatient();
            }
            else
            {
                PopUpsWarning.Content = MyTemplate.PopUps_Output
                (
                    "Ошибка",
                    "Вы уже записаны на приём к данному врачу",
                    new MyButton("ОК", null)
                );
            }

        }
        void CreateMakeAppointment()
        {
            var rdr = MyRequstSql_Return
            (
                "SELECT \"Doctor\".\"IDUser\",(CONCAT(\"Surname\",' ', \"Name\",' ', \"MiddleName\")) as \"ФИО\", \"Specialization\".\"Specialization\" FROM \"Doctor\"JOIN \"Specialization\" ON \"IDSpecialization\"= \"Doctor\".\"Specialization\" JOIN \"Users\" ON \"Users\".\"IDUser\"= \"Doctor\".\"IDUser\" AND \"State\"='active';"
            );

            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                $"{myuser.Surname} {myuser.Name} {myuser.MiddleName}"

            );
            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Врач",
                    "ФИО",
                    "Специальность"
                },
                rdr,
                null,
                CreateViewSchedulePatient_MouseDown,
                true


            );

        }
        private void CreateMakeAppointment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateMakeAppointment();
        }
        //..Запись на приём к врачу

        //Справки
        private void References_MouseDown(object sender, MouseButtonEventArgs e)
        {

            var rdr = MyRequstSql_Return
                (
                "SELECT (CONCAT(\"Surname\", ' ', \"Name\", ' ', \"MiddleName\")) as \"ФИО Врача\", \"Specialization\".\"Specialization\", \"DateTime\", \"Conclusion\" FROM \"Reference\"JOIN \"Users\" ON \"IDDoctor\"=\"Users\".\"IDUser\"JOIN \"Doctor\" ON \"IDDoctor\"=\"Doctor\".\"IDUser\"JOIN \"Specialization\" ON \"IDSpecialization\"=\"Doctor\".\"Specialization\" ;"
                );

            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                $"{myuser.Surname} {myuser.Name} {myuser.MiddleName}"

            );

            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Выдано",
                    "Специальность",
                    "Когда выдано",
                    "Заключение",
                },
                rdr

            );

        }
        //..Справки


        //Информация о приёмах
        private void InformationAboutReceptions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var rdr = MyRequstSql_Return
            (
            "SELECT (CONCAT(\"Surname\", ' ', \"Name\", ' ', \"MiddleName\")) as \"ФИО Врача\", \"Specialization\".\"Specialization\", \"DateTime\", \"Conclusion\" FROM \"InformationAboutReceptions\"JOIN \"Users\" ON \"IDDoctor\"=\"Users\".\"IDUser\"JOIN \"Doctor\" ON \"IDDoctor\"=\"Doctor\".\"IDUser\"JOIN \"Specialization\" ON \"IDSpecialization\"=\"Doctor\".\"Specialization\" ;"
            );

            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                $"{myuser.Surname} {myuser.Name} {myuser.MiddleName}"

            );

            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Врач",
                    "Специальность",
                    "Дата и время",
                    "Заключение",
                },
                rdr

            );
        }
        //..Информация о приёмах
        void CreatePatient()
        {
            TabsHead.Content = MyTemplate.TabsHeadCreate
            (
                new List<MyTextBlock>
                {
                    new MyTextBlock("Запись на приём",CreateMakeAppointment_MouseDown,true),
                    new MyTextBlock("Справки",References_MouseDown),
                    new MyTextBlock("Информация о приёмах",InformationAboutReceptions_MouseDown)
                }
            );
            CreateMakeAppointment();
        }

        //..Пациент


        //Врач

        //Получить справки
        void CreateGetReferences()
        {
            var rdr = MyRequstSql_Return("SELECT (CONCAT(\"Surname\", ' ', \"Name\", ' ', \"MiddleName\")) as \"ФИО Врача\",\"Specialization\".\"Specialization\", \"DateTime\", \"Conclusion\" FROM \"Reference\"JOIN \"Users\" ON \"IDPatient\"=\"Users\".\"IDUser\"JOIN \"Doctor\" ON \"IDDoctor\"=\"Doctor\".\"IDUser\"JOIN \"Specialization\" ON \"IDSpecialization\"=\"Doctor\".\"Specialization\";");

            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                Title = ((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][1]).Text
            );

            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Выдано",
                    "Специальность",
                    "Когда выдано",
                    "Заключение",
                },
                rdr

            );
        }
        private void GetReferences_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateGetReferences();
        }
        //..Получить справки
        //Получить информация о приёмах

        void GetInformationAboutReceptions()
        {
            var rdr = MyRequstSql_Return("SELECT (CONCAT(\"Surname\", ' ', \"Name\", ' ', \"MiddleName\")) as \"ФИО Врача\",\"Specialization\".\"Specialization\", \"DateTime\", \"Conclusion\" FROM \"InformationAboutReceptions\"JOIN \"Users\" ON \"IDPatient\"=\"Users\".\"IDUser\"JOIN \"Doctor\" ON \"IDDoctor\"=\"Doctor\".\"IDUser\"JOIN \"Specialization\" ON \"IDSpecialization\"=\"Doctor\".\"Specialization\" ;");
            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                Title = ((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][1]).Text
            );

            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Врач",
                    "Специальность",
                    "Дата и время",
                    "Заключение",
                },
                rdr

            );
        }
        private void GetInformationAboutReceptions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateViewRecordsPatient();
        }
        //..Получить информация о приёмах

        //Выдать заключение
        void GiveСonclusion()
        {
            MyRequstSql_Execute($"UPDATE \"InformationAboutReceptions\" SET \"Conclusion\"='{MyTemplate.contentPopUps["Заключение"].textBox.Text}' WHERE \"IDPatient\"={MyTemplate.contentMain[MyTemplate.MyTag][0]} AND \"IDDoctor\"={myuser.Id} AND \"DateTime\"='{((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][2]).Text}';");
            PopUps.Content = null;
        }
        private void GiveСonclusion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyTemplate.contentPopUps["Заключение"].textBox.Text != "")
            {
                GiveСonclusion();
                CreateViewRecordsPatient();
            }
        }
        void CreateGiveСonclusion(MouseButtonEventHandler mouseEvent)
        {
            MyTemplate.contentPopUps = new Dictionary<string, MyTextBox>
            {
                {"Заключение",new MyTextBox(900) }
            };
            PopUps.Content = MyTemplate.PopUps_Input
            (
                "Выдать зкалючение",
                new MyButton("ОК", mouseEvent),
                true,
                null,
                null,
                true

            );
        }
        private void CreateGiveСonclusion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateGiveСonclusion(GiveСonclusion_MouseDown);
        }
        //..Выдать заключение


        //Выдать справку
        private void GiveReferences_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyTemplate.contentPopUps["Заключение"].textBox.Text != "")
            {
                MyRequstSql_Execute($"INSERT INTO \"Reference\"(\"IDPatient\", \"IDDoctor\", \"DateTime\", \"Conclusion\")VALUES ({MyTemplate.contentMain[MyTemplate.MyTag][0]}, {myuser.Id}, '{((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][2]).Text}', '{MyTemplate.contentPopUps["Заключение"].textBox.Text}');");
                PopUps.Content = null;
            }
        }
        private void CreateGiveReferences_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MyTemplate.contentPopUps = new Dictionary<string, MyTextBox>
            {
                {"Заключение",new MyTextBox(900) }
            };
            PopUps.Content = MyTemplate.PopUps_Input
            (
                "Выдать справку",
                new MyButton("Ок", GiveReferences_MouseDown),
                false,
                null,
                null,
                true

            );
        }


        //..Выдать справкеу

        //Посмотреть все приёмы 
        private void CreateViewRecordsPatient()
        {
            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                $"{myuser.Surname} {myuser.Name} {myuser.MiddleName}",
                new SourceImage("Images/update1.ico", "Images/update2.ico", CreateViewRecordsPatient_MouseDown)
            );
            var rdr = MyRequstSql_Return
            (
                "SELECT \"IDPatient\", (CONCAT(\"Surname\", ' ', \"Name\", ' ', \"MiddleName\")) as \"ФИО\", \"DateTime\" FROM \"InformationAboutReceptions\" JOIN \"Users\" ON \"Users\".\"IDUser\" = \"IDPatient\" AND \"Conclusion\" IS NULL;"
            );
            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Пациент",
                    "ФИО",
                    "Дата и время"
                },
                rdr,
                new List<MyButton>
                {
                    new MyButton("Справки",GetReferences_MouseDown),
                    new MyButton("Информация о приёмах",GetReferences_MouseDown),
                    new MyButton("Выдать справку",CreateGiveReferences_MouseDown),
                    new MyButton("Выдать заключение",CreateGiveСonclusion_MouseDown),


                },
                null,
                true
            );
        }


        private void CreateViewRecordsPatient_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateViewRecordsPatient();
        }
        //..Посмотреть все приёмы 

        //Изменить заключени
        private void CreateOldGiveСonclusion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateGiveСonclusion(OldGiveСonclusion_MouseDown);
        }
        private void OldGiveСonclusion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MyTemplate.contentPopUps["Заключение"].textBox.Text != "")
            {
                GiveСonclusion();
                ((TextBlock)MyTemplate.contentMain[MyTemplate.MyTag][3]).Text = MyTemplate.contentPopUps["Заключение"].textBox.Text;
            }
        }
        //..Изменить заключени


        //Посмотерть Прошлые записи на приём
        private void CreateViewOldRecordsPatient()
        {
            var rdr = MyRequstSql_Return
            (
                $"SELECT \"IDPatient\", (CONCAT(\"Surname\", ' ', \"Name\", ' ', \"MiddleName\")) as \"ФИО\", \"DateTime\", \"Conclusion\" FROM \"InformationAboutReceptions\" JOIN \"Users\" ON \"Users\".\"IDUser\" =\"IDPatient\" AND \"IDDoctor\"={myuser.Id}  AND \"Conclusion\" IS NOT NULL;"
            );
            MainTitle.Content = MyTemplate.CreateMainTitle
            (
                $"{myuser.Surname} {myuser.Name} {myuser.MiddleName}",
                new SourceImage("Images/update1.ico", "Images/update2.ico", CreateViewOldRecordsPatient_MouseDown)


            );
            MainContent.Content = MyTemplate.CreateContent
            (
                new List<string>
                {
                    "Пациент",
                    "ФИО",
                    "Дата и время",
                    "Заключение"
                },
                rdr,
                new List<MyButton>
                {
                    new MyButton("Изменить заключение",CreateOldGiveСonclusion_MouseDown),
                },
                null,
                true
            );
        }
        private void CreateViewOldRecordsPatient_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateViewOldRecordsPatient();
        }
        //..Прошлые записи на приём

        void CreateDoctor()
        {
            TabsHead.Content = MyTemplate.TabsHeadCreate
            (
                new List<MyTextBlock>
                {
                    new MyTextBlock("Записи приём",CreateViewRecordsPatient_MouseDown,true),
                    new MyTextBlock("Прошлые записи на приём",CreateViewOldRecordsPatient_MouseDown),

                }
            );
            CreateViewRecordsPatient();
        }
        //..Врач

        //ToolBar
        private void GridToolBar_GragAndGrop(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        //..ToolBar


        //Кнопки
        private void ButtonWarp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            con.Close();
            this.WindowState = WindowState.Minimized;

        }

        private void ButtonExit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            con.Close();
            this.Close();

        }

        //..Кнопки
    }
}


