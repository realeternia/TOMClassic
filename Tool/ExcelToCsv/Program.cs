using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ExcelToCsv
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles("./Xlsx");
            StreamWriter swTotal = new StreamWriter("./ConfigData/ConfigData.cs", false, Encoding.Default);

            swTotal.WriteLine("using System;");
            swTotal.WriteLine("using System.Collections.Generic;");
            swTotal.WriteLine("using NarlonLib.Math;");            
            swTotal.WriteLine("namespace ConfigDatas");
            swTotal.WriteLine("{");
            swTotal.WriteLine("\tpublic class ConfigData");
            swTotal.WriteLine("\t{");
            string loadStr = "";
            try
            {
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.Extension == ".xlsx")
                    {
                        string keyname = fileInfo.Name.Substring(0, fileInfo.Name.Length - 5);
                        if (keyname.Length <= 1 || keyname[0] == '~')
                        {
                            continue;
                        }
                        Console.WriteLine(keyname);
                        loadStr += string.Format("\t\t\tLoad{0}();\r\n", keyname);

                        CopeFile(keyname, fileInfo, swTotal);
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadKey();
            }

            swTotal.WriteLine("\t\tpublic static void LoadData()");
            swTotal.WriteLine("\t\t{");
            swTotal.WriteLine(loadStr);
            swTotal.WriteLine("\t\t}");
            swTotal.WriteLine("\t}");
            swTotal.WriteLine("}");
            swTotal.Close();
        }

        private static void CopeFile(string keyname, FileInfo fileInfo, StreamWriter swTotal)
        {
            var datas = GetExcelToDataTableBySheet(fileInfo.FullName);
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
                for (int i = 3; i < tb.Rows.Count; i++)
                {
                    if (tb.Rows[i].ItemArray[0].ToString() == "")
                    {
                        Console.Write("omit null");
                        continue;
                    }

                    var idStr = tb.Rows[i].ItemArray[0].ToString();
                    int mainId = 0;
                    if (idStr.Contains("|")) //包含常量定义
                    {
                        var datasV = idStr.Split('|');
                        indexerBlock.Add(string.Format("public static readonly int {0} = {1};", datasV[1], datasV[0])); //后者是常量
                        mainId = int.Parse(datasV[0]);//id
                    }
                    else
                    {
                        mainId = int.Parse(idStr);//id
                    }
                    if (mainId <= 0)
                    {
                        Console.Write("omit " + mainId);
                        continue;
                    }
                  
                    swTotal.WriteLine(string.Format("\t\t\t{0}Dict.Add({1}, new {0}Config({1}{2}));", keyname,
                       mainId, GetString(tb.Rows[i].ItemArray, tb.Rows[1].ItemArray, tb.Rows[2].ItemArray)));
                }
            }

            swTotal.WriteLine("\t\t}");

            //再写各个类定义文件
            StreamWriter sw = new StreamWriter(string.Format("./ConfigData/{0}.cs", keyname), false, Encoding.Default);
         
            object[] infos = datas.Tables[0].Rows[0].ItemArray;
            object[] types = datas.Tables[0].Rows[1].ItemArray;
            if (infos[0].ToString() != "Id")
            {
                infos = datas.Tables[0].Rows[2].ItemArray;
            }

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
                infoStr.Add( ConvertType(types[i]) + " " + infos[i]);
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

            sw.WriteLine("\t\tpublic class Indexer");
            sw.WriteLine("\t\t{");
            foreach (var str in indexerBlock)
            {
                sw.WriteLine(str);
            }
            sw.WriteLine("\t\t}");

            sw.WriteLine("\t}");
            sw.WriteLine("}");
            sw.Close();
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

        private static string GetString(object[] datas, object[] types, object[] names)
        {
            List<string> strs=new List<string>();
            for (int i = 1; i < datas.Length; i++)
            {
                var nameStr = names[i].ToString();
                if (nameStr[0] == '~')
                {
                    continue;
                }

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
                                  : string.Format("delegate(IMonster o){{{0}}}", datas[i]);
                }
                else if (typeStr == "SkillInitialEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ITargetMeasurable sp,IMonster s,int lv){{{0}}}", datas[i]);
                }
                else if (typeStr == "SkillHitEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(IMonster s,IMonster d,ref int hit,int lv){{{0}}}", datas[i]);
                }
                else if (typeStr == "SkillDamageEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format(
                                      "delegate(IMonster s,IMonster d,bool isActive,HitDamage damage, ref bool nodef,int lv){{{0}}}",
                                      datas[i]);
                }
                else if (typeStr == "SkillAfterHitEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format(
                                      "delegate(ITargetMeasurable sp,IMonster s,IMonster d,HitDamage damage,int lv){{{0}}}",
                                      datas[i]);
                }
                else if (typeStr == "SkillBurstCheckDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format(
                                      "delegate(IMonster s,IMonster d,bool isMelee){{if({0})return true;return false;}}",
                                      datas[i]);
                }
                else if (typeStr == "SkillTimelyEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format("delegate(ITargetMeasurable sp,IMonster s,int lv){{{0}}}", datas[i]);
                }
                else if (typeStr == "SpellEffectDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format(
                                      "delegate(ISpell s, IMap m, IPlayer p, IPlayer r, IMonster t,System.Drawing.Point mouse,int lv){{{0}}}",
                                      datas[i]);
                }
                else if (typeStr == "SpellTrapAddCardDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format(
                                      "delegate(IPlayer p, IPlayer r, ITrap t, int cid, int type){{{0}}}",
                                      datas[i]);
                }
                else if (typeStr == "SpellTrapSummonDelegate")
                {
                    result = datas[i].ToString() == ""
                                  ? "null"
                                  : string.Format(
                                      "delegate(IPlayer p, IPlayer r, ITrap t, IMonster m,int lv){{{0}}}",
                                      datas[i]);
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
            return ","+ string.Join(",", strs.ToArray());
        }

        public static DataSet GetExcelToDataTableBySheet(string fileFullPath)
        {
            //string strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + FileFullPath +sheetNameed Properties='Excel 8.0; HDR=NO; IMEX=1'"; //此连接只能操作Excel2007之前(.xls)文件  
            string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + fileFullPath + ";Extended Properties='Excel 12.0; HDR=NO; IMEX=1'"; //此连接可以操作.xls与.xlsx文件  
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            System.Data.DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            DataSet ds = new DataSet();
            foreach (DataRow row in dt.Rows)
            {
                var sheetName = row["TABLE_NAME"].ToString();
                if (sheetName=="null" || sheetName.Replace("\'","").StartsWith("~"))
                {
                    continue;
                }
                Console.WriteLine(" -" + sheetName);
                OleDbDataAdapter odda = new OleDbDataAdapter(string.Format("SELECT * FROM [{0}]", sheetName), conn);                    //("select * from [Sheet1$]", conn);  
                odda.Fill(ds, sheetName);
            }
            conn.Close();
            return ds;
        } 
    }
}
