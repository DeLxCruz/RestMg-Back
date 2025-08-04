namespace Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Guarda un archivo y devuelve la URL pública para acceder a él.
        /// </summary>
        /// <param name="fileStream">El contenido del archivo.</param>
        /// <param name="fileName">El nombre original del archivo, para obtener su extensión.</param>
        /// <returns>La URL pública del archivo guardado.</returns>
        Task<string> SaveFileAsync(Stream fileStream, string fileName, Guid restaurantId);
    }
}