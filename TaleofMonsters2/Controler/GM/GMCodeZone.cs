using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TaleofMonsters.Controler.GM
{
    public static class GMCodeZone
    {
        private static List<string> lastHistory = new List<string>();
        private static int showIndex;
        private static string command = "";
        public static void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                if (command.Length > 0) command = command.Substring(0, command.Length - 1);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                GMCommand.ParseCommand(command);
                if (!lastHistory.Contains(command))
                {
                    lastHistory.Add(command);
                    if (lastHistory.Count >= 10)
                        lastHistory.RemoveAt(0);
                }
                command = "";
                showIndex = lastHistory.Count;
            }
            else if (e.KeyCode ==  Keys.V && e.Modifiers == Keys.Control)
            {
                GMCommand.ParseCommand(command);
                command += Clipboard.GetText().Trim();
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (showIndex - 1 >= 0 && showIndex -1 < lastHistory.Count)
                {
                    showIndex--;
                    command = lastHistory[showIndex];
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (showIndex + 1 >= 0 && showIndex + 1 < lastHistory.Count)
                {
                    showIndex++;
                    command = lastHistory[showIndex];
                }
            }
            else
            {
                command += (char) e.KeyCode;
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