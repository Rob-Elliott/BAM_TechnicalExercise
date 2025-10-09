using System;
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
    public class CreatePersonHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesNewPerson_AndReturnsId()
        {
            var mockRepo = new Mock<IPersonRepository>();

            mockRepo.Setup(r => r.AddAsync(It.IsAny<Person>()))
                .ReturnsAsync((Person p) =>
                {
                    // simulate DB assigning an Id
                    p.Id = 1234;
                    return p;
                });

            var handler = new CreatePersonHandler(mockRepo.Object);

            var uniqueName = "Person_" + Guid.NewGuid().ToString();

            var req = new CreatePerson { Name = uniqueName };

            var result = await handler.Handle(req, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.Id > 0);

            mockRepo.Verify(r => r.AddAsync(It.Is<Person>(p => p.Name == uniqueName)), Times.Once);
        }

        [Fact]
        public async Task Preprocessor_Throws_WhenPersonAlreadyExists()
        {
            var mockRepo = new Mock<IPersonRepository>();

            // simulate existing person returned for "John Doe"
            mockRepo.Setup(r => r.GetPersonByNameAsync("John Doe"))
                .ReturnsAsync(new Person { Id = 1, Name = "John Doe" });

            var pre = new CreatePersonPreProcessor(mockRepo.Object);

            // Seed includes "John Doe"
            var req = new CreatePerson { Name = "John Doe" };

            await Assert.ThrowsAsync<BadHttpRequestException>(() => pre.Process(req, CancellationToken.None));
        }
    }
}
