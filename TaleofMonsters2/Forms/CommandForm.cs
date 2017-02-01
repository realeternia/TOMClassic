using System.Drawing;
using System.Windows.Forms;
using TaleofMonsters.Controler.GM;

namespace TaleofMonsters.Forms
{
    internal partial class CommandForm : Form
    {
        private string cmd;
        private string hint;
        public CommandForm()
        {
            InitializeComponent();
        }

        private void textBoxTitle_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if(e.KeyCode == Keys.Enter)
            {
                GMCommand.ParseCommand(textBoxTitle.Text);
                Close();
            }
        }

        private void textBoxTitle_TextChanged(object sender, System.EventArgs e)
        {
            cmd = "";
            if (textBoxTitle.Text.Contains("exp"))
            {
                cmd = "exp";
                hint = "提升角色[p1]点经验值";
            }
            if (textBoxTitle.Text.Contains("atp"))
            {
                cmd = "atp";
                hint = "提升角色[p1]点阅历";
            }
            if (textBoxTitle.Text.Contains("emys"))
            {
                cmd = "emys";
                hint = "使所有角色进入对战列表";
            }
            if (textBoxTitle.Text.Contains("mov"))
            {
                cmd = "mov";
                hint = "直接移动到场景[p1]";
            }
            if (textBoxTitle.Text.Contains("eqp"))
            {
                cmd = "eqp";
                hint = "获得装备[p1]";
            }
            if (textBoxTitle.Text.Contains("cad"))
            {
                cmd = "cad";
                hint = "获得卡片[p1]";
            }
            if (textBoxTitle.Text.Contains("itm"))
            {
                cmd = "itm";
                hint = "获得物品[p1]x[p2]";
            }
            if (textBoxTitle.Text.Contains("gold"))
            {
                cmd = "gold";
                hint = "获得黄金[p1]";
            }
            if (textBoxTitle.Text.Contains("res"))
            {
                cmd = "res";
                hint = "获得资源[p1]";
            }
            if (textBoxTitle.Text.Contains("dmd"))
            {
                cmd = "dmd";
                hint = "获得钻石[p1]";
            }
            if (textBoxTitle.Text.Contains("tsk"))
            {
                cmd = "tsk";
                hint = "接受任务[p1]";
            }
            if (textBoxTitle.Text.Contains("acv"))
            {
                cmd = "acv";
                hint = "实现成就[p1]";
            }
            if (textBoxTitle.Text.Contains("view"))
            {
                cmd = "view";
                hint = "观看比赛[p1]Vs[p2]";
            }
            if (textBoxTitle.Text.Contains("fbat"))
            {
                cmd = "fbat";
                hint = "快速战斗[p1]Vs[p2]";
            }
            if (textBoxTitle.Text.Contains("sceq"))
            {
                cmd = "sceq";
                hint = "场景事件[p1]";
            }
            if (textBoxTitle.Text.Contains("cure"))
            {
                cmd = "cure";
                hint = "回复所有的健康，食物和精神";
            }
            if (textBoxTitle.Text.Contains("bls"))
            {
                cmd = "bls";
                hint = "添加一个祝福/诅咒[p1]";
            }
            Invalidate();
        }

        private void CommandForm_Paint(object sender, PaintEventArgs e)
        {
            if (cmd == "")
            {
                return;
            }

            Font ft = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(cmd, ft, Brushes.Lime, 0, 23);
            e.Graphics.DrawString(hint, ft, Brushes.White, 30, 23);
        }
    }

}