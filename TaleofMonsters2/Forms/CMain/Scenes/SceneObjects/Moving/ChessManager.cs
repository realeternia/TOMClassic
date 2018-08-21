using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Core;

namespace TaleofMonsters.Forms.CMain.Scenes.SceneObjects.Moving
{
    public class ChessManager
    {
        public List<ChessItem> ChessList = new List<ChessItem>();

        public ChessManager()
        {
            ChessList.Add(new ChessItemPlayer()); //玩家自己
        }

        public void OnChangeMap(int mapId)
        {
            if (ChessList.Count > 1)
                ChessList.RemoveRange(1, ChessList.Count-1);
            foreach (var pConfig in ConfigData.PeopleDict.Values)
            {
                if (pConfig.BornSceneId == mapId)
                    ChessList.Add(new ChessItem { PeopleId = pConfig.Id, CellId = Scene.Instance.SceneInfo.GetRandom(0, false) }); //把一个机器人放到随机位置
            }
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
            foreach (var chessItem in ChessList)
            {
                if (chessItem.PeopleId == 0)
                    continue;

                if(chessItem.CellId == playerCellId) //和玩家在同一格子就不动了
                    continue;

                SetChessState(chessItem.PeopleId, chessItem.CellId, Scene.Instance.SceneInfo.GetNearRandom(chessItem.CellId, chessItem.FormerDestId));
            }
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