using System;
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
        
        public MemMapPoint[,] Cells { get; private set; }

        private bool isDirty;
        private Image cachImage;
        private BattleMapInfo bMapInfo;
        private BattleMapConfig mapConfig;

        public MemRowColumnMap(string map, int tile)
        {
            bMapInfo = BattleMapBook.GetMap(map);
            mapConfig = BattleMapBook.GetMapConfig(map);
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
                    Cells[i, j] = new MemMapPoint(i, j, i*CardSize, j*CardSize, ColumnCount, tarTile);
                }
            }
        }

        public void InitUnit(Player player)
        {
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
                towerData.UpgradeToLevel(level);
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

        public MemMapPoint GetCell(int id)
        {
            foreach (var pickCell in Cells)
            {
                if (id == pickCell.Id) //地形和方向无关，随便填一个
                    return pickCell;
            }
            return null;
        }

        public MemMapPoint GetMouseCell(int x, int y)
        {
            return Cells[x / CardSize, y / CardSize];
        }

        public int GetRandomCellMiddle() //返回一个中心列的随机格子
        {
            var cellIdList = new List<int>();
            foreach (var pickCell in Cells)
            {
                if (Array.IndexOf(mapConfig.ColumnMiddle, pickCell.Xid) >= 0)
                    cellIdList.Add(pickCell.Id);
            }

            return cellIdList[MathTool.GetRandom(cellIdList.Count)];
        }
        public int GetRandomCellCompete() //返回一个竞争列的随机格子
        {
            var cellIdList = new List<int>();
            foreach (var pickCell in Cells)
            {
                if (Array.IndexOf(mapConfig.ColumnCompete, pickCell.Xid) >= 0)
                    cellIdList.Add(pickCell.Id);
            }

            return cellIdList[MathTool.GetRandom(cellIdList.Count)];
        }

        public void SetTile(Point point, int dis, int tile)
        {
            foreach (var pickCell in Cells)
            {
                if (BattleLocationManager.IsPointInRegionType(RegionTypes.Circle, point.X, point.Y, pickCell.ToPoint(), dis, true))//地形和方向无关，随便填一个
                    pickCell.Tile = tile;
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
            if (ownerId > 0)
                BattleManager.Instance.OnEnterCell(cell.Id, ownerId);
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
                if (cachImage != null)
                    cachImage.Dispose();
                cachImage = new Bitmap(StageWidth, StageHeight);
                Graphics cg = Graphics.FromImage(cachImage);

                foreach (var pickCell in Cells)
                {
                    cg.DrawImage(TileBook.GetTileImage(pickCell.Tile, CardSize, CardSize), pickCell.X, pickCell.Y, CardSize, CardSize);

                    var tileConfig = ConfigData.GetTileConfig(pickCell.Tile);
                    if (tileConfig.ShowBorder)
                    {
                        Pen pen = new Pen(Brushes.DarkRed, 1);
                        cg.DrawRectangle(pen, pickCell.X, pickCell.Y, CardSize - 1, CardSize);
                        pen.Dispose();
                    }
               
#if DEBUG
                    Font font = new Font("Arial", 7*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    g.DrawString(pickCell.Owner.ToString(), font, Brushes.White, pickCell.X, pickCell.Y+10);
                    font.Dispose();
#endif
                
                }
                cg.Dispose();
            }
            g.DrawImageUnscaled(cachImage, 0, 0, StageWidth, StageHeight);
        }

    }
}
