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

    public interface IRentalService
    {
        Task<CreateRentalDto> CreateAsync(CreateRentalDto dto, CancellationToken ct = default);
        Task<ReturnResultDto> ReturnAsync(string identifier, DateTime returnDate, CancellationToken ct = default);
        Task<RentalDetailsDto> GetByIdAsync(string identifier, CancellationToken ct = default);

    }

    public class RentalService : IRentalService
    {
        private readonly IPublishEndpoint _publish;

        private readonly IRepository<Rental> _repoRental;
        private readonly IMapper _mapper;
        private readonly IDeliveryDriverService _repoDriver;
        private readonly IMotorCycleService _repoMotor;

        public RentalService(IRepository<Rental> repoRental, IMapper mapper, IDeliveryDriverService drivers, IMotorCycleService repoMotor)
        {
            _repoRental = repoRental;
            _mapper = mapper;
            _repoDriver = drivers;
            _repoMotor = repoMotor;
        }

        public static readonly IReadOnlyDictionary<int, decimal> EarlyReturnPenaltyPercent = new Dictionary<int, decimal>
        {
            [7] = 0.20m,
            [15] = 0.40m,
        };

        public async Task<CreateRentalDto> CreateAsync(CreateRentalDto dto, CancellationToken ct = default)
        {

            var driverExist = await _repoDriver.isDriverRegistered(dto.DeliveryDriverId, ct);
            if (!driverExist) throw new InvalidOperationException("Entregador não registrado no banco de dados.");

            var isMotorcycleExist = await _repoMotor.isRegistered(dto.DeliveryDriverId, ct);
            if (!isMotorcycleExist) throw new InvalidOperationException("Moto não registrada no banco de dados.");

            var okA = await _repoDriver.HasCategoryAAsync(dto.DeliveryDriverId, ct);
            if (!okA) throw new InvalidOperationException("Entregador não habilitado na categoria A.");

            var today = DateTime.Now.Date;
            if (dto.StartAt.Date <= today)
                throw new InvalidOperationException("A data de início deve ser a partir de amanhã.");

            var endAt = dto.StartAt.Date.AddDays(dto.PlanDays - 1).AddHours(23).AddMinutes(59).AddSeconds(59); ;
            var expectedEndAt = dto.StartAt.Date.AddDays(dto.PlanDays - 1).AddHours(23).AddMinutes(59).AddSeconds(59);

            var rental = new Rental(
                identifier: dto.Identifier,
                deliveryDriverId: dto.DeliveryDriverId,
                motorcycleId: dto.MotorcycleId,
                startAt: dto.StartAt.Date,
                expectedEndAt: expectedEndAt,
                planDays: dto.PlanDays,
                endAt: endAt
            );

            await _repoRental.AddAsync(rental, ct);
            await _repoRental.SaveChangesAsync(ct);

            return _mapper.Map<CreateRentalDto>(rental);
        }

        public async Task<ReturnResultDto> ReturnAsync(string identifier, DateTime returnDate, CancellationToken ct = default)
        {
            var rental = await _repoRental.GetByIdAsync(identifier)
           ?? throw new KeyNotFoundException("Locação não encontrada.");

            var rd = returnDate.Date;
            if (rd < rental.StartAt.Date)
                throw new ArgumentException("Data de devolução não pode ser anterior ao início.");

            var usedDays = (rd - rental.StartAt.Date).Days + 1;
            if (usedDays < 1) usedDays = 1;

            var baseAmount = usedDays * rental.DailyRate;

            decimal penalty = 0m;
            decimal extra = 0m;

            if (rd < rental.ExpectedEndAt.Date)
            {
                var notUsedDays = (rental.ExpectedEndAt.Date - rd).Days;
                var notUsedAmount = notUsedDays * rental.DailyRate;

                var percent = EarlyReturnPenaltyPercent.TryGetValue(rental.PlanDays, out var p) ? p : 0m;
                penalty = Math.Round(notUsedAmount * percent, 2, MidpointRounding.AwayFromZero);
            }
            else if (rd > rental.ExpectedEndAt.Date)
            {
                var extraDays = (rd - rental.ExpectedEndAt.Date).Days;
                extra = extraDays * 50m;
            }

            var total = baseAmount + penalty + extra;

            rental.Close(returnDate, total);
            _repoRental.Update(rental);
            await _repoRental.SaveChangesAsync(ct);

            return new ReturnResultDto
            {
                StartAt = rental.StartAt,
                ExpectedEndAt = rental.ExpectedEndAt,
                ReturnDate = returnDate,
                UsedDays = usedDays,
                BaseAmount = baseAmount,
                PenaltyAmount = penalty,
                ExtraDaysAmount = extra,
                Total = total
            };
        }

        public async Task<RentalDetailsDto> GetByIdAsync(string identifier, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Id inválido.", nameof(identifier));

            var rental = await _repoRental.GetByIdAsync(identifier, ct);

            if (rental is null)
                throw new KeyNotFoundException("Locação não encontrada.");

            return _mapper.Map<RentalDetailsDto>(rental);
        }
    }

}
