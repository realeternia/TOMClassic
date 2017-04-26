using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using NarlonLib.Core;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Battle.Data;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.HeroSkills;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Controler.Battle
{
    internal sealed partial class BattleForm : BasePanel
    {
        #region 委托
        delegate void CloseFormCallback();
        private void CloseForm()
        {
            if (InvokeRequired)
            {
                CloseFormCallback d = CloseForm;
                Invoke(d, new object[] { });
            }
            else
            {
                ComClose();
            }
        }
        #endregion

        internal HsActionCallback BattleWin;
        internal HsActionCallback BattleLose;

        private bool showGround;
        private bool isGamePaused = true;
        private bool gameEnd;
        private bool isMouseIn;
        private int mouseX, mouseY;
        private int showPlayerState;
        private bool onTurn;

        private HSCursor myCursor;
        private ActiveCard leftSelectCard;
        private bool isHuman;

        private int leftId;
        private int rightId;
        private int rightLevel;
        private string mapName;

        private long lastMouseMoveTime;

        private VirtualRegion vRegion;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private MagicRegion magicRegion = new MagicRegion(); //可能会绘制多次
        private CardVisualRegion visualRegion = new CardVisualRegion(); //怪物选中后的地图效果

        private List<DateTime> fpsList = new List<DateTime>();

        public bool IsGamePaused
        {
            get { return isGamePaused; }
            set { isGamePaused = value; CheckEnable(); }
        }

        public BattleForm()
        {
            InitializeComponent();
            NeedBlackForm = true;
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");

            vRegion = new VirtualRegion(this);
            vRegion.RegionClicked += OnVRegionClick;
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;

            cardSelector1.StartClicked += buttonStart_Click;
#if DEBUG
            cardList2.Visible = true;
#endif
            myCursor = new HSCursor(this);
            lifeClock1.Init();
            lifeClock2.Init();
            lifeClock1.IsLeft = true;
            lifeClock2.IsLeft = false;
        }

        public void Init(int lid, int rid, string map, int rlevel, PeopleFightParm reason)
        {
            isHuman = lid == 0;
            rightId = rid;
            leftId = lid;
            rightLevel = rlevel;
            if (leftId > 0)
            {
                lifeClock1.SetPlayer(leftId);
                miniItemView1.Visible = false;
            }
            else
            {
                lifeClock1.SetPlayer(UserProfile.ProfileName, UserProfile.InfoBasic.Face);
            }
            lifeClock2.SetPlayer(rightId);
            mapName = map;
            timeViewer1.Init();
            cardsArray1.Init();
            cardList2.Init();
            miniItemView1.Init();
            miniItemView1.Enabled =false;
            cardsArray1.SetEnable(false);

            BattleManager.Instance.Init();
            BattleManager.Instance.RuleData.Parm = reason;
            BattleManager.Instance.PlayerManager.Init(leftId, rightId, rightLevel);
            int index = 0;//初始化英雄技能按钮
            foreach (var skillId in BattleManager.Instance.PlayerManager.LeftPlayer.HeroSkillList)
            {
                var region = new PictureAnimRegion(index+1, 25, 518+index*45, 40, 40, PictureRegionCellType.HeroSkill, skillId);
                region.AddDecorator(new RegionBorderDecorator(Color.Lime));
                vRegion.AddRegion(region);
                index++;
            }

            lifeClock1.Player = BattleManager.Instance.PlayerManager.LeftPlayer;
            lifeClock2.Player = BattleManager.Instance.PlayerManager.RightPlayer;
            BattleManager.Instance.PlayerManager.LeftPlayer.CardsDesk = cardsArray1;
            BattleManager.Instance.PlayerManager.LeftPlayer.InitialCards();
            BattleManager.Instance.PlayerManager.RightPlayer.CardsDesk = cardList2;
            BattleManager.Instance.PlayerManager.RightPlayer.InitialCards();
            cardSelector1.Init(BattleManager.Instance.PlayerManager.LeftPlayer);
            BattleManager.Instance.PlayerManager.LeftPlayer.HeroSkillChanged += LeftPlayer_HeroSkillChanged;
            cardsArray1.Visible = false;
            miniItemView1.Visible = false;
            vRegion.Visible = false;
        }

        private void StartGame() //初始化游戏
        {
            BattleManager.Instance.MemMap = new MemRowColumnMap(mapName, 0);
            BattleManager.Instance.MemMap.InitUnit(BattleManager.Instance.PlayerManager.LeftPlayer);
            BattleManager.Instance.MemMap.InitUnit(BattleManager.Instance.PlayerManager.RightPlayer);
            AIStrategy.OnInit(BattleManager.Instance.PlayerManager.RightPlayer);

            BattleManager.Instance.StatisticData.StartTime = DateTime.Now;
            BattleManager.Instance.StatisticData.EndTime = DateTime.Now;

            cardsArray1.Visible = true;
            miniItemView1.Visible = true;
            vRegion.Visible = true;
            IsGamePaused = false;
            showGround = true;
            Invalidate();
        }

        private void StopGame()
        {
            isGamePaused = true;
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            fpsList.Add(DateTime.Now);//fps计算
            while (fpsList.Count > 11)
                fpsList.RemoveAt(0);

            if (onTurn)
                return;
            onTurn = true;
            NLCoroutine.DoTimer();
            try
            {
                panelBattle.Invalidate();

                if (BattleManager.Instance.PlayerManager.LeftPlayer == null || BattleManager.Instance.PlayerManager.RightPlayer == null)
                    return;
                
                if (IsGamePaused)
                    return;

                if (!BattleManager.Instance.PlayerManager.LeftPlayer.IsAlive || !BattleManager.Instance.PlayerManager.RightPlayer.IsAlive)
                {
                    EndGame();
                    return;
                }

                CheckCursor();
                miniItemView1.NewTick();
                timeViewer1.TimeGo(BattleManager.Instance.Round);

                BattleManager.Instance.Next();
            }
            catch(Exception e)
            {
                NarlonLib.Log.NLog.Error(e);
            }
            finally
            {
                onTurn = false;
            }
        }

        private void EndGame()
        {
            if (!gameEnd)
            {
                gameEnd = true;
                StopGame();
                lifeClock1.Invalidate();
                lifeClock2.Invalidate();
                BattleManager.Instance.StatisticData.PlayerWin = !BattleManager.Instance.PlayerManager.RightPlayer.IsAlive;
                BattleManager.Instance.StatisticData.EndTime = DateTime.Now;
                CloseForm();
            }
        }

        private void panelBattle_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bmp = new Bitmap(panelBattle.Width, panelBattle.Height);
            Graphics g = Graphics.FromImage(bmp);

            try
            {
                if (showGround)
                {
                    BattleManager.Instance.Draw(g, magicRegion, visualRegion, mouseX, mouseY, isMouseIn);
                    
                    LiveMonster target = BattleLocationManager.GetPlaceMonster(mouseX, mouseY);
                    if (target != null && isMouseIn && !magicRegion.Active)
                    {
                        target.LiveMonsterToolTip.DrawCardToolTips(g);
                    }
                    if (showPlayerState == 1)
                        BattleManager.Instance.PlayerManager.LeftPlayer.DrawToolTips(g);
                    else if (showPlayerState == 2)
                        BattleManager.Instance.PlayerManager.RightPlayer.DrawToolTips(g);
#if !DEBUG
                if (IsGamePaused)
                {
                    Font font = new Font("微软雅黑", 30*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                    g.DrawString("游戏暂停", font, Brushes.OrangeRed, 370, 170);
                    font.Dispose();
                }
#endif
                }
            }
            catch (Exception err)
            {
                NarlonLib.Log.NLog.Error("panelBattle_Paint" + err);
            }

            e.Graphics.DrawImageUnscaled(bmp, 0, 0);
            g.Dispose();
            bmp.Dispose();

            if (fpsList.Count >= 10)//fps显示
            {
                int fps = (int)Math.Round((fpsList.Count - 1) * 1000 / fpsList[fpsList.Count - 1].Subtract(fpsList[0]).TotalMilliseconds);
                Font fontFps = new Font("黑体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                e.Graphics.DrawString(string.Format("fps {0}", fps), fontFps, Brushes.White, 3, 3);
                fontFps.Dispose();
            }
        }


        private void panelBattle_MouseClick(object sender, MouseEventArgs e)
        {
            if (BattleManager.Instance.PlayerManager.LeftPlayer == null)
                return;

            if (isGamePaused)
                return;

            int cardSize = BattleManager.Instance.MemMap.CardSize;
            if (e.Button == MouseButtons.Left)
            {
                if (leftSelectCard != null && (myCursor.Name == "summon" || myCursor.Name == "equip" || myCursor.Name == "cast" || myCursor.Name == "sidekick"))
                {
                    int result;
                    if ((result = BattleManager.Instance.PlayerManager.LeftPlayer.CheckUseCard(leftSelectCard, BattleManager.Instance.PlayerManager.LeftPlayer, BattleManager.Instance.PlayerManager.RightPlayer)) != HSErrorTypes.OK)
                    {
                        BattleManager.Instance.FlowWordQueue.Add(new FlowErrInfo(result, new Point(mouseX, mouseY), 0, 0), false);
                        return;
                    }

                    LiveMonster lm = BattleLocationManager.GetPlaceMonster(mouseX, mouseY);
                    if (myCursor.Name == "summon" && lm == null)
                    {
                        var pos = new Point(mouseX/cardSize*cardSize, mouseY/cardSize*cardSize);
                        BattleManager.Instance.PlayerManager.LeftPlayer.UseMonster(leftSelectCard, pos);
                    }
                    else if (myCursor.Name == "equip" && lm != null)
                    {
                        BattleManager.Instance.PlayerManager.LeftPlayer.UseWeapon(lm, leftSelectCard);
                    }
                    else if (myCursor.Name == "sidekick" && lm != null)
                    {
                        BattleManager.Instance.PlayerManager.LeftPlayer.UseSideKick(lm, leftSelectCard);
                    }
                    else if (myCursor.Name == "cast")
                    {
                        BattleManager.Instance.PlayerManager.LeftPlayer.DoSpell(lm, leftSelectCard, e.Location);
                    }

                    var cardData = CardConfigManager.GetCardConfig(leftSelectCard.CardId);
                    UserProfile.Profile.OnUseCard(cardData.Star, 0, cardData.TypeSub);

                    cardsArray1.DisSelectCard();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                leftSelectCard = null;
                cardsArray1.DisSelectCard();
            }
        }

        private void panelBattle_MouseMove(object sender, MouseEventArgs e)
        {
            if (lastMouseMoveTime + 50 > TimeTool.GetNowMiliSecond())
            {
                return;
            }
            lastMouseMoveTime = TimeTool.GetNowMiliSecond();

            mouseX = e.X;
            mouseY = e.Y;
        }

        private void panelBattle_MouseEnter(object sender, EventArgs e)
        {
            isMouseIn = true;
        }

        private void panelBattle_MouseLeave(object sender, EventArgs e)
        {
            mouseX = -1;
            isMouseIn = false;
        }

        private void CheckCursor()
        {
            string cursorname = "default";
            magicRegion.Active = false;
            if (leftSelectCard != null)
            {
                if (leftSelectCard.CardType == CardTypes.Monster)
                {
                    if (BattleLocationManager.IsPlaceCanSummon(leftSelectCard.CardId, mouseX, mouseY, true))
                    {
                        cursorname = "summon";
                        magicRegion.Active = true;
                    }
                    else
                    {
                        var placeMon = BattleLocationManager.GetPlaceMonster(mouseX, mouseY);
                        if (placeMon != null && placeMon.IsLeft && !placeMon.Avatar.MonsterConfig.IsBuilding)
                        {
                            if (MonsterBook.HasTag(leftSelectCard.CardId, "sidekicker") ||
                                MonsterBook.HasTag(placeMon.CardId, "sidekickee") || 
                                    BattleManager.Instance.PlayerManager.LeftPlayer.SpikeManager.HasSpike("sidekickall"))
                            {
                                cursorname = "sidekick";
                            }
                        }
                    }
                }
                else if (leftSelectCard.CardType == CardTypes.Weapon)
                {
                    LiveMonster lm = BattleLocationManager.GetPlaceMonster(mouseX, mouseY);
                    if (lm != null && lm.CanAddWeapon() && lm.IsLeft)
                    {
                        cursorname = "equip";
                    }
                }
                else if (leftSelectCard.CardType == CardTypes.Spell)
                {
                    if (mouseX > 0)
                    {
                        SpellConfig spellConfig = ConfigData.GetSpellConfig(leftSelectCard.CardId);
                        if (BattleLocationManager.IsPlaceCanCast(mouseX, mouseY, spellConfig.Target))
                        {
                            magicRegion.Active = true;
                            cursorname ="cast";
                        }
                        else
                        {
                            cursorname = "nocast";
                        }
                    }
                }
            }
            myCursor.ChangeCursor(cursorname);
        }

        private void timeViewer1_Click(object sender, EventArgs e)
        {
            IsGamePaused = !IsGamePaused;
        }

        private void lifeClock1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > 2 && e.X < 60 && e.Y > 2 && e.Y < 66)
                showPlayerState = 1;
            else
                showPlayerState = 0;
        }

        private void lifeClock1_MouseLeave(object sender, EventArgs e)
        {
            showPlayerState = 0;
        }

        private void lifeClock2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > 320 && e.X < 378 && e.Y > 2 && e.Y < 66)
                showPlayerState = 2;
            else
                showPlayerState = 0;
        }

        private void cardsArray1_SelectionChange(object sender, EventArgs e)
        {
            leftSelectCard = cardsArray1.GetSelectCard();
            if (leftSelectCard != null && leftSelectCard.CardType == CardTypes.Monster)
                visualRegion.Update(leftSelectCard.CardId);
            else
                visualRegion.Update(0);
            OnSelectCardChange();
            panelState.Invalidate();
        }

        private void panelState_Paint(object sender, PaintEventArgs e)
        {
            if (showGround)
            {
                //todo 可以用来画其他东西
            }
        }

        private void OnSelectCardChange()
        {
            magicRegion.Clear();
            if (leftSelectCard != null)
            {
                if (leftSelectCard.CardType == CardTypes.Monster)
                {
                    var monsterConfig = ConfigData.GetMonsterConfig(leftSelectCard.CardId);
                    magicRegion.Add(RegionTypes.Circle, monsterConfig.Range, Color.Magenta);
                    var skillConfig = MonsterBook.GetAreaSkill(leftSelectCard.CardId);
                    if (skillConfig != null)
                    {
                        var type = BattleTargetManager.GetRegionType(skillConfig.Target[2]);
                        if (type != RegionTypes.None)
                        {
                            magicRegion.Add(type, skillConfig.Range, MagicRegion.GetTargetColor(skillConfig.Target[1]));
                        }
                    }
                }
                else if (leftSelectCard.CardType == CardTypes.Spell)
                {
                    var spellConfig = ConfigData.GetSpellConfig(leftSelectCard.CardId);
                    var type = BattleTargetManager.GetRegionType(spellConfig.Target[2]);
                    if (type != RegionTypes.None)
                    {
                        magicRegion.Add(type, spellConfig.Range, MagicRegion.GetTargetColor(spellConfig.Target[1]));
                    }
                }
            }
        }

        private void CheckEnable()
        {
            cardsArray1.SetEnable(!isGamePaused);
            miniItemView1.Enabled = !isGamePaused;
        }

        private void buttonStart_Click()
        {
            StartGame();
            cardSelector1.Hide();
            miniItemView1.Enabled = true;
        }

        private bool CloseCheck()
        {
            if (!isHuman)
            {
                return false;
            }

            IsGamePaused = true;
            if (ControlPlus.MessageBoxEx2.Show("现在退出战斗，将会被判输哦！") == DialogResult.Cancel)
            {
                IsGamePaused = false;
                return false;
            }
            StopGame();
            return true;
        }

        private void ComClose()
        {
            OnGameOver();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (CloseCheck())
            {
                OnGameOver();
            }
        }

        private void OnVRegionClick(int id, int x, int y, MouseButtons button)
        {
            if (id > 0)
            {
                var key = vRegion.GetRegion(id).GetKeyValue();
                HeroSkillConfig heroSkillConfig = ConfigDatas.ConfigData.GetHeroSkillConfig(key);
                LevelExpConfig levelConfig = ConfigData.GetLevelExpConfig(UserProfile.Profile.InfoBasic.Level);
                leftSelectCard = new ActiveCard(heroSkillConfig.CardId, (byte)levelConfig.HeroSkillLevel, 0);
                leftSelectCard.IsHeroSkill = true;
                panelState.Invalidate();
                OnSelectCardChange();
            }
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id > 0)
            {
                Image image = HeroSkillBook.GetSkillPreview(key);
                tooltip.Show(image, this, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void LeftPlayer_HeroSkillChanged(bool active)
        {
            int index = 0;//初始化英雄技能按钮
            foreach (var skillId in BattleManager.Instance.PlayerManager.LeftPlayer.HeroSkillList)
            {
                if (active)
                {
                    vRegion.SetRegionDecorator(index + 1, 1, null);//移掉黑色遮罩
                }
                else
                {
                    var color = Color.FromArgb(150, Color.Black); //cd中就加一层黑色遮罩
                    var decorate = new RegionCoverDecorator(color);
                    vRegion.SetRegionDecorator(index + 1, 1, decorate);
                }
                index++;
            }
            Invalidate();
        }

        private void OnGameOver()
        {
            BattleManager.Instance.EffectQueue.Clear();
            BattleManager.Instance.MonsterQueue.Clear();
            BattleManager.Instance.FlowWordQueue.Clear();

            ImageManager.Compress();
            Close();
            if (BattleManager.Instance.PlayerManager.RightPlayer != null)
            {
                MainForm.Instance.DealPanel(new BattleResultForm());
            }
            if (BattleManager.Instance.StatisticData.PlayerWin)
            {
                if (BattleWin != null)
                {
                    BattleWin();
                }
            }
            else
            {
                if (BattleLose != null)
                {
                    BattleLose();
                }
            }
        }

        private void BattleForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            vRegion.Draw(e.Graphics);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString(" 模拟战斗 ", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();
        }
    }
}