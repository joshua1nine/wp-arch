public class DBCredentials
{
	public string DBName { get; set; } = string.Empty;
	public string DBUser { get; set; } = string.Empty;
	public string DBPass { get; set; } = string.Empty;
	public string DBHost { get; set; } = string.Empty;

	public void GetDBInfo(string rootDir)
	{
		string[] lines = System.IO.File.ReadAllLines($"{rootDir}/wp-config.php");

		// Display the file contents by using a foreach loop.
		foreach (string line in lines)
		{
			// Use a tab to indent each line of the file.
			if (line.Contains("DB_NAME"))
			{
				string[] subs = line.Split('\'', '\'', StringSplitOptions.RemoveEmptyEntries);
				DBName = subs[3];

			}
			if (line.Contains("DB_USER"))
			{
				string[] subs = line.Split('\'', '\'', StringSplitOptions.RemoveEmptyEntries);
				DBUser = subs[3];

			}
			if (line.Contains("DB_PASSWORD"))
			{
				string[] subs = line.Split('\'', '\'', StringSplitOptions.RemoveEmptyEntries);
				DBPass = subs[3];

			}
			if (line.Contains("DB_HOST"))
			{
				string[] subs = line.Split('\'', '\'', StringSplitOptions.RemoveEmptyEntries);
				DBHost = subs[3];

			}
		}
	}
}

