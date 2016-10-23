using System.Collections.Generic;
using System.Drawing;
using NarlonLib.Core;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.DataType.Cards.Monsters;
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
        private BattleMap bMap;

        public MemRowColumnMap(string map, int tile)
        {
            bMap = BattleMapBook.GetMap(map);
            CardSize = stageWidth/bMap.XCount;
            RowCount = bMap.YCount;
            ColumnCount = bMap.XCount;
            
            InitCells(tile);

            isDirty = true;
        }

        private void InitCells(int tile)
        {
            Cells = new MemMapPoint[ColumnCount,RowCount];
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
                    Cells[i, j] = new MemMapPoint(i, i*CardSize, j*CardSize, ColumnCount, tarTile);
                    tiles[tarTile == TileConfig.Indexer.DefaultTile ? 0 : tarTile]++;
                }
            }
        }

        public void InitUnit(Player player)
        {
            int x = player.IsLeft ? 0 : ColumnCount - 1;
            int y = RowCount / 2;

            foreach (var unitInfo in bMap.Info)
            {
                var id = World.WorldInfoManager.GetCardFakeId();
                var heroData = new Monster(unitInfo.UnitId);
                var level = ConfigData.GetLevelExpConfig(player.Level).TowerLevel;
                LiveMonster lm = new LiveMonster(id, level, heroData, new Point((x + (player.IsLeft ? unitInfo.X : (-unitInfo.X))) * CardSize, (y + unitInfo.Y) * CardSize), player.IsLeft);
                BattleManager.Instance.MonsterQueue.Add(lm);
            }

            if (player.InitialMonster != null && player.InitialMonster.Length >= 3)
            {
                for (int i = 0; i < player.InitialMonster.Length; i+=3)
                {
                    int mid = player.InitialMonster[i];
                    int xoff = player.InitialMonster[i+1];
                    int yoff = player.InitialMonster[i+2];

                    var id = World.WorldInfoManager.GetCardFakeId();
                    var level = ConfigData.GetLevelExpConfig(player.Level).TowerLevel;
                    var mon = new Monster(mid);
                    mon.UpgradeToLevel(level);
                    var pos = new Point((x + xoff)*CardSize, yoff*CardSize);
                    LiveMonster lm = new LiveMonster(id, level, mon, pos, player.IsLeft);
                    BattleManager.Instance.MonsterQueue.Add(lm);
                }
            }
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
            if (x < 0 || (x >= CardSize * sideCount && x < CardSize * (sideCount + 1)) || x >= CardSize * ColumnCount)
            {
                return false;
            }
            if (y < 0 || y >= CardSize * RowCount)
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
                xOrder = isShooter ? new int[] {10, 9, 8, 7, 6} : new int[] {6, 7, 8, 9 ,10};
            }
            else
            {
                xOrder = isShooter ? new int[] { 0,1,2,3,4 } : new int[] { 4,3,2,1,0 };
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

        public void SetTile(Point point, int dis, int tile)
        {
            foreach (var memMapPoint in Cells)
            {
                if (BattleLocationManager.IsPointInRegionType(RegionTypes.Circle, point.X, point.Y, memMapPoint.ToPoint(), dis, true))//地形和方向无关，随便填一个
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
                    if (!BattleLocationManager.IsPointInRegionType(rt, mouse.X, mouse.Y, mon.Position, range, isLeft))
                        continue;

                    monsters.Add(mon);
                }
            }

            return new MonsterCollection(monsters, mouse);
        }

        public MonsterCollection GetRangeMonsterGhost(bool isLeft, string target, string shape, int range, Point mouse)
        {
            List<IMonster> monsters = new List<IMonster>();
            RegionTypes rt = BattleTargetManager.GetRegionType(shape[0]);
            foreach (var mon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (!mon.IsGhost)
                    continue;

                if ((BattleTargetManager.IsSpellEnemyMonster(target[0]) && isLeft != mon.Owner.IsLeft) || (BattleTargetManager.IsSpellFriendMonster(target[0]) && isLeft == mon.Owner.IsLeft))
                {
                    if (!BattleLocationManager.IsPointInRegionType(rt, mouse.X, mouse.Y, mon.Position, range, isLeft))
                        continue;

                    monsters.Add(mon);
                }
            }

            return new MonsterCollection(monsters, mouse);
        }

        public void ReviveUnit(IPlayer player, IMonster mon, int addHp)
        {
            LiveMonster lm = mon as LiveMonster;
            lm.Revive();
            lm.DeleteWeapon();
            lm.Life += addHp;
            if (lm.Owner != player)//复活了对方的怪，就招过来了
            {
                lm.Rebel();
            }
        }

        public void ReviveUnit(IPlayer player, Point mouse, int addHp)
        {
            int oid = BattleManager.Instance.MemMap.GetMouseCell(mouse.X, mouse.Y).Owner;
            if (oid < 0)
            {
                ReviveUnit(player, BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(-oid), addHp);
            }
        }

        public void UpdateCellOwner(Point mouse, int ownerId)
        {
            if (ownerId == 0)
                BattleLocationManager.ClearCellOwner(mouse.X, mouse.Y);
            else
                BattleLocationManager.UpdateCellOwner(mouse.X, mouse.Y, ownerId);    
        }

        public void RemoveTomb(Point mouse)
        {
            int oid = BattleManager.Instance.MemMap.GetMouseCell(mouse.X, mouse.Y).Owner;
            if (oid < 0)
            {
                var mon = BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(-oid);
                mon.Disappear();
            }
        }

        public Point GetRandomPoint()
        {
            return BattleLocationManager.GetRandomPoint();
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
