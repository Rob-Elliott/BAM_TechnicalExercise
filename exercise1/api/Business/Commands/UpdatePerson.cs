using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    /// <summary>
    /// Updates an existing person record
    /// </summary>
    public class UpdatePerson : IRequest<UpdatePersonResult>
    {
        public required int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
    }

    public class UpdatePersonPreProcessor : IRequestPreProcessor<UpdatePerson>
    {
        private readonly StargateContext _context;
        public UpdatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }
        public Task Process(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking()
                .FirstOrDefault(z => z.Id == request.Id);
            
            if (person is null)
                throw new BadHttpRequestException($"Person [{request.Id}] not found");
            
            // don't allow changing the name to an existing name
            var duplicate = _context.People.AsNoTracking()
                .FirstOrDefault(z => z.Name.ToLower() == request.Name.ToLower() && z.Id != request.Id);
            
            if (duplicate is not null)
                throw new BadHttpRequestException($"Cannot update Person [{request.Id}], name '{request.Name}' already exists");
            
            return Task.CompletedTask;
        }
    }

    public class UpdatePersonHandler : IRequestHandler<UpdatePerson, UpdatePersonResult>
    {
        private readonly StargateContext _context;
        public UpdatePersonHandler(StargateContext context)
        {
            _context = context;
        }
        public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await _context.People.FirstOrDefaultAsync(z => z.Id == request.Id, cancellationToken);
            if (person == null) throw new BadHttpRequestException("Person not found");
            person.Name = request.Name;
            _context.People.Update(person);
            await _context.SaveChangesAsync(cancellationToken);
            return new UpdatePersonResult()
            {
                Id = person.Id
            };
        }
    }

    public class UpdatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}
