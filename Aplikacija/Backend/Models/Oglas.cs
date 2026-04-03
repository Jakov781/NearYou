namespace WebTemplate.Models;

public class Oglas
{
    [Key]
    public int ID { get; set; }

    [Required]
    public string Naziv { get; set; }
    public string Opis { get; set; }
    public string Adresa { get; set; }
    public string Grad { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    [Precision(10,4)] public decimal Cena { get; set; }
    public TipCene TipCene { get; set; }
    public string? SlikaUrl { get; set; }

    public DateTime DatumKreiranja { get; set; }
    //public bool IsActive { get; set; } = true; // admin moze da deaktivira oglase
    
    [ForeignKey(nameof(PostavljacOglasaId))]
    public Korisnik PostavljacOglasa { get; set; }
    public int PostavljacOglasaId { get; set; }
    
    
    [ForeignKey(nameof(KategorijaId))]
    public Kategorija Kategorija { get; set; }
    public int KategorijaId { get; set; }

    public string? Status { get; set; } //postavljen/obrisan

    
    public List<Prijava> Prijave { get; set; } = [];
    
    [NotMapped]// koristimo na frontu da prikazemo lepo cenu na frontu
    public string CenaDisplay => TipCene switch
    {
        TipCene.Fiksno => $"{Cena:F0} RSD",
        TipCene.Satnica => $"{Cena:F0} RSD/sat",
        TipCene.Dogovor => "Dogovor",
        TipCene.Besplatno => "Besplatno"
    };
}

