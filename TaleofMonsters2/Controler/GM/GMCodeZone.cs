using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NarlonLib.Core;

namespace TaleofMonsters.Controler.GM
{
    public static class GMCodeZone
    {
        private static List<string> lastHistory = new List<string>();
        private static int showIndex;
        private static string command = "";

        private static int lineOff; //光标的偏移，一般是<=0

        private static TimeCounter blinkCounter = new TimeCounter(0.5f);
        private static bool showLine;

        public static void OnFrame()
        {
            if (blinkCounter.OnTick())
            {
                showLine = !showLine;
                MainForm.Instance.RefreshView(); 
            }
        }

        public static void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                if (command.Length > 0 && command.Length > command.Length + lineOff - 1)
                    command = command.Remove(command.Length + lineOff - 1, 1);
            }
            else if (e.KeyCode == Keys.Delete)
            {
                if (command.Length > command.Length + lineOff)
                {
                    command = command.Remove(command.Length + lineOff, 1);
                    lineOff ++;
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                lineOff--;
                if (command.Length + lineOff < 0)
                {
                    lineOff++;
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                lineOff = Math.Min(lineOff + 1, 0);
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
                lineOff = 0;
                showIndex = lastHistory.Count;
            }
            else if (e.KeyCode ==  Keys.V && e.Modifiers == Keys.Control)
            {
                GMCommand.ParseCommand(command);
                lineOff = 0;
                command += Clipboard.GetText().Trim();
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (showIndex - 1 >= 0 && showIndex -1 < lastHistory.Count)
                {
                    showIndex--;
                    lineOff = 0;
                    command = lastHistory[showIndex];
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (showIndex + 1 >= 0 && showIndex + 1 < lastHistory.Count)
                {
                    showIndex++;
                    lineOff = 0;
                    command = lastHistory[showIndex];
                }
            }
            else
            {
                var charData = (char) e.KeyCode;
                if (charData >= 'a' && charData <= 'z' || charData >= 'A' && charData <= 'Z' ||
                    charData >= '0' && charData <= '9' || charData == ' ')
                {
                    command = command.Insert(command.Length+lineOff, charData.ToString());
                }
                command = command.ToLower();
            }
            MainForm.Instance.RefreshView();
        }

        public static void Paint(Graphics g, int width, int height)
        {
            g.FillRectangle(Brushes.Blue, 0, height - 12, width, 12);
            string toDraw = ">" + command;

            Font ft = new Font("Arial", 9);
            if (command.Contains(" ") && GMCommand.IsCommand(command.Split(' ')[0]))
            {
                var idx = toDraw.IndexOf(" ");
                g.DrawString(toDraw.Substring(0, idx), ft, Brushes.Lime, 0, height - 14);
                var width1 = GetWidth(g, ft, toDraw.Substring(0, idx));
                g.DrawString(toDraw.Substring(idx, toDraw.Length-idx), ft, Brushes.White, width1, height - 14);
            }
            else
            {
                g.DrawString(toDraw, ft, Brushes.White, 0, height - 14);
            }
            
            if (showLine)
            {
                var width1 = GetWidth(g, ft, toDraw.Substring(0, toDraw.Length + lineOff));
                g.DrawString("|", ft, Brushes.Lime, width1 - 4, height - 15);
            }
            ft.Dispose();
        }

        private static float GetWidth(Graphics g, Font ft, string s)
        {
            return g.MeasureString(s.Replace(' ', '2'), ft).Width;
        }
    }
}