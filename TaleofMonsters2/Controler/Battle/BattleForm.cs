using System;
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
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.HeroSkills;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.Items.Regions;

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
        private int roundMark;//目前一个roundmark代表0.05s
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
        private int defaultTile;

        private long lastMouseMoveTime;
        private int hitRound;
        private int hitMonsterId;

        private int itemCount;
        private VirtualRegion vRegion;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private MagicRegion magicRegion = new MagicRegion();

        public bool IsWin
        {
            get { return BattleManager.Instance.BattleInfo.PlayerWin; }
        }

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

        public void Init(int lid, int rid, string map, int tile, int rlevel)
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
            defaultTile = tile;
            timeViewer1.Init();
            cardsArray1.Init();
            cardList2.Init();
            miniItemView1.Init();
            miniItemView1.Enabled =false;
            cardsArray1.SetEnable(false);

            BattleManager.Instance.Init();
            BattleManager.Instance.PlayerManager.Init(leftId, rightId, rightLevel);
            int index = 0;//初始化英雄技能按钮
            foreach (var skillId in BattleManager.Instance.PlayerManager.LeftPlayer.HeroSkillList)
            {
                var region = new PictureAnimRegion(index+1, 25, 518+index*45, 40, 40, skillId, VirtualRegionCellType.HeroSkill, skillId);
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
            cardsArray1.Visible = false;
        }

        private void StartGame()//初始化游戏
        {
            BattleManager.Instance.MemMap = new MemRowColumnMap(mapName, defaultTile);
            BattleManager.Instance.MonsterQueue.AddInitialAction();
            BattleManager.Instance.PlayerManager.LeftPlayer.AddHeroUnit();
            BattleManager.Instance.PlayerManager.RightPlayer.AddHeroUnit();
            AIStrategy.OnInit(BattleManager.Instance.PlayerManager.RightPlayer);

            roundMark = 0;
            BattleManager.Instance.BattleInfo.StartTime = DateTime.Now;
            BattleManager.Instance.BattleInfo.EndTime = DateTime.Now;

            cardsArray1.Visible = true;
            IsGamePaused = false;
            showGround = true;
        }

        private void StopGame()
        {
            isGamePaused = true;
        }

        internal override void OnFrame(int tick)
        {
            base.OnFrame(tick);

            if (onTurn)
                return;
            onTurn = true;
            NLCoroutine.DoTimer();
            try
            {
                panelBattle.Invalidate();
                if (BattleManager.Instance.BattleInfo.Items != null && itemCount != BattleManager.Instance.BattleInfo.Items.Count) //获得新物品
                {
                    itemCount = BattleManager.Instance.BattleInfo.Items.Count;
                }

                if (BattleManager.Instance.PlayerManager.LeftPlayer == null || BattleManager.Instance.PlayerManager.RightPlayer == null)
                    return;

                BattleManager.Instance.FlowWordQueue.Next();
                BattleManager.Instance.EffectQueue.Next();
                if (IsGamePaused)
                    return;

                CheckCursor();
                miniItemView1.NewTick();
                roundMark++;
                if (!BattleManager.Instance.PlayerManager.LeftPlayer.Hero.IsAlive || !BattleManager.Instance.PlayerManager.RightPlayer.Hero.IsAlive)
                {
                    EndGame();
                    return;
                }

                if (hitRound >= 0) //战斗动画播放中
                {
                    hitRound--;
                }
                else
                {
                    if (roundMark % 4 == 0)
                    {
                        hitMonsterId =BattleManager.Instance.MonsterQueue.NextAction(); //1回合
                        if (hitMonsterId > 0)
                        {
                            hitRound = SysConstants.BattleAttackRoundWait;
                        }
                    }
                }
                
                if (roundMark%4 == 0) //200ms
                {
                    float pastTime = (float) 200/SysConstants.RoundTime;
                    BattleManager.Instance.PlayerManager.Update(false, pastTime, BattleManager.Instance.BattleInfo.Round);
                    if (timeViewer1.TimeGo(pastTime))
                        BattleManager.Instance.PlayerManager.CheckRoundCard(); //1回合
                }
                BattleManager.Instance.BattleInfo.Round = roundMark * 50 / SysConstants.RoundTime + 1;//50ms
                if (roundMark%10 == 0)
                {
                    AIStrategy.AIProc(BattleManager.Instance.PlayerManager.RightPlayer, isGamePaused);
                }
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
                BattleManager.Instance.BattleInfo.PlayerWin = !BattleManager.Instance.PlayerManager.RightPlayer.Hero.IsAlive;
                BattleManager.Instance.BattleInfo.EndTime = DateTime.Now;
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
                    BattleManager.Instance.MemMap.Draw(g);

                    if (magicRegion.Type != RegionTypes.None && isMouseIn)
                        magicRegion.Draw(g, roundMark, mouseX, mouseY);
                    for (int i = 0; i <BattleManager.Instance.MonsterQueue.Count; i++)
                    {
                        LiveMonster monster =BattleManager.Instance.MonsterQueue[i];
                        if (monster.Id == hitMonsterId)
                        {
                            continue;
                        }
                        Color color = Color.White;
                        if (isMouseIn)
                            color = magicRegion.GetColor(monster, mouseX, mouseY);
                        monster.Draw(g, color);
                    }
                    if (hitMonsterId > 0)
                    {
                        LiveMonster monster =BattleManager.Instance.MonsterQueue.GetMonsterByUniqueId(hitMonsterId);
                        Color color = Color.White;
                        if (isMouseIn)
                            color = magicRegion.GetColor(monster, mouseX, mouseY);
                        monster.Draw(g, color);
                    }
                    for (int i = 0; i < BattleManager.Instance.EffectQueue.Count; i++)
                        BattleManager.Instance.EffectQueue[i].Draw(g);
                    for (int i = 0; i < BattleManager.Instance.FlowWordQueue.Count; i++)
                        BattleManager.Instance.FlowWordQueue[i].Draw(g);

                    LiveMonster target = BattleLocationManager.GetPlaceMonster(mouseX, mouseY);
                    if (target != null && isMouseIn && magicRegion.Type == RegionTypes.None)
                    {
                        target.DrawCardToolTips(g);
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
        }

        private void panelBattle_MouseClick(object sender, MouseEventArgs e)
        {
            if (BattleManager.Instance.PlayerManager.LeftPlayer == null)
                return;

            if (isGamePaused)
                return;

            if (e.Button == MouseButtons.Left)
            {
                if (leftSelectCard != null && (myCursor.Name == "summon" || myCursor.Name == "equip" || myCursor.Name == "cast"))
                {
                    int result;
                    if ((result = BattleManager.Instance.PlayerManager.LeftPlayer.CheckUseCard(leftSelectCard, BattleManager.Instance.PlayerManager.LeftPlayer, BattleManager.Instance.PlayerManager.RightPlayer)) != HSErrorTypes.OK)
                    {
                        BattleManager.Instance.FlowWordQueue.Add(new FlowErrInfo(result, new Point(mouseX, mouseY), 0, 0), false);
                        return;
                    }

                    //可能有触发/状态等
                    BattleManager.Instance.PlayerManager.LeftPlayer.OnUseCard(leftSelectCard);

                    if (BattleManager.Instance.PlayerManager.RightPlayer.CheckTrapOnUseCard(leftSelectCard, BattleManager.Instance.PlayerManager.RightPlayer, BattleManager.Instance.PlayerManager.LeftPlayer))
                    {
                        return;
                    }

                    LiveMonster lm = BattleLocationManager.GetPlaceMonster(mouseX, mouseY);
                    if (myCursor.Name == "summon" && lm == null)
                    {
                        var mon = new Monster(leftSelectCard.CardId);
                        mon.UpgradeToLevel(leftSelectCard.Level);
                        BattleManager.Instance.PlayerManager.LeftPlayer.OnSummon(mon);
                        
                        LiveMonster newMon = new LiveMonster(leftSelectCard.Id, leftSelectCard.Level, mon, new Point(mouseX / 100 * 100, mouseY / 100 * 100), true);
                       BattleManager.Instance.MonsterQueue.Add(newMon);

                        BattleManager.Instance.PlayerManager.RightPlayer.CheckTrapOnSummon(newMon, BattleManager.Instance.PlayerManager.RightPlayer, BattleManager.Instance.PlayerManager.LeftPlayer);
                    }
                    else if (myCursor.Name == "equip" && lm != null)
                    {
                        Weapon wpn = new Weapon(leftSelectCard.CardId);
                        wpn.UpgradeToLevel(leftSelectCard.Level);
                        BattleManager.Instance.PlayerManager.LeftPlayer.OnUseWeapon(wpn);
                        
                        var tWeapon = new TrueWeapon(lm, leftSelectCard.Level, wpn);
                        lm.AddWeapon(tWeapon);
                    }
                    else if (myCursor.Name == "cast")
                    {
                        Spell spell = new Spell(leftSelectCard.CardId);
                        spell.Addon = BattleManager.Instance.PlayerManager.LeftPlayer.SpellEffectAddon;
                        spell.UpgradeToLevel(leftSelectCard.Level);
                        BattleManager.Instance.PlayerManager.LeftPlayer.OnDoSpell(spell);
                        
                        SpellAssistant.CheckSpellEffect(spell, true, lm, e.Location);
                    }

                    var cardData = CardConfigManager.GetCardConfig(leftSelectCard.CardId);
                    UserProfile.Profile.OnUseCard(cardData.Star, 0, cardData.TypeSub);

                    BattleManager.Instance.PlayerManager.LeftPlayer.CardManager.DeleteCardAt(BattleManager.Instance.PlayerManager.LeftPlayer.SelectId);
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
            magicRegion.Type = RegionTypes.None;
            mouseX = -1;
            isMouseIn = false;
        }

        private void CheckCursor()
        {
            string cursorname = "default";
            magicRegion.Type = RegionTypes.None;
            LiveMonster lm = BattleLocationManager.GetPlaceMonster(mouseX, mouseY);

            if (leftSelectCard != null)
            {
                if (leftSelectCard.CardType == CardTypes.Monster)
                {
                    if (lm == null && mouseX>100&&mouseX<400)
                    {
                        cursorname = "summon";
                    }
                }
                else if (leftSelectCard.CardType == CardTypes.Weapon)
                {
                    if (lm != null && !lm.IsGhost && lm.IsLeft)
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
                            cursorname ="cast";
                            magicRegion.Update(spellConfig);
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
            panelState.Invalidate();
        }

        private void panelState_Paint(object sender, PaintEventArgs e)
        {
            if (showGround)
            {
                if (leftSelectCard != null)
                {
                    var card = CardAssistant.GetCard(leftSelectCard.CardId);
                    card.SetData(leftSelectCard);
                    card.DrawOnStateBar(e.Graphics);
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

        private void OnVRegionClick(int info, MouseButtons button)
        {
            if (info > 0)
            {
                HeroSkillConfig heroSkillConfig = ConfigDatas.ConfigData.GetHeroSkillConfig(info);
                leftSelectCard = new ActiveCard(heroSkillConfig.CardId, UserProfile.Profile.InfoBasic.Level, 0);
                panelState.Invalidate();
            }
        }

        private void virtualRegion_RegionEntered(int info, int x, int y, int key)
        {
            if (info > 0)
            {
                Image image = HeroSkillBook.GetSkillPreview(info);
                tooltip.Show(image, this, x, y);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
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
            if (BattleManager.Instance.BattleInfo.PlayerWin)
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