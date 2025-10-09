using StargateAPI.Business.Data;

namespace StargateAPI.Business.Repositories
{
    public interface IPersonRepository : IGenericRepository<Person>
    {
        Task<Person?> GetPersonByNameAsync(string name);

        new Task<IEnumerable<Person>> GetAllAsync();
    }
}
