using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Core;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemEffect;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.DataType.Effects;
using TaleofMonsters.DataType.Maps;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Others;
using ConfigDatas;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data.MemMap
{
    internal class MemRowColumnMap:IMap
    {
        private const int stageWidth = 880;
        private const int stageHeight = 396;
        private AutoDictionary<int, int> tiles;

        public int CardSize { get; private set; }
        public int ColumnCount { get; private set; }
        public int RowCount { get; private set; }

        public int StageWidth
        {
            get { return stageWidth; }
        }

        public int StageHeight
        {
            get { return stageHeight; }
        }

        public MemMapPoint[,] Cells { get; set; }

        private bool isDirty;
        private Image cachImage;

        public MemRowColumnMap(string map, int tile)
        {
            BattleMap bMap = BattleMapBook.GetMap(map);
            CardSize = stageWidth/bMap.XCount;
            RowCount = bMap.YCount;
            ColumnCount = bMap.XCount;
            Cells = new MemMapPoint[ColumnCount, RowCount];
            tiles = new AutoDictionary<int, int>();
            for (int i = 0; i < ColumnCount; i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    int tarTile = bMap.Cells[i, j];
                    if (tarTile == 0)
                    {
                        tarTile = tile == 0 ? TileConfig.Indexer.DefaultTile : tile;
                    }
                    Cells[i, j] = new MemMapPoint(i, i * CardSize, j * CardSize, ColumnCount, tarTile);
                    tiles[tarTile == TileConfig.Indexer.DefaultTile ? 0 : tarTile]++;
                }
            }
            isDirty = true;
        }

        public MemMapPoint GetCell(int x, int y)
        {
            return Cells[x, y];
        }

        public MemMapPoint GetMouseCell(int x, int y)
        {
            return Cells[x / CardSize, y / CardSize];
        }

        public bool IsMousePositionCanSummon(int x, int y)
        {
            var sideCount = ColumnCount/2;
            if (x < CardSize || (x >= CardSize * sideCount && x < CardSize * (sideCount + 1)) || x >= CardSize * (ColumnCount-1))
            {
                return false;
            }
            if (y < 0 || y >= CardSize * sideCount)
            {
                return false;
            }
            return GetMouseCell(x, y).Owner <= 0;
        }

        public int GetEnemyId(int mid, bool isLeft, int y, bool isShooter) //pos方位
        {
            int[] xOrder;
            int rowid = y/CardSize;
            if (isLeft)
            {
                if (isShooter)
                {
                    xOrder = new int[] {10, 9, 8, 7, 6};
                }
                else
                {
                    xOrder = new int[] {6, 7, 8, 9 ,10};//todo 先这样
                }
            }
            else
            {
                if (isShooter)
                {
                    xOrder = new int[] { 0,1,2,3,4 };
                }
                else
                {
                    xOrder = new int[] { 4,3,2,1,0 };
                }
            }

            for (int j = rowid; j >= 0; j--)
            {
                foreach (var i in xOrder)
                {
                    var cell = Cells[i, j];
                    if (cell.Owner > 0 && cell.Owner != mid)
                    {
                        return cell.Owner;
                    }
                }
            }

            return 0;
        }

        public void SetTile(int itype, Point point, int dis, int tile)
        {
            RegionTypes type = (RegionTypes)itype;
            foreach (var memMapPoint in Cells)
            {
                if (BattleLocationManager.IsPointInRegionType(type, point.X, point.Y, memMapPoint.ToPoint(), dis))
                {
                    memMapPoint.Tile = tile;
                }
            }
            tiles.Clear();
            foreach (var memMapPoint in Cells)
            {
                tiles[memMapPoint.Tile == 9 ? 0 : memMapPoint.Tile]++;
            }
            isDirty = true;
        }

        public void ChangePositionWithRandom(IMonster target)
        {
            if (target.IsHero)
                return;

            RandomMaker rm = new RandomMaker();
            int count = 0;
            for (int i = 0; i < RowCount; i++)
            {
                int xoff = i > 1 ? 0 : i * 2 - 1;
                int yoff = i < 2 ? 0 : i * 2 - 5;
                Point pa = new Point(target.Position.X + xoff * CardSize, target.Position.Y + yoff * CardSize);
                LiveMonster lm = BattleLocationManager.GetPlaceMonster(pa.X, pa.Y);
                if (lm != null && !lm.IsHero)
                {
                    rm.Add(lm.Id, 1);
                    count++;
                }
            }
            if (count >= 1)
            {
                int id = rm.Process(1)[0];
                LiveMonster lm =BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(id);
                BattleLocationManager.UpdateCellOwner(lm.Position.X, lm.Position.Y, target.Id);
                BattleLocationManager.UpdateCellOwner(target.Position.X, target.Position.Y, lm.Id);
                Point temp = lm.Position;
                lm.Position = target.Position;
                target.Position = temp;
            }
        }

        public void DragRandomUnitNear(Point mouse)
        {
            MemMapPoint point = BattleManager.Instance.MemMap.GetMouseCell(mouse.X, mouse.Y);
            if (point.SideIndex < 1 || point.SideIndex > ColumnCount/2-1)//不能传
                return;

            RandomMaker rm = new RandomMaker();
            int count = 0;
            for (int i = 0; i < RowCount; i++)
            {
                int xoff = i > 1 ? 0 : i * 2 - 1;
                int yoff = i < 2 ? 0 : i * 2 - 5;
                Point pa = new Point(mouse.X + xoff * CardSize, mouse.Y + yoff * CardSize);
                LiveMonster lm = BattleLocationManager.GetPlaceMonster(pa.X, pa.Y);
                if (lm != null && !lm.IsHero)
                {
                    rm.Add(lm.Id, 1);
                    count++;
                }
            }
            if (count >= 1)
            {
                int id = rm.Process(1)[0];
                LiveMonster lm =BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(id);

                BattleLocationManager.SetToPosition(lm, point.ToPoint());
            }
        }

        public void SetRowUnitPosition(int y, bool isLeft, string type)
        {
            for (int i = 1; i < ColumnCount-1; i++)
            {
                LiveMonster lm = BattleLocationManager.GetPlaceMonster(i * CardSize, y);
                if (lm == null)
                    continue;

                if (lm.IsGhost || isLeft && lm.IsLeft)
                    continue;

                if (lm.IsHero)
                    continue;

                lm.SetToPosition(type);
            }
        }

        public MonsterCollection GetAllMonster(System.Drawing.Point mouse)
        {
            List<IMonster> monsters = new List<IMonster>();
            foreach (LiveMonster mon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (mon.IsGhost)
                    continue;

                monsters.Add(mon);
            }
            return new MonsterCollection(monsters, mouse);
        }

        public MonsterCollection GetRangeMonster(bool isLeft, string target, string shape, int range, Point mouse)
        {
            List<IMonster> monsters = new List<IMonster>();
            RegionTypes rt = BattleTargetManager.GetRegionType(shape[0]);
            foreach (var mon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (mon.IsGhost)
                    continue;

                if ((BattleTargetManager.IsSpellEnemyMonster(target[0]) && isLeft != mon.Owner.IsLeft) || (BattleTargetManager.IsSpellFriendMonster(target[0]) && isLeft == mon.Owner.IsLeft))
                {
                  
                    if (!BattleLocationManager.IsPointInRegionType(rt, mouse.X, mouse.Y, mon.Position, range))
                        continue;

                    monsters.Add(mon);
                }
            }

            return new MonsterCollection(monsters, mouse);
        }

        public void ReviveUnit(Point mouse, int addHp)
        {
            int oid = BattleManager.Instance.MemMap.GetMouseCell(mouse.X, mouse.Y).Owner;
            if (oid < 0)
            {
                LiveMonster lm =BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(-oid);
                lm.Revive();
                lm.DeleteWeapon();
                lm.Life += addHp;
            }
        }

        public void UpdateCellOwner(Point mouse, int ownerId)
        {
            BattleLocationManager.UpdateCellOwner(mouse.X, mouse.Y, ownerId);
        }

        public LiveMonster GetNearestMonster(bool isLeft, string target, Point mouse)
        {
            LiveMonster monster  = null;
            int dis = int.MaxValue;
            foreach (var mon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (mon.IsGhost)
                    continue;

                if ((BattleTargetManager.IsSpellEnemyMonster(target[0]) && isLeft != mon.Owner.IsLeft)
                    || (BattleTargetManager.IsSpellFriendMonster(target[0]) && isLeft == mon.Owner.IsLeft))
                {
                    var tpDis = MathTool.GetDistance(mon.Position, mouse);
                    if (tpDis < dis)
                    {
                        dis = tpDis;
                        monster = mon;
                    }
                }
            }
            return monster;
        }

        public void Draw(Graphics g)
        {
            if (isDirty)
            {
                isDirty = false;
                if (cachImage!=null)
                {
                    cachImage.Dispose();
                }
                cachImage = new Bitmap(stageWidth, stageHeight);
                Graphics cg = Graphics.FromImage(cachImage);

                foreach (var memMapPoint in Cells)
                {
                    cg.DrawImage(TileBook.GetTileImage(memMapPoint.Tile, CardSize, CardSize), memMapPoint.X, memMapPoint.Y, CardSize, CardSize);
                    Pen pen = new Pen(Brushes.DarkRed, 1);
                    cg.DrawRectangle(pen, memMapPoint.X, memMapPoint.Y, CardSize - 1, CardSize);
#if DEBUG
                    Font font = new Font("Arial", 7*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    g.DrawString(memMapPoint.Owner.ToString(), font, Brushes.White, memMapPoint.X, memMapPoint.Y+10);
                    font.Dispose();
#endif
                    pen.Dispose();
                }
                cg.Dispose();
            }
            g.DrawImageUnscaled(cachImage, 0, 0, stageWidth ,stageHeight);
        }

    }
}
