namespace MotoNow.Application.DTOs
{
    public sealed class RentalDto
    {
        public string Id { get; set; } = default!;
        public string Identifier { get; set; } = default!;
        public string DeliveryDriverId { get; set; } = default!;
        public string MotorcycleId { get; set; } = default!;
        public DateTime StartAt { get; set; }
        public DateTime ExpectedEndAt { get; set; }
        public int PlanDays { get; set; }
        public int DailyRate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
