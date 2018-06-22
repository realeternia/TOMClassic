using System.Collections.Generic;
using System.Drawing;

namespace TaleofMonsters.Forms.CMain.Scenes.SceneObjects.Moving
{
    public class ChessManager
    {
        public List<ChessItem> ChessList = new List<ChessItem>();

        public ChessManager()
        {
            ChessList.Add(new ChessItem()); //玩家自己
        }

        public bool IsChessMoving()
        {
            foreach (var chessItem in ChessList) //只要有一个旗子在动
                if (chessItem.Time > 0) return true;
            return false;
        }

        public void SetChessState(int peopleId, Point src, Point dest, int destId)
        {
            var myChess = ChessList.Find(cs => cs.PeopleId == peopleId);
            if (myChess != null)
            {
                myChess.Time = ChessItem.ChessMoveAnimTime;
                myChess.Source = src;
                myChess.Dest = dest;
                myChess.DestId = destId;
            }
            else
            {
                ChessList.Add(new ChessItem {PeopleId = peopleId, Source = src, Dest = dest, DestId = destId, Time = ChessItem.ChessMoveAnimTime });
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