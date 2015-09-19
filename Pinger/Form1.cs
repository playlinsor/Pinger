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

        string URL = "http://ya.ru";

        public Form1()
        {
            InitializeComponent();         
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
                PingReply reply = p.Send(host, 2000);
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
            PingInformation pi = e.Result as PingInformation;
            if (pi != null)
            {
                TrayIcon.BalloonTipTitle = "Pinger 0.0.1 DEBUG";
                TrayIcon.BalloonTipText = string.Format("Задержка {0} мс ", pi.Delay);
                PrintLogPingText(pi);
                VisibleIconFromDelay(pi.Delay);

            }
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
           Worker.RunWorkerAsync(URL);
        }


        // Logging information 
        private void PrintLogPingText(PingInformation pi)
        {
            string Line = textBox2.Text;

            if (Line.Length>2024) Line = "";

            if (!pi.checkError())
            {
                Line += string.Format("IP {0},Delay {1} мс, TTL {2}", pi.IP, pi.Delay, pi.TTL);
            }
            else Line += pi.getError();

            textBox2.Text = Line + "\r\n";
            textBox2.SelectionStart = textBox2.Text.Length;
            textBox2.ScrollToCaret();
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
            try
            {
                TrayIcon.Icon = Icon.FromHandle(bm.GetHicon());
            } catch {};
        }

        // Меню Выход
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        // Меню Показать
        private void показатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Focus();
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
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            TrayIcon.ShowBalloonTip(2000);
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
