using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using ControlPlus.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Blesses;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.Quests;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.CMain.Scenes.SceneObjects;
using TaleofMonsters.Forms.CMain.Scenes.SceneObjects.Moving;
using TaleofMonsters.Forms.CMain.Scenes.SceneRules;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Forms.CMain.Scenes
{
    internal class Scene
    {
        #region 委托
        delegate void TimelySceneCallback(SceneObject f);
        private void TimelyCheck(SceneObject f)
        {
            if (MainForm.Instance.InvokeRequired)
            {
                TimelySceneCallback d = CheckALiveAndQuestState;
                MainForm.Instance.Invoke(d, new object[] { f });
            }
            else
            {
                CheckALiveAndQuestState(f);
            }
        }

        public delegate void SwitchMapAction();
        #endregion

        public static Scene Instance { get; set; }

        private Image mainBottom;
        private Image miniMap;
        private Image mainTop;
        private Image mainTopRes;
        private Image mainTopTitle;
        private Image miniBack;
        private Image backPicture;
        private int cellTar = -1;
        public SceneInfoRT SceneInfo { get; private set; }
        private string sceneName;
        private Control parent;

        public ISceneRule Rule { get; set; }

        public int TimeMinutes { get; private set; }
        private int width, height;// 场景的宽度和高度

        private VirtualRegion vRegion;
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private ChessManager chessManager = new ChessManager();


        private bool allEventFinished; //所有的事件都完成了

        public Scene(Control p, int w, int h)
        {
            parent = p;
            width = w;
            height = h;

            vRegion = new VirtualRegion(parent);

            int resXOff = (width - 688) / 2;
            vRegion.AddRegion(new SubVirtualRegion(1, resXOff - 30 + 573, 13, 105, 20));

            vRegion.AddRegion(new SubVirtualRegion(2, resXOff - 30 + 82, 13, 77, 20)); //资源
            vRegion.AddRegion(new SubVirtualRegion(3, resXOff - 30 + 82*2, 13, 77, 20));
            vRegion.AddRegion(new SubVirtualRegion(4, resXOff - 30 + 82*3, 13, 77, 20));
            vRegion.AddRegion(new SubVirtualRegion(5, resXOff - 30 + 82*4, 13, 77, 20));
            vRegion.AddRegion(new SubVirtualRegion(6, resXOff - 30 + 82*5, 13, 77, 20));
            vRegion.AddRegion(new SubVirtualRegion(7, resXOff - 30 + 82*6, 13, 77, 20));
            
            vRegion.AddRegion(new SubVirtualRegion(10, 0, 0, 150, 50));//人物头像
            vRegion.AddRegion(new SubVirtualRegion(11, width - 145, 3, 115, 32));//场景信息
            vRegion.AddRegion(new SubVirtualRegion(12, (width - 688) / 2 + 30, 41, 630, 8));//exp bar

            for (int i = 0; i < GameConstants.BlessLimit; i++)
            {//bless
                vRegion.AddRegion(new PictureRegion(20+i, i*60+10, 55, 50, 50, PictureRegionCellType.Bless, 0));
            }

            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;
            vRegion.CellDrawAfter += VRegion_CellDraw;
        }

        public void Init()
        {
            mainTop = PicLoader.Read("System", "MainTop.JPG");
            mainTopRes = PicLoader.Read("System", "MainTopRes.PNG");
            mainTopTitle = PicLoader.Read("System", "MainTopTitle.PNG");
            mainBottom = PicLoader.Read("System", "MainBottom.JPG");
            miniBack = PicLoader.Read("System", "MiniBack.PNG");
        }

        public void ChangeMap(int mapid, bool isWarp, SwitchMapAction action = null)
        {
            if (backPicture != null)
                backPicture.Dispose();
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(mapid);
            backPicture = PicLoader.Read("Scene", string.Format("{0}.JPG", sceneConfig.Url));
            sceneName = sceneConfig.Name;

            GenerateMiniMap(mapid, MathTool.Clamp(sceneConfig.IconX - 110,0, 1688-300), MathTool.Clamp(sceneConfig.IconY - 110, 0, 1121 - 300));
            SoundManager.PlayBGMScene(string.Format("{0}.mp3", sceneConfig.BGM));
            allEventFinished = false;

            UserProfile.Profile.OnSwitchScene(isWarp);
            UserProfile.InfoBasic.MapId = mapid;//这句必须在存档后
            if (action != null)
                action();

            BlessManager.OnChangeMap();
            OnBlessChange();
            BlessManager.Update = OnBlessChange;
            
            SystemMenuManager.ResetIconState(); //reset main icon state todo remove check

            if (sceneConfig.Type == (int)SceneTypes.Dungeon)
                Rule = new SceneRuleDungeon();
            else if (sceneConfig.Type == (int)SceneTypes.Town)
                Rule = new SceneRuleTown();
            else
                Rule = new SceneRuleCommon();
            TimeMinutes = (int)DateTime.Now.TimeOfDay.TotalMinutes;
            Rule.Init(mapid, TimeMinutes);
            SceneInfo = SceneManager.RefreshSceneObjects(UserProfile.InfoBasic.MapId, width, height - 35, isWarp ? SceneFreshReason.Warp : SceneFreshReason.Load);
            chessManager.OnChangeMap(UserProfile.InfoBasic.MapId, isWarp);

            if (UserProfile.InfoBasic.Position == 0 && SceneInfo.Items.Count > 0)//兜底处理
                UserProfile.InfoBasic.Position = SceneInfo.Items[0].Id;
            parent.Invalidate();
        }

        public void EnterDungeon(int dungeonId)
        {
            UserProfile.InfoDungeon.GenStoryId(dungeonId);
            var dungeonConfig = ConfigData.GetDungeonConfig(dungeonId);//进入副本
            ChangeMap(dungeonConfig.EntryScene, true, ()=>
            {
                UserProfile.InfoDungeon.Enter(dungeonId);
                SystemMenuManager.CheckItemClick(SystemMenuIds.StoryForm);
            });
            MoveTo(SceneInfo.GetStartPos());
        }

        public void LeaveDungeon()
        {
            var dungeonId = UserProfile.InfoDungeon.DungeonId;
            var dungeonConfig = ConfigData.GetDungeonConfig(dungeonId);

            ChangeMap(dungeonConfig.ExitScene, true, () =>
            {
                UserProfile.InfoDungeon.Leave();
            });
            MoveTo(SceneInfo.GetStartPos());
        }

        private void OnBlessChange()
        {
            for (int i = 0; i < GameConstants.BlessLimit; i++)
                vRegion.SetRegionKey(20 + i, 0);//bless
            int index = 0;
            foreach (var key in UserProfile.InfoWorld.Blesses.Keys)
                vRegion.SetRegionKey(20 + index++, key);
            parent.Invalidate(new Rectangle(10, 55, 600, 50));
        }

        private void GenerateMiniMap(int mapid, int wx, int wy)
        {
            Image allMap = PicLoader.Read("Map", "worldmap.JPG"); //生成世界地图
            Graphics g = Graphics.FromImage(allMap);
            Font font = new Font("微软雅黑", 18*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            foreach (var sceneConfig in ConfigData.SceneDict.Values)
            {
                if (sceneConfig.Icon == "")
                    continue;
                if (sceneConfig.IconX < wx || sceneConfig.IconX > wx + 300 || sceneConfig.IconY < wy || sceneConfig.IconY > wy + 300)
                    continue;
                string tname = ConfigData.GetSceneConfig(sceneConfig.Id).Name;
                g.DrawString(tname, font, Brushes.Black, sceneConfig.IconX + 2, sceneConfig.IconY + 21);
                g.DrawString(tname, font, sceneConfig.Id == mapid ? Brushes.Lime : Brushes.White, sceneConfig.IconX, sceneConfig.IconY + 20);
            }
            font.Dispose();
            g.Dispose();

            if (miniMap != null)
                miniMap.Dispose();
            miniMap = new Bitmap(150, 150); //绘制小地图
            g = Graphics.FromImage(miniMap);
            Rectangle destRect = new Rectangle(0, 0, 150, 150);
            g.DrawImage(allMap, destRect, wx, wy, 300, 300, GraphicsUnit.Pixel);
            g.Dispose();
            allMap.Dispose();
        }

        public void TimeGo(float timePast)
        {
            foreach (var sceneObject in SceneInfo.Items)
                sceneObject.OnTick();

            ChessItem playerChess = null;
            foreach (var chessItem in chessManager.ChessList)
            {
                bool needUpdate;
                if (chessItem.TimeGo(timePast, out needUpdate))
                {
                    if (chessItem.PeopleId == 0) // is player
                        playerChess = chessItem;
               //     parent.Invalidate();
                }
                if (needUpdate)
                {
                    var targetPos = chessItem.GetPosPredict();
                    parent.Invalidate(new Rectangle(targetPos.X - 200, targetPos.Y - 200, 400, 400));
                }
            }

            if (playerChess != null)
            {
                var targetCell = SceneInfo.GetCell(playerChess.DestId);
                MoveTo(targetCell.Id);
                TimelyCheck(targetCell);

                chessManager.OnChessPlayerMoved(playerChess.DestId);
            }

            if (UserProfile.Profile != null)
            {
                int time = (int)DateTime.Now.TimeOfDay.TotalMinutes;
                if (TimeMinutes == 0 || time != TimeMinutes)
                {
                    if (TimeMinutes == 0 || (time % 60) == 0)
                    {
                        TimeMinutes = time;
                        if (time == 0)
                            UserProfile.Profile.OnNewDay();
                        parent.Invalidate();
                    }
                }
            }
        }

        private void CheckALiveAndQuestState(SceneObject o)
        {
            try //因为这一步会被invoke，所以单独套一层try
            {
                var peopleId = chessManager.GetPeopleIdOnCell(o.Id);
                if (peopleId > 0)//如果有npc，只触发npc事件
                {
                    var chessConfig = ConfigData.GetPeopleChessConfig(peopleId);
                    if (!string.IsNullOrEmpty(chessConfig.BindQuest))
                    {
                        var chess = chessManager.GetChess(peopleId);
                        chess.MeetCount ++;

                        var questId = SceneQuestBook.GetSceneQuestByName(chessConfig.BindQuest);
                        PanelManager.DealPanel(new NpcTalkForm { EventId = questId, CellId = o.Id });
                    }
                }
                else
                {
                    o.MoveEnd();
                }
                
                SoundManager.Play("System", "Move.mp3");
            }
            catch (Exception e)
            {
                NarlonLib.Log.NLog.Error(e);
                throw;
            }
            
        }

        public void OnEventEnd(int cellId, int evtId, string type)
        {
            var scenePos = GetObjectByPos(cellId);
            if (scenePos != null && !scenePos.Disabled) //可能被手动关闭了
            {
                var config = ConfigData.GetSceneQuestConfig(evtId);
                if (!config.TriggerMulti)
                    scenePos.SetEnable(false);
                else
                    scenePos.SetMapSetting(true);//多次触发都变成预设
            }
            UserProfile.InfoDungeon.OnEventEnd(evtId, type);
        }

        public void CheckALiveAndQuestState()
        {
            if (UserProfile.InfoBasic.HealthPoint <= 0 || UserProfile.InfoBasic.MentalPoint <= 0)
            {
                var goldSub = (uint)Math.Ceiling((double)UserProfile.InfoBag.Resource.Gold / 5);
                MessageBoxEx.Show(string.Format("你死了，失去了{0}的金钱", goldSub));
                UserProfile.Profile.OnDie();
              
                if (UserProfile.InfoDungeon.DungeonId > 0)
                {
                    LeaveDungeon();
                }
                else
                {
                    var config = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId);
                    ChangeMap(config.ReviveScene, true);
                    MoveTo(SceneInfo.GetRevivePos());
                }
            }

            CheckAllQuestOpened();
        }
        
        public void CheckMouseMove(int x, int y)
        {
            if(chessManager.IsChessMoving())
                return;

            int nTemp = -1;
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject.IsMouseIn(x, y))
                    nTemp = sceneObject.Id;
            }
            if (cellTar != nTemp)
            {
                cellTar = nTemp;
                parent.Invalidate();
            }
        }

        public void CheckMouseClick()
        {
            if (cellTar == -1) return;
            if (chessManager.IsChessMoving())
                return;

            SceneObject dest = SceneInfo.GetCell(cellTar);
            if (dest != null && dest.OnClick())
            {
                chessManager.SetChessState(0, UserProfile.InfoBasic.Position, cellTar);
                parent.Invalidate();
            }
        }


        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id == 10)
            {
                Image image = GetPlayerImage();
                tooltip.Show(image, parent, 0,50);
            }
            else if (id == 11)
            {
                Image image = SceneBook.GetScenePreview(this);
                tooltip.Show(image, parent, width - image.Width, 35);
            }
            else if (id == 12)
            {
                var expStr = string.Format("{0}/{1}", UserProfile.InfoBasic.Exp, ExpTree.GetNextRequired(UserProfile.InfoBasic.Level));
                Image image = DrawTool.GetImageByString("升级所需经验", expStr, 120, Color.White);
                tooltip.Show(image, parent, x-600, y+8);
            }
            else if (id < 10)
            {
                var resName = HSTypes.I2Resource(id - 1);
                string resStr = string.Format("{0}:{1}", resName, UserProfile.Profile.InfoBag.Resource.Get((GameResourceType)(id-1)));
                Image image = DrawTool.GetImageByString(resStr, HSTypes.I2ResourceTip(id-1), 120, Color.White);
                tooltip.Show(image, parent, x, y);
            }
            else if (id >= 20)
            {
                if (key > 0)
                {
                    Image image = BlessBook.GetPreview(key);
                    tooltip.Show(image, parent, x, y);
                }
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        private void VRegion_CellDraw(int id, int x, int y, int key, Graphics g)
        {
            if (id >= 20)
            {
                if (key > 0)
                {
                    var blessTime = UserProfile.InfoWorld.GetBlessTime(key);
                    Font font = new Font("宋体", 11 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                    g.DrawString(blessTime.ToString(), font, Brushes.White, x+15, y+33);
                    font.Dispose();
                }
            }
        }

        /// <summary>
        /// 主界面的元素绘制
        /// </summary>
        public void Paint(Graphics g)
        {
            g.DrawImage(backPicture, 0, 50, width, height-35);
            g.DrawImage(mainTop, 0, 0, width, 50);
            g.DrawImage(mainTopRes, (width-688)/2, 4, 688, 37);//688是图片尺寸
            g.DrawImage(mainTopTitle, width-145, 3, 115, 32);
            g.DrawImage(mainBottom, 0, height-35, width, 35);
            
            if (UserProfile.Profile == null || UserProfile.InfoBasic.MapId == 0)
                return;

            Image head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Head));
            g.DrawImage(head, 0, 0, 50, 50);
            head.Dispose();
            Font font2 = new Font("宋体", 9 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(UserProfile.InfoBasic.Level.ToString(), font2, Brushes.Black, 3, 3);
            g.DrawString(UserProfile.InfoBasic.Level.ToString(), font2, Brushes.White, 2, 2);
            font2.Dispose();

            { //属性条
                Font font3 = new Font("Arial", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                g.DrawImage(HSIcons.GetIconsByEName("buf0"), 58, 3, 14, 14);
                LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(78, 5, 100, 10), Color.LightCoral, Color.Red, LinearGradientMode.Horizontal);
                g.FillRectangle(b1, 78, 5, Math.Min(UserProfile.InfoBasic.HealthPoint, 100), 10);
                g.DrawRectangle(Pens.DarkGray, 78, 5, 100, 10);
                b1.Dispose();
                g.DrawString(string.Format("{0,3}/100", UserProfile.InfoBasic.HealthPoint), font3, Brushes.White, 78+25, 2);

                g.DrawImage(HSIcons.GetIconsByEName("buf1"), 58, 17, 14, 14);
                b1 = new LinearGradientBrush(new Rectangle(78, 19, 100, 10), Color.LightBlue, Color.Blue, LinearGradientMode.Horizontal);
                g.FillRectangle(b1, 78, 19, Math.Min(UserProfile.InfoBasic.MentalPoint, 100), 10);
                g.DrawRectangle(Pens.DarkGray, 78, 19, 100, 10);
                b1.Dispose();
                g.DrawString(string.Format("{0,3}/100", UserProfile.InfoBasic.MentalPoint), font3, Brushes.White, 78 + 25, 16);

                g.DrawImage(HSIcons.GetIconsByEName("oth7"), 58, 31, 14, 14);
                b1 = new LinearGradientBrush(new Rectangle(78, 33, 100, 10), Color.LightGreen, Color.Green, LinearGradientMode.Horizontal);
                g.FillRectangle(b1, 78, 33, Math.Min(UserProfile.InfoBasic.FoodPoint, 100), 10);
                g.DrawRectangle(Pens.DarkGray, 78, 33, 100, 10);
                b1.Dispose();
                g.DrawString(string.Format("{0,3}/100", UserProfile.InfoBasic.FoodPoint), font3, Brushes.White, 78 + 25, 30);

                font3.Dispose();

                int xOff = (width - 688) / 2 + 30;
                g.FillRectangle(Brushes.DimGray, xOff, 41, 630, 8);
                b1 = new LinearGradientBrush(new Rectangle(xOff, 41, 630, 8), Color.White, Color.Gray, LinearGradientMode.Vertical);
                g.FillRectangle(b1, xOff, 41, Math.Min(UserProfile.InfoBasic.Exp * 630 / ExpTree.GetNextRequired(UserProfile.InfoBasic.Level), 630), 8);
                g.DrawRectangle(new Pen(Brushes.Black, 2), xOff, 41, 630, 8);
                b1.Dispose();
            }

            if (TimeMinutes >= 960 && TimeMinutes < 1080)
            {
                Brush yellow = new SolidBrush(Color.FromArgb(50, 255, 200, 0));
                g.FillRectangle(yellow, 0, 50, width, height);
                yellow.Dispose();
            }
            else if (TimeMinutes < 360 || TimeMinutes >= 1080) //18点到6点
            {
                Brush blue = new SolidBrush(Color.FromArgb(120, 0, 0, 150));
                g.FillRectangle(blue, 0, 50, width, height);
                blue.Dispose();
            }

            Font font = new Font("微软雅黑", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            var len = TextRenderer.MeasureText(g, sceneName, font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
            g.DrawString(sceneName, font, Brushes.White, new PointF(width - 85 - len / 2, 8));
            font.Dispose();

            { //资源
                Font font3 = new Font("Arial", 11*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                var resBackX = (width - 688)/2;
                DrawRes(UserProfile.InfoBag.Resource.Lumber, g, font3, resBackX + 82, resBackX + 82 + 48, 15);
                DrawRes(UserProfile.InfoBag.Resource.Stone, g, font3, resBackX + 82*2, resBackX + 82*2 + 48, 15);
                DrawRes(UserProfile.InfoBag.Resource.Mercury, g, font3, resBackX + 82*3, resBackX + 82*3 + 48, 15);
                DrawRes(UserProfile.InfoBag.Resource.Carbuncle, g, font3, resBackX + 82*4, resBackX + 82*4 + 48, 15);
                DrawRes(UserProfile.InfoBag.Resource.Sulfur, g, font3, resBackX + 82*5, resBackX + 82*5 + 48, 15);
                DrawRes(UserProfile.InfoBag.Resource.Gem, g, font3, resBackX + 82*6, resBackX + 82*6 + 48, 15);
                DrawRes(UserProfile.InfoBag.Resource.Gold, g, font3, resBackX + 570, resBackX + 570 + 80, 15);
                font3.Dispose();
            }

            if (UserProfile.InfoDungeon.DungeonId <= 0)
            {
                g.DrawImage(miniMap, width - 160, 43, 150, 150);
                g.DrawImage(miniBack, width - 190, 38, 185, 160);
            }
            else
            {
                DrawDungeonAttr(g);
            }

            vRegion.Draw(g);

            DrawCellAndToken(g); //绘制地图格子
        }

        private void DrawRes(uint resCount, Graphics g, Font font, int xStart, int xEnd, int y)
        {
            float lenth = TextRenderer.MeasureText(g, resCount.ToString(), font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
            g.DrawString(resCount.ToString(), font, Brushes.White, new PointF(xStart + (xEnd-xStart - lenth)/2, y));
        }

        private static void DrawDungeonAttr(Graphics g)
        {
            Image tipBack = PicLoader.Read("System", "TipBack.PNG");
            Font font2 = new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            int index = 0;
            if (UserProfile.InfoDungeon.Str >= 0)
            {
                g.DrawImage(tipBack, 2, 120 + 30 * index, tipBack.Width, tipBack.Height);
                g.DrawString(string.Format("力量：{0}", UserProfile.InfoDungeon.Str), font2, Brushes.IndianRed, 8, 5 + 120 + 30 * index);
                index++;
            }
            if (UserProfile.InfoDungeon.Agi >= 0)
            {
                g.DrawImage(tipBack, 2, 120 + 30 * index, tipBack.Width, tipBack.Height);
                g.DrawString(string.Format("敏捷：{0}", UserProfile.InfoDungeon.Agi), font2, Brushes.LawnGreen, 8, 5 + 120 + 30 * index);
                index++;
            }
            if (UserProfile.InfoDungeon.Intl >= 0)
            {
                g.DrawImage(tipBack, 2, 120 + 30 * index, tipBack.Width, tipBack.Height);
                g.DrawString(string.Format("智慧：{0}", UserProfile.InfoDungeon.Intl), font2, Brushes.DeepSkyBlue, 8, 5 + 120 + 30 * index);
                index++;
            }
            if (UserProfile.InfoDungeon.Perc >= 0)
            {
                g.DrawImage(tipBack, 2, 120 + 30 * index, tipBack.Width, tipBack.Height);
                g.DrawString(string.Format("感知：{0}", UserProfile.InfoDungeon.Perc), font2, Brushes.MediumPurple, 8, 5 + 120 + 30 * index);
                index++;
            }
            if (UserProfile.InfoDungeon.Endu >= 0)
            {
                g.DrawImage(tipBack, 2, 120 + 30 * index, tipBack.Width, tipBack.Height);
                g.DrawString(string.Format("耐力：{0}", UserProfile.InfoDungeon.Endu), font2, Brushes.Bisque, 8, 5 + 120 + 30 * index);
                index++;
            }
            tipBack.Dispose();
            font2.Dispose();
        }

        private void DrawCellAndToken(Graphics g)
        {
            SceneObject selectTarget = null;
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject.Id == cellTar)
                    selectTarget = sceneObject;
                else
                    sceneObject.Draw(g, false);//先绘制非目标
            }

            if (selectTarget != null)
                selectTarget.Draw(g, true);

            chessManager.Draw(g);
        }

        public int GetDisableEventCount(int eid)
        {
            int count = 0;
            foreach (var sceneObject in SceneInfo.Items)
            {
                var sceQuest = sceneObject as SceneQuest;
                if (sceQuest != null && sceQuest.EventId == eid && sceneObject.Disabled)
                    count++;
            }
            return count;
        }

        public int GetWarpPosByMapId(int mapId)
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                var warp = sceneObject as SceneWarp;
                if (warp != null && warp.TargetMap == mapId)
                    return warp.Id;
            }
            return 0;
        }


        public SceneObject GetObjectByPos(int pos)
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject != null && sceneObject.Id == pos)
                    return sceneObject;
            }
            return null;
        }

        public int CountOpenedQuest(string name)
        {
            int count = 0;
            foreach (var sceneObject in SceneInfo.Items)
            {
                var quest = sceneObject as SceneQuest;
                if (quest != null && quest.EventId > 0 && quest.Disabled)
                {
                    var config = ConfigData.GetSceneQuestConfig(quest.EventId);
                    if (config.Ename == name)
                        count++;
                }
            }
            return count;
        }

        public void CheckAllQuestOpened()
        {
            if (allEventFinished)
                return;

            foreach (var sceneObject in SceneInfo.Items)
            {
                var quest = sceneObject as SceneQuest;
                if (quest != null && !quest.MapSetting && !quest.Disabled)
                    return;
            }
            QuestBook.CheckAllQuestWith("allopen");
            allEventFinished = true;
        }

        private Image GetPlayerImage()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine(string.Format("{0}(Lv{1})", UserProfile.Profile.Name, UserProfile.InfoBasic.Level), "LightBlue", 20);
            tipData.AddLine(2);
            tipData.AddTextNewLine(string.Format("生命:{0}",UserProfile.InfoBasic.HealthPoint), "Red");
            tipData.AddTextNewLine(string.Format("精神:{0}", UserProfile.InfoBasic.MentalPoint), "LightBlue");
            tipData.AddTextNewLine(string.Format("食物:{0}", UserProfile.InfoBasic.FoodPoint), "LightGreen");
            return tipData.Image;
        }

        public void ResetScene()
        {
            SceneInfo = SceneManager.RefreshSceneObjects(UserProfile.InfoBasic.MapId, width, height - 35, SceneFreshReason.Reset);
            UserProfile.InfoQuest.ResetQuest();
            parent.Invalidate();
        }

        public void Ruin()
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                var quest = sceneObject as SceneQuest;
                if (quest == null)
                    continue;
                var questConfig = ConfigData.GetSceneQuestConfig(quest.EventId);
                if (questConfig.Type != (int)SceneQuestTypes.MapSetting && questConfig.Danger == 0 && !sceneObject.Disabled)
                    sceneObject.SetEnable(false);
            }
            parent.Invalidate();
        }

        public void EnableTeleport()
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject is SceneWarp)
                    sceneObject.SetEnable(true);//激活所有的传说门
            }
            parent.Invalidate();
        }

        public void RandomPortal()
        {
            UserProfile.InfoBasic.Position = SceneInfo.GetRandom(UserProfile.InfoBasic.Position, false);
            parent.Invalidate();
        }

        public void MoveTo(int target)
        {
            UserProfile.InfoBasic.LastPosition = UserProfile.InfoBasic.Position;
            UserProfile.InfoBasic.Position = target;
            UserProfile.InfoBasic.MoveCount ++;

            parent.Invalidate();
        }

        public void HiddenWay()
        {    
            int fromId = UserProfile.InfoBasic.Position;
            var findCellId = SceneInfo.FindCell(fromId, "hiddeway");
            if (findCellId > 0)
            {
                UserProfile.InfoBasic.Position = findCellId;
            }
            else
            {
                int cellId = SceneInfo.GetRandom(UserProfile.InfoBasic.Position, true);
                SceneInfo.ReplaceCellQuest(cellId, "hiddeway");
                UserProfile.InfoBasic.Position = cellId; //移动过去
            }

            parent.Invalidate();
        }

        public void QuestNext(string qname)
        {
            int index = SceneInfo.GetRandom(UserProfile.InfoBasic.Position, true);
            SceneInfo.ReplaceCellQuest(index, qname);
            parent.Invalidate();
        }

        public void OpenHidden(string qname)
        {
            SceneInfo.OpenHidden(qname);
            parent.Invalidate();
        }

        public void DetectAll()
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject is SceneQuest)
                    sceneObject.AddFlag(SceneObject.ScenePosFlagType.Detected);
            }
        }

        public void DetectNear(int range)
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject is SceneQuest && SceneManager.GetDistance(sceneObject.Id, UserProfile.InfoBasic.Position) <= range)
                    sceneObject.AddFlag(SceneObject.ScenePosFlagType.Detected);
            }
        }
        public void DetectRandom(int count)
        {
            List<SceneObject> toChoose = new List<SceneObject>();
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject is SceneQuest && !sceneObject.Disabled && 
                    !sceneObject.MapSetting && !sceneObject.HasFlag(SceneObject.ScenePosFlagType.Detected))
                    toChoose.Add(sceneObject);
            }

            var results = NLRandomPicker<SceneObject>.RandomPickN(toChoose, (uint)count);
            foreach (var sceneObject in results)
                sceneObject.AddFlag(SceneObject.ScenePosFlagType.Detected);
        }
    }
}
