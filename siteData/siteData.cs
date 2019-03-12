using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QbeiAgencies_BL;
using QbeiAgencies_Common;
using System.Configuration;
using System.IO;

namespace siteData
{

    public static class Rfc4180Writer
    {     
    public static void WriteDataTable(DataTable dt, TextWriter writer, bool includeHeaders) 
    {
        if (includeHeaders) 
        {
            IEnumerable<String> headerValues = dt.Columns .OfType<DataColumn>().Select(column => QuoteValue(column.ColumnName));
            writer.WriteLine(String.Join(",", headerValues));
        }

        IEnumerable<String> items = null;

        foreach (DataRow row in dt.Rows) 
        {
            items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
            writer.WriteLine(String.Join(",", items));
        }

             writer.Flush();
        }

      private static string QuoteValue(string value)
      {
                return String.Concat("\"",value.Replace("\"", "\"\""), "\"");
      }

     }

    public static class siteData
    {
        static void Main(string[] args)
        {
            GenerateCSV(11, "マルイ");
            GenerateCSV(12, "カワシマ");
            GenerateCSV(13, "ミズタニ");
            GenerateCSV(14, "イワイ");
            //GenerateCSV(15, "アキ");
            GenerateCSV(16, "ライトウェイ");
            GenerateCSV(17, "インターマックス");
            GenerateCSV(18, "日直");
            GenerateCSV(19, "深谷_フカヤ_");
            GenerateCSV(20, "ダイアテック(高難易度)");
            //GenerateCSV(22, "ブリヂストンサイクル西日本販売(株)物流部専用");
            //GenerateCSV(23, "パナソニック");
            GenerateCSV(24, "東(アズマ)");
            GenerateCSV(30, "城東");
            GenerateCSV(31, " アキボウ");
            GenerateCSV(34, "シマノ");
            GenerateCSV(35, "インターテック");
            GenerateCSV(36, "インターナショナル");
            GenerateCSV(37, "東京サンエス(株)");
            GenerateCSV(38, "フタバ");
            //GenerateCSV(42, "アメア");
            GenerateCSV(46, " トライスポーツ");
            GenerateCSV(53, " 発注モジュールひな形");
            GenerateCSV(57, " モトクロス");
            GenerateCSV(59, "(株)ジェイピースポーツグループ");
            //GenerateCSV(58, " リンエイ");
            GenerateCSV(65, "野口");
            GenerateCSV(84, "(今期取り扱い商品無し保留)");
            GenerateCSV(87, "ダートフリーク");
            //GenerateCSV(104, "宮田(中川商会扱い)");
            //GenerateCSV(124, "ミズタニ自転車（下鴨アカウントのみ発注可能");
            GenerateCSV(139, "ウエイブワン株式会社");
            GenerateCSV(143, "（株）ポディウム");
            GenerateCSV(914, "（株）イノセントデザインワークス");
            GenerateCSV(916, "(株)あさひ");
        }

        private static void GenerateCSV(int siteID, string sitename)
        {
            QbeiUser_Entity que = new QbeiUser_Entity();
            QbeiUser_BL qubl = new QbeiUser_BL();
            DataTable dtresult = qubl.GetSiteData(siteID);
            if (dtresult!= null && dtresult.Rows.Count > 0)
            {
                int dtcount = Convert.ToInt32(dtresult.Rows.Count);
                string date = string.Format("{0:yyyyMMddhhmm}", DateTime.Now);
                DataTable dt = new DataTable();

                dt.Columns.Add("代理店ID");
                dt.Columns.Add("JANコード");
                dt.Columns.Add("発注コード");
                dt.Columns.Add("在庫情報");
                dt.Columns.Add("下代");
                dt.Columns.Add("入荷予定");
                dt.Columns.Add("purchaserURL");

                dt.Columns.Add("自社品番");
                dt.Columns.Add("メーカー情報日");
                dt.Columns.Add("最終反映日");
                dt.Columns.Add("Updated_Date");
                dt.Columns.Add("sitecode");

                for (int i = 0; i < dtcount; i++)
                {
                    dt.Rows.Add(dtresult.Rows[i]["siteID"].ToString(),
                                dtresult.Rows[i]["jancode"].ToString(),
                                dtresult.Rows[i]["orderCode"].ToString(),
                                dtresult.Rows[i]["quantity"].ToString(),
                                dtresult.Rows[i]["price"].ToString(),
                                dtresult.Rows[i]["stockDate"].ToString(),
                                dtresult.Rows[i]["purchaserURL"].ToString(),
                                dtresult.Rows[i]["partNo"].ToString(),
                                dtresult.Rows[i]["makerDate"].ToString(),
                                dtresult.Rows[i]["reflectDate"].ToString(),
                                dtresult.Rows[i]["Updated_Date"].ToString(),
                                dtresult.Rows[i]["sitecode"].ToString());
                }


                using (StreamWriter writer = new StreamWriter(new FileStream("C:\\Qbei_Log\\ExportCSV\\" + siteID + "_maker_stock_" + date + ".csv", FileMode.Create, FileAccess.ReadWrite), Encoding.GetEncoding(932)))
                {
                    Rfc4180Writer.WriteDataTable(dt, writer, true);
                }
            }
            else {
                
            }
        }
    }
}
       
    

