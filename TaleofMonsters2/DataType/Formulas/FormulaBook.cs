using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;

namespace TaleofMonsters.DataType.Formulas
{
    internal static class FormulaBook
    {
        [Obsolete("已经废弃")]
        public static double GetPhysicalDamage(int atk, int def)
        {
            Dictionary<string, string> rules = new Dictionary<string, string>();
            rules.Add("atk", atk.ToString());
            rules.Add("def", def.ToString());
            return MathTool.GetFormulaResult(ConfigData.GetFormulaConfig(FormulaConfig.Indexer.Pdamage).Rule, rules);
        }

        [Obsolete("已经废弃")]
        public static double GetMagicDamage(int atk, int def)
        {
            Dictionary<string, string> rules = new Dictionary<string, string>();
            rules.Add("atk", atk.ToString());
            rules.Add("def", def.ToString());
            return MathTool.GetFormulaResult(ConfigData.GetFormulaConfig(FormulaConfig.Indexer.Mdamage).Rule, rules);
        }

        [Obsolete("已经废弃")]
        public static double GetHitRate(int hit, int dhit)
        {
            Dictionary<string, string> rules = new Dictionary<string, string>();
            rules.Add("hit", hit.ToString());
            rules.Add("dit", dhit.ToString());
            return MathTool.GetFormulaResult(ConfigData.GetFormulaConfig(FormulaConfig.Indexer.Hit).Rule, rules);
        }
    }
}
