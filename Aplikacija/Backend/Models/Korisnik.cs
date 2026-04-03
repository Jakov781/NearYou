namespace WebTemplate.Models
{
    [Table("Korisnici")]
    public class Korisnik
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("Ime")]
        [MaxLength(20)]
        public string Ime { get; set; }

        [Required]
        [Column("Prezime")]
        [MaxLength(20)]
        public string Prezime { get; set; }

        [Required]
        [Column("Username")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [Column("Email")]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column("PasswordHash")]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Column("Role")]
        [MaxLength(20)]
        public string Role { get; set; } = "User";

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [InverseProperty("PostavljacOglasa")]
        public virtual ICollection<Oglas>? Oglasi { get; set; }

        [InverseProperty("Korisnik")]
        public virtual ICollection<Prijava>? Prijave { get; set; }
        
        public string Biografija { get; set; }

        public string? VestineJson { get; set; } = "";
        
        [MaxLength(15)]
        public string Telefon { get; set; }
        
        public string SlikaURL { get; set; }

       // public bool IsBanned { get; set; } = false;
    }

    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }
}
