using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class InsuranceRenewalDto
    {
        public int InsuranceId { get; set; }

        public int TruckId { get; set; }
        public string TruckPlate { get; set; } = string.Empty;

        public DateTime RenewalDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Cost { get; set; }
        public string? SondNumber { get; set; }
        public string? Notes { get; set; }

        // Audit
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
