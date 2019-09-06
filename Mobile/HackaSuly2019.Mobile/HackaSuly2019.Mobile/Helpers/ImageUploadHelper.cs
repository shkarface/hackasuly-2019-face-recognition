using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HackaSuly2019.Mobile.Helpers
{
    public static class ImageUploadHelper
    {
        private const string ContainerName = "finder";

        private static string _connectionString;
        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    // NOTE: For this to work, you have have a config.txt file at the root of the .net standard
                    // project. With a content like this:
                    // ConnectionString:DefaultEndpointsProtocol...

                    var lines = GetConfigurationLines();
                    foreach (var line in lines)
                    {
                        var parts = line.Split(':');
                        if (parts.Length == 2 && parts[0].ToLowerInvariant() == "connectionstring")
                        {
                            _connectionString = parts[1].Trim();
                        }
                    }

                    if (string.IsNullOrWhiteSpace(_connectionString))
                    {
                        throw new ArgumentNullException("You have to specify your own Azure Storage connection string!");
                    }
                }

                return _connectionString;
            }
        }

        public static string[] GetConfigurationLines()
        {
            using (var stream = typeof(ImageUploadHelper)
                        .Assembly.GetManifestResourceStream("HackaSuly2019.Mobile.config.txt"))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public static async Task<CloudBlockBlob> GetOrCreateFileAsync(string root, string name)
        {
            if (string.IsNullOrWhiteSpace(root))
                throw new ArgumentNullException(nameof(root));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var container = await GetContainerAsync(root);

            var blob = container.GetBlockBlobReference(name);
            blob.Properties.ContentType = "image/jpg";

            return blob;
        }

        public static async Task<Uri> UploadFileAsync(Stream file)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));

            var guid = Guid.NewGuid();

            var blob = await GetOrCreateFileAsync(ContainerName, guid.ToString() + ".jpg");
            await blob.UploadFromStreamAsync(file);

            return blob.Uri;
        }

        private static async Task<CloudBlobContainer> GetContainerAsync(string name)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(ConnectionString);
            var client = cloudStorageAccount.CreateCloudBlobClient();

            var container = client.GetContainerReference(name);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);
            return container;
        }
    }
}
