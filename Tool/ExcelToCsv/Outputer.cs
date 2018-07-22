using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ExcelToCsv
{
    class Outputer
    {
        public static void WriteConfigData(string loadStr)
        {
            StreamWriter swTotal = new StreamWriter("./ConfigData/ConfigData.cs", false, Encoding.UTF8);

            swTotal.WriteLine("using System;");
            swTotal.WriteLine("using System.Collections.Generic;");
            swTotal.WriteLine("using NarlonLib.Math;");
            swTotal.WriteLine("namespace ConfigDatas");
            swTotal.WriteLine("{");
            swTotal.WriteLine("\tpublic partial class ConfigData");
            swTotal.WriteLine("\t{");
            swTotal.WriteLine("\t\tpublic static void LoadData()");
            swTotal.WriteLine("\t\t{");
            swTotal.WriteLine(loadStr);
            swTotal.WriteLine("\t\t}");
            swTotal.WriteLine("\t}");
            swTotal.WriteLine("}");
            swTotal.Close();
        }

        public static void CopeFile(string keyname, FileInfo fileInfo)
        {
            StreamWriter swTotal = new StreamWriter(string.Format("./ConfigData/ConfigData{0}.cs", keyname), false, Encoding.UTF8);
            swTotal.WriteLine("using System;");
            swTotal.WriteLine("using System.Collections.Generic;");
            swTotal.WriteLine("using NarlonLib.Math;");
            swTotal.WriteLine("namespace ConfigDatas");
            swTotal.WriteLine("{");
            swTotal.WriteLine("\tpublic partial class ConfigData");
            swTotal.WriteLine("\t{");

            var datas = ExcelTool.GetExcelToDataTableBySheet(fileInfo.FullName);
            List<string> indexerBlock = new List<string>();

            //先写ConfigData.cs
            swTotal.WriteLine(string.Format("\t\tpublic static Dictionary<int, {0}Config> {0}Dict= new Dictionary<int, {0}Config>();", keyname));
            swTotal.WriteLine(string.Format("\t\tpublic static {0}Config None{0}= new {0}Config();", keyname));
            swTotal.WriteLine(string.Format("\t\tpublic static {0}Config Get{0}Config(int id)", keyname));
            swTotal.WriteLine("\t\t{");
            swTotal.WriteLine(string.Format("\t\t\tif ({0}Dict.ContainsKey(id))", keyname));
            swTotal.WriteLine(string.Format("\t\t\t\treturn {0}Dict[id];", keyname));
            swTotal.WriteLine("\t\t\telse");
            swTotal.WriteLine(string.Format("\t\t\t\treturn None{0};", keyname));
            swTotal.WriteLine("\t\t}");
            swTotal.WriteLine(string.Format("\t\tprivate static void Load{0}()", keyname));
            swTotal.WriteLine("\t\t{");

            foreach (var data in datas.Tables)
            {
                DataTable tb = (DataTable) data;
                var hasAlias = tb.Rows[2][1].ToString() == "Alias";
                for (int i = 3; i < tb.Rows.Count; i++)
                {
                    if (tb.Rows[i].ItemArray[0].ToString() == "")
                    {
                        Console.Write("omit null");
                        continue;
                    }

                    var idStr = tb.Rows[i].ItemArray[0].ToString();
                    int mainId = int.Parse(idStr);
                    if (hasAlias) //包含常量定义
                    {
                        var datasV = tb.Rows[i].ItemArray[1].ToString();
                        if (!string.IsNullOrEmpty(datasV))
                            indexerBlock.Add(string.Format("public static readonly int {0} = {1};", datasV, idStr)); //后者是常量
                    }

                    if (mainId <= 0)
                    {
                        Console.Write("omit " + mainId);
                        continue;
                    }

                    swTotal.WriteLine(string.Format("\t\t\t{0}Dict.Add({1}, new {0}Config({1}{2}));", keyname, mainId, GetString(tb.Rows[i].ItemArray, tb.Rows[1].ItemArray, tb.Rows[2].ItemArray)));
                }
            }

            swTotal.WriteLine("\t\t}");
            swTotal.WriteLine("\t}");
            swTotal.WriteLine("}");
            swTotal.Close();

            //再写各个类定义文件
            StreamWriter sw = new StreamWriter(string.Format("./ConfigData/{0}.cs", keyname), false, Encoding.UTF8);
         
            object[] infos = datas.Tables[0].Rows[2].ItemArray;
            object[] types = datas.Tables[0].Rows[1].ItemArray;

            sw.WriteLine("namespace ConfigDatas");
            sw.WriteLine("{");
            sw.WriteLine("\tpublic class " + keyname + "Config");
            sw.WriteLine("\t{");
            List<string> infoStr = new List<string>();
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].ToString()[0] == '~')
                    continue;

                sw.WriteLine("\t\tpublic {0} {1};", ConvertType(types[i]), infos[i]);
                infoStr.Add(ConvertType(types[i]) + " " + infos[i]);
            }
            sw.WriteLine("\t\tpublic " + keyname + "Config(){}");

            sw.WriteLine("\t\tpublic " + keyname + "Config(" + string.Join(",", infoStr.ToArray()) + ")");
            sw.WriteLine("\t\t{");
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].ToString()[0] == '~')
                    continue;

                sw.WriteLine("\t\t\tthis.{0}= {0};", infos[i]);
            }
            sw.WriteLine("\t\t}");

            if (indexerBlock.Count > 0)
            {
                sw.WriteLine("\t\tpublic class Indexer");
                sw.WriteLine("\t\t{");
                foreach (var str in indexerBlock)
                {
                    sw.WriteLine(str);
                }
                sw.WriteLine("\t\t}");
            }
            
            sw.WriteLine("\t}");
            sw.WriteLine("}");
            sw.Close();
        }

        private static string GetString(object[] datas, object[] types, object[] names)
        {
            List<string> strs = new List<string>();
            for (int i = 1; i < datas.Length; i++)
            {
                var nameStr = names[i].ToString();
                if (nameStr[0] == '~')
                    continue;

                var typeStr = types[i].ToString();
                string result;
                if (NeedParse(typeStr))
                {
                    result = string.Format("{0}.Parse(\"{1}\")", types[i], datas[i]);
                }
                else if (typeStr == "BuffEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(IBuff buf, IMonster o){{{0}}}", datas[i]);
                }
                else if (typeStr == "SkillInitialEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ISkill skl,IMonster s){{{0}}}", datas[i]);
                }
                else if (typeStr == "SkillHitEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ISkill skl,IMonster s,IMonster d,ref int hit){{{0}}}", datas[i]);
                }
                else if (typeStr == "SkillDamageEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ISkill skl,IMonster s,IMonster d,bool isActive,HitDamage damage, ref bool nodef){{{0}}}", datas[i]);
                }
                else if (typeStr == "SkillAfterHitEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ISkill skl,IMonster s,IMonster d,HitDamage damage){{{0}}}", datas[i]);
                }
                else if (typeStr == "SkillBurstCheckDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(IMonster s,IMonster d,bool isMelee){{if({0})return true;return false;}}", datas[i]);
                }
                else if (typeStr == "SkillTimelyEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ISkill skl,IMonster s){{{0}}}", datas[i]);
                }
                else if (typeStr == "SkillUseCardHandleDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ISkill skl,IMonster s, IPlayer p, int type, int lv, ref bool eff){{{0}}}", datas[i]);
                }
                else if (typeStr == "SpellEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ISpell spl, IMap m, IPlayer p, IPlayer r, IMonster t,System.Drawing.Point mouse){{{0}}}", datas[i]);
                }
                else if (typeStr == "SpellCheckDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ISpell spl, IPlayer p, IMonster t){{if({0})return true;return false;}}", datas[i]);
                }
                else if (typeStr == "RelicEventDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(IPlayer p, IRelic r, int cid, int type, IMonster m,ref bool result){{{0}}}", datas[i]);
                }
                else if (typeStr == "EquipMonsterPickDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format( "delegate(IMonster m){{if({0})return true;return false;}}", datas[i]);
                }
                else if (typeStr == "FormatStringDelegate")
                {
                    if (datas[i].ToString() == "")
                    {
                        result = "";
                    }
                    else
                    {
                        result = datas[i].ToString();
                        Regex regex = new Regex(@"{.*?}");
                        MatchCollection mc = regex.Matches(datas[i].ToString());
                        int index = 0;
                        if (mc.Count > 0)
                        {
                            strs[i] = "\"" + datas[i] + "\"";
                            string[] datas2 = new string[mc.Count];
                            foreach (Match match in mc)
                            {
                                datas2[index] = match.Value.Substring(1, match.Value.Length - 2);

                                int pindex = result.IndexOf(match.Value);
                                result = result.Substring(0, pindex) + "{" + index++ + ":0}" +
                                          result.Substring(pindex + match.Value.Length);
                            }
                            result = string.Format("{0},{1}", result, string.Join(",", datas2));
                        }
                        else
                        {
                            result = "\"" + result + "\"";
                        }
                    }
                    result = string.Format("delegate(int lv){{ return string.Format({0});}}", result);
                }
                else if (typeStr == "string[]") //数组
                {
                    string[] infos = datas[i].ToString().Split(';');
                    result = datas[i].ToString() == ""
                                  ? string.Format("new {0}{{}}", types[i])
                                  : string.Format("new {0}{{\"{1}\"}}", types[i], string.Join("\",\"", infos));
                }
                else if (typeStr.Contains("[]")) //数组
                {
                    string[] infos = datas[i].ToString().Split(';');
                    result = datas[i].ToString() == ""
                                  ? string.Format("new {0}{{}}", types[i])
                                  : string.Format("new {0}{{{1}}}", types[i], string.Join(",", infos));
                }
                else if (typeStr == "Color")
                {
                    string[] infos = datas[i].ToString().Split(';');
                    result = datas[i].ToString() == ""
                                  ? string.Format("new {0}{{}}", types[i])
                                  : string.Format("new {0}{{{1}}}", types[i], string.Join(",", infos));
                }
                else if (typeStr == "string")
                {
                    result = "\"" + datas[i] + "\"";
                }
                else if (typeStr == "bool")
                {
                    result = datas[i].ToString() == "" ? "false" : datas[i].ToString();
                }
                else//primitive type
                {
                    result = datas[i].ToString() == "" ? "0" : datas[i].ToString();
                }
                strs.Add(result);
            }
            return "," + string.Join(",", strs.ToArray());
        }

        private static string ConvertType(object tp)
        {
            switch (tp.ToString())
            {
                case "SkillActiveType": return "int";
                case "Color": return "int";
            }
            return tp.ToString();
        }

        private static bool NeedParse(string tp)
        {
            if (tp.Substring(0, 2) == "RL")
            {
                return true;
            }
            switch (tp)
            {
                case "SkillActiveType": return true;
            }
            return false;
        }

    }
}