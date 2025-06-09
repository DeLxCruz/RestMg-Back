namespace Application.Common.Interfaces
{
    public interface IQrCodeGenerator
    {
        /// <summary>
        /// Genera un código QR a partir de un texto y lo devuelve como un array de bytes de una imagen PNG.
        /// </summary>
        /// <param name="text">El texto a codificar en el QR.</param>
        /// <returns>Un array de bytes que representa la imagen PNG del código QR.</returns>
        byte[] Generate(string text);
    }
}