using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Syncfusion.XlsIO;

namespace accounting.Services
{
    public class AuditService
    {

        ExcelEngine excelEngine = new ExcelEngine();

        public void exportReport(DataTable data, string filePath)
        {
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheet = workbook.Worksheets[0];

            sheet.ImportDataTable(data, true, 1, 1, true);

            IListObject table = sheet.ListObjects.Create("Table", sheet.UsedRange);
            table.BuiltInTableStyle = TableBuiltInStyles.TableStyleMedium14;
            sheet.UsedRange.AutofitColumns();

            Stream excelStream = File.Create(Path.GetFullPath(filePath));
            workbook.SaveAs(excelStream);
            excelStream.Dispose();
        }
    }
}
