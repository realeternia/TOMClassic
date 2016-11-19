using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Controler.Resource;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Pops;
using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.Peoples
{
    internal static class PeopleBook
    {
        public static int GetRandPersonIdByLevel(int dlevel)
        {
            RandomMaker randomMaker = new RandomMaker();
            foreach (PeopleConfig peopleConfig in ConfigData.PeopleDict.Values)
            {
                if (IsMonster(peopleConfig.Id))
                    continue;

                if (peopleConfig.Level >= dlevel - 1 && peopleConfig.Level <= dlevel + 1)
                {
                    randomMaker.Add(peopleConfig.Id, 1);
                }
            }

            return randomMaker.Process(1)[0];
        }

        public static Image GetPreview(int id)
        {
            PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);

            ControlPlus.TipImage tipData = new ControlPlus.TipImage();
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

        static public bool IsPeople(int id)
       {
           PeopleConfig peopleConfig = ConfigData.GetPeopleConfig(id);
           return peopleConfig.Type > 0 && peopleConfig.Type < 100; 
        }

        static public bool IsMonster(int id)
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
                {
                    pids.Add(peopleConfig.Id);
                }
            }
            return RandomShuffle.Process(pids.ToArray()).GetRange(0, count).ToArray();
        }

        public static void Fight(int pid, string map, int tile, int rlevel, HsActionCallback winEvent, HsActionCallback lossEvent)
        {
            if (UserProfile.InfoBasic.Ap < GameConstants.FightAPCost)
            {
                MainForm.Instance.AddTip(HSErrorTypes.GetDescript(HSErrorTypes.SceneAPNotEnough), "Red");
                return;
            }

            bool rt = PopDeckChoose.Show(map, tile, UserProfile.InfoCard.GetDeckNames());
            if (!rt)
            {
                return;
            }

            UserProfile.InfoBasic.Ap -= GameConstants.FightAPCost;
            BattleForm bf = new BattleForm();
            bf.BattleWin = winEvent;
            bf.BattleLose = lossEvent;
            bf.Init(0, pid, map, tile, rlevel);
            MainForm.Instance.DealPanel(bf);
        }

        public static void ViewMatch(int left, int right, string map, int tile)
        {//todo
            //BattleForm bf = new BattleForm();
            //bf.Init(left, right, map, tile);
            //MainForm.Instance.DealPanel(new BlackWallForm());
            //bf.ShowDialog();
            //MainForm.Instance.DealPanel(new BlackWallForm());
            //MainForm.Instance.DealPanel(new BattleResultForm());
        }
    }
}
