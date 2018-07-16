using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using ControlPlus;
using NarlonLib.Tools;
using TaleofMonsters.Controler.Battle.Components.CardSelect;
using TaleofMonsters.Controler.Battle.Data;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.Core;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Core.Config;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Cards.Monsters;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.HeroPowers;
using TaleofMonsters.Datas.Peoples;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Controler.Battle
{
    public delegate void HsActionCallback();

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
        private bool gameEnd;
        private bool isMouseIn;
        private int mouseX, mouseY;
     //   private int showPlayerState;
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
        private ImageToolTip tooltip = SystemToolTip.Instance;
        private MagicRegion magicRegion = new MagicRegion(); //可能会绘制多次
        private CardVisualRegion visualRegion = new CardVisualRegion(); //怪物选中后的地图效果

        private List<DateTime> fpsList = new List<DateTime>();

        private bool isGamePaused = true;
        public bool IsGamePaused
        {
            get { return isGamePaused; }
            set { isGamePaused = value; CheckEnable(); }
        }

        public BattleForm()
        {
            InitializeComponent();
            NeedBlackForm = true;
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("Button.Panel", "CloseButton1.JPG");

            vRegion = new VirtualRegion(this);
            vRegion.RegionClicked += OnVRegionClick;
            vRegion.RegionEntered += virtualRegion_RegionEntered;
            vRegion.RegionLeft += virtualRegion_RegionLeft;

            cardSelector1.StartClicked += buttonStart_Click;
#if DEBUG
           // cardList2.Visible = true;
#endif
            myCursor = new HSCursor(this);
            lifeClock1.Init();
            lifeClock2.Init();
            lifeClock1.IsLeft = true;
            lifeClock2.IsLeft = false;
        }

        public void Init(int lid, DeckCard[] leftCards, int rid, string map, int rlevel, PeopleFightParm reason)
        {
            SetBgm("TOM006.mp3");

            isHuman = lid == 0;
            rightId = rid;
            leftId = lid;
            rightLevel = rlevel;
            mapName = map;
            timeViewer1.Init();
            cardsArray1.Init();
            cardList2.Init();
            miniItemView1.Init();
            miniItemView1.Enabled = false;
            cardsArray1.SetEnable(false);

            BattleManager.Instance.Init();
            BattleManager.Instance.RuleData.Parm = reason;
            BattleManager.Instance.PlayerManager.Init(leftId, leftCards, rightId, rightLevel);
            int index = 0;//初始化英雄技能按钮
            foreach (var skillId in BattleManager.Instance.PlayerManager.LeftPlayer.HeroSkillList)
            {
                var region = new PictureAnimRegion(index+1, 25, 538+index*45, 40, 40, PictureRegionCellType.HeroSkill, skillId);
                region.AddDecorator(new RegionBorderDecorator(Color.Lime));
                vRegion.AddRegion(region);
                index++;
            }
            if (leftId > 0)
            {
                lifeClock1.SetPlayer(BattleManager.Instance.PlayerManager.LeftPlayer, leftId);
                miniItemView1.Visible = false;
            }
            else
            {
                lifeClock1.SetPlayer(BattleManager.Instance.PlayerManager.LeftPlayer, UserProfile.Profile.Name, UserProfile.InfoBasic.Head);
            }
            lifeClock2.SetPlayer(BattleManager.Instance.PlayerManager.RightPlayer, rightId);
            BattleManager.Instance.PlayerManager.LeftPlayer.CardsDesk = cardsArray1;
            BattleManager.Instance.PlayerManager.LeftPlayer.InitialCards();
            BattleManager.Instance.PlayerManager.RightPlayer.CardsDesk = cardList2;
            BattleManager.Instance.PlayerManager.RightPlayer.InitialCards();
            cardSelector1.Init(BattleManager.Instance.PlayerManager.LeftPlayer, new CardSelectMethodInit());
            BattleManager.Instance.PlayerManager.LeftPlayer.HeroSkillChanged += LeftPlayerHeroSkillChanged;
            BattleManager.Instance.PlayerManager.LeftPlayer.OnShowCardSelector += LeftPlayerShowCardSelector;
            BattleManager.Instance.PlayerManager.LeftPlayer.OnUseCard += cardFlow1.OnPlayerUseCard;
            BattleManager.Instance.PlayerManager.RightPlayer.OnUseCard += cardFlow1.OnPlayerUseCard;
            BattleManager.Instance.RelicHolder.OnRelicRemove += cardFlow1.OnPlayerRelicTriggered;
            BattleManager.Instance.PlayerManager.LeftPlayer.OnKillEnemy += cardFlow1.OnPlayerKillMonster;
            BattleManager.Instance.PlayerManager.RightPlayer.OnKillEnemy += cardFlow1.OnPlayerKillMonster;
            BattleManager.Instance.MemMap = new MemRowColumnMap(mapName, 0);
            BattleManager.Instance.MemMap.InitUnit(BattleManager.Instance.PlayerManager.LeftPlayer);
            BattleManager.Instance.MemMap.InitUnit(BattleManager.Instance.PlayerManager.RightPlayer);
            showGround = true;
            cardsArray1.Visible = false;
            miniItemView1.Visible = false;
            vRegion.Visible = false;
        }

        private void StartGame() //初始化游戏
        {
            BattleManager.Instance.PlayerManager.RightPlayer.AIContext.OnInit();
            BattleManager.Instance.OnMatchStart();

            cardsArray1.Visible = true;
            miniItemView1.Visible = true;
            vRegion.Visible = true;
            IsGamePaused = false;

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

            try
            {
                panelBattle.Invalidate();

                if (BattleManager.Instance.PlayerManager.LeftPlayer == null || BattleManager.Instance.PlayerManager.RightPlayer == null)
                    return;

                if (!BattleManager.Instance.PlayerManager.LeftPlayer.IsAlive || !BattleManager.Instance.PlayerManager.RightPlayer.IsAlive)
                {
                    EndGame();
                    return;
                }

                if (!IsGamePaused)
                {
                    CheckCursor();
                    miniItemView1.OnFrame();
                    timeViewer1.OnFrame();

                    BattleManager.Instance.Next();
                }

                cardFlow1.OnFrame(IsGamePaused);
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
                        target.LiveMonsterToolTip.DrawCardToolTips(g);
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
                NarlonLib.Log.NLog.Error("panelBattle_Paint " + err);
            }

            e.Graphics.DrawImageUnscaled(bmp, 0, 0);
            g.Dispose();
            bmp.Dispose();

            if (fpsList.Count >= 10)//fps显示
            {
                int fps = (int)Math.Round((fpsList.Count - 1) * 1000 / fpsList[fpsList.Count - 1].Subtract(fpsList[0]).TotalMilliseconds);
                Font fontFps = new Font("黑体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                e.Graphics.DrawString(string.Format("fps={0}", fps), fontFps, Brushes.White, 3, 3);
                if (BattleManager.Instance.PlayerManager.RightPlayer.AIContext != null)
                    e.Graphics.DrawString(string.Format("ai={0}", BattleManager.Instance.PlayerManager.RightPlayer.AIContext.GetState()), fontFps, Brushes.White, 60, 3);
                fontFps.Dispose();
            }
        }

        private void panelBattle_MouseClick(object sender, MouseEventArgs e)
        {
            if (BattleManager.Instance.PlayerManager.LeftPlayer == null)
                return;

            if (isGamePaused)
                return;

            var pickPlayer = BattleManager.Instance.PlayerManager.LeftPlayer;
            int cardSize = BattleManager.Instance.MemMap.CardSize;
            if (e.Button == MouseButtons.Left)
            {
                if (leftSelectCard != null && (myCursor.Name == "summon" || myCursor.Name == "equip" || myCursor.Name == "cast" || myCursor.Name == "sidekick"))
                {
                    int result;
                    if ((result = pickPlayer.CheckUseCard(leftSelectCard, BattleManager.Instance.PlayerManager.LeftPlayer, BattleManager.Instance.PlayerManager.RightPlayer)) != ErrorConfig.Indexer.OK)
                    {
                        BattleManager.Instance.FlowWordQueue.Add(new FlowErrInfo(result, new Point(mouseX, mouseY), 0, 0));
                        return;
                    }

                    var cardData = CardConfigManager.GetCardConfig(leftSelectCard.CardId);
                    int race = 0;
                    LiveMonster lm = BattleLocationManager.GetPlaceMonster(mouseX, mouseY);
                    if (myCursor.Name == "summon" && lm == null)
                    {
                        race = (int)cardData.TypeSub;
                        var pos = new Point(mouseX/cardSize*cardSize, mouseY/cardSize*cardSize);
                        pickPlayer.UseMonster(leftSelectCard, pos);
                    }
                    else if (myCursor.Name == "equip" && lm != null)
                    {
                        pickPlayer.UseWeapon(lm, leftSelectCard);
                    }
                    else if (myCursor.Name == "sidekick" && lm != null)
                    {
                        race = (int)cardData.TypeSub;
                        pickPlayer.UseSideKick(lm, leftSelectCard);
                    }
                    else if (myCursor.Name == "cast")
                    {
                        var spellConfig = ConfigData.GetSpellConfig(leftSelectCard.CardId);
                        if (!BattleLocationManager.IsPlaceCanCast(e.Location.X, e.Location.Y, spellConfig.Target))
                            return;//做一次保证性检测
                        if (!pickPlayer.CanSpell(lm, leftSelectCard))
                            return;
                        pickPlayer.DoSpell(lm, leftSelectCard, e.Location);
                    }

                    UserProfile.Profile.OnUseCard(cardData.Type, cardData.Star, race, cardData.Attr);

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
                return;
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
                    var canRush = MonsterBook.HasTag(leftSelectCard.CardId, "rush");
                    if (BattleLocationManager.IsPlaceCanSummon(mouseX, mouseY, true, canRush))
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
                                cursorname = "sidekick";
                        }
                    }
                }
                else if (leftSelectCard.CardType == CardTypes.Weapon)
                {
                    LiveMonster lm = BattleLocationManager.GetPlaceMonster(mouseX, mouseY);
                    if (lm != null && lm.CanAddWeapon() && lm.IsLeft)
                        cursorname = "equip";
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

        private void cardsArray1_SelectionChange(object sender, EventArgs e)
        {
            leftSelectCard = cardsArray1.GetSelectCard();
            if (leftSelectCard != null && leftSelectCard.CardType == CardTypes.Monster)
                visualRegion.Update(leftSelectCard.CardId);
            else
                visualRegion.Update(0);
            OnSelectCardChange();
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
                            magicRegion.Add(type, skillConfig.Range, MagicRegion.GetTargetColor(skillConfig.Target[1]));
                    }
                }
                else if (leftSelectCard.CardType == CardTypes.Spell)
                {
                    var spellConfig = ConfigData.GetSpellConfig(leftSelectCard.CardId);
                    var type = BattleTargetManager.GetRegionType(spellConfig.Target[2]);
                    if (type != RegionTypes.None)
                        magicRegion.Add(type, spellConfig.Range, MagicRegion.GetTargetColor(spellConfig.Target[1]));
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
            miniItemView1.Enabled = true;
        }

        private bool CloseCheck()
        {
            if (!isHuman)
                return false;

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
                OnGameOver();
        }

        private void OnVRegionClick(int id, int x, int y, MouseButtons button)
        {
            if (id > 0)
            {
                var key = vRegion.GetRegion(id).GetKeyValue();
                HeroPowerConfig heroSkillConfig = ConfigData.GetHeroPowerConfig(key);
                LevelExpConfig levelConfig = ConfigData.GetLevelExpConfig(UserProfile.Profile.InfoBasic.Level);
                leftSelectCard = new ActiveCard(heroSkillConfig.CardId, (byte)levelConfig.TowerLevel);
                leftSelectCard.Mp = ConfigData.GetSpellConfig(heroSkillConfig.CardId).Cost;
                OnSelectCardChange();
            }
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            if (id > 0)
            {
                Image image = HeroPowerBook.GetPreview(key);
                tooltip.Show(image, this, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void LeftPlayerHeroSkillChanged(bool active)
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

        private void LeftPlayerShowCardSelector(Player p, ICardSelectMethod method)
        {
            cardSelector1.Init(p, method);
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
                PanelManager.DealPanel(new BattleResultForm());
            }
            if (BattleManager.Instance.StatisticData.PlayerWin)
            {
                if (BattleWin != null)
                    BattleWin();
            }
            else
            {
                if (BattleLose != null)
                    BattleLose();
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

        public override void OnRemove()
        {
            base.OnRemove();

            miniItemView1.DisposeItem();
        }
    }
}