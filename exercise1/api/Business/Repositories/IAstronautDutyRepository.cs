using StargateAPI.Business.Data;
using StargateAPI.Business.Queries;

namespace StargateAPI.Business.Repositories
{
    public interface IAstronautDutyRepository : IGenericRepository<AstronautDuty>
    {
        Task<IEnumerable<AstronautDuty>> GetAstronautDutiesByPersonIdAsync(int personId);
        Task<AstronautDuty?> GetAstronautDutyByPersonIdAndStartDateAsync(int personId, DateTime startDate);
    }
}
