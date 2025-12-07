using GoogleDriveWebhook.Models;
using GoogleDriveWebhook.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoogleDriveWebhook.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class RunController : ControllerBase
	{
		private readonly GoogleSettings _settings;
		private readonly FolderScanner _scanner;
		private readonly DeadlineUpdater _updater;
		private readonly GoogleSheetsService _sheetsService; // <--- Додали сервіс

		public RunController(
			GoogleSettings settings,
			FolderScanner scanner,
			DeadlineUpdater updater,
			GoogleSheetsService sheetsService) // <--- Додали в конструктор
		{
			_settings = settings;
			_scanner = scanner;
			_updater = updater;
			_sheetsService = sheetsService;
		}

		[HttpGet]
		public async Task<IActionResult> Run()
		{
			foreach (var group in _settings.Groups)
			{
				var driveData = await _scanner.ScanGroup(group.FolderId);

				var studentRows = await _sheetsService.GetStudentRows(group.SheetId, group.SheetName, "A", 5);

				foreach (var student in driveData)
				{
					string studentNameFromDrive = student.Key.Trim();

					if (studentRows.TryGetValue(studentNameFromDrive, out int row))
					{
						foreach (var task in student.Value)
						{
							if (task.Value == null)
								continue;

							await _updater.UpdateStudent(
								group.SheetId,
								group.SheetName,
								row,
								"G",
								task.Value.Value
							);
						}
					}
					else
					{
						Console.WriteLine($"Увага: Студента '{studentNameFromDrive}' не знайдено в таблиці.");
					}
				}
			}

			return Ok("Done");
		}
	}
}