using System;
using System.Drawing;
using System.Windows.Forms;

namespace TaleofMonsters.Controler.GM
{
    public static class GMCodeZone
    {
        private static string command = "";
        public static void OnKeyDown(Keys key)
        {
            if (key == Keys.Back)
            {
                if (command.Length > 0) command = command.Substring(0, command.Length - 1);
            }
            else if (key == Keys.Enter)
            {
                GMCommand.ParseCommand(command);
                command = "";
            }
            else
            {
                command += (char)key;
                command = command.ToLower();
            }
            MainForm.Instance.RefreshView();
        }

        public static void Paint(Graphics g, int width, int height)
        {
            g.FillRectangle(Brushes.Blue, 0, height - 12, width, 12);
            Font ft = new Font("Arial", 9);
            g.DrawString("//" + command, ft, Brushes.White, 0, height - 14);
            ft.Dispose();
        }
    }
}