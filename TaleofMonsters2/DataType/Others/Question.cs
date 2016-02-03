namespace TaleofMonsters.DataType.Others
{
    struct Question
    {
        public string info;
        public string[] ans;
        public string result;

        public string GetAns(int id)
        {
            if (ans[id].Length > 20)
            {
                return ans[id].Substring(0, 20) + "...";
            }
            return ans[id];
        }

        public string GetResult()
        {
            if (result.Length > 20)
            {
                return result.Substring(0, 20) + "...";
            }
            return result;
        }
    }
}
