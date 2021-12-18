using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace UiPathProjectAnalyser.Excel
{
    public class ExportReportExcel
    {
        public List<UiPathProjectAnalyser> UiPathProjects { get; set; }
        public string FileName { get; set; }

        IWorkbook workbook = new XSSFWorkbook();
        XSSFSheet worksheet;

        public ExportReportExcel(List<UiPathProjectAnalyser> projects, string fileName)
        {
            this.UiPathProjects = projects;
            this.FileName = fileName;
        }

        public void CreateProjectListSheets()
        {
            //新規のExcelブックを作成する

            worksheet = workbook.CreateSheet("プロジェクト一覧") as XSSFSheet;
            InsertProjectData();

            // Format Cell Range As Table
            XSSFTable xssfTable = worksheet.CreateTable();
            CT_Table ctTable = xssfTable.GetCTTable();
            AreaReference myDataRange = new AreaReference(new CellReference(0, 0), new CellReference(11, UiPathProjects.Count));
            ctTable.@ref = myDataRange.FormatAsString();
            ctTable.id = 1;
            ctTable.name = "プロジェクト一覧";
            ctTable.displayName = "プロジェクト一覧";
            ctTable.tableStyleInfo = new CT_TableStyleInfo();
            ctTable.tableStyleInfo.name = "TableStyleMedium2"; // TableStyleMedium2 is one of XSSFBuiltinTableStyle
            ctTable.tableStyleInfo.showRowStripes = true;
            ctTable.tableColumns = new CT_TableColumns();
            ctTable.tableColumns.tableColumn = new List<CT_TableColumn>();
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 1, name = "ID" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 2, name = "プロジェクト名" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 3, name = "プロジェクト説明" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 4, name = "Score" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 5, name = "WF数" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 6, name = "Bad WF数" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 7, name = "合計Activity数" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 8, name = "複雑度" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 9, name = "ネスト数" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 10, name = "Project Ver" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 11, name = "Studio Ver" });
            ctTable.tableColumns.tableColumn.Add(new CT_TableColumn() { id = 12, name = "プロジェクトフォルダパス" });

            using (FileStream file = new FileStream(this.FileName, FileMode.Create))
            {
                workbook.Write(file);
            }
        }
        private void InsertProjectData()
        {
            worksheet.CreateRow(0);
            worksheet.GetRow(0).CreateCell(0).SetCellValue("ID");
            worksheet.GetRow(0).CreateCell(1).SetCellValue("プロジェクト名");
            worksheet.GetRow(0).CreateCell(2).SetCellValue("プロジェクト説明");
            worksheet.GetRow(0).CreateCell(3).SetCellValue("Score");
            worksheet.GetRow(0).CreateCell(4).SetCellValue("WF数");
            worksheet.GetRow(0).CreateCell(5).SetCellValue("Bad WF数");
            worksheet.GetRow(0).CreateCell(6).SetCellValue("合計Activity数");
            worksheet.GetRow(0).CreateCell(7).SetCellValue("複雑度");
            worksheet.GetRow(0).CreateCell(8).SetCellValue("ネスト数");
            worksheet.GetRow(0).CreateCell(9).SetCellValue("Project Ver");
            worksheet.GetRow(0).CreateCell(10).SetCellValue("Studio Ver");
            worksheet.GetRow(0).CreateCell(11).SetCellValue("プロジェクトフォルダパス");

            foreach (var item in UiPathProjects.Select((value,index) => new { value, index }))
            {
                worksheet.CreateRow(item.index+1);
                worksheet.GetRow(item.index + 1).CreateCell(0).SetCellValue(item.index + 1);
                worksheet.GetRow(item.index + 1).CreateCell(1).SetCellValue(item.value.Project.name);
                worksheet.GetRow(item.index + 1).CreateCell(2).SetCellValue(item.value.Project.description);
                worksheet.GetRow(item.index + 1).CreateCell(3).SetCellValue(item.value.WorkflowScoreAverage);
                worksheet.GetRow(item.index + 1).CreateCell(4).SetCellValue(item.value.WorkflowFileCount);
                worksheet.GetRow(item.index + 1).CreateCell(5).SetCellValue(item.value.BadWorkflowFileCount);
                worksheet.GetRow(item.index + 1).CreateCell(6).SetCellValue(item.value.TotalAvtivityCount);
                worksheet.GetRow(item.index + 1).CreateCell(7).SetCellValue(item.value.TotalCyclomaticComplexity);
                worksheet.GetRow(item.index + 1).CreateCell(8).SetCellValue(item.value.MaxNestedCount);
                worksheet.GetRow(item.index + 1).CreateCell(9).SetCellValue(item.value.Project.projectVersion);
                worksheet.GetRow(item.index + 1).CreateCell(10).SetCellValue(item.value.Project.studioVersion);
                worksheet.GetRow(item.index + 1).CreateCell(11).SetCellValue(item.value.ProjectFolderPath);
            }

            worksheet.AutoSizeColumn(0, true);
            worksheet.AutoSizeColumn(1, true);
            worksheet.AutoSizeColumn(2, true);
            worksheet.AutoSizeColumn(3, true);
            worksheet.AutoSizeColumn(4, true);
            worksheet.AutoSizeColumn(5, true);
            worksheet.AutoSizeColumn(6, true);
            worksheet.AutoSizeColumn(7, true);
            worksheet.AutoSizeColumn(8, true);
            worksheet.AutoSizeColumn(9, true);
            worksheet.AutoSizeColumn(10, true);
            worksheet.AutoSizeColumn(11, true);

        }
    }
}
