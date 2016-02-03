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
public static readonly int Pdamage = 1;
public static readonly int Mdamage = 2;
public static readonly int Hit = 3;
		}
	}
}
