using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Formatting = Newtonsoft.Json.Formatting;

namespace LCMS.Utilities
{
    public static class DataConversionUtility
    {
        /// <summary>
        /// Converts list of objects to DataTable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects">The items.</param>
        /// <returns>DataTable.</returns>
        public static DataTable ConvertObjectListToDataTable<T>(List<T> objects)
        {
            var json = JsonConvert.SerializeObject(objects);
            return JsonConvert.DeserializeObject<DataTable>(json) ?? new DataTable();
        }

        /// <summary>
        /// Converts DataTable to JSON.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        /// <returns>JSON string.</returns>
        public static string ConvertDataTableToJson(DataTable dataTable)
        {
            return JsonConvert.SerializeObject(dataTable, Formatting.Indented);
        }

        /// <summary>
        /// Convert DataSet to Excel string.
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="title"></param>
        /// <param name="printHeaders"></param>
        /// <returns>Excel sheet as string.</returns>
        public static string DataSetToExcelString(DataSet ds, string title, bool printHeaders)
        {
            if (ds == null) return string.Empty;

            var sbTop = new StringBuilder();
            sbTop.Append("<html xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:x=\"urn:schemas-microsoft-com:office:excel\" ");
            sbTop.Append("xmlns=\"http://www.w3.org/TR/REC-html40\"><head><meta http-equiv=Content-Type content=\"text/html; charset=windows-1252\">");
            sbTop.Append("<meta name=ProgId content=Excel.Sheet><meta name=Generator content=\"Microsoft Excel 9\"><!--[if gte mso 9]>");
            sbTop.Append("<xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>" + title + "</x:Name><x:WorksheetOptions>");
            sbTop.Append("<x:Selected/><x:ProtectContents>False</x:ProtectContents><x:ProtectObjects>False</x:ProtectObjects>");
            sbTop.Append("<x:ProtectScenarios>False</x:ProtectScenarios></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets>");
            sbTop.Append("<x:ProtectStructure>False</x:ProtectStructure><x:ProtectWindows>False</x:ProtectWindows></x:ExcelWorkbook></xml>");
            sbTop.Append("<![endif]--></head><body>");
            var bottom = "</body></html>";

            var sb = new StringBuilder();
            for (int t = 0; t < ds.Tables.Count; t++)
            {
                sb.Append("<table>");
                if (printHeaders)
                {
                    sb.Append("<tr>");
                    for (int i = 0; i < ds.Tables[t].Columns.Count; i++)
                    {
                        sb.Append("<td>" + ds.Tables[t].Columns[i].ColumnName + "</td>");
                    }
                    sb.Append("</tr>");
                }

                for (int x = 0; x < ds.Tables[t].Rows.Count; x++)
                {
                    sb.Append("<tr>");
                    for (int i = 0; i < ds.Tables[t].Columns.Count; i++)
                    {
                        sb.Append("<td>" + ds.Tables[t].Rows[x][i] + "</td>");
                    }
                    sb.Append("</tr>");
                }
                sb.Append("</table>");
            }

            return sbTop.ToString() + sb.ToString() + bottom;
        }

        /// <summary>
        /// Converts NameValueCollection to Dictionary.
        /// </summary>
        /// <param name="nameValueCollection"></param>
        /// <returns>Dictionary.</returns>
        public static Dictionary<string, object> ConvertNameValueCollectionToDictionary(NameValueCollection nameValueCollection)
        {
            var dictionary = new Dictionary<string, object>();
            if (nameValueCollection == null) { return dictionary; }
            foreach (string key in nameValueCollection.Keys)
            {
                var values = nameValueCollection.GetValues(key);
                if (values != null)
                {
                    if (values.Length == 1)
                    {
                        dictionary.Add(key, values[0]);
                    }
                    else
                    {
                        dictionary.Add(key, values);
                    }
                }
            }
            return dictionary;
        }
    }
}
