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
			try
			{
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
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		public async Task<Dictionary<string, int>> GetStudentRows(
		string spreadsheetId,
		string sheetName,
		string nameColumn = "A",
		int startRow = 2)       
		{
			var map = new Dictionary<string, int>();

			var range = $"Аркуш1!{nameColumn}{startRow}:{nameColumn}200";

			try
			{
				var request = _sheets.Spreadsheets.Values.Get(spreadsheetId, range);
				var response = await request.ExecuteAsync();

				if (response.Values != null)
				{
					for (int i = 0; i < response.Values.Count; i++)
					{
						var value = response.Values[i].FirstOrDefault()?.ToString();

						if (!string.IsNullOrWhiteSpace(value))
						{
							string cleanName = value.Trim();
							if (!map.ContainsKey(cleanName))
							{
								map.Add(cleanName, startRow + i);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Помилка при зчитуванні імен: {ex.Message}");
			}

			return map;
		}
	}
}
