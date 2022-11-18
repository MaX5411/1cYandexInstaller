
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
        


        static async Task<FileInfo?> Response()

        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", "OAuth 15f84e936580493cbc6a6832f00e0b29");


                var fileInfo = await client.GetFromJsonAsync<FileInfo>("https://cloud-api.yandex.net/v1/disk/public/resources?public_key=https://disk.yandex.ru/d/L-0dC0sa2VHXiw&fields=file,name,size");
                return fileInfo;
            }
            
        }
        


        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var file = await Response();
            var destinationFilePath = $"D:\\temp\\{file.FileName}";
            
            Console.WriteLine(file.FileName);
            Console.WriteLine(file.DirectLink);
            Console.WriteLine(file.Size);
            var progress = new ProgressBar();
            


            using (var client = new HttpClientDownloadWithProgress(file.DirectLink, destinationFilePath))
            {
                
                    client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                    {

                        progress.Report((double)progressPercentage/100);

                        //Console.WriteLine($"{progressPercentage}% ({totalBytesDownloaded}/{totalFileSize})");
                    };
                

                await client.StartDownload();
                progress.Dispose();
            }
            string Foldername = Path.GetFullPath(@"D:\temp\");
            using (ZipArchive zip = ZipFile.OpenRead($"D:\\temp\\{file.FileName}"))
            {
                foreach(ZipArchiveEntry entry in zip.Entries)
                {
                    string destinationPath = Path.GetFullPath(Path.Combine(Foldername, entry.FullName));
                    Console.WriteLine(Encoding.GetEncoding(entry.FullName));
                    Console.WriteLine(Path.GetFileName(entry.FullName));
                    if (!Directory.Exists(Path.GetDirectoryName(destinationPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    }
                    if (!string.IsNullOrEmpty(Path.GetFileName(destinationPath)))
                        entry.ExtractToFile(destinationPath);
                        
                    
                }
            }

            Console.WriteLine("привет");
          
            while (Console.Read() != 'q'); 


           
            
        }
    }
}