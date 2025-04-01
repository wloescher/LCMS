using System.ComponentModel.DataAnnotations;
using static LCMS.Models.Enums;

namespace LCMS.Models
{
    public class CaseModel
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = new();

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int ClientId { get; set; }

        public int TypeId { get; set; }

        public int StatusId { get; set; }

        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string ClientName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = new DateTime(1900, 1, 1);

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime ModifiedDate { get; set; } = new DateTime(1900, 1, 1);

        public string ModifiedBy { get; set; } = string.Empty;

        public CaseType Type { get { return (CaseType)TypeId; } }

        public CaseStatus Status { get { return (CaseStatus)StatusId; } }
    }
}
