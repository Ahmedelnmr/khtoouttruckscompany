using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Entities
{
    public class InsuranceRenewal : BaseEntity
    {
        public int InsuranceId { get; set; }

        public int TruckId { get; set; }
        public virtual Truck Truck { get; set; } = null!;

        public DateTime RenewalDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; }
        public decimal Cost { get; set; }
        public string? SondNumber { get; set; }
        public string? Notes { get; set; }
    }
}
