using System.ComponentModel.DataAnnotations.Schema;

namespace ShopOnline.Models
{
    public class User
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = "";
        public string Name { get; set; }
        public string CI { get; set; } // pais - codigo postal
        public string City { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; } // codigo postal
        public string? Gender { get; set; } // direccion
        [NotMapped]
        public virtual string Rol { get; set; }
    }
}
