using WebTemplate.Services;

namespace WebTemplate.Controllers;


[Route("api/[controller]")]
[ApiController]
public class KategorijeController
    (ApplicationDbContext context, IAuthService authService)
    : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly IAuthService _authService = authService;

    
    // GET: api/Kategorije/all
    /// Pribavlja sve kategorije u bazi
    [HttpGet("all")]
    public async Task<ActionResult<List<Kategorija>>> GetAll()
    {
        var kategorije = await _context.Kategorije.AsNoTracking().ToListAsync();

        return Ok(kategorije);
    }
    
    // GET: api/Kategorije/popular
    /// Pribavlja top 3 najpopularnije kategorije za TheNavbar.vue, promenite ovo kako vam se svidi, samo je predlog
    [HttpGet("popular")]
    public async Task<ActionResult<List<Kategorija>>> GetPopular()
    {
        var kategorije = await _context.Oglasi
            .AsNoTracking()
            .Include(o => o.Kategorija)
            .GroupBy(o => o.KategorijaId)
            .OrderByDescending(o => o.Count())
            .Take(3)
            .Select(o => o.First().Kategorija)
            .ToListAsync();

        return Ok(kategorije);
    }
}