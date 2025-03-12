using Azure.Storage.Blobs;

namespace ShopOnline.Models
{
    public class UploadAzure
    {
        public static BlobServiceClient blobService;

        public async Task<string> UploadBlobAsync(string containerName, string blobName, Stream Imgam)
        {
            //Stream stream = new MemoryStream(Imgam);
            string resX = "algo";

            BlobContainerClient blobCont = blobService.GetBlobContainerClient(containerName);
            BlobClient blockBlod = blobCont.GetBlobClient(blobName);
            var resp = await blockBlod.UploadAsync(Imgam, overwrite: true);

            System.Diagnostics.Debug.Write("------------------------------------------" + resp.GetRawResponse);
            return resX;
        }
    }
}
