
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using ClosedXML.Excel;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.Entities;
using TaskStatus = TaskScheduler.Entities.TaskStatus;

namespace TaskScheduler.Business.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<byte[]> GenerateScheduleExcelAsync(int scheduleId)
        {
            // DÜZELTME: İlişkili verileri veritabanında sorgulamak için IQueryable (GetAll) kullanıyoruz.
            // Bu, tüm veriyi belleğe çekmek yerine veritabanında verimli bir JOIN sorgusu oluşturur.
            var schedule = await _unitOfWork.WeeklySchedule.GetAll()
                .Include(ws => ws.ScheduledTasks).ThenInclude(st => st.Personnel)
                .Include(ws => ws.ScheduledTasks).ThenInclude(st => st.Task)
                .FirstOrDefaultAsync(ws => ws.Id == scheduleId);

            if (schedule == null) return new byte[0];

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Haftalık Plan");

            worksheet.Cell("A1").Value = $"Haftalık Plan - {schedule.WeekStartDate:dd.MM.yyyy}";
            // Diğer excel doldurma işlemleri...

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateSchedulePdfAsync(int scheduleId)
        {
            // DÜZELTME: İlişkili verileri veritabanında sorgulamak için IQueryable (GetAll) kullanıyoruz.
            var schedule = await _unitOfWork.WeeklySchedule.GetAll()
                .Include(ws => ws.ScheduledTasks).ThenInclude(st => st.Personnel)
                .Include(ws => ws.ScheduledTasks).ThenInclude(st => st.Task)
                .FirstOrDefaultAsync(ws => ws.Id == scheduleId);


            if (schedule == null) return new byte[0];

            var personnelList = schedule.ScheduledTasks
                .Where(st => st.Personnel != null) // Veri tutarlılığı için bu kontrolü bırakmak iyidir.
                .Select(st => st.Personnel!) // Derleyiciye Personnel'in null olmayacağını bildiriyoruz.
                .Distinct()
                .ToList();

            var daysOfWeek = new Dictionary<int, string> { { 1, "Pzt" }, { 2, "Sal" }, { 3, "Çar" }, { 4, "Per" }, { 5, "Cum" }, { 6, "Cmt" } };

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Text($"Görev Planı - {schedule.WeekStartDate:dd MMMM yyyy} Haftası")
                        .SemiBold().FontSize(16).AlignCenter();

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Personel
                                foreach (var _ in daysOfWeek) columns.RelativeColumn(1.5f);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Border(1).Background(Colors.Grey.Lighten2).AlignCenter().Text("Personel");
                                foreach (var day in daysOfWeek)
                                    header.Cell().Border(1).Background(Colors.Grey.Lighten2).AlignCenter().Text(day.Value);
                            });

                            foreach (var personnel in personnelList)
                            {
                                table.Cell().Border(1).Padding(2).Text($"{personnel.FirstName} {personnel.LastName}");
                                foreach (var day in daysOfWeek)
                                {
                                    var task = schedule.ScheduledTasks.FirstOrDefault(st => st.PersonnelId == personnel.Id && st.DayOfWeek == day.Key);
                                    
                                    var text = "-";
                                    if (task != null && task.Task != null)
                                    {
                                        // EnumExtensions'ı burada doğrudan kullanamayız. Manuel yazalım.
                                        var statusText = task.Status switch
                                        {
                                            TaskStatus.Pending => "Bekliyor",
                                            TaskStatus.InProgress => "Devam",
                                            TaskStatus.Completed => "Tamamlandı",
                                            TaskStatus.Postponed => "Ertelendi",
                                            _ => ""
                                        };
                                        text = $"{task.Task.Name}\n(Z: {task.Task.DifficultyLevel}) - {statusText}";
                                    }

                                    table.Cell().Border(1).Padding(2).AlignCenter().Text(text);
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Sayfa ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}