namespace WebTemplate.DTOs;

public class OglasPrikazQuery
{
    public int OglasId { get; set; }
}

public class OglasPrikazDto
{
    public int ID { get; set; }
    public string Naziv { get; set; }
    public string Opis { get; set; }
    public string CenaDisplay { get; set; }
    public string SlikaOglasa { get; set; }
    
    public string Grad { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public DateTime DatumKreiranja { get; set; }

    public PostavljacPrikazDto Postavljac { get; set; }
}

public class PostavljacPrikazDto
{
    public int Id { get; set; }
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public string Username { get; set; }
    public string SlikaUrl { get; set; }
    public string CenaDisplay { get; set; }
    public int GodinaClanstva { get; set; } // "Član od 2023."
    public double ProsecnaOcena { get; set; } // "⭐ 4.9"
    public int BrojRecenzija { get; set; }    // "(12 recenzija)"
}

public static class OglasPrikazQueryExtensions
{
    public static async Task<OglasPrikazDto?> GetOglasPrikazAsync(this ApplicationDbContext _context, OglasPrikazQuery query)
    {
        var oglasQuery = _context.Oglasi
            .AsNoTrackingWithIdentityResolution()
            .Include(x => x.PostavljacOglasa) 
            .Where(x => x.ID == query.OglasId);

        var result = await oglasQuery.Select(o => new OglasPrikazDto
        {
            ID = o.ID,
            Naziv = o.Naziv,
            Opis = o.Opis,
            CenaDisplay = o.CenaDisplay,
            
            SlikaOglasa = o.SlikaUrl,
            
            Grad = o.Grad ?? string.Empty,
            Latitude = o.Latitude,
            Longitude = o.Longitude,
            DatumKreiranja = o.DatumKreiranja,

            Postavljac = new PostavljacPrikazDto
            {
                Id = o.PostavljacOglasa.ID,
                Ime = o.PostavljacOglasa.Ime,
                Prezime = o.PostavljacOglasa.Prezime,
                Username = o.PostavljacOglasa.Username,
                SlikaUrl = o.PostavljacOglasa.SlikaURL,
                GodinaClanstva = o.PostavljacOglasa.CreatedAt.Year,
                 
                // TODO: Momci, ovo je vas deo, kako god zelite popunite ova polja
                ProsecnaOcena = 4.6, 
                BrojRecenzija = 12
            }
        })
        .FirstOrDefaultAsync();

        return result;
    }
}