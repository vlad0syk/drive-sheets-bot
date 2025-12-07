namespace GoogleDriveWebhook.Models
{
	public class GoogleSettings
	{
		public string CredentialsPath { get; set; } = "";
		public string TokensDirectory { get; set; } = "";
		public List<GoogleGroup> Groups { get; set; } = new();
	}
}
