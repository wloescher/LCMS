using static LCMS.Models.Enums;

namespace LCMS.Models
{
    public class AuditModel
    {
        public int Id { get; set; }

        public AuditAction Action { get; set; }

        public int RecordId { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public string BeforeJson { get; set; } = string.Empty;

        public string AfterJson { get; set; } = string.Empty;

        public List<string> AffectedColumns { get; set; } = [];
    }
}
