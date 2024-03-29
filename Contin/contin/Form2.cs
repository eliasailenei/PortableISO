﻿using contin;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using CustomConfig;


namespace mainUI
{
    public partial class Form2 : Form
    {
        SQLCheck sql;
        DriveLetters drive;
        bool auto;
        private Rectangle lab1, lab2, lab3, lab4, lab5, but1, but2;
       public string topass { get; set; }
        public string language { get; set; }
       private string[] disk = Environment.GetCommandLineArgs();


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Opacity == 1)
            {
                timer1.Stop();
            }
            Opacity += .4;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try {
                if (this.IsHandleCreated)
                {
                    if (Opacity == 0)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            this.Hide();
                        });
                    }
                    else
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            Opacity -= 0.4;
                        });
                    }
                }
            } catch
            {
                
            }
            
        }



        private void Form2_Load(object sender, EventArgs e)
        {
            if (sql.xmlStatus())
            {
                auto = true;
                button2_Click(sender,e);
            }
        }

        private Size form;

        public Form2(SQLCheck sqls, DriveLetters drives)
        {
            this.sql = sqls;
            this.drive = drives;
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            this.Resize += Form2_rsize;
            form = this.Size;
            form = this.Size;
            lab1 = new Rectangle(label1.Location, label1.Size);
            lab2 = new Rectangle(label2.Location, label2.Size);
            lab3 = new Rectangle(label3.Location, label3.Size);
            lab4 = new Rectangle(label4.Location, label4.Size);
            lab5 = new Rectangle(label5.Location, label5.Size);
            but1 = new Rectangle(button1.Location, button1.Size);
            but2 = new Rectangle(button2.Location, button2.Size);
        }
        private void Form2_rsize(object sender, EventArgs e)
        {
            resizeControl(lab1, label1);
            resizeControl(lab2, label2);
            resizeControl(lab3, label3);
            resizeControl(lab4, label4);
            resizeControl(lab5, label5);
            resizeControl(but1, button1);
            resizeControl(but2, button2);
        }
        private void resizeControl(Rectangle r, Control c)
        {
            float xRatio = (float)this.Width / form.Width;
            float yRatio = (float)this.Height / form.Height;

            int newX = (int)(r.X * xRatio);
            int newY = (int)(r.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult cleans = MessageBox.Show("Are you sure you want to go clean? It will be a regular install except a POST script to clean up drives...", "Option", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (cleans == DialogResult.Yes)
            {
                File.WriteAllText(Environment.SystemDirectory + "\\done.txt", "done");
                MessageBox.Show("Thank you for using PortableISO");
                Process.Start(drive.TLetter.ToString() + ":\\contin\\DeployWindows.exe",  "topass='" + topass + "' disks='" + disk[1] + "' isExpress='False'");
            }
            else
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            DialogResult clean = DialogResult.No;
            if (auto)
            {
                clean = DialogResult.Yes;
            } else
            {
                clean = MessageBox.Show("Are you sure you want to go express? Its a quick and easy way but you are not guaranteed to have similar results due to different Windows version. Use at risk!", "Option", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            if (clean == DialogResult.Yes)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = drive.TLetter.ToString() + ":\\contin\\DeployWindows.exe",
                        Arguments =  "topass='" + topass + "' disks='" + disk[1] + "' isExpress='True'",
                        UseShellExecute = true
                    }
                };
                process.Start();
                Thread formThread = new Thread(() =>
                {
                    CustomDownload customDownload = new CustomDownload(sql, drive);
                timer2.Start();
                    customDownload.language = language;
                customDownload.ShowDialog();
                });
                formThread.SetApartmentState(ApartmentState.STA); 
                formThread.Start();
            }
            else
            {
            }
        }
    }
}
