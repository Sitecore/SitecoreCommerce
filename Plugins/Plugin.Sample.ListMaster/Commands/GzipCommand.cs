
namespace Plugin.Sample.ListMaster
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.IO.Compression;

    /// <summary>
    /// Defines the JsonCommander command.
    /// </summary>
    public class GzipCommand : CommerceCommand
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="GzipCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        public GzipCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        /// <summary>
        /// Compress using GZip
        /// </summary>
        /// <param name="commerceContext">A CommerceContext</param>
        /// <param name="directorySelected">The selected directory</param>
        public void Compress(CommerceContext commerceContext, DirectoryInfo directorySelected)
        {
            var directoryPath = @"C:\Users\kha\Documents\ExportedEntities";
            //DirectoryInfo directorySelected = new DirectoryInfo(directoryPath);

            foreach (FileInfo fileToCompress in directorySelected.GetFiles())
            {
                using (FileStream originalFileStream = fileToCompress.OpenRead())
                {
                    if ((File.GetAttributes(fileToCompress.FullName) &
                    FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                    {
                        using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                        {
                            using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                            CompressionMode.Compress))
                            {
                                originalFileStream.CopyTo(compressionStream);

                            }
                        }
                        FileInfo info = new FileInfo(directoryPath + "\\" + fileToCompress.Name + ".gz");
                        commerceContext.Logger.LogInformation("Compressed {0} from {1} to {2} bytes.",
                        fileToCompress.Name, fileToCompress.Length.ToString(), info.Length.ToString());
                    }

                }
            }
        }

        /// <summary>
        /// Decompress using Gzip
        /// </summary>
        /// <param name="commerceContext">A CommerceContext</param>
        /// <param name="fileToDecompress"></param>
        public void Decompress(CommerceContext commerceContext, FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        commerceContext.Logger.LogInformation("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }
        }


    }
}