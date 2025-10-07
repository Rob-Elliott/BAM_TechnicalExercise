using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
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

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly StargateContext _context;
        public CreatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }
        public Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            var existingPerson = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (existingPerson is not null)
                throw new BadHttpRequestException($"Bad Request, person with name '{request.Name}' already exists");

            return Task.CompletedTask;
        }
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
    {
        private readonly StargateContext _context;

        public CreatePersonHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var newPerson = new Person()
            {
                Name = request.Name
            };

            await _context.People.AddAsync(newPerson);

            await _context.SaveChangesAsync();

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
