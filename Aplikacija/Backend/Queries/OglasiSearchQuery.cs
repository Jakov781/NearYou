namespace WebTemplate.DTOs;

public class OglasiSearchQuery
{
    public string? Q { get; set; }
    public string? Kategorija { get; set; }
    public OglasSort Sort { get; set; } = OglasSort.DateDesc;
    
    // Za mapu i lokaciju
    public double? SearchLat { get; set; } = null;
    public double? SearchLon { get; set; } = null;
    public double? RadiusKm { get; set; } = 20;
}

public class OglasSearchDto
{
    public int ID { get; set; }
    public string Naziv { get; set; }
    public string Opis { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Slika { get; set; }
    public string PostavljacIme { get; set; }
    public string? PostavljacSlika { get; set; }
    public string CenaDisplay { get; set; }
}

public enum OglasSort
{
    DateDesc,
    PriceAsc,
    PriceDesc,
}

public static class OglasiQueryExtensions
{
    public static async Task<List<OglasSearchDto>> GetOglasiAsync(this ApplicationDbContext _context, OglasiSearchQuery query)
    {
        var oglasi = _context.Oglasi
            .AsNoTrackingWithIdentityResolution()
            .Include(x => x.PostavljacOglasa)
            .Where(x => x.Status != "obrisan")
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Q))
        {
            var q = query.Q.ToLower();
            foreach (var token in q.Split(' '))
            {
                oglasi = oglasi.Where(o => o.Naziv.ToLower().Contains(token) || o.Opis.ToLower().Contains(token));
            }
        }

        if (!string.IsNullOrWhiteSpace(query.Kategorija))
        {
            var dbKategorija = await _context.Kategorije
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefaultAsync(k => k.Naziv.ToLower() == query.Kategorija.ToLower());
            oglasi = oglasi.Where(o => o.KategorijaId == dbKategorija.Id);
        }

        if (query.SearchLat.HasValue && query.SearchLon.HasValue && query.RadiusKm.HasValue)
        {
            double lat1 = query.SearchLat.Value;
            double lon1 = query.SearchLon.Value;
            double radius = query.RadiusKm.Value;
            const double r = 6371; // Radius zemlje u km
            //ovu formulu sam nasao ovde: https://www.movable-type.co.uk/scripts/latlong.html
            //ne pitajte me nista
            oglasi = oglasi.Where(x =>
                (r * 2 * Math.Atan2(
                    Math.Sqrt(
                        Math.Pow(Math.Sin(((x.Latitude - lat1) * (Math.PI / 180)) / 2), 2) +
                        Math.Cos(lat1 * (Math.PI / 180)) *
                        Math.Cos(x.Latitude * (Math.PI / 180)) *
                        Math.Pow(Math.Sin(((x.Longitude - lon1) * (Math.PI / 180)) / 2), 2)
                    ),
                    Math.Sqrt(1 - (
                        Math.Pow(Math.Sin(((x.Latitude - lat1) * (Math.PI / 180)) / 2), 2) +
                        Math.Cos(lat1 * (Math.PI / 180)) *
                        Math.Cos(x.Latitude * (Math.PI / 180)) *
                        Math.Pow(Math.Sin(((x.Longitude - lon1) * (Math.PI / 180)) / 2), 2)
                    ))
                )) <= radius);
        }
        
        if (query.Sort == OglasSort.DateDesc)
            oglasi = oglasi.OrderByDescending(o => o.DatumKreiranja);
        else if (query.Sort == OglasSort.PriceAsc)
            oglasi = oglasi.OrderBy(o => o.Cena);
        else if (query.Sort == OglasSort.PriceDesc)
            oglasi = oglasi.OrderByDescending(o => o.Cena);

        var result = oglasi.Select(o => new OglasSearchDto()
        {
            ID = o.ID,
            Naziv = o.Naziv,
            Opis = o.Opis,
            CenaDisplay = o.CenaDisplay,
            Slika = o.SlikaUrl,
            Latitude = o.Latitude,
            Longitude = o.Longitude,
            PostavljacIme = o.PostavljacOglasa.Ime,
            PostavljacSlika = o.PostavljacOglasa.SlikaURL,
        });
        
        return await result.ToListAsync();
    }
}