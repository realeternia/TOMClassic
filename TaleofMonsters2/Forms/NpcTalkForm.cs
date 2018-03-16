using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using NarlonLib.Control;
using NarlonLib.Drawing;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas;
using TaleofMonsters.Datas.Others;
using TaleofMonsters.Datas.Quests;
using TaleofMonsters.Datas.Scenes;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.CMain.Blesses;
using TaleofMonsters.Forms.CMain.Quests;
using TaleofMonsters.Forms.CMain.Quests.SceneQuests;
using TaleofMonsters.Forms.CMain.Scenes;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcTalkForm : BasePanel
    {
        private int tar = -1;
        private bool showImage;
        private SceneQuestBlock interactBlock;

        private ColorWordRegion colorWord;//问题区域
        private VirtualRegion vRegion;
        private SceneQuestConfig config;
        private List<SceneQuestBlock> answerList; //回答区
        private TalkEventItem evtItem; //事件交互区
        private ImageToolTip tooltip = SystemToolTip.Instance;

        public int EventId { get; set; }
        public int CellId { get; set; } //格子id
        private int eventLevel;

        private Dictionary<int, string> dnaChangeDict = new Dictionary<int, string>();

        public NpcTalkForm()
        {
            InitializeComponent();
            NeedBlackForm = true;
            colorWord = new ColorWordRegion(160, 38, Width-170, new Font("宋体", 14 * 1.33f, GraphicsUnit.Pixel), Color.White);
            vRegion = new VirtualRegion(this);
            vRegion.RegionEntered += VRegion_RegionEntered;
            vRegion.RegionLeft += VRegion_RegionLeft;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            showImage = true;
            config = ConfigData.GetSceneQuestConfig(EventId);
            if (config.Level > 0)
                eventLevel = config.Level;
            else
                eventLevel = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId).Level;

            int regionIndex = 1;
            if (config.TriggerDNAHard != null && config.TriggerDNAHard.Length > 0)
            {
                for (int i = 0; i < config.TriggerDNAHard.Length; i ++)
                {
                    var dnaId = DnaBook.GetDnaId(config.TriggerDNAHard[i]);
                    if (UserProfile.InfoBasic.HasDna(dnaId))
                    {
                        vRegion.AddRegion(new ImageRegion(dnaId, 28*regionIndex,55, 24,24, ImageRegionCellType.None, DnaBook.GetDnaImage(dnaId)));
                        dnaChangeDict[dnaId] = "事件难度 " + config.TriggerDNAHard[i].Substring(3);
                        //dnaChangeDict[dnaId] += "$经验资源 " + GetDnaStr(-int.Parse(config.TriggerDNAHard[i + 1]));
                        regionIndex++;
                    }
                }
            }
            if (config.TriggerDNARate != null && config.TriggerDNARate.Length > 0)
            {
                for (int i = 0; i < config.TriggerDNARate.Length; i ++)
                {
                    var dnaId = DnaBook.GetDnaId(config.TriggerDNARate[i]);
                    if (UserProfile.InfoBasic.HasDna(dnaId))
                    {
                        var dataStr = "出现几率 " + config.TriggerDNARate[i].Substring(3);
                        if (dnaChangeDict.ContainsKey(dnaId))
                        {
                            dnaChangeDict[dnaId] += "$" + dataStr;
                        }
                        else
                        {
                            vRegion.AddRegion(new ImageRegion(dnaId, 28 * regionIndex, 55, 24, 24, ImageRegionCellType.None, DnaBook.GetDnaImage(dnaId)));
                            dnaChangeDict[dnaId] = dataStr;
                        }
                        regionIndex++;
                    }
                }
            }
            interactBlock = SceneQuestBook.GetQuestData(EventId, eventLevel, config.Script);
            answerList = new List<SceneQuestBlock>();
            SetupQuestItem();
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            if (evtItem != null && evtItem.RunningState == TalkEventItem.TalkEventState.Running)
            {
                evtItem.OnFrame(tick);
                if (evtItem.RunningState == TalkEventItem.TalkEventState.Finish)
                {
                    interactBlock = evtItem.GetResult();
                    if (interactBlock is SceneQuestSay)
                        SetupQuestItem();
                    else if (interactBlock == null)
                    {
                        answerList.Clear();
                        var block = new SceneQuestBlock(EventId, eventLevel, "结束", 999, 999);
                        answerList.Add(block);
                    }
                }
                Invalidate();
            }
        }

        private void NpcTalkForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (tar != -1)
            {
                if (interactBlock == null)//一般是最后一选了
                {
                    Close();
                    Scene.Instance.OnEventEnd(CellId, config.Id, evtItem != null ? evtItem.Type : "");
                    Scene.Instance.CheckALiveAndQuestState();
                    return;
                }
                
                if (evtItem != null && evtItem.RunningState == TalkEventItem.TalkEventState.Running)
                {
                    //事件过程中无视点击
                    return;
                }

                interactBlock = answerList[tar]; //对话换页

                if (evtItem != null)
                {
                    evtItem.Dispose();
                    evtItem = null;
                }
           
                if (interactBlock.Children.Count == 1 && interactBlock.Children[0] is SceneQuestSay)
                {
                    interactBlock = interactBlock.Children[0];
                    SetupQuestItem();
                }
                else if (interactBlock.Children.Count == 1 && interactBlock.Children[0] is SceneQuestEvent)
                {
                    var evt = interactBlock.Children[0] as SceneQuestEvent;
                    if (evt.Type == "npc")
                        tar = -1; //为了修一个显示bug
                    var region = new Rectangle(10, Height - 10 - 5*20 - 160, Width - 20, 160);
                    evtItem = TalkEventItem.CreateEventItem(CellId, EventId, eventLevel, this, region, evt);
                    evtItem.Init();
                }

                if (evtItem != null && evtItem.AutoClose())
                {
                    Close();
                    Scene.Instance.OnEventEnd(CellId, config.Id, evtItem != null ? evtItem.Type : "");
                    Scene.Instance.CheckALiveAndQuestState();
                }
                this.Invalidate();
            }
        }

        private void SetupQuestItem()
        {
            colorWord.UpdateText(interactBlock.Script);
            answerList.Clear();
            foreach (var sceneQuestBlock in interactBlock.Children)
            {
                if (sceneQuestBlock.Disabled)
                    continue;
                answerList.Add(sceneQuestBlock);

                if (sceneQuestBlock.Children != null && sceneQuestBlock.Children[0].Script.StartsWith("fight"))
                {
                    sceneQuestBlock.Prefix = "fight";
                    if (config.CanBribe)//判断战斗贿赂
                    {
                        int fightLevel = Math.Max(1, eventLevel + BlessManager.FightLevelChange);
                        var cost = GameResourceBook.OutCarbuncleBribe(UserProfile.InfoBasic.Level, fightLevel);
                        if (UserProfile.InfoBag.HasResource(GameResourceType.Carbuncle, cost))
                        {
                            var questBlock = SceneQuestBook.GetQuestData(EventId, eventLevel, "blockbribe");
                            questBlock.Prefix = "bribe";
                            questBlock.Children[0].Children[0].Children[0] = sceneQuestBlock.Children[0].Children[1].Children[0].Children[0];//找到成功的结果
                            answerList.Add(questBlock);
                        }
                    }
                }

            }

            if (interactBlock!=null && interactBlock.Depth==0)
            {//额外的任务目标
                foreach (var questConfig in ConfigData.QuestDict.Values)
                {
                    if (questConfig.StartNpcId == EventId)
                    {
                        if (UserProfile.InfoQuest.IsQuestCanReceive(questConfig.Id))
                        {
                            var questBlock = SceneQuestBook.GetQuestData(EventId, eventLevel, "blockquest");
                            questBlock.Script = string.Format("【{0}】{1}", questConfig.TypeR == 0 ? "主线" : "支线", questConfig.Name);
                            questBlock.Prefix = "quest";
                            questBlock.Children[0].Script = questConfig.Descript + "$$报酬:" + QuestBook.GetRewardStr(questConfig.Id);
                            (questBlock.Children[0].Children[0].Children[0] as SceneQuestEvent).ParamList[0] = questConfig.Id.ToString();
                            answerList.Add(questBlock);
                        }
                    }
                    if (questConfig.EndNpcId == EventId || questConfig.EndNpcId == 0 && questConfig.StartNpcId == EventId) //如果end=0，则使用startnpc作为奖励npc
                    {
                        if (UserProfile.InfoQuest.IsQuestCanReward(questConfig.Id))
                        {
                            var questBlock = SceneQuestBook.GetQuestData(EventId, eventLevel, "blockquestfin");
                            questBlock.Script = questConfig.Name + "(提交)";
                            questBlock.Prefix = "questfin";
                            (questBlock.Children[0] as SceneQuestEvent).ParamList[0] = questConfig.Id.ToString();
                            answerList.Add(questBlock);
                        }
                    }
                    if (questConfig.CheckSceneQuest == config.Ename && UserProfile.InfoQuest.IsQuestCanProgress(questConfig.Id))
                    {//增加一个选项的任务
                        var questBlock = SceneQuestBook.GetQuestData(EventId, eventLevel, questConfig.QuestScript);
                        questBlock.Prefix = "addon";
                        ModifyQuestState(questBlock, questConfig);
                        answerList.Add(questBlock);
                    }
                }
            }
        }

        private void ModifyQuestState(SceneQuestBlock sb, QuestConfig questConfig)
        {
            var event1 = sb as SceneQuestEvent; 
            if (event1 != null && event1.Type == "questp")
            {
                event1.ParamList[0] = questConfig.Id.ToString();
                event1.ParamList[1] = questConfig.ProgressAdd.ToString();
            }
            foreach (var child in sb.Children)
                ModifyQuestState(child, questConfig);
        }

        private void NpcTalkForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (evtItem == null || evtItem.RunningState != TalkEventItem.TalkEventState.Running) //evtItem运行期间无法选择
            {
                if (e.Y > Height - 10 - answerList.Count * 20 && e.Y < Height - 10)
                {
                    int val = (e.Y - (Height - 10) + answerList.Count * 20) / 20;
                    if (val != tar)
                    {
                        tar = val;
                        Invalidate();
                        return;
                    }
                }
                else
                {
                    tar = -1;
                }
            }
        }

        private void VRegion_RegionEntered(int id, int x, int y, int key)
        {
            var dnaConfig = ConfigData.GetPlayerDnaConfig(id);
            Image image = DrawTool.GetImageByString(string.Format("{0}[DNA效果]",dnaConfig.Name), dnaChangeDict[id], 120, Color.White);
            tooltip.Show(image, this, x, y);
        }

        private void VRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private void NpcTalkForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            if (showImage)
            {
                Font font2 = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString(string.Format("{0}(Lv{1})",config.Name, eventLevel), font2, Brushes.White, Width / 2 - 40, 8);
                font2.Dispose();

                e.Graphics.DrawImage(SceneQuestBook.GetSceneQuestImage(config.Id), 15, 40, 140, 140);
                Image border = PicLoader.Read("Border", "questb1.PNG"); //边框
                e.Graphics.DrawImage(border, 15, 40, 140, 140);
                border.Dispose();

                if (evtItem != null)
                    evtItem.Draw(e.Graphics);

                colorWord.Draw(e.Graphics);
                if (vRegion != null)
                    vRegion.Draw(e.Graphics);

                if (answerList != null && (evtItem == null || evtItem.RunningState != TalkEventItem.TalkEventState.Running))
                {
                    int id = 0;
                    foreach (var word in answerList)
                    {
                        word.Draw(e.Graphics, id * 20 + Height - 10 - answerList.Count * 20, Width, id == tar);
                        id++;
                    }
                }
            }
        }

    }
}