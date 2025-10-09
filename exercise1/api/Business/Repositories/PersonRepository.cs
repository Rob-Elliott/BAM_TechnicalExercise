using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using StargateAPI.Business.Data;

namespace StargateAPI.Business.Repositories
{
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepository(StargateContext context)
            : base(context) { }

        public new async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await base._context.Set<Person>()
                .Include(p => p.AstronautDetail)
                .ToListAsync();
        }

        public async Task<Person?> GetPersonByNameAsync(string name)
        {
            return await base._context.Set<Person>()
                .Include(p => p.AstronautDetail)
                .Where(x => x.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();
        }
    }
}
