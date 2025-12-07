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

		public RunController(
			GoogleSettings settings,
			FolderScanner scanner,
			DeadlineUpdater updater)
		{
			_settings = settings;
			_scanner = scanner;
			_updater = updater;
		}

		[HttpGet]
		public async Task<IActionResult> Run()
		{
			foreach (var group in _settings.Groups)
			{
				var data = await _scanner.ScanGroup(group.FolderId);

				int row = 6;

				foreach (var student in data)
				{
					foreach (var task in student.Value)
					{
						if (task.Value == null)
							continue;

						await _updater.UpdateStudent(
							group.SheetId,
							group.SheetName,
							row,
							"G", // TODO: колонка
							task.Value.Value
						);
					}

					row++;
				}
			}

			return Ok("Done");
		}
	}
}