using System.Collections.Generic;
using System.IO;
using System.Text;
using GameServer.Tools;
using JLM.NetSocket;

namespace GameServer.Logic
{
    public class RankManager
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


        private List<RankData> rankList = new List<RankData>();
        private const int RankLimit = 30;

        public RankManager()
        {
            if (!Directory.Exists("./Rank"))
                Directory.CreateDirectory("./Rank");
            Load();
        }

        public void RpcUpdateLevelExp(string name, int job, int level, int exp)
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

            rankList.Sort(new CompareByLevelExp());
            if (rankList.Count > RankLimit)
                rankList.RemoveAt(rankList.Count-1);

            Save();
        }

        private void Load()
        {
            StreamReader sr = new StreamReader("./Rank/level.txt");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var datas = line.Split('\t');
                rankList.Add(new RankData {Name = datas[0], Job = int.Parse(datas[1]), Level = int.Parse(datas[2]), Exp = int.Parse(datas[3])});
            }
            sr.Close();

            Logger.Log("Load rank count=" + rankList.Count);
        }

        private void Save()
        {
            StreamWriter sw = new StreamWriter("./Rank/level.txt", false, Encoding.UTF8);
            foreach (var rankData in rankList)
            sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}", rankData.Name, rankData.Job, rankData.Level, rankData.Exp));
            sw.Close();
        }

        public void RpcGetRank(GamePlayer player, int type)
        {
            player.S2C.GetRankResult(rankList);
            Logger.Log("RpcGetRank " + player.Name);
        }
    }
}