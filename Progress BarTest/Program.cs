
using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace Progress_BarTest
{
    internal class Program

    {

        static async Task<FileInfo?> Response(string token, string yandexLink)

        {
            using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromDays(1) })
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", token);


                var fileInfo = await client.GetFromJsonAsync<FileInfo>($"https://cloud-api.yandex.net/v1/disk/public/resources?public_key={yandexLink}=file,name,size");
                return fileInfo;
            }
        }

        static async Task<Setting?> GetSettings(string settingFilePath)
        {
            Setting? setting = new Setting();

            using (FileStream file = new FileStream(settingFilePath, FileMode.Open))
            {
                setting = await JsonSerializer.DeserializeAsync<Setting>(file);
                return setting;


            }

        }
        static void MsiInstall(string fileDirectory, bool IsX86)
        {
            string? fileMsiName;
           
            if (!IsX86)
                fileMsiName = "1CEnterprise 8 Thin client (x86-64).msi";
            else
                fileMsiName = "1CEnterprise 8 Thin client.msi";

            string? msiFilePath = Path.GetFullPath(fileDirectory +"\\"+ fileMsiName);

            if (!File.Exists(msiFilePath))
                throw new Exception($"Установочный фаил {fileMsiName} не найден");
            
            

            Process process = new Process();
            process.StartInfo.FileName = "msiexec.exe";
            process.StartInfo.Arguments = string.Format("/qb /i \"{0}\" TRANSFORMS=1049.mst", Path.GetFullPath(msiFilePath));
            process.Start();
            process.WaitForExit();
            

        }

        static async Task Main(string[] args)
        {
            
            string folderName = Path.GetFullPath($"{Environment.GetEnvironmentVariable("temp")}\\tmp1C\\");
            
            try
            {
                string fileSettingPath = Path.GetFullPath(@"\\192.168.1.200\Install\yandex64\setting.json");
                Setting? setting = await GetSettings(fileSettingPath);
                FileInfo? file = await Response(setting.Token, setting.Link);
                string archivePath = Path.GetFullPath($"{folderName}{file.FileName}");
                string directoryToExtract = Path.GetFullPath($"{folderName}{file.FileName}".Replace(".zip", ""));


                if (!File.Exists(fileSettingPath))
                    throw new Exception($"Фаил setting.json ненайден");

                
                string? oneSdirectory;
                if (!setting.IsX86)
                    oneSdirectory = "C:\\Program Files\\1cv8\\";
                else  
                   oneSdirectory = "C:\\Program Files (x86)\\1cv8\\";



                Directory.CreateDirectory(folderName);
                //Console.WriteLine(setting.IsX86.ToString());
                //Console.WriteLine(oneSdirectory);


                string destinationFilePathToDownload = $"{folderName}{file.FileName}";
                string? destinationPathToExtract;

                if (Directory.Exists(Path.GetFullPath(oneSdirectory + setting.Version)))
                                    throw new Exception("Данная версия 1с уже установлена");



                Console.WriteLine("Идет загрузка не закрывайте окно.");
                var progress = new ProgressBar();
                using (var client = new HttpClientDownloadWithProgress(file.DirectLink, destinationFilePathToDownload))
                {

                    client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                    {

                        progress.Report((double)progressPercentage / 100);

                    };


                    await client.StartDownload();
                    progress.Dispose();
                }

                /*using (ZipArchive zip = ZipFile.OpenRead($"{folderName}{file.FileName}"))
                {
                    if (!Directory.Exists(directoryToExtract))
                    {
                        Directory.CreateDirectory(directoryToExtract);

                    }               
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {

                        destinationPathToExtract = Path.GetFullPath(Path.Combine(folderName, entry.FullName));
                        Console.WriteLine(Path.GetFileName(entry.FullName));

                        if (!string.IsNullOrEmpty(Path.GetFileName(destinationPathToExtract)))
                            entry.ExtractToFile(destinationPathToExtract);


                    }
                }*/
                Console.WriteLine();
                var progresszip = new ProgressBar();
                using (var zip = new ZipExtractWithProgress(archivePath, directoryToExtract, folderName))
                {
                    zip.ProgressChanged += (progressPercentage) =>
                    {
                        progresszip.Report((double)progressPercentage / 100);
                    };

                    await zip.Unzip();
                    progresszip.Dispose();
                }

                Console.WriteLine("Идет установка не закрывайте окно.");
                MsiInstall(directoryToExtract, setting.IsX86);

                if (Directory.Exists(folderName))
                    Directory.Delete(folderName, true);

                Console.WriteLine("Установка завершена нажмите любую клавишу для завершения");
                Console.ReadKey();
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Ошибка: {ex.Message}\n");

                if (Directory.Exists(folderName))
                    Directory.Delete(folderName, true);

                Console.WriteLine("Нажмите любую клавишу для завершения");
                Console.ReadKey();

            }




            }
    }
}
