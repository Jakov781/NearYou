namespace WebTemplate.Models;

public class Recenzija
{
    public Korisnik Ocenjivac { get; set; }
    public int OcenjivacId { get; set; }
    
    public Korisnik Ocenjeni { get; set; }
    public int OcenjeniKorisnikId { get; set; }
    
    public Oglas Oglas { get; set; }
    public int OglasId { get; set; }
    
    public decimal Ocena { get; set; }
}

public enum TipRecenzije
{
    RecenzijaKlijenta,
    RecenzijaOglasivaca,
}