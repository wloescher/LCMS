using System.ComponentModel.DataAnnotations;
using static LCMS.Models.Enums;

namespace LCMS.Models
{
    public class ContractModel
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = new();

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int ClientId { get; set; }

        public int DocumentTypeId { get; set; }

        public int ContractTypeId { get; set; }

        public int StatusId { get; set; }

        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string ClientName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = new DateTime(1900, 1, 1);

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime ModifiedDate { get; set; } = new DateTime(1900, 1, 1);

        public string ModifiedBy { get; set; } = string.Empty;

        public DocumentType DocumentType { get { return (DocumentType)DocumentTypeId; } }

        public ContractType ContractType { get { return (ContractType)ContractTypeId; } }

        public ContractStatus Status { get { return (ContractStatus)StatusId; } }

        public string FileName { get; set; } = string.Empty;
    }
}
