using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Repositories;
using StargateAPI.Business.Data;

namespace StarGateTests
{
    public class CreateAstronautDutyHandlerTests
    {

        [Fact]
        public async Task Handle_CreatesNewAstronautDuty_AndUpdatesDetail()
        {
            var person = new Person { Id = 1, Name = "John Doe", AstronautDetail = new AstronautDetail() };

            var mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonRepo.Setup(r => r.GetPersonByNameAsync("John Doe")).ReturnsAsync(person);
            mockPersonRepo.Setup(r => r.UpdateAsync(It.IsAny<Person>())).ReturnsAsync((Person p) => p);

            var mockDutyRepo = new Mock<IAstronautDutyRepository>();
            mockDutyRepo.Setup(r => r.GetAstronautDutiesByPersonIdAsync(1)).ReturnsAsync(new List<AstronautDuty>());
            mockDutyRepo.Setup(r => r.UpdateAsync(It.IsAny<AstronautDuty>())).ReturnsAsync((AstronautDuty d) => d);
            mockDutyRepo.Setup(r => r.AddAsync(It.IsAny<AstronautDuty>())).ReturnsAsync((AstronautDuty d) => { d.Id = 55; return d; });

            var handler = new CreateAstronautDutyHandler(mockDutyRepo.Object, mockPersonRepo.Object);

            var req = new CreateAstronautDuty
            {
                Name = "John Doe",
                Rank = "MAJ",
                DutyTitle = "Mission Lead",
                DutyStartDate = new DateTime(2026, 01, 01)
            };

            var result = await handler.Handle(req, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Id.HasValue);
            // handler adds the new duty to the Person and updates the Person via personRepository
            mockPersonRepo.Verify(r => r.UpdateAsync(It.Is<Person>(p => p.AstronautDuties.Any(d => d.DutyTitle == req.DutyTitle) && p.AstronautDetail.CurrentRank == req.Rank && p.AstronautDetail.CurrentDutyTitle == req.DutyTitle)), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Preprocessor_Throws_WhenPersonNotFound()
        {
            var mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonRepo.Setup(r => r.GetPersonByNameAsync(It.IsAny<string>())).ReturnsAsync((Person?)null);

            var mockDutyRepo = new Mock<IAstronautDutyRepository>();

            var pre = new CreateAstronautDutyPreProcessor(mockDutyRepo.Object, mockPersonRepo.Object);

            var req = new CreateAstronautDuty
            {
                Name = "Non Existent",
                Rank = "1LT",
                DutyTitle = "Pilot",
                DutyStartDate = DateTime.UtcNow.Date
            };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => pre.Process(req, CancellationToken.None));
        }

        [Fact]
        public async Task Preprocessor_Throws_WhenDutyStartDateAlreadyExists()
        {
            var person = new Person { Id = 1, Name = "John Doe" };
            var existingDuty = new AstronautDuty { Id = 2, PersonId = 1, DutyStartDate = new DateTime(2025, 04, 05) };

            var mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonRepo.Setup(r => r.GetPersonByNameAsync("John Doe")).ReturnsAsync(person);

            var mockDutyRepo = new Mock<IAstronautDutyRepository>();
            mockDutyRepo.Setup(r => r.GetAstronautDutyByPersonIdAndStartDateAsync(1, new DateTime(2025, 04, 05))).ReturnsAsync(existingDuty);

            var pre = new CreateAstronautDutyPreProcessor(mockDutyRepo.Object, mockPersonRepo.Object);

            var req = new CreateAstronautDuty
            {
                Name = "John Doe",
                Rank = "1LT",
                DutyTitle = "Commander",
                DutyStartDate = new DateTime(2025, 04, 05)
            };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => pre.Process(req, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CreatesAstronautDetail_WhenNoneExists()
        {
            var person = new Person { Id = 2, Name = "Jane Doe", AstronautDetail = null };

            var mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonRepo.Setup(r => r.GetPersonByNameAsync("Jane Doe")).ReturnsAsync(person);
            mockPersonRepo.Setup(r => r.UpdateAsync(It.IsAny<Person>())).ReturnsAsync((Person p) => p);

            var mockDutyRepo = new Mock<IAstronautDutyRepository>();
            mockDutyRepo.Setup(r => r.GetAstronautDutiesByPersonIdAsync(2)).ReturnsAsync(new List<AstronautDuty>());
            mockDutyRepo.Setup(r => r.AddAsync(It.IsAny<AstronautDuty>())).ReturnsAsync((AstronautDuty d) => { d.Id = 66; return d; });

            var handler = new CreateAstronautDutyHandler(mockDutyRepo.Object, mockPersonRepo.Object);

            var req = new CreateAstronautDuty
            {
                Name = "Jane Doe",
                Rank = "2LT",
                DutyTitle = "Navigator",
                DutyStartDate = new DateTime(2026, 05, 01)
            };

            var result = await handler.Handle(req, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Id.HasValue);
            mockPersonRepo.Verify(r => r.UpdateAsync(It.Is<Person>(p => p.AstronautDetail != null && p.AstronautDetail.CurrentRank == req.Rank)), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_UpdatesMostRecentDutyEndDate_WhenAddingNewChronologicalDuty()
        {
            var person = new Person { Id = 3, Name = "John Young", AstronautDetail = new AstronautDetail() };
            var mostRecent = new AstronautDuty { Id = 10, PersonId = 3, DutyStartDate = new DateTime(1983, 01, 01) };

            var mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonRepo.Setup(r => r.GetPersonByNameAsync("John Young")).ReturnsAsync(person);
            mockPersonRepo.Setup(r => r.UpdateAsync(It.IsAny<Person>())).ReturnsAsync((Person p) => p);

            var mockDutyRepo = new Mock<IAstronautDutyRepository>();
            mockDutyRepo.Setup(r => r.GetAstronautDutiesByPersonIdAsync(3)).ReturnsAsync(new List<AstronautDuty> { mostRecent });
            mockDutyRepo.Setup(r => r.UpdateAsync(It.IsAny<AstronautDuty>())).ReturnsAsync((AstronautDuty d) => d);
            mockDutyRepo.Setup(r => r.AddAsync(It.IsAny<AstronautDuty>())).ReturnsAsync((AstronautDuty d) => { d.Id = 77; return d; });

            var handler = new CreateAstronautDutyHandler(mockDutyRepo.Object, mockPersonRepo.Object);

            var newStart = new DateTime(1984, 01, 01);

            var req = new CreateAstronautDuty
            {
                Name = person.Name,
                Rank = "GEN",
                DutyTitle = "Supreme Commander",
                DutyStartDate = newStart
            };

            var result = await handler.Handle(req, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Id.HasValue);
            mockDutyRepo.Verify(r => r.UpdateAsync(It.Is<AstronautDuty>(d => d.Id == mostRecent.Id && d.DutyEndDate == newStart.AddDays(-1).Date)), Times.Once);
        }

        [Fact]
        public async Task Handle_SetsCareerEndDate_OnRetirement_WhenDetailExists()
        {
            var person = new Person { Id = 1, Name = "John Doe", AstronautDetail = new AstronautDetail() };

            var mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonRepo.Setup(r => r.GetPersonByNameAsync("John Doe")).ReturnsAsync(person);
            mockPersonRepo.Setup(r => r.UpdateAsync(It.IsAny<Person>())).ReturnsAsync((Person p) => p);

            var mockDutyRepo = new Mock<IAstronautDutyRepository>();
            mockDutyRepo.Setup(r => r.GetAstronautDutiesByPersonIdAsync(1)).ReturnsAsync(new List<AstronautDuty>());
            mockDutyRepo.Setup(r => r.AddAsync(It.IsAny<AstronautDuty>())).ReturnsAsync((AstronautDuty d) => { d.Id = 88; return d; });

            var handler = new CreateAstronautDutyHandler(mockDutyRepo.Object, mockPersonRepo.Object);

            var req = new CreateAstronautDuty
            {
                Name = "John Doe",
                Rank = "CPT",
                DutyTitle = "RETIRED",
                DutyStartDate = new DateTime(2026, 12, 31)
            };

            var result = await handler.Handle(req, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Id.HasValue);
            mockPersonRepo.Verify(r => r.UpdateAsync(It.Is<Person>(p => p.AstronautDetail.CareerEndDate == req.DutyStartDate.AddDays(-1).Date)), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_SetsCareerEndDate_OnRetirement_WhenDetailDidNotExist()
        {
            var person = new Person { Id = 2, Name = "Jane Doe", AstronautDetail = null };

            var mockPersonRepo = new Mock<IPersonRepository>();
            mockPersonRepo.Setup(r => r.GetPersonByNameAsync("Jane Doe")).ReturnsAsync(person);
            mockPersonRepo.Setup(r => r.UpdateAsync(It.IsAny<Person>())).ReturnsAsync((Person p) => p);

            var mockDutyRepo = new Mock<IAstronautDutyRepository>();
            mockDutyRepo.Setup(r => r.GetAstronautDutiesByPersonIdAsync(2)).ReturnsAsync(new List<AstronautDuty>());
            mockDutyRepo.Setup(r => r.AddAsync(It.IsAny<AstronautDuty>())).ReturnsAsync((AstronautDuty d) => { d.Id = 99; return d; });

            var handler = new CreateAstronautDutyHandler(mockDutyRepo.Object, mockPersonRepo.Object);

            var req = new CreateAstronautDuty
            {
                Name = "Jane Doe",
                Rank = "CPT",
                DutyTitle = "RETIRED",
                DutyStartDate = new DateTime(2027, 03, 15)
            };

            var result = await handler.Handle(req, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Id.HasValue);
            mockPersonRepo.Verify(r => r.UpdateAsync(It.Is<Person>(p => p.AstronautDetail != null && p.AstronautDetail.CareerEndDate == req.DutyStartDate.Date)), Times.AtLeastOnce);
        }
    }
}
