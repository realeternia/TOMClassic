using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameServer.Logic
{
    public class RankManager
    {
        class RankData
        {
            internal class CompareByLevelExp : IComparer<RankData>
            {
                #region IComparer<RankData> 成员

                public int Compare(RankData x, RankData y)
                {
                    if (y.Level != x.Level)
                        return x.Level.CompareTo(y.Level);

                    return x.Exp.CompareTo(y.Exp);
                }

                #endregion
            }
            public string Name;
            public int Job;
            public int Level;
            public int Exp;
        }

        private List<RankData> rankList = new List<RankData>();

        static RankManager()
        {
            if (!Directory.Exists("./Rank"))
                Directory.CreateDirectory("./Rank");
        }

        public void UpdateLevelExp(string name, int job, int level, int exp)
        {
            bool found = false;
            foreach (var rankData in rankList)
            {
                if (rankData.Name == name)
                {
                    rankData.Level = level;
                    rankData.Exp = exp;
                    rankData.Job = job;
                    found = true;
                    break;
                }
            }

            if (!found)
                rankList.Add(new RankData {Name = name, Job = job, Exp = exp, Level = level});

            rankList.Sort(new RankData.CompareByLevelExp());
            if (rankList.Count > 10)
                rankList.RemoveAt(rankList.Count-1);

            Save();
        }

        private void Save()
        {
            StreamWriter sw = new StreamWriter("./Rank/level.txt", false, Encoding.UTF8);
            foreach (var rankData in rankList)
            sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", rankData.Name, rankData.Job, rankData.Level, rankData.Exp));
            sw.Close();
        }
    }
}