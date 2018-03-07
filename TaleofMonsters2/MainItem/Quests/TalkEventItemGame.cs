using System.Drawing;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemGame : TalkEventItem
    {
        private bool isEndGame = false;

        public TalkEventItemGame(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
        
        }

        public override void Init()
        {
            int gameId = config.MiniGameId;
            PanelManager.ShowGameWindow(gameId, hardness, OnResult);
        }

        private void OnResult(int value)
        {
            result = evt.ChooseTarget(value); //0,s,1,a,2,b,3,c
            isEndGame = true;
        }

        public override void OnFrame(int tick)
        {
            if (isEndGame)
            {
                RunningState = TalkEventState.Finish;
                isEndGame = false;
            }
        }
    }
}

