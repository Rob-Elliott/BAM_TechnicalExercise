using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using StargateAPI.Business.Data;

namespace StargateAPI.Business.Repositories
{
    public class AstronautDutyRepository : GenericRepository<AstronautDuty>, IAstronautDutyRepository
    {
        public AstronautDutyRepository(StargateContext context)
            : base(context) { }

        public new async Task<IEnumerable<AstronautDuty>> GetAllAsync()
        {
            return await base._context.Set<AstronautDuty>()
                .ToListAsync();
        }

        public async Task<IEnumerable<AstronautDuty>> GetAstronautDutiesByPersonIdAsync(int personId)
        {
            return await base._context.Set<AstronautDuty>()
                .Where(x => x.PersonId == personId)
                .OrderByDescending(d => d.DutyStartDate)
                .ToListAsync();
        }

        public async Task<AstronautDuty?> GetAstronautDutyByPersonIdAndStartDateAsync(int personId, DateTime startDate)
        {
            return await base._context.Set<AstronautDuty>()
                .FirstOrDefaultAsync(x => x.PersonId == personId && x.DutyStartDate == startDate);
        }

    }
}
