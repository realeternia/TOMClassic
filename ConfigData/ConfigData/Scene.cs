namespace ConfigDatas
{
	public class SceneConfig
	{
		public int Id;
		public string Name;
		public int Level;
		public int WindowX;
		public int WindowY;
		public string Func;
		public int Url;
		public SceneConfig(){}
		public SceneConfig(int Id,string Name,int Level,int WindowX,int WindowY,string Func,int Url)
		{
			this.Id= Id;
			this.Name= Name;
			this.Level= Level;
			this.WindowX= WindowX;
			this.WindowY= WindowY;
			this.Func= Func;
			this.Url= Url;
		}
		public class Indexer
		{
public static readonly int BornMapId = 13000001;
		}
	}
}
