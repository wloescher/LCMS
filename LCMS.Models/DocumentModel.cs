using System.ComponentModel.DataAnnotations;
using static LCMS.Models.Enums;

namespace LCMS.Models
{
    public class DocumentModel
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = new();

        public int TypeId { get; set; }

        public bool IsDeleted { get; set; }

        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        [MaxLength(255)]
        public string OriginalFileName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = new DateTime(1900, 1, 1);

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime ModifiedDate { get; set; } = new DateTime(1900, 1, 1);

        public string ModifiedBy { get; set; } = string.Empty;

        public DocumentType Type { get { return (DocumentType)TypeId; } }

    }
}
