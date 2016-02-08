namespace ConfigDatas
{
	public class EquipConfig
	{
		public int Id;
		public string Name;
		public int Quality;
		public int Level;
		public int Position;
		public int LvNeed;
		public int Value;
		public int AtkP;
		public int VitP;
		public int Job;
		public int[] EnergyRate;
		public int SpecialSkill;
		public string Url;
		public EquipConfig(){}
		public EquipConfig(int Id,string Name,int Quality,int Level,int Position,int LvNeed,int Value,int AtkP,int VitP,int Job,int[] EnergyRate,int SpecialSkill,string Url)
		{
			this.Id= Id;
			this.Name= Name;
			this.Quality= Quality;
			this.Level= Level;
			this.Position= Position;
			this.LvNeed= LvNeed;
			this.Value= Value;
			this.AtkP= AtkP;
			this.VitP= VitP;
			this.Job= Job;
			this.EnergyRate= EnergyRate;
			this.SpecialSkill= SpecialSkill;
			this.Url= Url;
		}
		public class Indexer
		{
public static readonly int InitJobEquipStart = 21100101;
public static readonly int InitJobEquipEnd = 21100108;
		}
	}
}
