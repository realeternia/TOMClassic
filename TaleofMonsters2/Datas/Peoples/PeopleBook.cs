using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Log;
using NarlonLib.Math;
using NarlonLib.Tools;
using TaleofMonsters.Controler.Battle;
using TaleofMonsters.Core;
using TaleofMonsters.Core.Loader;
using TaleofMonsters.Datas.Decks;
using TaleofMonsters.Datas.User;
using TaleofMonsters.Forms.CMain;
using TaleofMonsters.Forms.Pops;
using TaleofMonsters.Tools;

namespace TaleofMonsters.Datas.Peoples
{
    internal static class PeopleBook
    {
        private static Dictionary<string, int> itemNameIdDict;
        public static int GetPeopleId(string ename)
        {
            if (itemNameIdDict == null)
            {
                itemNameIdDict = new Dictionary<string, int>();
                foreach (var peopleConfig in ConfigData.PeopleDict.Values)
                {
                    if (itemNameIdDict.ContainsKey(peopleConfig.Ename))
                    {
                        NLog.Warn("GetPeopleId key={0} exsited", peopleConfig.Ename);
                        continue;
                    }
                    itemNameIdDict[peopleConfig.Ename] = peopleConfig.Id;
                }
            }
            return itemNameIdDict[ename];
        }

        public static int GetRandomPeopleId(int levelMin, int levelMax)
        {
            List<int> ids = new List<int>();
            foreach (var peopleData in ConfigData.PeopleDict.Values)
            {
                if (peopleData.InRandomQuest && peopleData.Level>=levelMin && peopleData.Level<=levelMax)
                    ids.Add(peopleData.Id);
            }
            return ids[MathTool.GetRandom(ids.Count)];
        }

        public static List<int> GetPeopleChessesOnScene(int sceneId)
        {
            List<int> chessList = new List<int>();
            foreach (var chessConfig in ConfigData.PeopleChessDict.Values)
            {
                if (chessConfig.BornSceneId == null || chessConfig.BornSceneId.Length == 0)
                    continue;

                if (Array.IndexOf(chessConfig.BornSceneId, sceneId) >= 0)
                    chessList.Add(chessConfig.Id);
            }
            return chessList;
        }

        public static Image GetPreview(int id)
        {
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage(PaintTool.GetTalkColor);
            tipData.AddTextNewLine(peopleConfig.Name, "White", 20);
            tipData.AddTextNewLine(string.Format("{0}级{1}", peopleConfig.Level, ConfigData.GetJobConfig(peopleConfig.Job).Name), "White");
            tipData.AddLine();
            //int[] attrs = JobBook.GetJobLevelAttr(peopleConfig.Job, peopleConfig.Level);
            //tipData.AddTextNewLine(string.Format("战斗 {0,3:D}  守护 {1,3:D}", attrs[0], attrs[1]), "Lime");
            //tipData.AddTextNewLine(string.Format("法术 {0,3:D}  技巧 {1,3:D}", attrs[2], attrs[3]), "Lime");
            //tipData.AddTextNewLine(string.Format("速度 {0,3:D}  幸运 {1,3:D}", attrs[4], attrs[5]), "Lime");
            //tipData.AddTextNewLine(string.Format("体质 {0,3:D}  生存 {1,3:D}", attrs[6], attrs[7]), "Lime");
            //tipData.AddLine();
            //tipData.AddTextNewLine("王牌", "White", 30);
            //foreach (int cid in peopleConfig.Cards)
            //{
            //    tipData.AddImage(MonsterBook.GetMonsterImage(cid, 40, 40), 28);
            //}
            return tipData.Image;
        }

        public static bool IsPeople(int id)
       {
           PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
           return peopleConfig.Type > 0 && peopleConfig.Type < 100; 
        }

        public static bool IsMonster(int id)
        {
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
            return peopleConfig.Type == 0;
        }

        public static Image GetPersonImage(int id)
        {
            string fname = string.Format("People/{0}.PNG", ConfigData.GetPeopleConfig(id).Figue);
            if (!ImageManager.HasImage(fname))
            {
                Image image = PicLoader.Read("People", string.Format("{0}.PNG", ConfigData.GetPeopleConfig(id).Figue));
                ImageManager.AddImage(fname, image);
            }
            return ImageManager.GetImage(fname);
        }

        public static int[] GetRandNPeople(int count, int minLevel, int maxLevel)
        {
            List<int> pids = new List<int>();
            foreach (PeopleConfig peopleConfig in ConfigData.PeopleDict.Values)
            {
                if (IsPeople(peopleConfig.Id) && peopleConfig.Level >= minLevel && peopleConfig.Level <= maxLevel)
                    pids.Add(peopleConfig.Id);
            }

            ArraysUtils.RandomShuffle(pids);
            return pids.GetRange(0, count).ToArray();
        }

        public static void Fight(int pid, string map, int rlevel, PeopleFightParm reason, 
            HsActionCallback winEvent, HsActionCallback lossEvent, HsActionCallback cancelEvent)
        {
            List<DeckCard> cards = new List<DeckCard>();
            if (UserProfile.InfoDungeon.DungeonId <= 0)//在副本中不需要选择卡组
            {
                bool rt = PopDeckChoose.Show(map);
                if (!rt)
                {
                    if (cancelEvent != null)
                        cancelEvent();
                    return;
                }
                for (int i = 0; i < GameConstants.DeckCardCount; i++)
                {
                    int id = UserProfile.InfoCard.SelectedDeck.GetCardAt(i);
                    cards.Add(new DeckCard(UserProfile.InfoCard.GetDeckCardById(id)));
                }
            }
            else
            {
                foreach (var cardData in UserProfile.InfoCard.DungeonDeck)
                    cards.Add(new DeckCard(cardData.BaseId, cardData.Level, cardData.Exp));
            }

            BattleForm bf = new BattleForm();
            bf.BattleWin = winEvent;
            bf.BattleLose = lossEvent;
            bf.Init(0, cards.ToArray(), pid, map, rlevel, reason);
            PanelManager.DealPanel(bf);
        }
    }
}
