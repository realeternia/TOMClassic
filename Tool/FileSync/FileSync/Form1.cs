using System;
using System.Threading;
using System.Windows.Forms;

namespace FileSync
{
    public partial class Form1 : Form
    {
        public delegate void SetValCallback(int val);

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
                this.progressBar1.Value = (int)(dataGet*100/ dataTotal);
                if (DateTime.Now.Subtract(beginTime).TotalSeconds > 5 && dataGet > 0)
                {
                    var secondsNeed = DateTime.Now.Subtract(beginTime).TotalSeconds*dataTotal/dataGet;
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
                label1.Hide();
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

        public Form1()
        {
            InitializeComponent();

            dataTotal = 60*1024*1024;
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
            Download(tool, "AltSerialize.dll");
            Download(tool, "BaseType.dll");
            Download(tool, "ConfigData.dll");
            Download(tool, "ControlPlus.dll");
            Download(tool, "DataResource.vfs");
            Download(tool, "fmod.dll");
            Download(tool, "log4net.dll");
            Download(tool, "NarlonLib.dll");
            Download(tool, "NLVFS.dll");
            Download(tool, "PicResource.vfs");
            Download(tool, "SoundResource.vfs");
            Download(tool, "TaleofMonsters2.exe");
            DownloadFinish(1);
        }

        private void Download(FtpTool tool, string path)
        {
            SetLabel1Text("开始下载" + path);
            tool.Download(path, SetVal);
        }
    }
}
