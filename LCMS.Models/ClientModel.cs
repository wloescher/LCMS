using System.ComponentModel.DataAnnotations;
using static LCMS.Models.Enums;

namespace LCMS.Models
{
    public class ClientModel
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int TypeId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string AddressLine1 { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? AddressLine2 { get; set; }

        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Region { get; set; } = string.Empty;

        [MaxLength(10)]
        public string PostalCode { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Country { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Url { get; set; }

        public DateTime CreatedDate { get; set; } = new DateTime(1900, 1, 1);

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime ModifiedDate { get; set; } = new DateTime(1900, 1, 1);

        public string ModifiedBy { get; set; } = string.Empty;

        public ClientType Type { get { return (ClientType)TypeId; } }

        public string FullAddress
        {
            get
            {
                var fullAddress = string.Empty;
                if (!string.IsNullOrEmpty(AddressLine1))
                {
                    fullAddress += AddressLine1 + "<br />";
                }
                if (!string.IsNullOrEmpty(AddressLine2))
                {
                    fullAddress += AddressLine2 + "<br />";
                }
                if (!string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(Region))
                {
                    fullAddress += City + ", " + Region + "<br />";
                }
                else if (!string.IsNullOrEmpty(City))
                {
                    fullAddress += City + "<br />";
                }
                else if (!string.IsNullOrEmpty(Region))
                {
                    fullAddress += Region + "<br />";
                }
                if (!string.IsNullOrEmpty(Country) || !string.IsNullOrEmpty(PostalCode))
                {
                    fullAddress += Country + " " + PostalCode + "<br />";
                }
                return fullAddress;
            }
        }
    }
}
