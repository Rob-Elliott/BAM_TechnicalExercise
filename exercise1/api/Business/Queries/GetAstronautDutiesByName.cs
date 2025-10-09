using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Business.Repositories;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    /// <summary>
    /// Retrieves a specific Person and related AstronautDuties
    /// </summary>
    public class GetAstronautDutiesByName
        : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler(IAstronautDutyRepository repository, IPersonRepository personRepository)
        : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {
            var result = new GetAstronautDutiesByNameResult();

            var person = await personRepository.GetPersonByNameAsync(request.Name);

            // if no person was found
            if (person == null)
            {
                throw new BadHttpRequestException($"No Person with name '{request.Name}' exists");
            }

            result.Person = new PersonAstronaut(person, person.AstronautDetail);
            result.AstronautDuties = (await repository.GetAstronautDutiesByPersonIdAsync(person.Id))
                .ToList();

            return result;
        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public PersonAstronaut Person { get; set; }
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}