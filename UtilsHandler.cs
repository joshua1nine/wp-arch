using System.Diagnostics;
using ShellProgressBar;

class UtilsHandler
{
	public static bool ConfirmWP(string rootDir)
	{

		bool valid;
		// Determine if the root directory is a WordPress root directory.
		var files = Directory.GetFiles(rootDir, "wp-config.php", SearchOption.TopDirectoryOnly);

		if (files.Length > 0)
		{
			valid = true;
		}
		else
		{
			valid = false;
		}

		return valid;
	}

	public static void BackupDatabase(string MySqlDefaultsFilePath, string DBName, string localDatabasePath)
	{
		var process = new Process();
		var startInfo = new ProcessStartInfo();
		startInfo.FileName = "mysqldump";
		startInfo.Arguments = $@"--defaults-file=""{MySqlDefaultsFilePath}"" --no-tablespaces {DBName} -r {localDatabasePath}";
		startInfo.CreateNoWindow = true;
		startInfo.UseShellExecute = false;
		process.StartInfo = startInfo;
		process.Start();
		process.WaitForExit();
		process.Close();
	}

	public static void CreateMySqlDefaults(string DBUser, string DBPass, string tempDir)
	{
		string path = $"{tempDir}/MysqlDefaults.ini";
		if (!File.Exists(path))
		{
			// Create a file to write to.
			using (StreamWriter sw = File.CreateText(path))
			{
				sw.WriteLine("[mysqldump]");
				sw.WriteLine($"user={DBUser}");
				sw.WriteLine($"password={DBPass}");
			}
		}

	}
	public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
	{
		// Get information about the source directory
		var dir = new DirectoryInfo(sourceDir);

		// Check if the source directory exists
		if (!dir.Exists)
			throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

		// Cache directories before we start copying
		DirectoryInfo[] dirs = dir.GetDirectories();

		// Create the destination directory
		Directory.CreateDirectory(destinationDir);

		// Get the files in the source directory and copy to the destination directory
		foreach (FileInfo file in dir.GetFiles())
		{
			string targetFilePath = Path.Combine(destinationDir, file.Name);
			FileInfo newfi = file.CopyTo(targetFilePath);
			Console.WriteLine(newfi.FullName);
		}

		// If recursive and copying subdirectories, recursively call this method
		if (recursive)
		{
			foreach (DirectoryInfo subDir in dirs)
			{
				string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
				CopyDirectory(subDir.FullName, newDestinationDir, true);
			}
		}
	}

}
