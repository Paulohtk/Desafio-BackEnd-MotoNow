using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoNow.Application.Abstractions
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default);
        Task DeleteAsync(string path, CancellationToken ct = default);
    }
}
