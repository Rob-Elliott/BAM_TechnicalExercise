using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Repositories;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    /// <summary>
    /// Updates an existing person record
    /// </summary>
    public class UpdatePerson
        : IRequest<UpdatePersonResult>
    {
        public required int Id { get; set; }
        public required string Name { get; set; } = string.Empty;
    }

    public class UpdatePersonPreProcessor(IPersonRepository repository)
        : IRequestPreProcessor<UpdatePerson>
    {

        public async Task Process(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await repository.GetByIdAsync(request.Id);

            if (person is null)
                throw new BadHttpRequestException($"Person [{request.Id}] not found");

            // don't allow changing the name to an existing name
            var duplicate = await repository.GetPersonByNameAsync(request.Name);

            if (duplicate is not null)
                throw new BadHttpRequestException($"Cannot update Person [{request.Id}], name '{request.Name}' already exists");
        }
    }

    public class UpdatePersonHandler(IPersonRepository repository)
        : IRequestHandler<UpdatePerson, UpdatePersonResult>
    {

        public async Task<UpdatePersonResult> Handle(UpdatePerson request, CancellationToken cancellationToken)
        {
            var person = await repository.GetByIdAsync(request.Id);

            person.Name = request.Name;
            person = await repository.UpdateAsync(person);
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
