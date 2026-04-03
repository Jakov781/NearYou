using System.Text.Json.Serialization;

namespace WebTemplate.DTOs
{
    public class KorisnikDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Username { get; set; }
        public List<OglasDto> Oglasi { get; set; }
    }

    public class OglasDto
    {
        public int ID { get; set; }

        public string Naziv { get; set; }
        public string? Opis { get; set; }

        public string? Adresa { get; set; }
        public string? Grad { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public decimal Cena { get; set; }
        public TipCene TipCene { get; set; }

        // Prikaz cene za frontend
        public string CenaDisplay { get; set; }

        public string? SlikaUrl { get; set; }
        public DateTime DatumKreiranja { get; set; }

        // Postavljač oglasa
        public int PostavljacOglasaId { get; set; }
        public string PostavljacIme { get; set; }

        // Kategorija
        public int KategorijaId { get; set; }
        public string KategorijaNaziv { get; set; }
    }

    public class OglasCreateDto
    {
        [Required]
        public string Naziv { get; set; }

        public string? Opis { get; set; }
        public string? Adresa { get; set; }
        public string? Grad { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public decimal Cena { get; set; }
        public TipCene TipCene { get; set; }

        public IFormFile? Slika { get; set; }

        [Required]
        public int KategorijaId { get; set; }
    }
    public class PrijavaDtos
    {
        public int ID { get; set; }
        public int OglasId { get; set; }

        public DateTime VremePrijave { get; set; }
        public string Status { get; set; }
        public string? Poruka { get; set; }

        // 👇 DODAJ OVO
        public PrijavaKorisnikDto Korisnik { get; set; }
    }
    public class PrijavaKorisnikDto
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Username { get; set; }
    }
    public class PrijavaStatusUpdateDto
    {
        [Required]
        public string Status { get; set; } 
    }
    public class PrijavaDto
    {
        public int ID { get; set; }
        public int KorisnikId { get; set; }
        public string KorisnikIme { get; set; }
        public int OglasId { get; set; }
        public string OglasNaziv { get; set; }
        public DateTime VremePrijave { get; set; }
        public string Status { get; set; }
        public string? Poruka { get; set; }
    }

    public class PrijavaCreateDto
    {
        [Required]
        public int OglasId { get; set; }

        [MaxLength(500)]
        public string? Poruka { get; set; }
    }

    public class PrijavaUpdateDto
    {
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } // Prihvaćena, Odbijena

        [MaxLength(500)]
        public string? Poruka { get; set; }
    }

    public class PrijavaStatsDto
    {
        public int UkupnoPrijava { get; set; }
        public int NaCekanju { get; set; }
        public int Prihvacene { get; set; }
        public int Odbijene { get; set; }
    }
    public class RegisterDto
    {
        [Required]
        [MaxLength(20)]
        public string Ime { get; set; }

        [Required]
        [MaxLength(20)]
        public string Prezime { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MaxLength(512)]
        public string Biografija { get; set; }

        [Required]
        [MaxLength(100)]
        public string SlikaURL { get; set; }

        [Required]
        [MaxLength(15)]
        public string Telefon { get; set; }

        [Required]
        public string VestineJson { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public UserInfoDto User { get; set; }
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Username { get; set; }
        public string Biografija { get; set; }
        public string BrojTelefona { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Vestine { get; set; }
        public string SlikaUrl { get; set; }
    }

    public class KorisnikCreateDto
    {
        [Required]
        [MaxLength(20)]
        public string Ime { get; set; }

        [Required]
        [MaxLength(20)]
        public string Prezime { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }

    public class KorisnikUpdateDto
    {
        [MaxLength(20)]
        public string? Ime { get; set; }

        [MaxLength(20)]
        public string? Prezime { get; set; }

        [MaxLength(512)]
        public string? Bio { get; set; }

        [MaxLength(20)]
        public string? Telefon { get; set; }

        public string Vestine { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }
    }

    public class ProfileImageUploadDto
    {
        public IFormFile Image { get; set; }
    }
    public class ChatDto
    {
        public int Id { get; set; }
        public int OglasId { get; set; }
        public string OglasNaziv { get; set; }
        public int KlijentId { get; set; }
        public string KlijentUsername { get; set; }
        public int OglasivacId { get; set; }
        public string OglasivacUsername { get; set; }
        public DateTime Kreiran { get; set; }
        public string? PoslednjaPoruka { get; set; }
        public DateTime? PoslednjaPorukaVreme { get; set; }
        public string? PoslednjaPorukaPosiljalac { get; set; }

        public string? KlijentSlikaUrl { get; set; }
        public string? OglasivacSlikaUrl { get; set; }
    }

    public class PorukaDto
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int PosiljalacId { get; set; }
        public string PosiljalacUsername { get; set; }
        public string Tekst { get; set; }
        public DateTime VremeSlanja { get; set; }
    }

    public class KreirajChatDto
    {
        [Required]
        public int PrijavaId { get; set; }
    }

    public class ChatPorukeDto
    {
        public ChatDto Chat { get; set; }
        public List<PorukaDto> Poruke { get; set; }
    }
    public class PromenaSifreDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
