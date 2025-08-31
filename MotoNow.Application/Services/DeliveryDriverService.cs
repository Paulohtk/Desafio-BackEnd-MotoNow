using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoNow.Application.Abstractions;
using MotoNow.Application.DTOs;
using MotoNow.Domain.Entities;
using MotoNow.Domain.Repositories;

namespace MotoNow.Application.Services
{
    public interface IDeliveryDriverService
    {
        Task<string> CreateAsync(string identifier, string name, string cnpj, DateTime birthDate,
                           string driverLicenseNumber, string driverLicenseClass, string base64,
                           CancellationToken ct = default);

        Task<string> UpdateDriverLicense(string id, UploadCnhBase64Dto body, CancellationToken ct = default);

        Task UploadDriverLicenseImageAsync(string driverId, Stream file, string fileName, string contentType, CancellationToken ct = default);

        Task<bool> HasCategoryAAsync(string driverId, CancellationToken ct = default);

        Task<bool> isDriverRegistered(string driverId, CancellationToken ct = default);

    }

    public sealed class DeliveryDriverService : IDeliveryDriverService
    {
        private readonly IRepository<DeliveryDriver> _drivers;
        private readonly IFileStorage _storage;

        public DeliveryDriverService(IRepository<DeliveryDriver> drivers, IFileStorage storage)
        {
            _drivers = drivers;
            _storage = storage;
        }

        public async Task<bool> isDriverRegistered(string driverId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(driverId)) throw new ArgumentException("Identificador do entregador é obrigatorio!");
            var driver = await _drivers.GetByIdAsync(driverId);

            return driver != null;
        }


        public async Task UploadDriverLicenseImageAsync(string driverId, Stream file, string fileName, string contentType, CancellationToken ct = default)
        {
            var driver = await _drivers.GetByIdAsync(driverId, ct)
                ?? throw new KeyNotFoundException("Entregador não encontrado.");

            if (!string.IsNullOrWhiteSpace(driver.DriverLicenseImageUrl))
            {
                await _storage.DeleteAsync(driver.DriverLicenseImageUrl, ct);
            }

            var savedPath = await _storage.SaveAsync(file, fileName, contentType, ct);

            driver.SetDriverLicenseImageUrl(savedPath);
            _drivers.Update(driver);
            await _drivers.SaveChangesAsync(ct);
        }


        public async Task<string> UpdateDriverLicense(string id, UploadCnhBase64Dto body, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(body.Base64))
                throw new ArgumentException("imagem_cnh é obrigatória.");

            if (!TryDecodeBase64Image(body.Base64, out var bytes, out var contentType, out var extension, out var error))
                throw new ArgumentException(error);

            await using var ms = new MemoryStream(bytes, writable: false);
            var fileName = $"cnh_{id}{extension}";
            await UploadDriverLicenseImageAsync(id, ms, fileName, contentType, ct);

            return "CNH atualizada com sucesso!";
        }


        public async Task<string> CreateAsync(
        string identifier,
        string name,
        string cnpj,
        DateTime birthDate,
        string driverLicenseNumber,
        string driverLicenseClass,
        string base64,
        CancellationToken ct = default)
        {
            identifier = identifier?.Trim() ?? throw new ArgumentNullException(nameof(identifier));
            name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
            var cnpjDigits = DigitsOnly(cnpj);
            var cnhDigits = DigitsOnly(driverLicenseNumber);
            var licenseClass = NormalizeLicenseClass(driverLicenseClass);

            if (identifier.Length == 0) throw new ArgumentException("Identificador é obrigatório.", nameof(identifier));
            if (name.Length == 0) throw new ArgumentException("Nome é obrigatório.", nameof(name));
            if (cnpjDigits.Length != 14) throw new ArgumentException("CNPJ deve conter 14 dígitos.", nameof(cnpj));
            if (cnhDigits.Length != 11) throw new ArgumentException("Número da CNH deve conter 11 dígitos.", nameof(driverLicenseNumber));
            if (birthDate == default) throw new ArgumentException("Data de nascimento inválida.", nameof(birthDate));
            if (birthDate > DateTime.UtcNow.Date) throw new ArgumentException("Data de nascimento no futuro.", nameof(birthDate));

            if (await _drivers.AnyAsync(d => d.Identifier == identifier, ct))
                throw new InvalidOperationException("Já existe entregador com este identificador.");

            if (await _drivers.AnyAsync(d => d.Cnpj == cnpjDigits, ct))
                throw new InvalidOperationException("Já existe entregador com este CNPJ.");

            if (await _drivers.AnyAsync(d => d.DriverLicenseNumber == cnhDigits, ct))
                throw new InvalidOperationException("Já existe entregador com este número de CNH.");

            if (string.IsNullOrWhiteSpace(base64))
                throw new ArgumentException("imagem_cnh é obrigatória.");

            if (!TryDecodeBase64Image(base64, out var bytes, out var contentType, out var extension, out var error))
                throw new ArgumentException(error);

            await using var ms = new MemoryStream(bytes, writable: false);
            var fileName = $"cnh_{identifier}{extension}";

            var driver = new DeliveryDriver(
                identifier: identifier,
                name: name,
                cnpj: cnpjDigits,
                birthDate: birthDate,
                driverLicenseNumber: cnhDigits,
                driverLicenseClass: licenseClass
            );

            var savedPath = await _storage.SaveAsync(ms, fileName, contentType, ct);

            driver.SetDriverLicenseImageUrl(savedPath);

            await _drivers.AddAsync(driver, ct);
            await _drivers.SaveChangesAsync(ct);

            return driver.Identifier;
        }

        
        private static string DigitsOnly(string s)
        {
            if (s is null) throw new ArgumentNullException(nameof(s));
            var sb = new System.Text.StringBuilder(s.Length);
            foreach (var ch in s)
                if (ch >= '0' && ch <= '9') sb.Append(ch);
            return sb.ToString();
        }

        private static string NormalizeLicenseClass(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) throw new ArgumentException("Tipo de CNH é obrigatório.", nameof(s));
            var norm = s.Trim().ToUpperInvariant();
            if (norm is "AB") norm = "AB";

            return norm switch
            {
                "A" => "A",
                "B" => "B",
                "AB" => "AB",
                _ => throw new ArgumentException("Tipo de CNH inválido. Use A, B ou A+B.")
            };
        }

       private static bool TryDecodeBase64Image(
       string input,
       out byte[] bytes,
       out string contentType,
       out string extension,
       out string error)
        {
            bytes = Array.Empty<byte>();
            contentType = "";
            extension = "";
            error = "";

            try
            {
                string base64Part = input.Trim();

                var commaIdx = base64Part.IndexOf(',');
                if (base64Part.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIdx > 0)
                {
                    var header = base64Part[..commaIdx].ToLowerInvariant();
                    base64Part = base64Part[(commaIdx + 1)..];

                    if (header.Contains("image/png")) { contentType = "image/png"; extension = ".png"; }
                    else if (header.Contains("image/bmp")) { contentType = "image/bmp"; extension = ".bmp"; }
                }

                base64Part = base64Part.Replace("\r", "").Replace("\n", "").Replace(" ", "");

                bytes = Convert.FromBase64String(base64Part);

                if (string.IsNullOrEmpty(contentType))
                {
                    if (IsPng(bytes)) { contentType = "image/png"; extension = ".png"; }
                    else if (IsBmp(bytes)) { contentType = "image/bmp"; extension = ".bmp"; }
                }

                if (contentType is not ("image/png" or "image/bmp"))
                {
                    error = "Apenas PNG ou BMP são permitidos.";
                    return false;
                }

                return true;
            }
            catch (FormatException)
            {
                error = "Base64 inválido.";
                return false;
            }
            catch (Exception ex)
            {
                error = $"Falha ao processar a imagem: {ex.Message}";
                return false;
            }
        }

        private static bool IsPng(byte[] b) =>
            b.Length > 8 &&
            b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47 &&
            b[4] == 0x0D && b[5] == 0x0A && b[6] == 0x1A && b[7] == 0x0A;

        private static bool IsBmp(byte[] b) =>
            b.Length > 2 && b[0] == 0x42 && b[1] == 0x4D;

        public async Task<bool> HasCategoryAAsync(string driverId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(driverId)) throw new ArgumentException("Identificador do entregador é obrigatorio!");
            var driver = await _drivers.GetByIdAsync(driverId) ?? throw new KeyNotFoundException("Entregador não encontrado.");

            return driver.DriverLicenseClass.Contains('A');
        }
    }
}
