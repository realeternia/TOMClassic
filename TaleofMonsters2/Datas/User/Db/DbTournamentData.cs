using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Battle;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Forms.TourGame;

namespace TaleofMonsters.Datas.User.Db
{
    public class DbTournamentData
    {
        [FieldIndex(Index = 1)] public int Id;
        [FieldIndex(Index = 2)] public bool Engage;
        [FieldIndex(Index = 3)] public int[] Pids;
        [FieldIndex(Index = 4)] public MatchResult[] Results;

        private int currentMid;
        private int currentRvId;
        private bool currentAutoGiveUp;

        public DbTournamentData()
        {
        }

        public DbTournamentData(int tid)
        {
            Id = tid;
            TournamentConfig tournamentConfig = ConfigData.GetTournamentConfig(tid);
            Pids = new int[tournamentConfig.PlayerCount];
            Results = new MatchResult[tournamentConfig.MatchCount];
        }

        public void CheckMatch(int mid, bool autoGiveup)
        {
            TournamentConfig tournamentConfig = ConfigData.GetTournamentConfig(Id);

            TournamentMatchConfig match = ConfigData.GetTournamentMatchConfig(mid);
            int left;
            int right;
            if (match.LeftType == 1)
                left = Pids[match.LeftValue];
            else
                left = Results[match.LeftValue].Winner;
            if (match.RightType == 1)
                right = Pids[match.RightValue];
            else
                right = Results[match.RightValue].Winner;

            currentMid = mid;
            currentAutoGiveUp = autoGiveup;
            currentRvId = left == -1 ? right : left;

            Results[mid] = new MatchResult();
            if (left != -1 && right != -1)
            {
                FastBattle.Instance.StartGame(left, right, tournamentConfig.Map, -1);
                Results[mid].Winner = FastBattle.Instance.LeftWin ? left : right;
                Results[mid].Loser = FastBattle.Instance.LeftWin ? right : left;
            }
            else
            {
                PeopleBook.Fight(currentRvId, tournamentConfig.Map, 1, new PeopleFightParm(), OnWin, OnLose, null);
            }
        }

        private void OnWin()
        {
            if (!currentAutoGiveUp)
            {
                Results[currentMid].Winner = -1;
                Results[currentMid].Loser = currentRvId;
            }
        }

        private void OnLose()
        {
            Results[currentMid].Winner = currentRvId;
            Results[currentMid].Loser = -1;
        }

        public List<int[]> GetRanks()
        {
            List<int[]> ranks = new List<int[]>();
            for (int i = 0; i < Pids.Length; i++)
            {
                int[] dat = new int[] {0, 0, 0, 0};
                dat[0] = Pids[i];
                foreach (MatchResult matchResult in Results)
                {
                    if (matchResult.Winner == dat[0])
                    {
                        dat[1]++;
                        dat[2]++;
                        dat[3] += 3;
                    }
                    else if (matchResult.Loser == dat[0])
                    {
                        dat[1]++;
                    }
                }
                ranks.Add(dat);
            }
            ranks.Sort(new CompareByWin());
            return ranks;
        }

        public void Award()
        {
            List<int[]> ranks = GetRanks();
            TournamentConfig tournamentConfig = ConfigData.GetTournamentConfig(Id);
            for (int i = 0; i < tournamentConfig.Awards.Length; i++)
                UserProfile.InfoWorld.UpdatePeopleRank(ranks[i][0], tournamentConfig.Awards[i]);

            for (int i = 0; i < tournamentConfig.Resource.Count; i++)
            {
                if (ranks[i][0] == -1)
                {
                    if (tournamentConfig.Resource[i].Id == 99)
                    {
                        UserProfile.InfoBag.AddDiamond(tournamentConfig.Resource[i].Value);
                    }
                    else
                    {
                        UserProfile.InfoBag.AddResource((GameResourceType) tournamentConfig.Resource[i].Id,
                            (uint) tournamentConfig.Resource[i].Value);
                    }
                }
            }

            Engage = false;
        }

        private class CompareByWin : IComparer<int[]>
        {
            #region IComparer<int[]> ≥…‘±

            public int Compare(int[] x, int[] y)
            {
                if (x[2] != y[2])
                    return y[2] - x[2];
                if (x[1] != y[1])
                    return x[1] - y[1];
                return x[0] - y[0];
            }

            #endregion
        }
    }

}
