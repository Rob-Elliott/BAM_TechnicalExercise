using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Queries;

namespace StarGateTests
{
    public class QueryHandlerTests
    {
        [Fact]
        public async Task GetPeople_ReturnsSeededPeople()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new GetPeopleHandler(ctx);

            var result = await handler.Handle(new GetPeople(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.People.Count >= 1);
            Assert.True(result.Success);
        }

        [Fact]
        public async Task GetPersonByName_ReturnsPerson_WhenExists()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new GetPersonByNameHandler(ctx);

            var result = await handler.Handle(new GetPersonByName { Name = "John Doe" }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Person);
            Assert.Equal("John Doe", result.Person.Name);
        }

        [Fact]
        public async Task GetPersonByName_Throws_WhenNotFound()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new GetPersonByNameHandler(ctx);

            await Assert.ThrowsAsync<BadHttpRequestException>(() => handler.Handle(new GetPersonByName { Name = "NoSuchPerson" }, CancellationToken.None));
        }

        [Fact]
        public async Task GetAstronautDutiesByName_ReturnsDuties_WhenExists()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new GetAstronautDutiesByNameHandler(ctx);

            // John Young (seeded) has multiple duties
            var result = await handler.Handle(new GetAstronautDutiesByName { Name = "John Young" }, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Person);
            Assert.True(result.AstronautDuties.Count > 0);
        }

        [Fact]
        public async Task GetAstronautDutiesByName_Throws_WhenNotFound()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new GetAstronautDutiesByNameHandler(ctx);

            await Assert.ThrowsAsync<BadHttpRequestException>(() => handler.Handle(new GetAstronautDutiesByName { Name = "Nobody" }, CancellationToken.None));
        }
    }
}
