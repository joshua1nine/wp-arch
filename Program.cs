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
				string? archiveName;
				string? rootDir;
				string? outDir;
				string? tempDir;

				// Set User Args
				switch (args.Length)
				{
					case 1:
						archiveName = args[0];
						Console.Write("Directory to be archived:");
						rootDir = Console.ReadLine();
						Console.Write("Destination directory:");
						outDir = Console.ReadLine();
						tempDir = $"{rootDir}/wpa-temp";
						break;
					case 2:
						archiveName = args[0];
						rootDir = args[1];
						Console.Write("Destination directory:");
						outDir = Console.ReadLine();
						tempDir = $"{rootDir}/wpa-temp";
						break;
					case 3:
						archiveName = args[0];
						rootDir = args[1];
						outDir = args[2];
						tempDir = $"{rootDir}/wpa-temp";
						break;
					default:
						Console.Write("Archive name:");
						archiveName = Console.ReadLine();
						Console.Write("Directory to be archived:");
						rootDir = Console.ReadLine();
						Console.Write("Destination directory:");
						outDir = Console.ReadLine();
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
						if (File.Exists($"{outDir}/{archiveName}_archive.zip"))
						{
							Console.Write($"{outDir}/{archiveName}_archive.zip already exists. Would you like to overwrite? [y/n]: ");
							string? overwrite = Console.ReadLine();
							if (overwrite == "y")
							{
								File.Delete($"{outDir}/{archiveName}_archive.zip");
								ZipFile.CreateFromDirectory(tempDir, $"{outDir}/{archiveName}_archive.zip");
							}
							else if (overwrite == "n")
							{
								Environment.Exit(1);
							}
						}
						else
						{
							ZipFile.CreateFromDirectory(tempDir, $"{outDir}/{archiveName}_archive.zip");
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


