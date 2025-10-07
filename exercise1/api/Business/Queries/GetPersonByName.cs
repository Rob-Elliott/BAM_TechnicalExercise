using System;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetPersonByName : IRequest<GetPersonByNameResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
    {
        private readonly StargateContext _context;
        public GetPersonByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
        {
            var result = new GetPersonByNameResult();

            //var query = $"SELECT a.Id as PersonId, a.Name, b.CurrentRank, b.CurrentDutyTitle, b.CareerStartDate, b.CareerEndDate FROM [Person] a LEFT JOIN [AstronautDetail] b on b.PersonId = a.Id WHERE '{request.Name}' = a.Name";
            //var person = await _context.Connection.QueryAsync<PersonAstronaut>(query);
            //result.Person = person.FirstOrDefault();

            var person = _context.People
                .Include(p => p.AstronautDetail)
                .FirstOrDefault(p => p.Name.ToLower() == request.Name.ToLower());

            // if a person was found by name
            if (person != null)
            {
                result.Person = new PersonAstronaut(person, person.AstronautDetail);
            }
            // if no person was found
            else
            {
                result.Success = false;
                result.ResponseCode = 404; 
                result.Message = $"No Person with name '{request.Name}' exists.";
            }
            
            return result;
        }
    }

    public class GetPersonByNameResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}
