using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Repositories;
using StargateAPI.Business.Data;

namespace StarGateTests
{
    public class QueryHandlerTests
    {
        [Fact]
        public async Task GetPeople_ReturnsSeededPeople()
        {
            var people = new List<Person>
            {
                new Person { Id = 1, Name = "John Doe", AstronautDetail = new AstronautDetail { CurrentRank = "CAPT", CurrentDutyTitle = "Pilot", CareerStartDate = DateTime.UtcNow } }
            };

            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(people);

            var handler = new GetPeopleHandler(mockRepo.Object);

            var result = await handler.Handle(new GetPeople(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.People.Count >= 1);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task GetPersonByName_ReturnsPerson_WhenExists()
        {
            var person = new Person { Id = 1, Name = "John Doe", AstronautDetail = new AstronautDetail { CurrentRank = "CAPT", CurrentDutyTitle = "Pilot", CareerStartDate = DateTime.UtcNow } };

            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(r => r.GetPersonByNameAsync("John Doe")).ReturnsAsync(person);

            var handler = new GetPersonByNameHandler(mockRepo.Object);

            var result = await handler.Handle(new GetPersonByName { Name = "John Doe" }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Person);
            Assert.Equal("John Doe", result.Person.Name);
        }

        [Fact]
        public async Task GetPersonByName_Throws_WhenNotFound()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(r => r.GetPersonByNameAsync(It.IsAny<string>())).ReturnsAsync((Person?)null);

            var handler = new GetPersonByNameHandler(mockRepo.Object);

            await Assert.ThrowsAsync<BadHttpRequestException>(() => handler.Handle(new GetPersonByName { Name = "NoSuchPerson" }, CancellationToken.None));
        }

        [Fact]
        public async Task GetAstronautDutiesByName_ReturnsDuties_WhenExists()
        {
            var person = new Person { Id = 3, Name = "John Young", AstronautDetail = new AstronautDetail { CurrentRank = "GEN", CurrentDutyTitle = "Commander", CareerStartDate = DateTime.UtcNow } };
            var duties = new List<AstronautDuty> { new AstronautDuty { Id = 1, PersonId = 3, DutyStartDate = new DateTime(1980, 1, 1), DutyTitle = "Pilot", Rank = "CAPT" } };

            var mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonRepo.Setup(r => r.GetPersonByNameAsync("John Young")).ReturnsAsync(person);

            var mockDutyRepo = new Mock<IAstronautDutyRepository>();
            mockDutyRepo.Setup(r => r.GetAstronautDutiesByPersonIdAsync(3)).ReturnsAsync(duties);

            var handler = new GetAstronautDutiesByNameHandler(mockDutyRepo.Object, mockPersonRepo.Object);

            var result = await handler.Handle(new GetAstronautDutiesByName { Name = "John Young" }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Person);
            Assert.True(result.AstronautDuties.Count > 0);
        }

        [Fact]
        public async Task GetAstronautDutiesByName_Throws_WhenNotFound()
        {
            var mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonRepo.Setup(r => r.GetPersonByNameAsync(It.IsAny<string>())).ReturnsAsync((Person?)null);

            var mockDutyRepo = new Mock<IAstronautDutyRepository>();

            var handler = new GetAstronautDutiesByNameHandler(mockDutyRepo.Object, mockPersonRepo.Object);

            await Assert.ThrowsAsync<BadHttpRequestException>(() => handler.Handle(new GetAstronautDutiesByName { Name = "Nobody" }, CancellationToken.None));
        }
    }
}
