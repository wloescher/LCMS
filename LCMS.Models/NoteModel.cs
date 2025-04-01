namespace LCMS.Models
{
    public class NoteModel
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = new();

        public bool IsDeleted { get; set; }

        public string Note { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = new DateTime(1900, 1, 1);

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime ModifiedDate { get; set; } = new DateTime(1900, 1, 1);

        public string ModifiedBy { get; set; } = string.Empty;
    }
}
