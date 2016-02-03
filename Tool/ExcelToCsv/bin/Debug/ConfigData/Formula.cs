namespace ConfigDatas
{
	public class FormulaConfig
	{
		public int Id;
		public string Name;
		public string Rule;
		public FormulaConfig(){}
		public FormulaConfig(int Id,string Name,string Rule)
		{
			this.Id= Id;
			this.Name= Name;
			this.Rule= Rule;
		}
		public class Indexer
		{
		}
	}
}
