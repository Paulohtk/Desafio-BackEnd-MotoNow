using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotoNow.Domain.Entities
{
    public sealed class Rental : BaseEntity
    {
        private Rental() { }
        
        public static readonly int[] ValidPlans = { 7, 15, 30, 45, 50 };


        public static readonly IReadOnlyDictionary<int, int> DailyRatesByDays = new Dictionary<int, int>
        {
            [7] = 30,
            [15] = 28,
            [30] = 22,
            [45] = 20,
            [50] = 18
        };

        public static bool IsValidPlan(int days) => ValidPlans.Contains(days);

        public Rental(string identifier, string deliveryDriverId, string motorcycleId, DateTime startAt, DateTime? endAt, DateTime expectedEndAt, int planDays)
        {
            if (planDays <= 0) throw new ArgumentException("");
            if (!IsValidPlan(planDays))
                throw new ArgumentException($"Plano inválido ({planDays}). Valores aceitos: 7, 15, 30, 45, 50.");

            Identifier = identifier;
            DeliveryDriverId = deliveryDriverId;
            MotorcycleId = motorcycleId;
            StartAt = startAt;
            EndAt = endAt;
            ExpectedEndAt = expectedEndAt;
            PlanDays = planDays;
            ReturnDate = null;
            DailyRate = DailyRatesByDays[planDays];
            TotalAmount = 0;
        }

        public string DeliveryDriverId { get; private set; }
        public string MotorcycleId { get; private set; }

        public DateTime StartAt { get; private set; }
        public DateTime? EndAt { get; private set; }
        public DateTime ExpectedEndAt { get; private set; }
        public int PlanDays { get; private set; }
        public DateTime? ReturnDate { get; private set; }
        public int DailyRate { get; private set; }
        public decimal TotalAmount { get; private set; }

        public void Close(DateTime returnDate, decimal total)
        {
            ReturnDate = returnDate;
            TotalAmount = total < 0 ? 0 : total;
        }
    }

}
