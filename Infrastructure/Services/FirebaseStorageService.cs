using Application.Common.Interfaces;
using Firebase.Storage;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FirebaseStorageService(IConfiguration configuration) : IFileStorageService
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
        {
            var bucketName = _configuration["Firebase:BucketName"]
                ?? throw new InvalidOperationException("Firebase:BucketName no está configurado.");

            var serviceAccountJsonPath = _configuration["Firebase:AdminSdkPath"]
                ?? throw new InvalidOperationException("Firebase:AdminSdkPath no está configurado.");

            // 1. Cargar las credenciales desde el archivo JSON de la cuenta de servicio
            GoogleCredential credential;
            await using (var stream = new FileStream(serviceAccountJsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream);
            }

            // 2. Obtener el token de acceso (se refresca automáticamente si es necesario)
            var oauthToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            // 3. Generar un nombre de archivo único
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

            // 4. Crear la tarea de almacenamiento con el token de autenticación
            var task = new FirebaseStorage(
                bucketName,
                new FirebaseStorageOptions
                {
                    // El token de la cuenta de servicio se pasa aquí
                    AuthTokenAsyncFactory = () => Task.FromResult(oauthToken),
                    ThrowOnCancel = true
                })
                .Child("uploads") // Carpeta dentro del bucket
                .Child(uniqueFileName)
                .PutAsync(fileStream, CancellationToken.None);

            // 5. Esperar la subida y devolver la URL de descarga
            var downloadUrl = await task;
            return downloadUrl;
        }
    }
}