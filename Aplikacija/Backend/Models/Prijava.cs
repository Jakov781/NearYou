namespace WebTemplate.Models
{
    [Table("Prijave")]
    public class Prijava
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Required]
        [Column("KorisnikId")]
        [ForeignKey("Korisnik")]
        public int KorisnikId { get; set; }

        [Required]
        [Column("OglasId")]
        [ForeignKey("Oglas")]
        public int OglasId { get; set; }

        [Required]
        [Column("VremePrijave")]
        public DateTime VremePrijave { get; set; }

        [Column("Status")]
        [MaxLength(20)]
        public string Status { get; set; } = "Na čekanju"; // Na čekanju, Prihvaćena, Odbijena

        [Column("Poruka")]
        [MaxLength(500)]
        public string? Poruka { get; set; }

        // Navigacioni property-ji
        public virtual Korisnik Korisnik { get; set; }
        public virtual Oglas Oglas { get; set; }
    }
}
