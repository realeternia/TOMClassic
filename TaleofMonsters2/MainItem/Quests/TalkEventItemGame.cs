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
            int gameId = config.MiniGameId;
            PanelManager.ShowGameWindow(gameId, OnWin, OnFail);
        }

        private void OnFail()
        {
            result = evt.ChooseTarget(0);
            isEndGame = true;
        }

        private void OnWin()
        {
            result = evt.ChooseTarget(1);
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

