using System;
using System.Data;
using System.Data.OleDb;
using System.Threading;

namespace ExcelToCsv
{
    class ExcelTool
    {
        public static DataSet GetExcelToDataTableBySheet(string fileFullPath)
        {
            //string strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + FileFullPath +sheetNameed Properties='Excel 8.0; HDR=NO; IMEX=1'"; //此连接只能操作Excel2007之前(.xls)文件  
            string strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + fileFullPath + ";Extended Properties='Excel 12.0; HDR=NO; IMEX=1'"; //此连接可以操作.xls与.xlsx文件  
            OleDbConnection conn = new OleDbConnection(strConn);

            bool isLoad = false;
            while (!isLoad)
            {
                try
                {
                    conn.Open();
                    isLoad = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetExcelToDataTableBySheet failed " + fileFullPath);
                    Thread.Sleep(50);
                }
            }
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