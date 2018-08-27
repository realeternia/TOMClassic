using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Tools;
using TaleofMonsters.Core;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Forms.CMain.Scenes.SceneObjects.Moving
{
    internal class ChessManager
    {
        public List<ChessItem> ChessList = new List<ChessItem>();
        private List<int> chessIdList = new List<int>(); //可能出现在这个地图的pid
        private int nowMapId;
        
        public ChessManager()
        {
            ChessList.Add(new ChessItemPlayer()); //玩家自己
        }

        public void OnChangeMap(int mapId, bool isWarp)
        {
            nowMapId = mapId;
               chessIdList = PeopleBook.GetPeopleChessesOnScene(mapId);
            if (!isWarp) //读档
            {
                foreach (var chessPo in UserProfile.Profile.InfoWorld.ChessPos)
                {
                    ChessList.Add(new ChessItem
                    {
                        PeopleId = chessPo.Key,
                        CellId = chessPo.Value
                    });
                }
            }
            else
            {
                if (ChessList.Count > 1)
                    ChessList.RemoveRange(1, ChessList.Count - 1);

                foreach (var pid in GetNowPeopleList())
                {
                    ChessList.Add(new ChessItem
                    {
                        PeopleId = pid,
                        CellId = Scene.Instance.SceneInfo.GetRandom(0, false)
                    }); //把一个机器人放到随机位置
                }

                UserProfile.Profile.InfoWorld.SaveChessData(ChessList);
            }
        }

        private List<int> GetNowPeopleList()
        {
            List<int> peopleList = new List<int>();
            var totalSteps = UserProfile.InfoBasic.MoveCount;
            foreach (var chessId in chessIdList)
            {
                var chessConfig = ConfigData.GetPeopleChessConfig(chessId);

                int sum = 0;
                foreach (var i in chessConfig.BornSceneChecker)
                    sum += i;

                int adder = 0;
                var minor = (totalSteps + 1000 - chessConfig.BornSceneBeginer)%sum;
                for (int i = 0; i < chessConfig.BornSceneId.Length; i++)
                {
                    if (chessConfig.BornSceneId[i] == nowMapId)
                    {
                        if (minor < chessConfig.BornSceneChecker[i] + adder && minor >= adder)
                            peopleList.Add(chessConfig.Id);
                    }

                    adder += chessConfig.BornSceneChecker[i];
                }
            }
            return peopleList;
        }

        public int GetPeopleIdOnCell(int playerCellId)
        {
            foreach (var chessItem in ChessList)
            {
                if (chessItem.PeopleId == 0)
                    continue;

                if (chessItem.CellId == playerCellId)
                    return chessItem.PeopleId;
            }
            return 0;
        }

        public void OnChessPlayerMoved(int playerCellId)
        {
            if (UserProfile.InfoBasic.MoveCount % 5 == 0)
            {
                CheckChessChange();
            }
            foreach (var chessItem in ChessList)
            {
                if (chessItem.PeopleId == 0)
                    continue;

                if(chessItem.CellId == playerCellId) //和玩家在同一格子就不动了
                    continue;

                if (chessItem.IsEscaping && chessItem.StepLeft == 0) //准备退出地图
                    continue;

                var destCell = chessItem.NextMove();
                SetChessState(chessItem.PeopleId, chessItem.CellId, destCell);
            }

            UserProfile.Profile.InfoWorld.SaveChessData(ChessList);
        }

        private void CheckChessChange()
        {
            var realPeopleList = GetNowPeopleList();
            var nowPeopleList = ChessList.ConvertAll(item => item.PeopleId);
            nowPeopleList.Remove(0); //移除玩家
            var toAdd = ArraysUtils.ArrayNotIn(nowPeopleList, realPeopleList);
            var toRemove = ArraysUtils.ArrayNotIn(realPeopleList, nowPeopleList);
            foreach (var pid in toAdd)
            {
                ChessList.Add(new ChessItem
                {
                    PeopleId = pid,
                    CellId = Scene.Instance.SceneInfo.GetRandomWarp()
                }); //把一个机器人放到随机传送门
            }
            foreach (var pid in toRemove)
            {
                var pickChess = ChessList.Find(t => t.PeopleId == pid);
                if (!pickChess.IsEscaping)
                    pickChess.Escape();
            }
            ChessList.RemoveAll(chess => chess.PeopleId > 0 && chess.IsEscaping && chess.StepLeft == 0);
        }

        public bool IsChessMoving()
        {
            foreach (var chessItem in ChessList) //只要有一个旗子在动
                if (chessItem.IsMoving)
                    return true;
            return false;
        }

        internal void SetChessState(int peopleId, int start, int end)
        {
            SceneObject src = Scene.Instance.SceneInfo.GetCell(start);
            SceneObject dest = Scene.Instance.SceneInfo.GetCell(end);

            int drawWidth = 57 * src.Width / GameConstants.SceneTileStandardWidth;
            int drawHeight = 139 * src.Height / GameConstants.SceneTileStandardHeight;
            var srcP = new Point(src.X - drawWidth/2 + src.Width/8, src.Y - drawHeight + src.Height/3);
            var destP = new Point(dest.X - drawWidth/2 + dest.Width/8, dest.Y - drawHeight + dest.Height/3);

            var myChess = ChessList.Find(cs => cs.PeopleId == peopleId);
            if (myChess == null)
            {
                myChess = new ChessItem
                {
                    PeopleId = peopleId,
                };
                ChessList.Add(myChess);
            }
            myChess.Time = ChessItem.ChessMoveAnimTime;
            myChess.Source = srcP;
            myChess.Dest = destP;
            myChess.DestId = dest.Id;
            myChess.FormerDestId = start;
        }
        
        public void Draw(Graphics g)
        {
            foreach (var chessItem in ChessList)
            {
                chessItem.Draw(g);
            }
        }
    }
}