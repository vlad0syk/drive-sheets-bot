namespace GoogleDriveWebhook.Services
{
	public class DeadlineUpdater
	{
		private readonly GoogleSheetsService _sheets;

		public DeadlineUpdater(GoogleSheetsService sheets)
		{
			_sheets = sheets;
		}

		public async Task UpdateStudent(
			string spreadsheetId,
			string sheetName,
			int row,
			string column,
			DateTime date)
		{
			try
			{
				string dateText = date.ToString("dd/MM/yyyy");
				string formula = $"=DAYS($C$1; \"{dateText}\")/7";

				//string range = $"{sheetName}!{column}{row}";

				string range = $"Аркуш1!{column}{row}";

				await _sheets.UpdateFormula(spreadsheetId, range, formula);
			}
			catch (Exception ex)
			{
				Console.Write(ex.ToString());
			}
		}
	}
}
