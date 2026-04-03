namespace WebTemplate.Models
{
    [Table("Poruke")]
    public class Poruka
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChatId { get; set; }

        [ForeignKey(nameof(ChatId))]
        public virtual Chat Chat { get; set; }

        [Required]
        public int PosiljalacId { get; set; }

        [ForeignKey(nameof(PosiljalacId))]
        public virtual Korisnik Posiljalac { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Tekst { get; set; }

        public DateTime VremeSlanja { get; set; } = DateTime.UtcNow;
    }
}
