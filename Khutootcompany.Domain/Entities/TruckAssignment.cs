using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Entities
{
    public class TruckAssignment : BaseEntity
    {
        public int AssignmentId { get; set; }

        public int TruckId { get; set; }
        public virtual Truck Truck { get; set; } = null!;

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;

        public DateTime AssignmentDate { get; set; } = DateTime.Now;
        public bool IsCurrent { get; set; } = true;
        public DateTime? EndDate { get; set; }

        public string? Notes { get; set; }
    }
}
