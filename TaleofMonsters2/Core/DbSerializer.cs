using System;
using System.Reflection;
using AltSerialize;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.Forms.TourGame;

namespace TaleofMonsters.Core
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FieldIndexAttribute : Attribute
    {
        public byte Index { get; set; }
    }

    internal class DbSerializer
    {
        private class Register4AltSerializer : TypeRegisterIF
        {
            public void Register(Type t, int id)
            {
                AltSerializer.AddType(t, id);
            }

            public void RegisterEnum<T>(int id)
            {
                AltSerializer.AddEnumType<T>(id);
            }
        }

        public static void Init()
        {
            AltSerializer.Assembly = Assembly.GetAssembly(typeof(FieldIndexAttribute));
            TypeRegister.RegisterType(new Register4AltSerializer());

        }

        private static Serializer serialize = new Serializer();
        public static byte[] CustomTypeToBytes(object o, Type type)//都是自定义类型
        {
            byte[] targetData = null;
            try
            {
                targetData = serialize.Serialize(o, type);
            }
            catch (Exception e)
            {
                NarlonLib.Log.NLog.Error(e);

                throw;
            }
            return targetData;
        }

        public static void BytesToCustomType(byte[] buffer, out object o, Type type)
        {
            try
            {
                o = serialize.Deserialize(buffer);
            }
            catch (Exception e)
            {
                NarlonLib.Log.NLog.Error(e);

                throw;
            }
        }
    }

    public interface TypeRegisterIF
    {
        void Register(Type t, int id);
        void RegisterEnum<T>(int id);
    }

    public static class TypeRegister
    {
        public static void RegisterType(TypeRegisterIF register)
        {
            register.Register(typeof(Profile), 1);
            register.Register(typeof(IntPair), 3);
            register.Register(typeof(InfoBag), 4);
            register.Register(typeof(DbDeckCard), 5);
            register.Register(typeof(InfoGismo), 6);
            register.Register(typeof(InfoEquip), 7);
            register.Register(typeof(DbFarmState), 8);
            register.Register(typeof(DbEquip), 9);
            register.Register(typeof(DbCardProduct), 10);
            register.Register(typeof(InfoQuest), 11);
            register.Register(typeof(InfoDungeon), 12);
            register.Register(typeof(MatchResult), 13);
            register.Register(typeof(InfoFarm), 14);
            register.Register(typeof(DbQuestData), 15);
            register.Register(typeof(DbSceneSpecialPosData), 16);
            register.Register(typeof(DbGismoState), 17);
            register.Register(typeof(DbDeckData), 18);
            register.Register(typeof(DbMergeData), 21);
            register.Register(typeof(DbRivalState), 23);
            register.Register(typeof(InfoRival), 24);
            register.Register(typeof(InfoBasic), 25);
            register.Register(typeof(InfoWorld), 26);
            register.Register(typeof(DbTournamentData), 27);
            register.Register(typeof(InfoRecord), 28);
            register.Register(typeof(InfoCard), 29);
            register.Register(typeof(GameResource), 30);

            //在此处上面添加
            // register.RegisterEnum<CardProductMarkTypes>(23);
            //register.RegisterEnum<KittyFightMode>(24);
            //register.RegisterEnum<BindType>(25);
            //register.RegisterEnum<PlayerLifeStage>(26);
            //register.RegisterEnum<PkProtectMode>(27);
            //register.RegisterEnum<ModeState>(59);
            //register.RegisterEnum<PieceStar>(60);

        }
    }
}
