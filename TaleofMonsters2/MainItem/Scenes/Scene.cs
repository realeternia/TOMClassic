using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Control;
using NarlonLib.Drawing;
using NarlonLib.Math;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Blesses;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.Quests;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.MainItem.Blesses;
using TaleofMonsters.MainItem.Scenes.SceneObjects;
using TaleofMonsters.MainItem.Scenes.SceneRules;

namespace TaleofMonsters.MainItem.Scenes
{
    internal class Scene
    {
        #region 委托
        delegate void TimelySceneCallback(SceneObject f);
        private void TimelyCheck(SceneObject f)
        {
            if (MainForm.Instance.InvokeRequired)
            {
                TimelySceneCallback d = OnMoveEnd;
                MainForm.Instance.Invoke(d, new object[] { f });
            }
            else
            {
                OnMoveEnd(f);
            }
        }
        #endregion
        private class MovingData
        {
            public float Time { get; set; }
            public Point Source { get; set; }
            public Point Dest { get; set; }
            public int DestId { get; set; }
        }
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
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private MovingData movingData = new MovingData();

        private const float ChessMoveAnimTime =0.5f;//旗子跳跃的动画时间

        private bool allEventFinished; //所有的事件都完成了

        public Scene(Control p, int w, int h)
        {
            parent = p;
            width = w;
            height = h;

            int xOff = (width - 688) / 2 + 103;
            vRegion = new VirtualRegion(parent);

            vRegion.AddRegion(new SubVirtualRegion(1, xOff - 60 + 82 * 6, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(2, xOff-60, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(3, xOff - 60+82, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(4, xOff - 60 + 82*2, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(5, xOff - 60 + 82*3, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(6, xOff - 60 + 82*4, 13, 80, 20));
            vRegion.AddRegion(new SubVirtualRegion(7, xOff - 60 + 82*5, 13, 80, 20));
            
            vRegion.AddRegion(new SubVirtualRegion(10, 0, 0, 150, 50));//人物头像
            vRegion.AddRegion(new SubVirtualRegion(11, width - 145, 3, 115, 32));//场景信息

            for (int i = 0; i < 10; i++)
            {//bless
                vRegion.AddRegion(new PictureRegion(20+i, i*60+10, 55, 50, 50, PictureRegionCellType.Bless, 0));
            }

            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;
        }

        public void Init()
        {
            mainTop = PicLoader.Read("System", "MainTop.JPG");
            mainTopRes = PicLoader.Read("System", "MainTopRes.PNG");
            mainTopTitle = PicLoader.Read("System", "MainTopTitle.PNG");
            mainBottom = PicLoader.Read("System", "MainBottom.JPG");
            miniBack = PicLoader.Read("System", "MiniBack.PNG");
        }

        public void ChangeMap(int mapid, bool isWarp)
        {
            if (backPicture != null)
                backPicture.Dispose();
            SceneConfig sceneConfig = ConfigData.GetSceneConfig(mapid);
            backPicture = PicLoader.Read("Scene", string.Format("{0}.JPG", sceneConfig.Url));
            sceneName = sceneConfig.Name;

            GenerateMiniMap(mapid, MathTool.Clamp(sceneConfig.IconX - 110,0, 1688-300), MathTool.Clamp(sceneConfig.IconY - 110, 0, 1121 - 300));

            allEventFinished = false;

            UserProfile.Profile.OnSwitchScene(isWarp);
            UserProfile.InfoBasic.MapId = mapid;//这句必须在存档后

            BlessManager.OnChangeMap();
            OnBlessChange();
            BlessManager.Update = OnBlessChange;
            
            SystemMenuManager.ResetIconState(); //reset main icon state todo remove check

            if (sceneConfig.Type == (int)SceneTypes.Dungeon)
            {
                Rule = new SceneRuleDungeon();
            }
            else if (sceneConfig.Type == (int)SceneTypes.Town)
            {
                Rule = new SceneRuleTown();
            }
            else
            {
                Rule = new SceneRuleCommon();
            }
            TimeMinutes = (int)DateTime.Now.TimeOfDay.TotalMinutes;
            Rule.Init(mapid, TimeMinutes);
            SceneInfo = SceneManager.RefreshSceneObjects(UserProfile.InfoBasic.MapId, width, height - 35, isWarp ? SceneFreshReason.Warp : SceneFreshReason.Load );
            if (UserProfile.InfoBasic.Position == 0 && SceneInfo.Items.Count > 0)//兜底处理
                UserProfile.InfoBasic.Position = SceneInfo.Items[0].Id;
            parent.Invalidate();
        }

        private void OnBlessChange()
        {
            for (int i = 0; i < 10; i++)
            {//bless
                vRegion.SetRegionKey(20 + i, 0);
            }
            int index = 0;
            foreach (var key in UserProfile.InfoWorld.Blesses.Keys)
            {
                vRegion.SetRegionKey(20 + index++, key);
            }
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
            {
                miniMap.Dispose();
            }
            miniMap = new Bitmap(150, 150); //绘制小地图
            g = Graphics.FromImage(miniMap);
            Rectangle destRect = new Rectangle(0, 0, 150, 150);
            g.DrawImage(allMap, destRect, wx, wy, 300, 300, GraphicsUnit.Pixel);
            g.Dispose();
            allMap.Dispose();
        }

        public void TimeGo(float timePast)
        {
            if (movingData.Time > 0)
            {
                movingData.Time = Math.Max(0, movingData.Time - timePast);
                var x = movingData.Source.X/2 + movingData.Dest.X/2;
                var y = movingData.Source.Y/2 + movingData.Dest.Y/2;
                parent.Invalidate(new Rectangle(x - 200, y - 200, 400, 400));

                if (movingData.Time <= 0)
                {
                    movingData.Time = 0;

                    foreach (var sceneObject in SceneInfo.Items)
                    {
                        if (sceneObject.Id == movingData.DestId)
                        {
                            UserProfile.Profile.InfoBasic.Position = sceneObject.Id;
                            TimelyCheck(sceneObject);
                            parent.Invalidate();
                        }
                    }
                }
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
                        {
                            UserProfile.Profile.OnNewDay();
                        }
                        parent.Invalidate();
                    }
                }
            }
        }

        private static void OnMoveEnd(SceneObject o)
        {
            try //因为这一步会被invoke，所以单独套一层try
            {
                o.MoveEnd();
            }
            catch (Exception e)
            {
                NarlonLib.Log.NLog.Error(e);
                throw;
            }
            
        }

        public void OnEventFinish()
        {
            if (UserProfile.InfoBasic.HealthPoint <= 0 || UserProfile.InfoBasic.MentalPoint <= 0)
            {
                var goldSub = (uint)Math.Ceiling((double)UserProfile.InfoBag.Resource.Gold / 5);
                MessageBoxEx.Show(string.Format("你死了，失去了{0}的金钱", goldSub));
                UserProfile.Profile.OnDie();
                var config = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId);
                ChangeMap(config.ReviveScene, true);
                UserProfile.InfoBasic.Position = SceneInfo.GetRevivePos();
            }

            CheckAllQuestOpened();
        }
        
        public void CheckMouseMove(int x, int y)
        {
            if (movingData.Time > 0) return;
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
            if (movingData.Time > 0) return;
            SceneObject src = null;
            SceneObject dest = null;
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject.Id == cellTar)
                {
                    dest = sceneObject;
                }
                if (sceneObject.Id == UserProfile.InfoBasic.Position)
                {
                    src = sceneObject;
                }
            }

            if (dest!=null && dest.OnClick())
            {
                int drawWidth = 57 * src.Width / GameConstants.SceneTileStandardWidth;
                int drawHeight = 139 * src.Height / GameConstants.SceneTileStandardHeight;
                movingData.Source = new Point(src.X - drawWidth / 2 + src.Width / 8, src.Y - drawHeight + src.Height / 3);
                movingData.DestId = dest.Id;
                movingData.Dest = new Point(dest.X - drawWidth / 2 + dest.Width / 8, dest.Y - drawHeight + dest.Height / 3);
                movingData.Time = ChessMoveAnimTime;
                parent.Invalidate();
            }
        }


        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id >= 20)
            {
                if (key > 0)
                {
                    Image image = BlessBook.GetPreview(key);
                    tooltip.Show(image, parent, x, y);
                }
            }
            else if (id == 10)
            {
                Image image = GetPlayerImage();
                tooltip.Show(image, parent, 0,50);
            }
            else if (id == 11)
            {
                Image image = GetSceneImage();
                tooltip.Show(image, parent, width - image.Width, 35);
            }
            else if (id < 10)
            {
                var resName = HSTypes.I2Resource(id - 1);
                string resStr = string.Format("{0}:{1}", resName, UserProfile.Profile.InfoBag.Resource.Get((GameResourceType)(id-1)));
                Image image = DrawTool.GetImageByString(resStr, 100);
                tooltip.Show(image, parent, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }


        public void Paint(Graphics g)
        {
            g.DrawImage(backPicture, 0, 50, width, height-35);
            g.DrawImage(mainTop, 0, 0, width, 50);
            g.DrawImage(mainTopRes, (width-688)/2, 4, 688, 37);//688是图片尺寸
            g.DrawImage(mainTopTitle, width-145, 3, 115, 32);
            g.DrawImage(mainBottom, 0, height-35, width, 35);
            
            if (UserProfile.Profile == null || UserProfile.InfoBasic.MapId == 0)
            {
                return;
            }

            Font font = new Font("微软雅黑", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            Font font2 = new Font("宋体", 9*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);

            Image head = PicLoader.Read("Player", string.Format("{0}.PNG", UserProfile.InfoBasic.Face));
            g.DrawImage(head, 0, 0, 50, 50);
            head.Dispose();
            g.DrawString(UserProfile.InfoBasic.Level.ToString(), font2, Brushes.Black, 3, 3);
            g.DrawString(UserProfile.InfoBasic.Level.ToString(), font2, Brushes.White, 2, 2);
           // g.DrawString(UserProfile.ProfileName, font, Brushes.White, 50, 10);

            LinearGradientBrush b1 = new LinearGradientBrush(new Rectangle(55, 5, 100, 5), Color.LightCoral, Color.Red, LinearGradientMode.Horizontal);
            g.FillRectangle(b1, 55, 5, Math.Min(UserProfile.InfoBasic.HealthPoint, 100), 5);
            g.DrawRectangle(Pens.DarkGray, 55, 5, 100, 5);
            b1.Dispose();

            b1 = new LinearGradientBrush(new Rectangle(55, 12, 100, 5), Color.LightBlue, Color.Blue, LinearGradientMode.Horizontal);
            g.FillRectangle(b1, 55, 12, Math.Min(UserProfile.InfoBasic.MentalPoint, 100), 5);
            g.DrawRectangle(Pens.DarkGray, 55, 12, 100, 5);
            b1.Dispose();

            b1 = new LinearGradientBrush(new Rectangle(55, 19, 100, 5), Color.LightGreen, Color.Green, LinearGradientMode.Horizontal);
            g.FillRectangle(b1, 55, 19, Math.Min(UserProfile.InfoBasic.FoodPoint, 100), 5);
            g.DrawRectangle(Pens.DarkGray, 55, 19, 100, 5);
            b1.Dispose();

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

            var len = TextRenderer.MeasureText(g, sceneName, font, new Size(0, 0), TextFormatFlags.NoPadding).Width;
            g.DrawString(sceneName, font, Brushes.White, new PointF(width - 85 - len / 2, 8));

            int xOff = (width - 688)/2 + 103;
            g.DrawString(UserProfile.InfoBag.Resource.Lumber.ToString(), font, Brushes.White, new PointF(xOff, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Stone.ToString(), font, Brushes.White, new PointF(xOff+82, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Mercury.ToString(), font, Brushes.White, new PointF(xOff+82*2, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Carbuncle.ToString(), font, Brushes.White, new PointF(xOff + 82 * 3, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Sulfur.ToString(), font, Brushes.White, new PointF(xOff + 82 * 4, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Gem.ToString(), font, Brushes.White, new PointF(xOff + 82 * 5, 13));
            g.DrawString(UserProfile.InfoBag.Resource.Gold.ToString(), font, Brushes.White, new PointF(xOff + 82 * 6, 13));

            xOff = (width - 688) / 2 + 30;
            g.FillRectangle(Brushes.DimGray, xOff, 41, 630, 8);
            b1 = new LinearGradientBrush(new Rectangle(xOff, 41, 630, 8), Color.White, Color.Gray, LinearGradientMode.Vertical);
            g.FillRectangle(b1, xOff, 41, Math.Min(UserProfile.InfoBasic.Exp*630/ExpTree.GetNextRequired(UserProfile.InfoBasic.Level), 630), 8);
            g.DrawRectangle(new Pen(Brushes.Black, 2), xOff, 41, 630, 8);
            b1.Dispose();

            font.Dispose();
            font2.Dispose();

            g.DrawImage(miniMap, width-160, 43, 150, 150);
            g.DrawImage(miniBack, width - 190, 38, 185, 160);

            vRegion.Draw(g);

            DrawCellAndToken(g);
        }

        private void DrawCellAndToken(Graphics g)
        {
            SceneObject possessCell = null;
            SceneObject selectTarget = null;
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject.Id == cellTar)
                {
                    selectTarget = sceneObject;
                }
                else
                {//先绘制非目标
                    sceneObject.Draw(g, false);
                }
                
                if (sceneObject.Id == UserProfile.Profile.InfoBasic.Position)
                {
                    possessCell = sceneObject;
                }
            }

            if (selectTarget != null)
            {
                selectTarget.Draw(g, true);
            }


            if (possessCell != null)
            {
                Image token = PicLoader.Read("Map", "Token.PNG");
                int drawWidth = token.Width*possessCell.Width/GameConstants.SceneTileStandardWidth;
                int drawHeight = token.Height*possessCell.Height/GameConstants.SceneTileStandardHeight;
                if (movingData.Time <= 0)
                {
                    g.DrawImage(token, possessCell.X - drawWidth/2 + possessCell.Width/8,
                        possessCell.Y - drawHeight + possessCell.Height/3, drawWidth, drawHeight);
                }
                else
                {
                    int realX = (int)(movingData.Source.X*(movingData.Time)/ChessMoveAnimTime +
                             movingData.Dest.X*(ChessMoveAnimTime - movingData.Time)/ChessMoveAnimTime);
                    int yOff = (int) (Math.Pow(realX - (movingData.Source.X + movingData.Dest.X)/2, 2)*(4*80)/
                                      Math.Pow(movingData.Source.X - movingData.Dest.X, 2) - 80);
                    var pos = new Point(realX,yOff +(int)(movingData.Source.Y*(movingData.Time)/ChessMoveAnimTime +
                                 movingData.Dest.Y*(ChessMoveAnimTime - movingData.Time)/ChessMoveAnimTime));
                    g.DrawImage(token, pos.X, pos.Y, drawWidth, drawHeight);
                }
                token.Dispose();
            }
        }

        private int GetDisableEventCount(int eid)
        {
            int count = 0;
            foreach (var sceneObject in SceneInfo.Items)
            {
                var sceQuest = sceneObject as SceneQuest;
                if (sceQuest != null && sceQuest.EventId == eid && sceneObject.Disabled)
                {
                    count++;
                }
            }
            return count;
        }

        public int GetWarpPosByMapId(int mapId)
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                var warp = sceneObject as SceneWarp;
                if (warp != null && warp.TargetMap == mapId)
                {
                    return warp.Id;
                }
            }
            return 0;
        }


        public SceneObject GetObjectByPos(int pos)
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject != null && sceneObject.Id == pos)
                {
                    return sceneObject;
                }
            }
            return null;
        }

        public bool HasSceneItemWithName(string name)
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                var quest = sceneObject as SceneQuest;
                if (quest != null && quest.EventId > 0)
                {
                    var config = ConfigData.GetSceneQuestConfig(quest.EventId);
                    if (config.Ename.StartsWith(name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void CheckAllQuestOpened()
        {
            if (allEventFinished)
                return;

            foreach (var sceneObject in SceneInfo.Items)
            {
                var quest = sceneObject as SceneQuest;
                if (quest != null && !quest.MapSetting && !quest.Disabled)
                {
                    return;
                }
            }
            QuestBook.CheckAllQuestWith("allopen");
            allEventFinished = true;
        }

        private Image GetSceneImage()
        {
            var config = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId);
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(string.Format("{0}(Lv{1})", sceneName, config.Level), "LightBlue", 20);
            tipData.AddLine(2);
            tipData.AddTextNewLine(string.Format("格子:{0}", SceneInfo.Items.Count), "White");
            foreach (var questData in SceneQuestBook.GetQuestConfigData(UserProfile.InfoBasic.MapId))
            {
                var questConfig = ConfigData.GetSceneQuestConfig(questData.Id);
                if (questConfig.Type == (int)SceneQuestTypes.Hidden)
                    continue;
                var happend = GetDisableEventCount(questData.Id);
                var evtLevel = questConfig.Level == 0 ? config.Level : questConfig.Level;
                tipData.AddTextNewLine(string.Format(" {0}Lv{3}({1}/{2})", questConfig.Name,
                    happend, questData.Value, evtLevel), happend == questData.Value ? "DimGray" : HSTypes.I2QuestDangerColor(questConfig.Danger));
            }
            return tipData.Image;
        }

        private Image GetPlayerImage()
        {
            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
            tipData.AddTextNewLine(string.Format("{0}(Lv{1})", UserProfile.ProfileName, UserProfile.InfoBasic.Level), "LightBlue", 20);
            tipData.AddLine(2);
            tipData.AddTextNewLine(string.Format("生命:{0}",UserProfile.InfoBasic.HealthPoint), "Red");
            tipData.AddTextNewLine(string.Format("精神:{0}", UserProfile.InfoBasic.MentalPoint), "LightBlue");
            tipData.AddTextNewLine(string.Format("食物:{0}", UserProfile.InfoBasic.FoodPoint), "LightGreen");
            return tipData.Image;
        }

        public void ResetScene()
        {
            SceneInfo = SceneManager.RefreshSceneObjects(UserProfile.InfoBasic.MapId, width, height - 35, SceneFreshReason.Reset);
            parent.Invalidate();
        }

        public void EnableTeleport()
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject is SceneWarp)
                {
                    sceneObject.SetEnable(true);//激活所有的传说门
                }
            }
            parent.Invalidate();
        }

        public void RandomPortal()
        {
            UserProfile.InfoBasic.Position = SceneInfo.Items[MathTool.GetRandom(SceneInfo.Items.Count)].Id; //todo 这样会随机到隐藏格子，不大对
            parent.Invalidate();
        }

        public void MoveTo(int target)
        {
            UserProfile.InfoBasic.Position = target;
            parent.Invalidate();
        }

        public void HiddenWay()
        {    
            int fromId = UserProfile.InfoBasic.Position;
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject is SceneQuest)
                {
                    var config = ConfigData.GetSceneQuestConfig((sceneObject as SceneQuest).EventId);
                    if (config.Ename == "hiddeway" && sceneObject.Id != fromId)
                    {
                        UserProfile.InfoBasic.Position = sceneObject.Id;
                        parent.Invalidate();
                        return;
                    }
                }
            }

            while (true)
            {
                int index = MathTool.GetRandom(SceneInfo.Items.Count);
                var targetCell = SceneInfo.Items[index];
                if (targetCell.Id == fromId)
                    continue;
                if (!targetCell.CanBeReplaced())
                    continue;
                int qId = SceneQuestBook.GetSceneQuestByName("hiddeway");
                SceneInfo.Items[index] = new SceneQuest(targetCell.Id, targetCell.X, targetCell.Y, targetCell.Width, targetCell.Height, qId);
                SceneInfo.Items[index].MapSetting = true;
                UserProfile.InfoBasic.Position = targetCell.Id;
                UserProfile.InfoWorld.UpdatePosInfo(targetCell.Id, qId);
                UserProfile.InfoWorld.UpdatePosMapSetting(targetCell.Id, true);
                parent.Invalidate();
                break;
            }
        }

        public void QuestNext(string qname)
        {
            int fromId = UserProfile.InfoBasic.Position;
            while (true)
            {
                int index = MathTool.GetRandom(SceneInfo.Items.Count);
                var targetCell = SceneInfo.Items[index];
                if (targetCell.Id == fromId)
                    continue;
                if (!targetCell.CanBeReplaced())
                    continue;
                int qId = SceneQuestBook.GetSceneQuestByName(qname);
                SceneInfo.Items[index] = new SceneQuest(targetCell.Id, targetCell.X, targetCell.Y, targetCell.Width, targetCell.Height, qId);
                SceneInfo.Items[index].MapSetting = true;
                UserProfile.InfoWorld.UpdatePosInfo(targetCell.Id, qId);
                UserProfile.InfoWorld.UpdatePosMapSetting(targetCell.Id, true);
                break;
            }
           
            parent.Invalidate();
        }

        public void DetectAll()
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject is SceneQuest)
                {
                    sceneObject.AddFlag(SceneObject.ScenePosFlagType.Detected);
                }
            }
        }

        public void DetectNear(int range)
        {
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject is SceneQuest && SceneManager.GetDistance(sceneObject.Id, UserProfile.InfoBasic.Position) <= range)
                {
                    sceneObject.AddFlag(SceneObject.ScenePosFlagType.Detected);
                }
            }
        }
        public void DetectRandom(int count)
        {
            List<SceneObject> toChoose = new List<SceneObject>();
            foreach (var sceneObject in SceneInfo.Items)
            {
                if (sceneObject is SceneQuest && !sceneObject.Disabled && 
                    !sceneObject.MapSetting && !sceneObject.HasFlag(SceneObject.ScenePosFlagType.Detected))
                {
                    toChoose.Add(sceneObject);
                }
            }

            var results = NLRandomPicker<SceneObject>.RandomPickN(toChoose, (uint)count);
            foreach (var sceneObject in results)
            {
                sceneObject.AddFlag(SceneObject.ScenePosFlagType.Detected);
            }
        }
    }
}
