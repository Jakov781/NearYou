using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTemplate.Models;
[Table("Chatovi")]
public class Chat
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OglasId { get; set; }

    [ForeignKey(nameof(OglasId))]
    public virtual Oglas Oglas { get; set; }

    [Required]
    public int KlijentId { get; set; }

    [ForeignKey(nameof(KlijentId))]
    public virtual Korisnik Klijent { get; set; }

    [Required]
    public int OglasivacId { get; set; }

    [ForeignKey(nameof(OglasivacId))]
    public virtual Korisnik Oglasivac { get; set; }

    public DateTime Kreiran { get; set; } = DateTime.UtcNow;

    public DateTime? PoslednjaPorukaVreme { get; set; }

    public string? PoslednjaPoruka { get; set; }

    public string? PoslednjaPorukaPosiljalac { get; set; }

    [InverseProperty("Chat")]
    public virtual ICollection<Poruka> Poruke { get; set; } = new List<Poruka>();
}
