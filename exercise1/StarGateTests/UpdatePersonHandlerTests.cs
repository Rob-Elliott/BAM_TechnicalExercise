using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Commands;

namespace StarGateTests
{
    public class UpdatePersonHandlerTests
    {
        [Fact]
        public async Task Preprocessor_Throws_WhenPersonNotFound()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var pre = new UpdatePersonPreProcessor(ctx);

            var req = new UpdatePerson { Id = 9999, Name = "No One" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => pre.Process(req, CancellationToken.None));
        }

        [Fact]
        public async Task Preprocessor_Throws_WhenDuplicateNameExists()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var pre = new UpdatePersonPreProcessor(ctx);

            // Seed contains John Doe (Id=1) and Jane Doe (Id=2)
            var req = new UpdatePerson { Id = 1, Name = "Jane Doe" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => pre.Process(req, CancellationToken.None));
        }

        [Fact]
        public async Task Handler_Throws_WhenPersonNotFound()
        {
            var ctx = TestDbFactory.CreateSqliteInMemoryContext();

            var handler = new UpdatePersonHandler(ctx);

            var req = new UpdatePerson { Id = 9999, Name = "Nobody" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => handler.Handle(req, CancellationToken.None));
        }
    }
}
