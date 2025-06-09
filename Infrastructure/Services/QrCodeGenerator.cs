using Application.Common.Interfaces;
using QRCoder;

namespace Infrastructure.Services
{
    public class QrCodeGenerator : IQrCodeGenerator
    {
        public byte[] Generate(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            byte[] qrCodeImage = qrCode.GetGraphic(20); // Tamaño de los píxeles

            return qrCodeImage;
        }
    }
}