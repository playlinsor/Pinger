using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace Pinger
{
    public partial class Form1 : Form
    {
        // Перечисление доступных иконок
        enum ShowIcons {GreenOk,Green,Yellow,Orange,Purple,Red,RedMinus }
        // Задержка пинга в этом потоке
        long MainDelay = 0;
        string URL = "http://ya.ru";

        public Form1()
        {
            InitializeComponent();

            DelayTimer.Enabled = true;
            Worker.RunWorkerAsync(URL);
        }

        // Фоновый Пинг до сервака
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string cUrl = e.Argument.ToString();
            if (cUrl.IndexOf("http") == -1) cUrl = "http://" + cUrl;
            Uri url = new Uri(cUrl);
            string pingurl = string.Format("{0}", url.Host);
            string host = pingurl;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                {
                    e.Result = new PingInformation(reply.Address, reply.RoundtripTime, reply.Buffer.Length, reply.Options.Ttl);
                }
            }
            catch (Exception ex)
            {
                e.Result = new PingInformation(ex.InnerException.Message);
            }
        }

        // Пинг окончен.
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MainDelay = 0;
            DelayTimer.Enabled = false;
            PrintLogPingText(e.Result as PingInformation);
            VisibleIconFromDelay((e.Result as PingInformation).Delay);
            SleepWorker.RunWorkerAsync();
        }

        // Поток Промежутока 
        private void SleepWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
        }

        // Промежуток закончен
        private void SleepWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DelayTimer.Enabled = true;
            Worker.RunWorkerAsync(URL);
        }



        // Logging information 
        private void PrintLogPingText(PingInformation pi)
        {
            string Line = textBox2.Text;
            if (Line.Length > 240) Line = "";

            if (!pi.checkError())
            {
                Line += string.Format("IP {0},Delay {1} мс,send {2} b, TTL {3}", pi.IP, pi.Delay, pi.SendByte, pi.TTL);
            }
            else Line += pi.getError();

            textBox2.Text = Line + "\r\n";
        }

        /// <summary>
        /// Показываем иконку
        /// </summary>
        /// <param name="ico"></param>
        private void ShowToolIcon(ShowIcons ico)
        {
            Bitmap bm = new Bitmap(Properties.Resources.Green_Ball);

            switch (ico)
            {
                case ShowIcons.GreenOk: bm = Properties.Resources.Clear_Green; break;
                case ShowIcons.Green: bm = Properties.Resources.Green_Ball; break;
                case ShowIcons.Orange: bm = Properties.Resources.Orange_Ball; break;
                case ShowIcons.Purple: bm = Properties.Resources.Purple_Ball; break;
                case ShowIcons.Red: bm = Properties.Resources.Red_Ball; break;
                case ShowIcons.RedMinus: bm = Properties.Resources.Minus_Red_Button; break;
                case ShowIcons.Yellow: bm = Properties.Resources.Yellow_Ball; break;
            }

            TrayIcon.Icon = Icon.FromHandle(bm.GetHicon());
            
        }

        // Меню Выход
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        // Меню Показать
        private void показатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = true;
        }

        // Анти закрытие формы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            if (e.CloseReason.ToString() == "UserClosing") e.Cancel = true;
        }

        private void VisibleIconFromDelay(long Delay)
        {
            if (Delay < 50) ShowToolIcon(ShowIcons.GreenOk);
            else if (Delay > 50 && Delay < 200) ShowToolIcon(ShowIcons.Green);
            else if (Delay > 200 && Delay < 400) ShowToolIcon(ShowIcons.Yellow);
            else if (Delay > 400 && Delay < 800) ShowToolIcon(ShowIcons.Orange);
            else if (Delay > 800 && Delay < 1500) ShowToolIcon(ShowIcons.Red);
            else if (Delay > 1500) ShowToolIcon(ShowIcons.RedMinus);
        }

        /// Задержка Инета.
        private void DelayTimer_Tick(object sender, EventArgs e)
        {
            /*
            MainDelay++;
            switch (MainDelay)
            {
                case 1: ShowToolIcon(ShowIcons.GreenOk); break;
                case   5: ShowToolIcon(ShowIcons.Green); break;
                case  20: ShowToolIcon(ShowIcons.Yellow); break;
                case  40: ShowToolIcon(ShowIcons.Orange); break;
                case 100: ShowToolIcon(ShowIcons.Red); break;
                case 200: ShowToolIcon(ShowIcons.RedMinus); break;
            }
             */
        }
    }
}
