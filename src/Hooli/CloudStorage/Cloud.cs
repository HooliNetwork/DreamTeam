using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNet.Http;

namespace Hooli.CloudStorage
{
    public class Cloud
    {
        public async Task<string> GetUri(string mycontainer, string myblob, IFormFile file)
        {
            IConfigurationSourceRoot config = new Configuration()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
            // Add cloud storage
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                config.Get("StorageConnectionString:ConnectionString"));

            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container named “mycontainer.”
            CloudBlobContainer container = blobClient.GetContainerReference(mycontainer);
            // If “mycontainer” doesn’t exist, create it.
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
            // Get a reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(myblob);

            // Create or overwrite the "myblob" blob with the contents of a local file
            // named “myfile”.
            using (var fileStream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }
            return blockBlob.Uri.ToString();
        }
    }
}
