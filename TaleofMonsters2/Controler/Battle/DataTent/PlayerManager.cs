using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.Players;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class PlayerManager
    {
        public Player LeftPlayer { get; set; }
        public Player RightPlayer { get; set; }

        public void Init(int left, int right, int rlevel)
        {
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(right);
            if (left == 0)
            {
                switch (peopleConfig.Method)
                {
                    case "common": LeftPlayer = new HumanPlayer(true); break;
                    case "rand": LeftPlayer = new RandomPlayer(right, true, true); break;
                    default: LeftPlayer = new AIPlayer(right, peopleConfig.Method, true, rlevel, true); break;
                }
            }
            else //观看比赛
            {
                LeftPlayer = new AIPlayer(left, ConfigData.GetPeopleConfig(left).Emethod, true, rlevel, false);
            }

            switch (peopleConfig.Emethod)
            {
                case "common": RightPlayer = new HumanPlayer(false); RightPlayer.PeopleId = right; break;
                case "rand": RightPlayer = new RandomPlayer(right, false, false); break;
                case "mirror": RightPlayer = new MirrorPlayer(right, LeftPlayer.Cards, false); break;
                default: RightPlayer = new AIPlayer(right, peopleConfig.Emethod, false, rlevel, false); break;
            }
        }

        public void Clear()
        {
            LeftPlayer = null;
            RightPlayer = null;
        }

        public void Update(bool isFast, float pastRound, int round)
        {
            LeftPlayer.Update(isFast, pastRound, round);
            RightPlayer.Update(isFast, pastRound, round);
        }

        public void CheckRoundCard()
        {
            LeftPlayer.CardManager.GetNextCard();
            if (LeftPlayer.SpikeManager.HasSpike("doublecard"))
            {
                LeftPlayer.CardManager.GetNextCard();
            }
            RightPlayer.CardManager.GetNextCard();
            if (RightPlayer.SpikeManager.HasSpike("doublecard"))
            {
                RightPlayer.CardManager.GetNextCard();
            }
        }
    }
}
