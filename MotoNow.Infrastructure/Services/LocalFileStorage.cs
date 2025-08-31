using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MotoNow.Application.Abstractions;

namespace MotoNow.Infrastructure.Services
{
    public sealed class LocalFileStorage : IFileStorage
    {
        private readonly string _basePath;

        public LocalFileStorage(IConfiguration cfg)
        {
            var configured = cfg["Storage:Local:BasePath"];

            var downloads = GetDownloadsFolderPath(); 
            var defaultBase = Path.Combine(downloads, "fotos_cnh");

            _basePath = string.IsNullOrWhiteSpace(configured) ? defaultBase : configured;
            Directory.CreateDirectory(_basePath);
        }
        public Task DeleteAsync(string path, CancellationToken ct = default)
        {
            if (File.Exists(path))
                File.Delete(path);
            return Task.CompletedTask;
        }

        public async Task<string> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
        {
            if (!IsAllowed(contentType, fileName))
                throw new InvalidOperationException("Somente imagens PNG ou BMP são permitidas.");

            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(ext)) ext = GuessExtension(contentType);

            var safeName = SanitizeFileName(Path.GetFileNameWithoutExtension(fileName));
            var finalName = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}_{safeName}{ext.ToLowerInvariant()}";
            var fullPath = Path.Combine(_basePath, finalName);

            using var fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 64 * 1024, useAsync: true);
            await stream.CopyToAsync(fs, ct);

            return fullPath; 
        }

        private static bool IsAllowed(string contentType, string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return contentType is "image/png" or "image/bmp"
                || ext is ".png" or ".bmp";
        }

        private static string GuessExtension(string contentType) => contentType switch
        {
            "image/png" => ".png",
            "image/bmp" => ".bmp",
            _ => ".bin"
        };

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return string.IsNullOrWhiteSpace(name) ? "arquivo" : name;
        }

        private static string GetDownloadsFolderPath()
        {
            try
            {
                var downloads = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var dir = Path.Combine(downloads, "Downloads");
                return Directory.Exists(dir) ? dir : Path.Combine(Directory.GetCurrentDirectory(), "Downloads");

            }
            catch
            {
                return Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
            }
        }
    }
}
