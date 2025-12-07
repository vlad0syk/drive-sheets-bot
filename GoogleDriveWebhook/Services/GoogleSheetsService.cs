using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleDriveWebhook.Models;

namespace GoogleDriveWebhook.Services
{
	public class GoogleSheetsService
	{
		private readonly SheetsService _sheets;

		public GoogleSheetsService(GoogleSettings config)
		{
			var credential = GoogleCredential
				.FromFile(config.CredentialsPath)
				.CreateScoped(SheetsService.Scope.Spreadsheets);

			_sheets = new SheetsService(new BaseClientService.Initializer
			{
				HttpClientInitializer = credential,
				ApplicationName = "DeadlineChecker"
			});
		}

		public async Task UpdateFormula(
			string spreadsheetId,
			string range,
			string formula)
		{
			try {
				var body = new ValueRange
				{
					Values = new List<IList<object>>
			{
				new List<object> { formula }
			}
				};

				var request = _sheets.Spreadsheets.Values.Update(
					body, spreadsheetId, range);

				request.ValueInputOption =
					SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

				await request.ExecuteAsync();
			}
			catch(Exception ex) { 
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
