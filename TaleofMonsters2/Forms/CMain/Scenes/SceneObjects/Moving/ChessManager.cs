using System.Collections.Generic;
using System.Drawing;
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

        public void OnChangeMap()
        {
            if (ChessList.Count > 1)
                ChessList.RemoveAt(1);
            ChessList.Add(new ChessItem { PeopleId = 1, CellId = Scene.Instance.SceneInfo.GetRandom(0, false) }); //把一个机器人放到随机位置
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
            if (myChess != null)
            {
                myChess.Time = ChessItem.ChessMoveAnimTime;
                myChess.Source = srcP;
                myChess.Dest = destP;
                myChess.DestId = dest.Id;
            }
            else
            {
                ChessList.Add(new ChessItem {PeopleId = peopleId, Source = srcP, Dest = destP, DestId = dest.Id, Time = ChessItem.ChessMoveAnimTime });
            }
        }

        public void OnChessPlayerMoved()
        {
            int index = 1;
            foreach (var chessItem in ChessList)
            {
                if (chessItem.PeopleId == 0)
                    continue;

                SetChessState(chessItem.PeopleId, chessItem.CellId, Scene.Instance.SceneInfo.GetRandom(0, false));

                index++;
            }
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