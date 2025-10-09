using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Repositories;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    /// <summary>
    /// Creates a Person record
    /// </summary>
    public class CreatePerson : IRequest<CreatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor(IPersonRepository repository)
        : IRequestPreProcessor<CreatePerson>
    {

        public Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            var existingPerson = repository.GetPersonByNameAsync(request.Name);

            if (existingPerson is not null)
                throw new BadHttpRequestException($"Bad Request, person with name '{request.Name}' already exists");

            return Task.CompletedTask;
        }
    }

    public class CreatePersonHandler(IPersonRepository repository)
        : IRequestHandler<CreatePerson, CreatePersonResult>
    {
        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var newPerson = new Person()
            {
                Name = request.Name
            };

            newPerson = await repository.AddAsync(newPerson);

            return new CreatePersonResult()
            {
                Id = newPerson.Id
            };
        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
