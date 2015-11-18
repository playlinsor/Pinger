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
        enum ShowIcons {GreenOk,Green,Yellow,Orange,Red,RedMinus,Arrow}

        string URL;
        bool Error = false;

        public Form1()
        {
            
            InitializeComponent();
            TrayIcon.Text = Text;

            if (Properties.Settings.Default.NeedUpdate)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.NeedUpdate = false;
            }

            URL = Properties.Settings.Default.Server;
            textBox1.Text = URL;

            OnPing(URL);
            button2.Enabled = false;
        }
        

        private void OnPing(string URL)
        {
            Worker.RunWorkerAsync(URL);
            TimerMaxDelay.Enabled = true;
        }

        // Фоновый Пинг до сервака
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Ping p = new Ping();
            string host = "";
            string cUrl = e.Argument.ToString();
            if (cUrl.IndexOf("ip:") != -1)
            {
                host = cUrl.Substring(3, cUrl.Length - 3);
            } else {
                if (cUrl.IndexOf("http") == -1) cUrl = "http://" + cUrl;
                Uri url = new Uri(cUrl);
                string pingurl = string.Format("{0}", url.Host);
                host = pingurl;    
            }
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
            TimerMaxDelay.Enabled = false;
            if (pi != null)
            {
                if (pi.checkError()) Error = true;
                TrayIcon.BalloonTipTitle = Text;
                TrayIcon.BalloonTipText = string.Format("Сервер: {0} \nЗадержка {1} мс \n\nДа прибудет с вами сила ;-)",URL,pi.Delay);
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
            OnPing(URL);
        }

        // Logging information 
        private void PrintLogPingText(PingInformation pi)
        {
            string Line = textBox2.Text;

            if (Line.Length>2024) Line = "";

            if (!pi.checkError())
            {
                Line += string.Format("Ответ от {0}: Время={1} мс, TTL={2}", pi.IP, pi.Delay, pi.TTL);
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
            Icon cIco;
            cIco = Properties.Resources.greenOk;

            switch (ico)
            {
                case ShowIcons.Arrow: cIco = Properties.Resources.Transfer; break;
                case ShowIcons.GreenOk: cIco = Properties.Resources.greenOk; break;
                case ShowIcons.Green:   cIco = Properties.Resources.Yellow; break;
                case ShowIcons.Yellow: cIco = Properties.Resources.Orange; break;
                case ShowIcons.Orange: cIco = Properties.Resources.DarkOrange ; break;
                case ShowIcons.Red: cIco = Properties.Resources.Red; break;
                case ShowIcons.RedMinus: cIco = Properties.Resources.redMinus; break;
                
            }
            try
            {
                TrayIcon.Icon = cIco;
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
       
        // Иконка в зависимости от времени задержки
        private void VisibleIconFromDelay(long Delay)
        {
            if (Delay == 0) ShowToolIcon(ShowIcons.Arrow);
            else if (Delay < 50) ShowToolIcon(ShowIcons.GreenOk);
            else if (Delay > 50 && Delay < 200) ShowToolIcon(ShowIcons.Green);
            else if (Delay > 200 && Delay < 500) ShowToolIcon(ShowIcons.Yellow);
            else if (Delay > 500 && Delay < 1000) ShowToolIcon(ShowIcons.Orange);
            else if (Delay > 1000 && Delay < 1500) ShowToolIcon(ShowIcons.Red);
            else if (Delay > 1500) ShowToolIcon(ShowIcons.RedMinus);
        }
        
        // Двойное нажатие на трей
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            TrayIcon.ShowBalloonTip(2000);
        }
        
        // Редактирование сервака и его сохранение
        private void button1_Click(object sender, EventArgs e)
        {
            if (!textBox1.Enabled)
            {
                button2.Enabled = true;
                textBox1.Enabled = true;
                button1.Image = (Properties.Resources.save).ToBitmap();
            }
            else
            {
               URL = textBox1.Text;
               Properties.Settings.Default.Server = URL;
               Properties.Settings.Default.Save();
               textBox1.Enabled = false;
               button2.Enabled = false;
               button1.Image = Properties.Resources.save_edit1;
            }
            
        }

        // Переход на сайт
        private void label2_Click(object sender, EventArgs e)
        {
            label2.Text = "Пасибки за переход :-)";
            System.Diagnostics.Process.Start("http://salactor.ru");
        }

        // Hover надписи перехода на сайт
        private void label2_MouseMove(object sender, MouseEventArgs e)
        { label2.ForeColor = Color.Blue; }
        private void label2_MouseLeave(object sender, EventArgs e)
        { label2.ForeColor = Color.Black; }

        private void TimerMaxDelay_Tick(object sender, EventArgs e)
        {
            if (!Error)
            {
                PrintLogPingText(new PingInformation("Запрос прерван из за таймаута"));
                ShowToolIcon(ShowIcons.RedMinus);
            }
            Error = false;
            TimerMaxDelay.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = URL;
            textBox1.Enabled = false;
            button2.Enabled = false;
            button1.Image = Properties.Resources.save_edit1;
        }
    }
}
