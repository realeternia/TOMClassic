using TaleofMonsters.Core;
using System.IO;

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

        public static InfoSkill InfoSkill
        {
            get { return Profile.InfoSkill; }
        }

        public static InfoTask InfoTask
        {
            get { return Profile.InfoTask; }
        }

        public static InfoRival InfoRival
        {
            get { return Profile.InfoRival; }
        }

        public static InfoRecord InfoRecord
        {
            get { return Profile.InfoRecord; }
        }

        public static InfoWorld InfoWorld
        {
            get { return Profile.InfoWorld; }
        }

        public static bool LoadFromDB(string passport)
        {
            if (File.Exists(string.Format("./Save/{0}.db", passport)))
            {
                Profile = new Profile();
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
            FileStream fs = new FileStream(string.Format("./Save/{0}.db", ProfileName), FileMode.OpenOrCreate);
            var dts= DbSerializer.CustomTypeToBytes(Profile, typeof(Profile));
            fs.Write(dts, 0, dts.Length);
            fs.Close();
        }
    }
}
