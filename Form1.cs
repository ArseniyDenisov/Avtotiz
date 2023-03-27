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

namespace Avtotiz
{
    public partial class Form1 : Form
    {
        NpgsqlConnection con = new NpgsqlConnection("Host=localhost:5433;Username=postgres;Password=2510123a;Database=Hairdresser");

        int tryCounter = 0;     

        Timer timer = new Timer();
        int sec = 30;

        public int accId = 0;
        public int rulesId = 0;



        public Form1()
        {
            InitializeComponent();
            timer.Interval = 1000;
            textBox2.PasswordChar = '•';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
                tryAuth();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void tryAuth()
        {
            con.Open();
            bool haveAcc = false;
            using (var cmd = new NpgsqlCommand($"select exists(select ид from пользователь where логин = '{textBox1.Text}')", con))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        haveAcc = reader.GetBoolean(0);
                }
            }

            if (haveAcc)
            {
                using (var cmd = new NpgsqlCommand($"select пароль, ид_прав, ид from пользователь where логин = '{textBox1.Text}'", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetString(0) == textBox2.Text)
                            {
                                MessageBox.Show("done");
                            }
                            else
                            {
                                tryAdd(2);
                            }
                        }
                    }
                }
            }
            else
            {
                tryAdd(1);
            }
            con.Close();
        }



        private void tryAdd(int reason)
        {
            tryCounter++;
            if (tryCounter == 3)
            {
                startTimer();
                tryCounter = 0;

            }
            else
            {
                switch (reason)
                {
                    case 1:
                        MessageBox.Show("Такого аккаунта не существует.", "Ошибка входа");
                        break;
                    case 2:
                        MessageBox.Show("Пароль неверен.", "Ошибка входа");
                        break;
                    default: break;
                }
            }
        }

        private void startTimer()
        {
            button1.Enabled = false;
            button1.Text = "Вход (20 с.)";
            timer.Tick += new EventHandler(timer_tick);
            timer.Start();
        }

        private void timer_tick(Object o, EventArgs e)
        {
            sec -= 1;
            button1.Text = $"Вход ({sec} с.)";
            if (sec == 0)
            {
                sec = 30;
                button1.Text = "Вход";
                button1.Enabled = true;
                timer.Stop();
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
