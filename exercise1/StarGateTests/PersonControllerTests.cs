using System.Net;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Repositories;
using StargateAPI.Controllers;

namespace StarGateTests
{
    public class PersonControllerTests
    {


        [Fact]
        public async Task GetPeople_ReturnsOkResponse_WhenMediatorReturnsSuccess()
        {
            var mediatorMock = new Mock<IMediator>();
            var mediatorResult = new GetPeopleResult { Success = true, Message = "ok", ResponseCode = (int)HttpStatusCode.OK };
            mediatorMock.Setup(m => m.Send(It.IsAny<GetPeople>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mediatorResult);

            var controller = new PersonController(mediatorMock.Object, Mock.Of<ILogger<PersonController>>());

            var result = await controller.GetPeople();

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objResult.StatusCode);
            var body = Assert.IsType<GetPeopleResult>(objResult.Value);
            Assert.True(body.Success);
        }

        [Fact]
        public async Task GetPersonByName_PropagatesBadRequest_WhenMediatorThrowsBadHttpRequestException()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(It.IsAny<GetPersonByName>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new BadHttpRequestException("bad"));

            var controller = new PersonController(mediatorMock.Object, Mock.Of<ILogger<PersonController>>());

            var result = await controller.GetPersonByName("Buzz Aldrin");

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("bad", bad.Value);
        }

        [Fact]
        public async Task CreatePerson_ReturnsResultFromMediator()
        {
            var mediatorMock = new Mock<IMediator>();
            var mediatorResult = new CreatePersonResult { Success = true, Message = "created", ResponseCode = (int)HttpStatusCode.OK, Id = 42 };
            mediatorMock.Setup(m => m.Send(It.IsAny<StargateAPI.Business.Commands.CreatePerson>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mediatorResult);

            var controller = new PersonController(mediatorMock.Object, Mock.Of<ILogger<PersonController>>());

            var result = await controller.CreatePerson("Buzz Aldrin");

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objResult.StatusCode);
            var body = Assert.IsType<CreatePersonResult>(objResult.Value);
            Assert.Equal(42, body.Id);
        }

        [Fact]
        public async Task UpdatePerson_ReturnsResultFromMediator()
        {
            var mediatorMock = new Mock<IMediator>();
            var mediatorResult = new UpdatePersonResult { Success = true, Message = "updated", ResponseCode = (int)HttpStatusCode.OK, Id = 99 };
            mediatorMock.Setup(m => m.Send(It.IsAny<StargateAPI.Business.Commands.UpdatePerson>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mediatorResult);

            var controller = new PersonController(mediatorMock.Object, Mock.Of<ILogger<PersonController>>());

            var request = new StargateAPI.Business.Commands.UpdatePerson { Id = 99, Name = "Buzz TheOther" };

            var result = await controller.UpdatePerson(request);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objResult.StatusCode);
            var body = Assert.IsType<UpdatePersonResult>(objResult.Value);
            Assert.Equal(99, body.Id);
        }
    }
}
