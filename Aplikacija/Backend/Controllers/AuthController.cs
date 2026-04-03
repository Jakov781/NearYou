using Microsoft.AspNetCore.Mvc;
using WebTemplate.Services;
using WebTemplate.DTOs;

namespace WebTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public AuthController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Provera da li username postoji
            if (await _context.Korisnici.AnyAsync(u => u.Username == registerDto.Username))
                return BadRequest(new { message = "Username već postoji" });

            // Provera da li email postoji
            if (await _context.Korisnici.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest(new { message = "Email već postoji" });

            // Kreiranje korisnika
            var korisnik = new Korisnik
            {
                Ime = registerDto.Ime,
                Prezime = registerDto.Prezime,
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = _authService.HashPassword(registerDto.Password),
                Role = UserRoles.User,
                CreatedAt = DateTime.UtcNow,
                Biografija = registerDto.Biografija,
                SlikaURL = registerDto.SlikaURL,
                Telefon = registerDto.Telefon,
                VestineJson = registerDto.VestineJson
            };

            // Prvi korisnik postaje admin
            if (!await _context.Korisnici.AnyAsync())
            {
                korisnik.Role = UserRoles.Admin;
            }

            _context.Korisnici.Add(korisnik);
            await _context.SaveChangesAsync();

            // Generisanje tokena
            var token = _authService.GenerateJwtToken(korisnik);

            var response = new LoginResponseDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddHours(24),
                User = new UserInfoDto
                {
                    Id = korisnik.ID,
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Username = korisnik.Username,
                    Email = korisnik.Email,
                    Role = korisnik.Role,
                    CreatedAt = korisnik.CreatedAt
                }
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Pronalaženje korisnika po username ili email
            var korisnik = await _context.Korisnici
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username || u.Email == loginDto.Username);

            if (korisnik == null)
                return Unauthorized(new { message = "Pogrešno korisničko ime ili lozinka" });

            if (!_authService.VerifyPassword(loginDto.Password, korisnik.PasswordHash))
                return Unauthorized(new { message = "Pogrešno korisničko ime ili lozinka" });

         //   if (korisnik.IsBanned)
              //  return Unauthorized(new { message = "Ovaj nalog je banovan od strane admina." });
            
            // Generisanje tokena
            var token = _authService.GenerateJwtToken(korisnik);

            var response = new LoginResponseDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddHours(24),
                User = new UserInfoDto
                {
                    Id = korisnik.ID,
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Username = korisnik.Username,
                    Biografija = korisnik.Biografija,
                    BrojTelefona = korisnik.Telefon,
                    Email = korisnik.Email,
                    Role = korisnik.Role,
                    CreatedAt = korisnik.CreatedAt,
                    Vestine = korisnik.VestineJson,
                    SlikaUrl = korisnik.SlikaURL
                }
            };

            return Ok(response);
        }
    }
}
