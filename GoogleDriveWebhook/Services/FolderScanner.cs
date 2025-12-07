namespace GoogleDriveWebhook.Services
{
	public class FolderScanner
	{
		private readonly GoogleDriveService _drive;

		public FolderScanner(GoogleDriveService drive)
		{
			_drive = drive;
		}

		public async Task<Dictionary<string, Dictionary<string, DateTime?>>> ScanGroup(string groupFolderId)
		{
			var result = new Dictionary<string, Dictionary<string, DateTime?>>();

			var students = await _drive.GetFiles(groupFolderId);

			foreach (var student in students.Where(f => f.MimeType == "application/vnd.google-apps.folder"))
			{
				var labs = await _drive.GetFiles(student.Id);

				var studentData = new Dictionary<string, DateTime?>();

				foreach (var lab in labs.Where(f => f.MimeType == "application/vnd.google-apps.folder"))
				{
					var tasks = await _drive.GetFiles(lab.Id);

					var cpp = await _drive.GetEarliestCpp(lab.Id);

					foreach (var task in tasks.Where(f => f.MimeType == "application/octet-stream"))
					{
						studentData[$"{lab.Name}/{task.Name}"] =
							cpp?.CreatedTime?.ToUniversalTime();
					}
				}

				result[student.Name] = studentData;
			}

			return result;
		}
	}
}
