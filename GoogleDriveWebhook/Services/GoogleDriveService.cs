using GoogleDriveWebhook.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace GoogleDriveWebhook.Services;

public class GoogleDriveService
{
	private readonly DriveService _drive;

	public GoogleDriveService(GoogleSettings config)
	{
		var credential = GoogleCredential
			.FromFile(config.CredentialsPath)
			.CreateScoped(DriveService.ScopeConstants.Drive);

		_drive = new DriveService(new BaseClientService.Initializer
		{
			HttpClientInitializer = credential,
			ApplicationName = "DeadlineChecker"
		});
	}

	public async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFiles(string folderId)
	{
		var request = _drive.Files.List();
		request.Q = $"'{folderId}' in parents and trashed = false";
		request.Fields = "files(id, name, mimeType, createdTime)";
		return (await request.ExecuteAsync()).Files;
	}

	public async Task<Google.Apis.Drive.v3.Data.File?> GetEarliestCpp(string folderId)
	{
		var request = _drive.Files.List();
		request.Q = $"'{folderId}' in parents and trashed = false and name contains '.cpp'";
		request.Fields = "files(id, name, createdTime)";

		var files = (await request.ExecuteAsync()).Files;

		return files
			.OrderBy(f => f.CreatedTime)
			.FirstOrDefault();
	}
}
