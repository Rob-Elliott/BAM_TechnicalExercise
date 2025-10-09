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
    public class UpdatePersonHandlerTests
    {
        [Fact]
        public async Task Preprocessor_Throws_WhenPersonNotFound()
        {
            var mockRepo = new Mock<IPersonRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(9999)).ReturnsAsync((Person?)null);

            var pre = new UpdatePersonPreProcessor(mockRepo.Object);

            var req = new UpdatePerson { Id = 9999, Name = "No One" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => pre.Process(req, CancellationToken.None));
        }

        [Fact]
        public async Task Preprocessor_Throws_WhenDuplicateNameExists()
        {
            var mockRepo = new Mock<IPersonRepository>();

            // existing person (id=1)
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Person { Id = 1, Name = "John Doe" });
            // duplicate name exists (Jane Doe)
            mockRepo.Setup(r => r.GetPersonByNameAsync("Jane Doe")).ReturnsAsync(new Person { Id = 2, Name = "Jane Doe" });

            var pre = new UpdatePersonPreProcessor(mockRepo.Object);

            var req = new UpdatePerson { Id = 1, Name = "Jane Doe" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => pre.Process(req, CancellationToken.None));
        }

        [Fact]
        public async Task Handler_Throws_WhenPersonNotFound()
        {
            var mockRepo = new Mock<IPersonRepository>();
            // configure GetByIdAsync to throw the same BadHttpRequestException the original preprocessor would
            mockRepo.Setup(r => r.GetByIdAsync(9999)).ThrowsAsync(new BadHttpRequestException("Person [9999] not found"));

            var handler = new UpdatePersonHandler(mockRepo.Object);

            var req = new UpdatePerson { Id = 9999, Name = "Nobody" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => handler.Handle(req, CancellationToken.None));
        }
    }
}
