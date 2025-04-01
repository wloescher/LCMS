using Microsoft.Extensions.Configuration;
using System.Data;

namespace LCMS.Utilities
{
    public static class ExcelExportUtility
    {
        /// <summary>
        /// Export DataTable to Excel.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="input"></param>
        /// <param name="tableName"></param>
        /// <returns>URL of Excel file.</returns>
        public static string ExportDataTabelToExcel(IConfiguration configuration, DataTable input, string tableName)
        {
            var fileName = tableName + DateTime.Now.Ticks.ToString() + ".xlsx";
            var excelReportDirectory = ConfigurationUtility.GetConfigurationKeyValue(configuration, "ExcelReportDirectory");
            var path = string.Format("{0}{1}", excelReportDirectory, fileName);

            using (var wb = new ClosedXML.Excel.XLWorkbook())
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                wb.Worksheets.Add(input, tableName);
                wb.SaveAs(fileStream);
            }

            var excelReportUrl = ConfigurationUtility.GetConfigurationKeyValue(configuration, "ExcelReportUrl");
            var url = string.Format("{0}{1}", excelReportUrl, fileName);
            return url;
        }
    }
}
