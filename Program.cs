using System.IO.Compression;
using ShellProgressBar;

namespace ConsoleUI
{
	class Program
	{
		public static void Main(string[] args)
		{
			// Add try catch
			try
			{
				// Define Variables
				string rootDir;
				string outDir;
				string tempDir;

				// Set User Args
				switch (args.Length)
				{
					case 1:
						rootDir = args[0];
						outDir = rootDir;
						tempDir = $"{rootDir}/wpa-temp";
						break;
					case 2:
						rootDir = args[0];
						outDir = args[1];
						tempDir = $"{rootDir}/wpa-temp";
						break;
					default:
						rootDir = ".";
						outDir = rootDir;
						tempDir = $"{rootDir}/wpa-temp";
						break;
				}

				// Check if rootDir and outDir exist
				if (Directory.Exists(rootDir) && Directory.Exists(outDir))
				{

					// Determine if the root directory is a WordPress root directory.
					if (UtilsHandler.ConfirmWP(rootDir))
					{
						// Create Temp Folder
						if (Directory.Exists(tempDir))
						{
							Console.Write($"{tempDir} already exists. Would you like to overwrite it? [y/n]: ");
							string? overwrite = Console.ReadLine();
							if (overwrite == "y")
							{
								Directory.Delete(tempDir, true);
								Directory.CreateDirectory(tempDir);
							}
							else
							{
								Environment.Exit(1);
							}
						}
						else
						{
							Directory.CreateDirectory(tempDir);
						}

						// Grab database and site information from wp-config file.
						DBCredentials dbInfo = new DBCredentials();
						dbInfo.GetDBInfo(rootDir);

						// Create login credentials file for mysql with dbInfo
						UtilsHandler.CreateMySqlDefaults(dbInfo.DBUser, dbInfo.DBPass, tempDir);

						// Export the database in to temp folder.
						UtilsHandler.BackupDatabase($"{tempDir}/MySqlDefaults.ini", dbInfo.DBName, $"{tempDir}/{dbInfo.DBName}_backup.sql");

						// Copy wp-content folder into temp folder.
						UtilsHandler.CopyDirectory($"{rootDir}/wp-content", $"{tempDir}/wp-content", true);

						// Delete MySqlDefaults.ini file
						if (File.Exists($"{tempDir}/MySqlDefaults.ini"))
						{
							File.Delete($"{tempDir}/MySqlDefaults.ini");
						}

						// Zip temp folder and place it in the users chosen directory location.
						if (File.Exists($"{outDir}/{dbInfo.DBName}_archive.zip"))
						{
							Console.Write($"{outDir}/{dbInfo.DBName}_archive.zip already exists. Would you like to overwrite? [y/n]: ");
							string? overwrite = Console.ReadLine();
							if (overwrite == "y")
							{
								File.Delete($"{outDir}/{dbInfo.DBName}_archive.zip");
								ZipFile.CreateFromDirectory(tempDir, $"{outDir}/{dbInfo.DBName}_archive.zip");
							}
							else if (overwrite == "n")
							{
								Environment.Exit(1);
							}
						}
						else
						{
							ZipFile.CreateFromDirectory(tempDir, $"{outDir}/{dbInfo.DBName}_archive.zip");
						}

						// Remove temp folder
						if (Directory.Exists(tempDir))
						{
							Directory.Delete(tempDir, true);
						}
					}
					else
					{
						Console.WriteLine($"{rootDir} is NOT a WordPress Directory.");
					}
				}
				else if (Directory.Exists(rootDir))
				{
					Console.WriteLine($"Could not find {outDir} directory.");
				}
				else
				{
					Console.WriteLine($"Could not find {rootDir} directory.");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				Environment.Exit(1);
			}
		}
	}
}


