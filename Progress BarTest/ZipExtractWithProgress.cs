using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress_BarTest
{
    internal class ZipExtractWithProgress : IDisposable
    {   
        private readonly string? _zipFileName;
        private readonly string? _directoryToExtract;
        private readonly string? _folderName;

        private ZipArchive? zip;

        public delegate void ProgressChangedHandler(double? progressPercentage);

        public event ProgressChangedHandler ProgressChanged;

        public ZipExtractWithProgress(string? zipFileName, string? directoryToExtract, string folderName)
        {
            _zipFileName = zipFileName;
            _directoryToExtract = directoryToExtract;
            _folderName = folderName;
        }

        public async Task Unzip() 
        {
            await Task.Run(() =>
            {
                int? arhiveTotalElementCount = 0;
                int arhiveCurrentElemetIndex = 0;
                string? pathToExtract;
                zip = ZipFile.OpenRead(_zipFileName);

                if (!Directory.Exists(_directoryToExtract))
                {
                    Directory.CreateDirectory(_directoryToExtract);

                }
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    pathToExtract = Path.GetFullPath(Path.Combine(_folderName, entry.FullName));
                    if (!string.IsNullOrEmpty(Path.GetFileName(pathToExtract)))
                            arhiveTotalElementCount++;

                }
                foreach(ZipArchiveEntry entry in zip.Entries) 
                {
                    pathToExtract = Path.GetFullPath(Path.Combine(_folderName, entry.FullName));
                    if (!string.IsNullOrEmpty(Path.GetFileName(pathToExtract)))
                    {   
                        arhiveCurrentElemetIndex++;
                        entry.ExtractToFile(pathToExtract);
                        //Console.WriteLine(arhiveCurrentElemetIndex +"//"+arhiveTotalElementCount);
                        TriggerProgressChanged(arhiveTotalElementCount, arhiveCurrentElemetIndex);
                        //Thread.Sleep(1000);
                    }
                }


            });
        }

        private void TriggerProgressChanged(int? totalFileIndex, int curentFileIndex) 
        {
            if (ProgressChanged == null)
                return;

            double? progressPercentage = null;
            
                progressPercentage = (curentFileIndex * 100)/totalFileIndex;

            ProgressChanged(progressPercentage);

        }

        public void Dispose() 
        {
            zip.Dispose();   
        }
    }
}
