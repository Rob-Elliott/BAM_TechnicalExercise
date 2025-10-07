using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

public class IndexModel : PageModel
{
    private readonly StargateContext _context;
    public IndexModel(StargateContext context) => _context = context;

    public List<PersonAstronaut> People { get; set; } = new();

    public void OnGet()
    {
        People = _context.People
            .Include(p => p.AstronautDetail)
            .Select(p => new PersonAstronaut(p, p.AstronautDetail))
            .ToList();
    }
}
