using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace FileSync
{
    public partial class Form1 : Form
    {
        public delegate void SetValCallback(int val);

        private bool isStartUpdate;
        private void SetVal(int val)
        {
            dataGet += val;
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.progressBar1.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.progressBar1.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.progressBar1.Disposing || this.progressBar1.IsDisposed)
                        return;
                }
                SetValCallback d = new SetValCallback(SetVal);
                this.progressBar1.Invoke(d, new object[] { (int)(dataGet * 100 / dataTotal )});
            }
            else
            {
                if(!isStartUpdate)
                    return;
                this.progressBar1.Value = (int)(dataGet*100/ dataTotal);
                if (DateTime.Now.Subtract(beginTime).TotalSeconds > 5 && dataGet > 0)
                {
                    var secondsNeed = DateTime.Now.Subtract(beginTime).TotalSeconds*(dataTotal- dataGet )/ dataGet;
                    label2.Text = "剩余时间:" + new DateTime().AddSeconds(secondsNeed).ToString("HH:mm:ss");
                }
                else
                {
                    label2.Text = "剩余时间估算中";
                }
            }
        }

        private void DownloadFinish(int val)
        {
            dataGet += val;
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.progressBar1.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.progressBar1.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.progressBar1.Disposing || this.progressBar1.IsDisposed)
                        return;
                }
                SetValCallback d = new SetValCallback(DownloadFinish);
                this.progressBar1.Invoke(d, new object[] { 1 });
            }
            else
            {
                this.progressBar1.Value = 100;
                label2.Hide();
                label1.Text = "更新完成!";
            }
        }

        public delegate void SetTextCallback(string val);

        private void SetLabel1Text(string val)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (this.label1.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!this.label1.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (this.label1.Disposing || this.label1.IsDisposed)
                        return;
                }
                SetTextCallback d = new SetTextCallback(SetLabel1Text);
                this.label1.Invoke(d, new object[] { val });
            }
            else
            {
                this.label1.Text = val;
            }
        }

        private Thread workThread;
        private long dataGet;
        private long dataTotal;
        private DateTime beginTime;
        private Dictionary<string,string> fileMd5Dict = new Dictionary<string, string>(); 

        public Form1()
        {
            InitializeComponent();

       
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!isStartUpdate)
                return;

            if (workThread == null)
            {
                button1.Enabled = false;
                beginTime = DateTime.Now;
                workThread = new Thread(Work);
                workThread.IsBackground = true;
                workThread.Start();
            }
        }

        private void Work()
        {
            FtpTool tool = new FtpTool("narlon.cn", "TOMClassic", "anonymous", "anonymous");
            foreach (var fileInfo in fileMd5Dict)
            {
                if (File.Exists("./" + fileInfo.Key) && Md5Helper.GetMD5WithFilePath("./" + fileInfo.Key) == fileInfo.Value)
                    continue;
                Download(tool, fileInfo.Key);
            }
            DownloadFinish(1);
        }

        private void Download(FtpTool tool, string path)
        {
            if (isStartUpdate)
                SetLabel1Text("开始下载" + path);
            tool.Download(path, SetVal);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "TaleofMonsters2.exe"; //启动的应用程序名称  
            Process.Start(startInfo);
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            
            FtpTool tool = new FtpTool("narlon.cn", "TOMClassic", "anonymous", "anonymous");
            Download(tool, "check.txt");
            StreamReader sr = new StreamReader("./check.txt");
            string line;
            dataTotal =long.Parse(sr.ReadLine()); //先读包大小
            while ((line = sr.ReadLine())!=null)
            {
                string[] datas = line.Split('\t');
                fileMd5Dict[datas[0]] = datas[1];
            }
            sr.Close();
            label4.Text = dataTotal.ToString();

            var needUpdate = false;
            foreach (var fileInfo in fileMd5Dict)
            {
                if (File.Exists("./" + fileInfo.Key) && Md5Helper.GetMD5WithFilePath("./" + fileInfo.Key) == fileInfo.Value)
                    continue;
                needUpdate = true;
            }

            if (needUpdate)
            {
                isStartUpdate = true;
                button1.Enabled = true;
            }
            else
            {
                button1.Text = "已经最新";
            }
        }
    }
}
