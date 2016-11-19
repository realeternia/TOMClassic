namespace TaleofMonsters.DataType.Others
{
    internal static class JobBook
    {
        //public static int GetSkillCommonRate(int id, int index)
        //{
        //    JobConfig jobConfig = ConfigData.GetJobConfig(id);

        //    switch (index+1)
        //    {
        //        case 1: return jobConfig.SkillCommon1;
        //        case 2: return jobConfig.SkillCommon2;
        //        case 3: return jobConfig.SkillCommon3;
        //        case 4: return jobConfig.SkillCommon4;
        //        case 5: return jobConfig.SkillCommon5;
        //        case 6: return jobConfig.SkillCommon6;
        //        case 7: return jobConfig.SkillCommon7;
        //        case 8: return jobConfig.SkillCommon8;
        //        case 9: return jobConfig.SkillCommon9;
        //        case 10: return jobConfig.SkillCommon10;
        //        case 11: return jobConfig.SkillCommon11;
        //        case 12: return jobConfig.SkillCommon12;
        //        case 13: return jobConfig.SkillCommon13;
        //    }
        //    return 0;
        //}

        //public static int[] GetJobLevelAttr(int id, int level)
        //{
        //    JobConfig jobConfig = ConfigData.GetJobConfig(id);

        //    int[] datas = new int[8];
        //    Array.Clear(datas, 0, datas.Length);

        //    datas[0] = jobConfig.Atk + jobConfig.Atkp*(level - 1)/10;
        //    datas[1] = jobConfig.Def + jobConfig.Defp*(level - 1)/10;
        //    datas[2] = jobConfig.Mag + jobConfig.Magp*(level - 1)/10;
        //    datas[3] = jobConfig.Luk + jobConfig.Lukp*(level - 1)/10;
        //    datas[4] = jobConfig.Spd + jobConfig.Spdp*(level - 1)/10;
        //    datas[5] = jobConfig.Luk + jobConfig.Lukp*(level - 1)/10;
        //    datas[6] = jobConfig.Hp + jobConfig.Vitp*(level - 1)/10;
        //    datas[7] = jobConfig.Adp + jobConfig.Adpp*(level - 1)/10;

        //    return datas;
        //}

        //public static Image GetPreview(int id)
        //{
        //    JobConfig jobConfig = ConfigData.GetJobConfig(id);

        //    const string stars = "★★★★★";
        //    const string dstars = "☆☆☆☆☆";
        //    ControlPlus.TipImage tipData = new ControlPlus.TipImage();
        //    tipData.AddTextNewLine(jobConfig.Name, "White", 20);
        //    tipData.AddTextNewLine("战斗", "White");
        //    int starCount = (jobConfig.Atk * 10 + jobConfig.Atkp) / 20;
        //    tipData.AddText(string.Format("{0}{1}", stars.Substring(4 - starCount), dstars.Substring(starCount + 1)), "Yellow");
        //    tipData.AddText("守护", "White");
        //    starCount = (jobConfig.Def * 10 + jobConfig.Defp) / 20;
        //    tipData.AddText(string.Format("{0}{1}", stars.Substring(4 - starCount), dstars.Substring(starCount + 1)), "Yellow");

        //    tipData.AddTextNewLine("法术", "White");
        //    starCount = (jobConfig.Mag * 10 + jobConfig.Magp) / 20;
        //    tipData.AddText(string.Format("{0}{1}", stars.Substring(4 - starCount), dstars.Substring(starCount + 1)), "Yellow");
        //    tipData.AddText("幸运", "White");
        //    starCount = (jobConfig.Luk * 10 + jobConfig.Lukp) / 20;
        //    tipData.AddText(string.Format("{0}{1}", stars.Substring(4 - starCount), dstars.Substring(starCount + 1)), "Yellow");

        //    tipData.AddTextNewLine("速度", "White");
        //    starCount = (jobConfig.Spd * 10 + jobConfig.Spdp) / 20;
        //    tipData.AddText(string.Format("{0}{1}", stars.Substring(4 - starCount), dstars.Substring(starCount + 1)), "Yellow");
        //    tipData.AddText("幸运", "White");
        //    starCount = (jobConfig.Luk * 10 + jobConfig.Lukp) / 20;
        //    tipData.AddText(string.Format("{0}{1}", stars.Substring(4 - starCount), dstars.Substring(starCount + 1)), "Yellow");

        //    tipData.AddTextNewLine("体质", "White");
        //    starCount = (jobConfig.Hp * 10 + jobConfig.Vitp) / 20;
        //    tipData.AddText(string.Format("{0}{1}", stars.Substring(4 - starCount), dstars.Substring(starCount + 1)), "Yellow");
        //    tipData.AddText("生存", "White");
        //    starCount = (jobConfig.Adp * 10 + jobConfig.Adpp) / 20;
        //    tipData.AddText(string.Format("{0}{1}", stars.Substring(4 - starCount), dstars.Substring(starCount + 1)), "Yellow");
        //    tipData.AddLine();
        //    tipData.AddTextNewLine("专业技能", "White");
        // //   tipData.AddTextNewLine(GetGoodSkill(id), "LightSkyBlue");
        //    return tipData.Image;
        //}
    }
}
