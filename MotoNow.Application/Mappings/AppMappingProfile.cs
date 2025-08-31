using AutoMapper;
using MotoNow.Domain.Entities;
using MotoNow.Application.DTOs; 

namespace MotoNow.Application.Mappings;

public sealed class AppMappingProfile : Profile
{
    public AppMappingProfile()
    {
        
        CreateMap<CreateMotorCycleDto, Motorcycle>()
            .ConstructUsing(s => new Motorcycle(
            s.Identifier, 
            s.Plate, 
            s.Model, 
            s.Year));

        CreateMap<CreateDelivererDto, DeliveryDriver>()
            .ConstructUsing(s => new DeliveryDriver(
                s.Identifier,
                s.Name,
                s.Cnpj,                 
                s.BirthDate,
                s.DriverLicenseNumber,
                s.DriverLicenseClass
            ));

        CreateMap<CreateRentalDto, Rental>()
            .ConstructUsing(s => new Rental(
            s.Identifier,
            s.DeliveryDriverId,
            s.MotorcycleId,
            s.StartAt,
            s.EndAt,
            s.ExpectedEndAt,
            s.PlanDays));

        CreateMap<Rental, RentalDetailsDto>()
               .ForMember(d => d.Identifier, m => m.MapFrom(s => s.Identifier))
               .ForMember(d => d.DeliveryDriverId, m => m.MapFrom(s => s.DeliveryDriverId))
               .ForMember(d => d.MotorcycleId, m => m.MapFrom(s => s.MotorcycleId))
               .ForMember(d => d.StartAt, m => m.MapFrom(s => s.StartAt))
               .ForMember(d => d.EndAt, m => m.MapFrom(s => s.EndAt))
               .ForMember(d => d.ExpectedEndAt, m => m.MapFrom(s => s.ExpectedEndAt))
               .ForMember(d => d.ReturnDate, m => m.MapFrom(s => s.ReturnDate))
               .ForMember(d => d.DailyRate, m => m.MapFrom(s => s.DailyRate))
               .ForMember(d => d.TotalAmount, m => m.MapFrom(s => s.TotalAmount));

        CreateMap<Motorcycle, CreateMotorCycleDto>();
        CreateMap<DeliveryDriver, CreateDelivererDto>();
        CreateMap<Rental, CreateRentalDto>();

    }
}
