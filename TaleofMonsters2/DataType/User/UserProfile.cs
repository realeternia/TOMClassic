using TaleofMonsters.Core;
using System.IO;
using TaleofMonsters.Controler.World;

namespace TaleofMonsters.DataType.User
{
    internal static class UserProfile
    {
        public static string ProfileName { get; set; }
        public static Profile Profile { get; set; }

        public static InfoBasic InfoBasic
        {
            get { return Profile.InfoBasic; }
        }

        public static InfoBag InfoBag
        {
            get { return Profile.InfoBag; }
        }

        public static InfoEquip InfoEquip
        {
            get { return Profile.InfoEquip; }
        }

        public static InfoCard InfoCard
        {
            get { return Profile.InfoCard; }
        }

        public static InfoDungeon InfoDungeon
        {
            get { return Profile.InfoDungeon; }
        }

        public static InfoRival InfoRival
        {
            get { return Profile.InfoRival; }
        }

        public static InfoRecord InfoRecord
        {
            get { return Profile.InfoRecord; }
        }

        public static InfoQuest InfoQuest
        {
            get { return Profile.InfoQuest; }
        }

        public static InfoWorld InfoWorld
        {
            get { return Profile.InfoWorld; }
        }

        public static bool LoadFromDB(string passport)
        {
            if (File.Exists(string.Format("./Save/{0}.db", passport)))
            {
                FileStream fs = new FileStream(string.Format("./Save/{0}.db", passport), FileMode.Open);
                byte[] dts = new byte[fs.Length];
                fs.Read(dts, 0, dts.Length);
                object tmp;
                DbSerializer.BytesToCustomType(dts, out  tmp, typeof(Profile));
                Profile = (Profile)tmp;
                fs.Close();
                return true;
            }
            return false;
        }

        public static void SaveToDB()
        {
            WorldInfoManager.Save();

            FileStream fs = new FileStream(string.Format("./Save/{0}.db", ProfileName), FileMode.OpenOrCreate);
            var dts= DbSerializer.CustomTypeToBytes(Profile, typeof(Profile));
            fs.Write(dts, 0, dts.Length);
            fs.Close();
        }
    }
}
