using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items.Core;

namespace TaleofMonsters.Forms
{
    internal partial class QuestionForm : BasePanel
    {
        private ColorWordRegion colorWord;
        private string result;
        private string guess;

        public QuestionForm()
        {
            InitializeComponent();
            colorWord = new ColorWordRegion(21, 44, 274, new Font("微软雅黑", 11 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel), Color.White);
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");
            this.bitmapButtonC1.ImageNormal = PicLoader.Read("Button.Panel", "ButtonBack2.PNG");
            bitmapButtonC1.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonC1.ForeColor = Color.White;
            bitmapButtonC1.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("right");
            bitmapButtonC1.IconSize = new Size(16, 16);
            bitmapButtonC1.IconXY = new Point(4, 5);
            bitmapButtonC1.TextOffX = 8;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);

            Question qes = QuestionBook.GetQuestion();
            colorWord.UpdateText(qes.info);
            radioButton1.Text = qes.GetAns(0);
            radioButton2.Text = qes.GetAns(1);
            radioButton3.Text = qes.GetAns(2);
            radioButton4.Text = qes.GetAns(3);
            result = qes.GetResult();
            guess = qes.GetAns(0);
        }

        private void ConnectForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("幻兽问答", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            colorWord.Draw(e.Graphics);
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastQuestionTime, TimeTool.DateTimeToUnixTime(DateTime.Now) + GameConstants.QuestionCooldownDura);
            if (result == guess)
            {
                UserProfile.InfoBag.AddResource(GameResourceType.Gold, 20);
                MessageBoxEx.Show("回答正确，奖励20金币");
            }
            else
            {
                MessageBoxEx.Show("回答错误");
            }
            Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null) 
                guess = rb.Text;
        }
    }
}