using log4net;
using log4net.Config;
using System;
//using System.Collections.Generic;
using System.Configuration;
using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace ToDeleteFile
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        static void Main()
        {
            XmlConfigurator.Configure();

            try
            {
                log.Info("Utility Started successfully.");
                string directoryPath = ConfigurationManager.AppSettings["DirectoryPath"];
                string[] fileExtensions = ConfigurationManager.AppSettings["FileExtensions"].Split(',');
                bool includeSubdirectories = Convert.ToBoolean(ConfigurationManager.AppSettings["IncludeSubdirectories"]);

                DeleteFiles(directoryPath, fileExtensions, includeSubdirectories);
                log.Info("Utility completed successfully.");
            }
            catch (Exception ex)
            {
                log.Error($"An error occurred: {ex.Message}", ex);
            }
        }

        static void DeleteFiles(string directoryPath, string[] fileExtensions, bool includeSubdirectories)
        {
            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");
            }

            try
            {
                // Get files based on file extensions and search option
                SearchOption searchOption = includeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                foreach (string extension in fileExtensions)
                {
                    string[] filesToDelete = Directory.GetFiles(directoryPath, $"*{extension}", searchOption);

                    // Delete each file
                    foreach (string filePath in filesToDelete)
                    {
                        // Get the file size as a string
                        string fileSize = GetFileSize(filePath);

                        // Set the file size property
                        GlobalContext.Properties["FileSize"] = fileSize;

                        // Log the deletion
                        log.Info($"Deleting file: {filePath}, Size: {fileSize}");

                        File.Delete(filePath);
                        log.Info($"Deleted: {filePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error deleting files: {ex.Message}", ex);
                throw new Exception($"Error deleting files: {ex.Message}", ex);
            }
        }

        //static string GetFileSize(string filePath)
        //{
        //    // Logic to get the file size as a string
        //    if (File.Exists(filePath))
        //    {
        //        return new FileInfo(filePath).Length.ToString();
        //    }

        //    return "File not found";
        //}
        static string GetFileSize(string filePath)
        {
            if (File.Exists(filePath))
            {
                long fileSizeInBytes = new FileInfo(filePath).Length;

                // Convert bytes to KB, MB, or GB
                double fileSizeInKB = fileSizeInBytes / 1024.0;
                double fileSizeInMB = fileSizeInKB / 1024.0;
                double fileSizeInGB = fileSizeInMB / 1024.0;

                if (fileSizeInGB >= 1.0)
                {
                    return $"{fileSizeInGB:F2} GB";
                }
                else if (fileSizeInMB >= 1.0)
                {
                    return $"{fileSizeInMB:F2} MB";
                }
                else if (fileSizeInKB >= 1.0)
                {
                    return $"{fileSizeInKB:F2} KB";
                }
                else
                {
                    return $"{fileSizeInBytes} Bytes";
                }
            }
            else
            {
                return "File not found";
            }
        }

    }
}
