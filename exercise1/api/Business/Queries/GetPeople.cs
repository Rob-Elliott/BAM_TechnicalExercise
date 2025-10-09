using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Repositories;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    /// <summary>
    /// Retrieves all Persons and related AstronautDetails
    /// </summary>
    public class GetPeople
        : IRequest<GetPeopleResult>
    { }

    public class GetPeopleHandler(IPersonRepository repository)
        : IRequestHandler<GetPeople, GetPeopleResult>
    {
        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
        {
            var result = new GetPeopleResult();

            result.People = (await repository.GetAllAsync())
                .Select(p => new PersonAstronaut(p, p.AstronautDetail))
                .ToList();

            return result;
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut> { };

    }
}
