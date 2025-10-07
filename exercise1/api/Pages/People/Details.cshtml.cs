using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;

public class DetailsModel : PageModel
{
    private readonly StargateContext _context;
    public DetailsModel(StargateContext context) => _context = context;

    public PersonAstronaut? Person { get; set; }
    public List<AstronautDuty> Duties { get; set; } = new();

    public IActionResult OnGet(int id)
    {
        var person = _context.People
            .Include(p => p.AstronautDetail)
            .Include(p => p.AstronautDuties)
            .FirstOrDefault(p => p.Id == id);

        if (person == null) return NotFound();

        Person = new PersonAstronaut(person, person.AstronautDetail);
        Duties = person.AstronautDuties.OrderByDescending(d => d.DutyStartDate).ToList();

        return Page();
    }
}
