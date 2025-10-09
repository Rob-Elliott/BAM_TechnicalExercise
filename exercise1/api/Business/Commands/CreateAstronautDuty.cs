using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Repositories;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    /// <summary>
    /// Creates an AstronautDuty for an existing person, updates previous duty
    /// </summary>
    public class CreateAstronautDuty
        : IRequest<CreateAstronautDutyResult>
    {
        public required string Name { get; set; }

        public required string Rank { get; set; }

        public required string DutyTitle { get; set; }

        public DateTime DutyStartDate { get; set; }
    }

    public class CreateAstronautDutyPreProcessor(IAstronautDutyRepository repository, IPersonRepository personRepository)
        : IRequestPreProcessor<CreateAstronautDuty>
    {
        public async Task Process(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = await personRepository.GetPersonByNameAsync(request.Name);

            if (person is null)
                throw new BadHttpRequestException($"Bad Request, person '{request.Name}' not found");

            // make sure we don't have an existing startdate for this person
            var existingDuty = repository.GetAstronautDutyByPersonIdAndStartDateAsync(person.Id, request.DutyStartDate);

            if (existingDuty is not null)
                throw new BadHttpRequestException($"Bad Request, Duty for '{request.Name}' and start date {request.DutyStartDate} already exists");
        }
    }

    public class CreateAstronautDutyHandler(IAstronautDutyRepository repository, IPersonRepository personRepository)
        : IRequestHandler<CreateAstronautDuty, CreateAstronautDutyResult>
    {
        public async Task<CreateAstronautDutyResult> Handle(CreateAstronautDuty request, CancellationToken cancellationToken)
        {
            var person = await personRepository.GetPersonByNameAsync(request.Name);

            if (person is null)
                throw new BadHttpRequestException($"Bad Request, person '{request.Name}' not found");

            // if the person has no detail, that means they have no duty history. Add one.
            if (person.AstronautDetail == null)
            {
                person.AstronautDetail = new AstronautDetail();
                person.AstronautDetail.PersonId = person.Id;
                person.AstronautDetail.CurrentDutyTitle = request.DutyTitle;
                person.AstronautDetail.CurrentRank = request.Rank;
                person.AstronautDetail.CareerStartDate = request.DutyStartDate.Date;
                if (request.DutyTitle.ToUpper() == "RETIRED")
                {
                    person.AstronautDetail.CareerEndDate = request.DutyStartDate.Date;
                }
            }
            else
            {
                person.AstronautDetail.CurrentDutyTitle = request.DutyTitle;
                person.AstronautDetail.CurrentRank = request.Rank;
                if (request.DutyTitle.ToUpper() == "RETIRED")
                {
                    person.AstronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }
            }

            await personRepository.UpdateAsync(person); // probably not necessary, called again below

            // we are assuming the new Duty is chronologically after the most recent Duty in the DB.
            // update most recent Duty End Date.

            var mostRecentDuty = (await repository.GetAstronautDutiesByPersonIdAsync(person.Id)).FirstOrDefault();

            if (mostRecentDuty is not null)
            {
                mostRecentDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                await repository.UpdateAsync(mostRecentDuty);
            }

            var newAstronautDuty = new AstronautDuty()
            {
                PersonId = person.Id,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            person.AstronautDuties.Add(newAstronautDuty);

            await personRepository.UpdateAsync(person);

            return new CreateAstronautDutyResult()
            {
                Id = newAstronautDuty.Id
            };
        }
    }

    public class CreateAstronautDutyResult : BaseResponse
    {
        public int? Id { get; set; }
    }
}
