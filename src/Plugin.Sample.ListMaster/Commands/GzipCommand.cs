namespace Plugin.Sample.ListMaster.Commands
{
    using System;
    using System.IO;
    using System.IO.Compression;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class GzipCommand : CommerceCommand
    {
        public GzipCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void Compress(CommerceContext commerceContext, DirectoryInfo directorySelected)
        {
            var directoryPath = @"C:\Users\kha\Documents\ExportedEntities";

            foreach (var fileToCompress in directorySelected.GetFiles())
            {
                var info = new FileInfo(directoryPath + "\\" + fileToCompress.Name + ".gz");
                using (var originalFileStream = fileToCompress.OpenRead())
                {
                    if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                    {
                        using (var compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                        {
                            using (var compressionStream = new GZipStream(compressedFileStream,
                                CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);
                            }
                        }

                        commerceContext.Logger.LogInformation("Compressed {0} from {1} to {2} bytes.",
                            fileToCompress.Name, fileToCompress.Length.ToString(), info.Length.ToString());
                    }
                }
            }
        }
    }
}