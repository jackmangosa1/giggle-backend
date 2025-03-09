using ServiceManagementAPI.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceManagementAPI.Entities
{
    public partial class Provider
    {
        [NotMapped]
        public AvailabilityStatus Status { get; set; } = AvailabilityStatus.Offline;
    }
}
