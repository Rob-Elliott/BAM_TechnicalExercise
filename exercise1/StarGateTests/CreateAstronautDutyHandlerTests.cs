using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Commands;

namespace StarGateTests
{
    public class CreateAstronautDutyHandlerTests
    {

        [Fact]
        public async Task Handle_CreatesNewAstronautDuty_AndUpdatesDetail()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new CreateAstronautDutyHandler(ctx);

            // Use an existing seeded person name (John Doe has Id = 1)
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

            var created = ctx.AstronautDuties.FirstOrDefault(d => d.Id == result.Id.Value);
            Assert.NotNull(created);
            Assert.Equal(req.DutyTitle, created.DutyTitle);

            var detail = ctx.AstronautDetails.FirstOrDefault(d => d.PersonId == created.PersonId);
            Assert.NotNull(detail);
            Assert.Equal(req.Rank, detail.CurrentRank);
            Assert.Equal(req.DutyTitle, detail.CurrentDutyTitle);
        }

        [Fact]
        public async Task Preprocessor_Throws_WhenPersonNotFound()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var pre = new CreateAstronautDutyPreProcessor(ctx);

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
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var pre = new CreateAstronautDutyPreProcessor(ctx);

            // John Doe (seeded Id=1) has an existing duty at 2025-04-05 per seed data
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
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new CreateAstronautDutyHandler(ctx);

            // Jane Doe (seeded Id=2) has no AstronautDetail
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

            var detail = ctx.AstronautDetails.FirstOrDefault(d => d.PersonId == ctx.People.First(p => p.Name == "Jane Doe").Id);
            Assert.NotNull(detail);
            Assert.Equal(req.Rank, detail.CurrentRank);
            Assert.Equal(req.DutyTitle, detail.CurrentDutyTitle);
            Assert.Equal(req.DutyStartDate.Date, detail.CareerStartDate);
            Assert.Null(detail.CareerEndDate);
        }

        [Fact]
        public async Task Handle_UpdatesMostRecentDutyEndDate_WhenAddingNewChronologicalDuty()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new CreateAstronautDutyHandler(ctx);

            // John Young (seeded Id=3) has a most recent duty that ends 1983-01-01
            var person = ctx.People.First(p => p.Name == "John Young");
            var mostRecent = ctx.AstronautDuties.Where(d => d.PersonId == person.Id).OrderByDescending(d => d.DutyStartDate).First();

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

            var updatedMostRecent = ctx.AstronautDuties.First(d => d.Id == mostRecent.Id);
            Assert.NotNull(updatedMostRecent.DutyEndDate);
            Assert.Equal(newStart.AddDays(-1).Date, updatedMostRecent.DutyEndDate.Value.Date);
        }

        [Fact]
        public async Task Handle_SetsCareerEndDate_OnRetirement_WhenDetailExists()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new CreateAstronautDutyHandler(ctx);

            // John Doe (Id=1) has an AstronautDetail without CareerEndDate
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

            var detail = ctx.AstronautDetails.First(d => d.PersonId == ctx.People.First(p => p.Name == "John Doe").Id);
            Assert.NotNull(detail.CareerEndDate);
            Assert.Equal(req.DutyStartDate.AddDays(-1).Date, detail.CareerEndDate.Value.Date);
        }

        [Fact]
        public async Task Handle_SetsCareerEndDate_OnRetirement_WhenDetailDidNotExist()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new CreateAstronautDutyHandler(ctx);

            // Jane Doe (Id=2) has no detail; requesting RETIRED should set CareerEndDate to DutyStartDate
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

            var detail = ctx.AstronautDetails.First(d => d.PersonId == ctx.People.First(p => p.Name == "Jane Doe").Id);
            Assert.NotNull(detail);
            Assert.NotNull(detail.CareerEndDate);
            Assert.Equal(req.DutyStartDate.Date, detail.CareerEndDate.Value.Date);
        }
    }
}
