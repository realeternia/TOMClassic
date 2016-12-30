using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Maps;

namespace TaleofMonsters.DataType.Tournaments
{
    internal static class TournamentBook
    {
        public static int GetDayApplyId(int date)
        {
            foreach (TournamentConfig tournamentConfig in ConfigData.TournamentDict.Values)
            {
                if (tournamentConfig.ApplyDate == date)
                    return tournamentConfig.Id;
            }
            return 0;
        }

        public static int GetDayMatchId(int date)
        {
            foreach (TournamentConfig tournamentConfig in ConfigData.TournamentDict.Values)
            {
                if (tournamentConfig.BeginDate <= date && tournamentConfig.EndDate >= date)
                    return tournamentConfig.Id;
            }
            return 0;
        }

        public static int[] GetTournamentMatchIds(int id)
        {
            List<int> ids=new List<int>();
            foreach (TournamentMatchConfig tournamentMatchConfig in ConfigData.TournamentMatchDict.Values)
            {
                if (tournamentMatchConfig.Tid == id)
                    ids.Add(tournamentMatchConfig.Id);
            }
            return ids.ToArray();
        }

        public static Image GetPreview(int id)
        {
            TournamentConfig tournamentConfig = ConfigData.GetTournamentConfig(id);

            int[] awardData = tournamentConfig.Awards;
            RLIdValueList resourceData = tournamentConfig.Resource;

            int wid = 175;
            int heg = awardData.Length * 16;
            wid += 5; heg += 5;
            heg += 16 * 4 + 50;
            Bitmap bmp = new Bitmap(wid, heg);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.Black, 0, 0, wid, heg);
            g.FillRectangle(new SolidBrush(Color.FromArgb(30, 30, 30)), 0, 0, wid, 18);
            Pen pen = new Pen(Brushes.Gray, 2);
            g.DrawRectangle(pen, 1, 1, wid - 3, heg - 3);
            pen.Dispose();

            int y = 3;
            Font fontsong = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontsongB = new Font("宋体", 9*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString("赛事名", fontsongB, Brushes.LightGray, 3, y);
            g.DrawString(tournamentConfig.Name, fontsong, Brushes.White, 53, y);
            y += 16;
            g.DrawString("比赛时间", fontsongB, Brushes.LightGray, 3, y);

            //     g.DrawString(string.Format("{0}-{1}", HSTime.GetByDate(begin_date).ToShortString(), HSTime.GetByDate(end_date).ToShortString()), fontsong, Brushes.LightGreen, 66, y);
            y += 16;
            g.DrawString("比赛地图", fontsongB, Brushes.LightGray, 3, y);
            Image mapback = BattleMapBook.GetMapImage(tournamentConfig.Map, TileConfig.Indexer.DefaultTile);
            g.DrawImage(mapback, new Rectangle(70, y, 100, 50), new Rectangle(0, 25, 100, 50), GraphicsUnit.Pixel);
            mapback.Dispose();
            y += 50;
            g.DrawString("等级限制", fontsongB, Brushes.LightGray, 3, y);
            g.DrawString(string.Format("{0}-{1}", tournamentConfig.MinLevel, tournamentConfig.MaxLevel), fontsong, Brushes.LightPink, 66, y);
            y += 16;
            g.DrawString("比赛奖励", fontsongB, Brushes.LightGray, 3, y);
            y += 16;
            for (int i = 0; i < awardData.Length; i++)
            {
                g.DrawString(string.Format("第{0}名 {1}积分", i + 1, awardData[i].ToString().PadLeft(2, ' ')), fontsong, Brushes.White, 33, y);
                if (i < resourceData.Count)
                {
                    if (resourceData[i].Id == 99)
                    {
                        g.DrawString(resourceData[i].Value.ToString().PadLeft(2, ' '), fontsong, Brushes.Cyan, 123, y);
                        g.DrawImage(HSIcons.GetIconsByEName("res8"), 143, y, 16, 16);
                    }
                    else
                    {
                        Brush brush = new SolidBrush(Color.FromName(HSTypes.I2ResourceColor(resourceData[i].Id)));
                        g.DrawString(resourceData[i].Value.ToString().PadLeft(2, ' '), fontsong, brush, 123, y);
                        g.DrawImage(HSIcons.GetIconsByEName("res" + (resourceData[i].Id + 1)), 143, y, 14, 14);
                        brush.Dispose();
                    }
                }
                y += 16;
            }
            fontsong.Dispose();
            fontsongB.Dispose();
            g.Dispose();
            return bmp;
        }
    }

    internal class CompareTournamentByADay : IComparer<int>
    {
        #region IComparer<int> 成员

        public int Compare(int a, int b)
        {
            TournamentConfig ta = ConfigData.GetTournamentConfig(a);
            TournamentConfig tb = ConfigData.GetTournamentConfig(b);
            return ta.ApplyDate.CompareTo(tb.ApplyDate);
        }

        #endregion
    }
}
