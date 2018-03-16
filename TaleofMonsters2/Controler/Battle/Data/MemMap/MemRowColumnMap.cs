using System.Collections.Generic;
using System.Drawing;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Maps;
using TaleofMonsters.Datas.Others;

namespace TaleofMonsters.Controler.Battle.Data.MemMap
{
    internal class MemRowColumnMap : IMap
    {
        public readonly int StageWidth = 880;
        public readonly int StageHeight = 396;
        public int CardSize { get; private set; }
        public int ColumnCount { get; private set; }
        public int RowCount { get; private set; }
        
        public MemMapPoint[,] Cells { get; set; }

        private bool isDirty;
        private Image cachImage;
        private BattleMapInfo bMapInfo;

        public MemRowColumnMap(string map, int tile)
        {
            bMapInfo = BattleMapBook.GetMap(map);
            CardSize = StageWidth/bMapInfo.XCount;
            RowCount = bMapInfo.YCount;
            ColumnCount = bMapInfo.XCount;
            
            InitCells(tile);

            isDirty = true;
        }

        private void InitCells(int tile)
        {
            Cells = new MemMapPoint[ColumnCount,RowCount];
            for (int i = 0; i < ColumnCount; i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    int tarTile = bMapInfo.Cells[i, j];
                    if (tarTile == 0)
                        tarTile = tile == 0 ? TileConfig.Indexer.DefaultTile : tile;
                    Cells[i, j] = new MemMapPoint(i, i*CardSize, j*CardSize, ColumnCount, tarTile);
                }
            }
        }

        public void InitUnit(Player player)
        {
            var mapConfig = BattleMapBook.GetMapConfig(bMapInfo.Name);
            var unitsPos = player.IsLeft ? mapConfig.LeftMon : mapConfig.RightMon;
            for (int i = 0; i < unitsPos.Length; i+=3)
            {
                var xPos = unitsPos[i];
                var yPos = unitsPos[i+1];
                var unitId = unitsPos[i+2];
                var oldTowerConfig = ConfigData.GetMonsterConfig(unitId);
                var isKingTower = oldTowerConfig.Type == (int) CardTypeSub.KingTower;
                var towerData = new Monster(unitId);
                if (isKingTower)
                {
                    if (player.PeopleId > 0) //王塔替换
                    {
                        var peopleConfig = ConfigData.GetPeopleConfig(player.PeopleId);
                        if (peopleConfig.KingTowerId > 0)
                        {
                            unitId = peopleConfig.KingTowerId;
                            var newTowerConfig = ConfigData.GetMonsterConfig(unitId);
                            towerData = new Monster(unitId);
                            if (newTowerConfig.Type != (int) CardTypeSub.KingTower) //普通单位转化为王塔
                            {
                                towerData.AtkP += oldTowerConfig.AtkP;
                                towerData.VitP += oldTowerConfig.VitP;
                            }
                        }
                    }
                    towerData.Star = mapConfig.TowerStar;
                }

                var level = ConfigData.GetLevelExpConfig(player.Level).TowerLevel;
                var towerUnit = new TowerMonster(level, towerData, isKingTower, new Point(xPos * CardSize, yPos * CardSize), player.IsLeft);

                BattleManager.Instance.RuleData.CheckTowerData(towerUnit);
                BattleManager.Instance.MonsterQueue.Add(towerUnit);
            }

            var monList = player.GetInitialMonster();//只有aiplayer有效
            if (monList != null && monList.Count >= 3)
            {
                for (int i = 0; i < monList.Count; i += 3)
                {
                    int mid = monList[i];
                    int xoff = monList[i + 1];
                    int yoff = monList[i + 2];

                    var level = ConfigData.GetLevelExpConfig(player.Level).TowerLevel;
                    var mon = new Monster(mid);
                    mon.UpgradeToLevel(level);
                    var pos = new Point(xoff*CardSize, yoff*CardSize);
                    LiveMonster lm = new LiveMonster(level, mon, pos, player.IsLeft);
                    BattleManager.Instance.MonsterQueue.Add(lm);
                }
            }
        }

        private MemMapPoint GetCell(int x, int y)
        {
            return Cells[x, y];
        }

        public MemMapPoint GetMouseCell(int x, int y)
        {
            return Cells[x / CardSize, y / CardSize];
        }

        public int GetEnemyId(int mid, bool isLeft, int y, bool isShooter) //pos方位
        {
            int[] xOrder;
            int rowid = y/CardSize;
            if (isLeft)
                xOrder = isShooter ? new int[] {10, 9, 8, 7, 6} : new int[] {6, 7, 8, 9, 10};
            else
                xOrder = isShooter ? new int[] {0, 1, 2, 3, 4} : new int[] {4, 3, 2, 1, 0};

            for (int j = rowid; j >= 0; j--)
            {
                foreach (var i in xOrder)
                {
                    var cell = Cells[i, j];
                    if (cell.Owner > 0 && cell.Owner != mid)
                        return cell.Owner;
                }
            }

            return 0;
        }

        public void SetTile(Point point, int dis, int tile)
        {
            foreach (var memMapPoint in Cells)
            {
                if (BattleLocationManager.IsPointInRegionType(RegionTypes.Circle, point.X, point.Y, memMapPoint.ToPoint(), dis, true))//地形和方向无关，随便填一个
                    memMapPoint.Tile = tile;
            }
            isDirty = true;
        }

        public void SetRowUnitPosition(int y, bool isLeft, string type)
        {
            for (int i = 1; i < ColumnCount-1; i++)
            {
                LiveMonster pickMon = BattleLocationManager.GetPlaceMonster(i * CardSize, y);
                if (pickMon == null)
                    continue;

                if (pickMon.IsGhost || isLeft && pickMon.IsLeft)
                    continue;

                pickMon.Action.SetToPosition(type, 1);
            }
        }
        
        public bool IsPlaceCanMove(int tx, int ty)
        {
            if (tx < 0 || ty < 0 || tx >= StageWidth || ty >= StageHeight)
                return false;

            MemMapPoint point = GetMouseCell(tx, ty);
            return point.CanMove && point.Owner == 0;
        }

        public MonsterCollection GetAllMonster(System.Drawing.Point mouse)
        {
            List<IMonster> monsters = new List<IMonster>();
            foreach (var pickMon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (pickMon.IsGhost)
                    continue;

                monsters.Add(pickMon);
            }
            return new MonsterCollection(monsters, mouse);
        }

        public MonsterCollection GetRangeMonster(bool isLeft, string target, string shape, int range, Point mouse)
        {
            List<IMonster> monsters = new List<IMonster>();
            RegionTypes rt = BattleTargetManager.GetRegionType(shape[0]);
            foreach (var pickMon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (pickMon.IsGhost)
                    continue;

                if ((BattleTargetManager.IsSpellEnemyMonster(target[0]) && isLeft != pickMon.Owner.IsLeft) || (BattleTargetManager.IsSpellFriendMonster(target[0]) && isLeft == pickMon.Owner.IsLeft))
                {
                    if (!BattleLocationManager.IsPointInRegionType(rt, mouse.X, mouse.Y, pickMon.Position, range, isLeft))
                        continue;

                    monsters.Add(pickMon);
                }
            }

            return new MonsterCollection(monsters, mouse);
        }

        public MonsterCollection GetRangeMonsterGhost(bool isLeft, string target, string shape, int range, Point mouse)
        {
            List<IMonster> monsters = new List<IMonster>();
            RegionTypes rt = BattleTargetManager.GetRegionType(shape[0]);
            foreach (var pickMon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (!pickMon.IsGhost)
                    continue;

                if ((BattleTargetManager.IsSpellEnemyMonster(target[0]) && isLeft != pickMon.Owner.IsLeft) || (BattleTargetManager.IsSpellFriendMonster(target[0]) && isLeft == pickMon.Owner.IsLeft))
                {
                    if (!BattleLocationManager.IsPointInRegionType(rt, mouse.X, mouse.Y, pickMon.Position, range, isLeft))
                        continue;

                    monsters.Add(pickMon);
                }
            }

            return new MonsterCollection(monsters, mouse);
        }

        public void ReviveUnit(IPlayer player, IMonster mon, int addHp)
        {
            LiveMonster lm = mon as LiveMonster;
            lm.Revive();
            lm.DeleteWeapon();
            lm.AddHp(addHp);
            if (lm.Owner != player)//复活了对方的怪，就招过来了
                lm.Action.Rebel();
        }

        public void ReviveUnit(IPlayer player, Point mouse, int addHp)
        {
            int oid = GetMouseCell(mouse.X, mouse.Y).Owner;
            if (oid < 0)
                ReviveUnit(player, BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(-oid), addHp);
        }

        public void UpdateCellOwner(Point mouse, int ownerId)
        {
            var cell = GetMouseCell(mouse.X, mouse.Y);
            cell.UpdateOwner(ownerId);
        }

        public void RemoveTomb(Point mouse)
        {
            int oid = GetMouseCell(mouse.X, mouse.Y).Owner;
            if (oid < 0)
            {
                var mon = BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(-oid);
                mon.Action.Disappear();
            }
        }

        public Point GetRandomPoint()
        {
            bool paavail = false;
            Point pa = new Point(0);
            while (!paavail)
            {
                pa.X = MathTool.GetRandom(Cells.GetLength(0)) * CardSize;
                pa.Y = MathTool.GetRandom(Cells.GetLength(1)) * CardSize;
                paavail = IsPlaceCanMove(pa.X, pa.Y);
            }
            return pa;
        }

        public void Draw(Graphics g)
        {
            if (isDirty)
            {
                isDirty = false;
                if (cachImage!=null)
                    cachImage.Dispose();
                cachImage = new Bitmap(StageWidth, StageHeight);
                Graphics cg = Graphics.FromImage(cachImage);

                foreach (var memMapPoint in Cells)
                {
                    cg.DrawImage(TileBook.GetTileImage(memMapPoint.Tile, CardSize, CardSize), memMapPoint.X, memMapPoint.Y, CardSize, CardSize);

                    var tileConfig = ConfigData.GetTileConfig(memMapPoint.Tile);
                    if (tileConfig.ShowBorder)
                    {
                        Pen pen = new Pen(Brushes.DarkRed, 1);
                        cg.DrawRectangle(pen, memMapPoint.X, memMapPoint.Y, CardSize - 1, CardSize);
                        pen.Dispose();
                    }
               
#if DEBUG
                    Font font = new Font("Arial", 7*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    g.DrawString(memMapPoint.Owner.ToString(), font, Brushes.White, memMapPoint.X, memMapPoint.Y+10);
                    font.Dispose();
#endif
                
                }
                cg.Dispose();
            }
            g.DrawImageUnscaled(cachImage, 0, 0, StageWidth, StageHeight);
        }

    }
}
