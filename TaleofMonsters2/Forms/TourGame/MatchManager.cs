using System.Drawing;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.DataType.User;
using System.Windows.Forms;

namespace TaleofMonsters.Forms.TourGame
{
    internal class MatchManager
    {
        private static Image head;

        public static void DrawCrossing(Graphics g, int x1, int y1, int x2, int y2)
        {
            g.DrawLine(Pens.White, (x1 + x2)/2, (y1 + y2)/2, x2, (y1 + y2)/2);
            g.DrawLine(Pens.White, (x1 + x2)/2, y1, (x1 + x2)/2, y2);
            g.DrawLine(Pens.White, (x1 + x2)/2, y1, x1, y1);
            g.DrawLine(Pens.White, (x1 + x2)/2, y2, x1, y2);
        }

        public static Image GetHeadImage(int pid)
        {
            if (pid > 0)
            {
                return PeopleBook.GetPersonImage(pid);
            }
            if(pid == -1)
            {
                if (head == null)
                {
                    head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Head));
                }
                return head;
            }
            return null;
        }

        public static string GetPlayerName(int pid)
        {
            if (pid > 0)
            {
                return ConfigDatas.ConfigData.GetPeopleConfig(pid).Name;
            }
            if (pid == -1)
            {
                return UserProfile.Profile.Name;
            }
            return "";
        }

        public static Color GetNameColor(int winner, int pid)
        {
            if (winner!=-0)
            {
                if (winner == pid)
                {
                    return Color.Red;
                }
                return Color.DimGray;
            }
            if (pid==-1)
            {
                return Color.Lime;
            }
            return Color.White;
        }

        public static Button GetButton(int id, int x, int y)
        {
            Button btn = new Button();
            btn.Location = new Point(x, y);
            btn.Size = new Size(75, 23);
            btn.FlatStyle = FlatStyle.Flat;
            btn.Tag = id.ToString();
            btn.ForeColor = Color.Lime;
            btn.Text = @"Ä£Äâ";
            return btn;
        }
    }
}
