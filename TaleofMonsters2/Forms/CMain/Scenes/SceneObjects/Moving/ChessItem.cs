using System;
using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.User;

namespace TaleofMonsters.Forms.CMain.Scenes.SceneObjects.Moving
{
    public class ChessItem
    {
        public const float ChessMoveAnimTime = 0.5f;//旗子跳跃的动画时间

        public int PeopleId { get; set; } //0表示玩家

        public float Time { get; set; } //经过时间
        public Point Source { get; set; } //移动源
        public Point Dest { get; set; } //移动目标

        public virtual int CellId { get; set; } //当前所在的id
        public int DestId { get; set; }

        public float MoveDelay { get; set; } //移动延迟，主要是npc需要
        protected float jumpHeight;

        public virtual bool IsMoving
        {
            get { return Time + MoveDelay > 0; }
        }

        public ChessItem()
        {
            jumpHeight = 30;
        }

        public bool TimeGo(float timePast, out bool needUpdate)
        {
            needUpdate = false;
            if (MoveDelay > 0)
            {
                MoveDelay = Math.Max(0, MoveDelay - timePast);
                if (MoveDelay > 0)
                    return false;
            }
           
            if (Time > 0)
            {
                Time = Math.Max(0, Time - timePast);
                needUpdate = true;

                if (Time <= 0)
                {
                    Time = 0;
                    CellId = DestId;
                    return true;
                }
            }
            return false;
        }
        public Point GetPosPredict()
        {
            var possessCell = Scene.Instance.SceneInfo.GetCell(CellId);

            int drawWidth = 100 * possessCell.Width / GameConstants.SceneTileStandardWidth;
            int drawHeight = 100 * possessCell.Height / GameConstants.SceneTileStandardHeight;
            int realX = 0;
            int realY = 0;
            if (Time <= 0)
            {
                realX = possessCell.X - drawWidth / 2 + possessCell.Width / 8;
                realY = possessCell.Y - drawHeight + possessCell.Height / 3;
            }
            else
            {
                realX = (int)(Source.X * (Time) / ChessMoveAnimTime +
                               Dest.X * (ChessMoveAnimTime - Time) / ChessMoveAnimTime);
                int yOff = 0;
                if (Source.X != Dest.X)
                    yOff = (int)(Math.Pow(realX - (Source.X + Dest.X) / 2, 2) * (4 * jumpHeight) / Math.Pow(Source.X - Dest.X, 2) - jumpHeight);
                else
                    yOff = (int)(Math.Pow(Time / ChessMoveAnimTime - 1f / 2, 2) * (4 * jumpHeight) - jumpHeight/2);
                realY = yOff + (int)(Source.Y * (Time) / ChessMoveAnimTime + Dest.Y * (ChessMoveAnimTime - Time) / ChessMoveAnimTime);

                realX -= possessCell.Width / 5; //todo 玄学调整
                realY -= possessCell.Height / 3;
            }
            return new Point(realX, realY);
        }

        public void Draw(Graphics g)
        {
            var possessCell = Scene.Instance.SceneInfo.GetCell(CellId);
            if (possessCell != null)
            {
                var pos = GetPosPredict();
                DrawIcon(g, pos.X, pos.Y, 100, 100);

            }
        }

        protected virtual void DrawIcon(Graphics g, int realX, int realY, int drawWidth, int drawHeight)
        {
            Image head = PicLoader.Read("People", string.Format("{0}.PNG", "humartrom"));
            var rect = new RectangleF(realX + drawWidth * 0.16f, realY + drawHeight * 0.3f, drawWidth * 0.6f, drawHeight * 0.6f);
            g.FillRectangle(Brushes.Black, rect);
            g.DrawImage(head, rect);
            head.Dispose();

            Image token = PicLoader.Read("Border", "npcbg.PNG");
            g.DrawImage(token, rect);
            token.Dispose();
        }
    }

    public class ChessItemPlayer : ChessItem
    {
        public override int CellId
        {
            get { return UserProfile.Profile.InfoBasic.Position; }
        }

        public ChessItemPlayer()
        {
            jumpHeight = 80;
        }

        protected override void DrawIcon(Graphics g, int realX, int realY, int drawWidth, int drawHeight)
        {
            Image head = PicLoader.Read("Player.Token", string.Format("{0}.PNG", UserProfile.InfoBasic.Head));
            var rect = new RectangleF(realX + drawWidth * 0.06f, realY + drawHeight * 0.1f, drawWidth * 0.8f, drawHeight * 0.8f);
            g.DrawImage(head, rect);
            head.Dispose();

            Image token = PicLoader.Read("Player.Token", "ring.PNG");
            g.DrawImage(token, realX, realY, 100, 100);
            token.Dispose();
        }
    }
}