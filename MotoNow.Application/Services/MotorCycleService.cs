using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MotoNow.Application.Abstractions.Messages;
using MotoNow.Application.DTOs;
using MotoNow.Domain.Entities;
using MotoNow.Domain.Repositories;

namespace MotoNow.Application.Services
{

    public interface IMotorCycleService
    {
        Task<CreateMotorCycleDto> CreateAsync(string identifier, string placa, string modelo, int ano, CancellationToken ct = default);
        Task<string> DeleteMotorcycleAsync(string identifier, CancellationToken ct = default);
        Task<CreateMotorCycleDto> GetByIdAsync(string identifier, CancellationToken ct = default);
        Task<Motorcycle> UpdateMotorcyclePlate(string identifier, string plate, CancellationToken ct = default);
        Task<List<Motorcycle>> GetAllAsync(CancellationToken ct = default);
        Task<List<CreateMotorCycleDto>> ListMotorcycleAsync(string? plate, CancellationToken ct = default);
        Task<bool> isRegistered(string id, CancellationToken ct = default);

    }

    public class MotorCycleService : IMotorCycleService
    {
        private readonly IPublishEndpoint _publish;

        private readonly IRepository<Motorcycle> _repoMotor;
        private readonly IRepository<Rental> _repoRental;
        private readonly IMapper _mapper;

        public MotorCycleService(IRepository<Motorcycle> repoMotor, IPublishEndpoint publish, IRepository<Rental> repoRental, IMapper mapper)
        {
            _repoMotor = repoMotor;
            _publish = publish;
            _repoRental = repoRental;
            _mapper = mapper;
        }

        public async Task<bool> isRegistered(string id, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Identificador da moto é obrigatorio!");
            var driver = await _repoMotor.GetByIdAsync(id);

            return driver != null;
        }

        public async Task<CreateMotorCycleDto> CreateAsync(string identifier, string plate, string model, int year, CancellationToken ct = default)
        {
            if (await _repoMotor.AnyAsync(m => m.Plate == plate, ct))
                throw new InvalidOperationException("Já existe moto com essa placa.");

            var moto = new Motorcycle(identifier, plate, model, year);

            await _repoMotor.AddAsync(moto, ct);
            await _repoMotor.SaveChangesAsync(ct);

            await _publish.Publish(new MotorcycleRegisteredEvent
            {
                Identifier = moto.Identifier,
                Plate = moto.Plate,
                Model = moto.Model,
                Year = moto.Year,
                OccurredAt = DateTime.UtcNow
            }, ct);

            return _mapper.Map<CreateMotorCycleDto>(moto);
        }

        public async Task<string> DeleteMotorcycleAsync(string identifier, CancellationToken ct = default)
        {
            var motorcycle = await _repoMotor.GetByIdAsync(identifier) ?? throw new KeyNotFoundException("Moto não se encontra cadastrada no banco de dados");

            var IsRented = await _repoRental.AnyAsync(r => r.MotorcycleId == identifier);

            if (IsRented) throw new InvalidOperationException("Não foi possivel remover, moto alugada");

            _repoMotor.Remove(motorcycle);

            await _repoMotor.SaveChangesAsync();

            return $"Moto com o identificador {motorcycle.Identifier} e com a placa {motorcycle.Plate} removido com sucesso!";
        }

        public async Task<CreateMotorCycleDto> GetByIdAsync(string identifier, CancellationToken ct = default)
        {
            Motorcycle motorcycle = await _repoMotor.GetByIdAsync(identifier) ?? throw new KeyNotFoundException("Não existe uma moto cadastrada com este Identificador.");
            return _mapper.Map<CreateMotorCycleDto>(motorcycle);
        }

        public async Task<List<Motorcycle>> GetAllAsync(CancellationToken ct = default)
        {
            List<Motorcycle> motorcycles = await _repoMotor.ListAsync() ?? throw new KeyNotFoundException("Não existe motos cadastradas no sistema.");
            return motorcycles;
        }

        public async Task<List<CreateMotorCycleDto>> ListMotorcycleAsync(string? plate, CancellationToken ct = default)
        {
            List<Motorcycle> items;

            if (string.IsNullOrWhiteSpace(plate))
                items = await _repoMotor.ListAsync();
            else
                items = await _repoMotor.ListAsync(m => m.Plate == plate); 


            await _repoMotor.ListAsync(r => r.Plate == plate);

            return _mapper.Map<List<CreateMotorCycleDto>>(items);
        }

        public async Task<Motorcycle> UpdateMotorcyclePlate(string identifier, string plate, CancellationToken ct = default)
        {
            Motorcycle motorcycle = await _repoMotor.GetByIdAsync(identifier) ?? throw new KeyNotFoundException("Não existe uma moto cadastrada com este Identificador.");

            var normalized = plate?.Trim().ToUpperInvariant()
                             ?? throw new ArgumentException("Placa é obrigatória.", nameof(plate));

            if (await _repoMotor.AnyAsync(m => m.Plate == normalized && m.Identifier != motorcycle.Identifier, ct))
                throw new InvalidOperationException("Já existe moto com essa placa.");

            motorcycle.ChangePlate(normalized); 
            _repoMotor.Update(motorcycle);
            await _repoMotor.SaveChangesAsync(ct);

            return motorcycle;
        }
    }
}
