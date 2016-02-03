using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.DataType.NPCs
{
    class TalkWord
    {
        private int target;
        private string word;
        private Dictionary<string, string> parameter = new Dictionary<string, string>();

        public int Target
        {
            get { return target; }
            set { target = value; }
        }

        public string Word
        {
            get { return word; }
            set { word = value; }
        }

        public TalkWord(string s, int tar)
        {
            target = tar;
            if (s.Contains("["))
            {
                word = s.Substring(s.IndexOf(']') + 1);
                string p = s.Substring(1, s.IndexOf(']') - 1);
                string[] datas;
                if (!p.Contains("&"))
                {
                    datas = new string[1];
                    datas[0] = p;
                }
                else
                {
                    datas = p.Split('&');
                }
                foreach (string data in datas)
                {
                    parameter.Add(data.Substring(0, 3), data.Substring(3));
                }
            }
            else
            {
                word = s;
            }
        }

        public bool IsAvail()
        {
            if (parameter.ContainsKey("Ned")) //检查对话前提
            {
                string[] pa = parameter["Ned"].Split('-');
                if (UserProfile.InfoTask.GetTaskStateById(int.Parse(pa[0])) != byte.Parse(pa[1]))
                    return false;
            }
            return true;
        }

        public void CheckParameter()
        {
            if (parameter.ContainsKey("Asp"))
            {
                string[] pa = parameter["Asp"].Split('-');
            //    UserProfile.InfoBasic.AddSkillValueById(int.Parse(pa[0]), int.Parse(pa[1]));
            }
        }

        public void CheckAction(out string type, out int data)
        {
            type = "";
            data = 0;
            //如果有多个动作就有优先级问题
            if (parameter.ContainsKey("Shp"))
            {
                type = "Shp";
                data = int.Parse(parameter["Shp"]);
            }
            else if (parameter.ContainsKey("Maz"))
            {
                type = "Maz";
                data = int.Parse(parameter["Maz"]);
            }
            else if (parameter.ContainsKey("Upd"))
            {
                string[] pa = parameter["Upd"].Split('-');

                if (pa[1] == "1")//接收任务
                {
                    type = "Tbe";
                }
                else if (pa[1] == "3")
                {
                    type = "Ted";
                }
                data = int.Parse(pa[0]);
            }
            else if (parameter.ContainsKey("Upw"))//任务杀怪
            {
                type = "Tbe";

                string[] pa = parameter["Upw"].Split('-');
                data = int.Parse(pa[0]);
            }
            else if (parameter.ContainsKey("Mon") && !parameter.ContainsKey("Upw")) //切磋
            {
                type = "Mon";

                string[] pa = parameter["Mon"].Split('-');
                data = int.Parse(pa[0]);
            }
            else if (parameter.ContainsKey("Job"))
            {
                type = "Job";
            }
        }

        private string GetIcon()
        {
            string iconname = "";
            if (parameter.ContainsKey("Wap"))
            {
                iconname = "abl3";
            }
            else if (parameter.ContainsKey("Upd"))
            {
                string[] pa = parameter["Upd"].Split('-');
                iconname = string.Format("npc{0}", pa[1]);
            }
            else if (parameter.ContainsKey("Upw"))
            {
                iconname = "npc3";
            }
            else if (parameter.ContainsKey("Mon"))
            {
                iconname = "abl1";
            }
            else if (parameter.ContainsKey("Shp"))
            {
                iconname = "npc4";
            }
            else if (parameter.ContainsKey("Maz"))
            {
                iconname = "npc5";
            }
            else if (parameter.ContainsKey("Job"))
            {
                iconname = "abl7";
            }
            return iconname;
        }

        public void Draw(Graphics g, int x, int y, bool selected)
        {
            string iconname = GetIcon();

            if (selected)
                g.FillRectangle(Brushes.DarkBlue, x, y, 258, 20);
            Font font = new Font("宋体", 10*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            if (iconname == "")
            {
                g.DrawString(word, font, Brushes.White, x, y + 4);
            }
            else
            {
                g.DrawImage(Core.HSIcons.GetIconsByEName(iconname), x, y + 1, 18, 18);
                g.DrawString(word, font, Brushes.White, x+20, y + 4);
            }
            font.Dispose();
        }

    }
}
