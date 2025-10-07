using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    /// <summary>
    /// Retrieves a specific Person and related AstronautDuties
    /// </summary>
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;

        public GetAstronautDutiesByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {

            var result = new GetAstronautDutiesByNameResult();

            var person = _context.People
                    .Include(p => p.AstronautDetail)
                    .Include(p => p.AstronautDuties)
                    .FirstOrDefault(p => p.Name.ToLower() == request.Name.ToLower());

            //var query = $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE \'{request.Name}\' = a.Name";
            //var person = await _context.Connection.QueryFirstOrDefaultAsync<PersonAstronaut>(query);


            // if no person was found
            if (person == null)
            {
                throw new BadHttpRequestException($"No Person with name '{request.Name}' exists");
            }

            result.Person = new PersonAstronaut(person, person.AstronautDetail);

            //var query = $"SELECT * FROM [AstronautDuty] WHERE {person.Id} = PersonId Order By DutyStartDate Desc";
            //var duties = await _context.Connection.QueryAsync<AstronautDuty>(query);

            result.AstronautDuties = person.AstronautDuties.OrderByDescending(d => d.DutyStartDate).Select(d => { d.Person = null; return d;}).ToList();

            return result;
        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public PersonAstronaut Person { get; set; }
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}