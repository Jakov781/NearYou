namespace WebTemplate.Models;

[Table("Kategorije")]
public class Kategorija
{
    [Key]
    public int Id { get; set; }
    public string Naziv { get; set; }
}