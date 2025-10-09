using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Repositories;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    /// <summary>
    /// Retrieves a specific Person and related AstronautDetail
    /// </summary>
    public class GetPersonByName : IRequest<GetPersonByNameResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class GetPersonByNameHandler(IPersonRepository repository)
        : IRequestHandler<GetPersonByName, GetPersonByNameResult>
    {
        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
        {
            var result = new GetPersonByNameResult();

            var person = await repository.GetPersonByNameAsync(request.Name);

            // if a person was found by name
            if (person != null)
            {
                result.Person = new PersonAstronaut(person, person.AstronautDetail);
            }
            // if no person was found
            else
            {
                throw new BadHttpRequestException($"No Person with name '{request.Name}' exists");
            }

            return result;
        }
    }

    public class GetPersonByNameResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}
