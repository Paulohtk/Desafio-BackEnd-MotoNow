using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoNow.Domain.Entities
{
    public sealed class DeliveryDriver : BaseEntity
    {
        private DeliveryDriver() { }

        public DeliveryDriver(
            string identifier,
            string name,
            string cnpj,
            DateTime birthDate,
            string driverLicenseNumber,
            string driverLicenseClass)
        {
            Identifier = string.IsNullOrWhiteSpace(identifier) ? throw new ArgumentException("Invalid identifier.") : identifier.Trim();
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Invalid name.") : name.Trim();
            Cnpj = string.IsNullOrWhiteSpace(cnpj) ? throw new ArgumentException("Invalid CNPJ.") : cnpj.Trim();
            BirthDate = birthDate;
            DriverLicenseNumber = string.IsNullOrWhiteSpace(driverLicenseNumber) ? throw new ArgumentException("Invalid driver license number.") : driverLicenseNumber.Trim();
            DriverLicenseClass = string.IsNullOrWhiteSpace(driverLicenseClass) ? throw new ArgumentException("Invalid driver license class.") : driverLicenseClass.Trim().ToUpperInvariant();
        }

        public string Name { get; private set; }
        public string Cnpj { get; private set; }
        public DateTime BirthDate { get; private set; }
        public string? DriverLicenseImageUrl { get; private set; }
        public string DriverLicenseNumber { get; private set; } 
        public string DriverLicenseClass { get; private set; }
        public void SetDriverLicenseImageUrl(string? url) => DriverLicenseImageUrl = url;
    }
}
