using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MotoNow.Domain.Entities
{
    public sealed class Motorcycle : BaseEntity
    {

        public Motorcycle(string identifier, string plate, string model, int year)
        {
            const int MinYear = 1900;
            int currentYear = DateTime.UtcNow.Year;

            if (string.IsNullOrWhiteSpace(plate)) throw new ArgumentException("Placa obrigatória.");
            if (year < MinYear || year > currentYear)
                throw new ArgumentException("Ano inválido."); Identifier = identifier;
            Plate = plate.Trim().ToUpperInvariant();
            Model = model;
            Year = year;
        }

        public void ChangePlate(string newPlate)
        {
            if (string.IsNullOrWhiteSpace(newPlate))
                throw new ArgumentException("Placa inválida.", nameof(newPlate));

            Plate = newPlate.Trim().ToUpperInvariant();
        }

        public int Year { get; set; }
        public string Model { get; set; } = default!;
        public string Plate { get; set; } = default!;
    }
}
