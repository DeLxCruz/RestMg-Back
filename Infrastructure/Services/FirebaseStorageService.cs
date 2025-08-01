// Infrastructure/Services/FirebaseStorageService.cs
using Application.Common.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1; // <-- Necesario para StorageClient y UploadObjectOptions
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FirebaseStorageService(IConfiguration configuration, GoogleCredential credential) : IFileStorageService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly StorageClient _storageClient = StorageClient.Create(credential);

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
        {
            var bucketName = _configuration["Firebase:BucketName"]
                ?? throw new InvalidOperationException("Firebase:BucketName no está configurado.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException($"La extensión del archivo '{fileName}' no está permitida.");
            }

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var objectName = $"uploads/{uniqueFileName}";
            var contentType = GetContentType(fileExtension);


            // 1. Crear las opciones de subida.
            var uploadOptions = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            };

            // 2. Subir el archivo, pasando el objeto de opciones.
            var storageObject = await _storageClient.UploadObjectAsync(
                bucketName,
                objectName,
                contentType,
                fileStream,
                uploadOptions
            );

            // 3. Devolver la URL pública.
            return storageObject.MediaLink;
        }

        private static string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}