using System.ComponentModel.DataAnnotations;
using static LCMS.Models.Enums;

namespace LCMS.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public Guid Guid { get; set; } = new();

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int? UserAccountId { get; set; }

        public string? UserName { get; set; }

        public int TypeId { get; set; }

        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? MiddleName { get; set; }

        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EmailAddress { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? AddressLine1 { get; set; }

        [MaxLength(255)]
        public string? AddressLine2 { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? Region { get; set; }

        [MaxLength(10)]
        public string? PostalCode { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = new DateTime(1900, 1, 1);

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime ModifiedDate { get; set; } = new DateTime(1900, 1, 1);

        public string ModifiedBy { get; set; } = string.Empty;

        public UserType Type { get { return (UserType)TypeId; } }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                {
                    return $"{FirstName} {LastName}";
                }
                return $"{FirstName} {MiddleName} {LastName}";
            }
        }

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
