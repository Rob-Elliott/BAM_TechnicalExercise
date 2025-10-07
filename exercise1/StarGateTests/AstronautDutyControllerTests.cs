using System.Net;
using System.Threading;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StargateAPI.Controllers;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using Xunit;

namespace StarGateTests
{
    public class AstronautDutyControllerTests
    {
        [Fact]
        public async Task GetAstronautDutiesByName_ReturnsOk_WhenMediatorReturnsSuccess()
        {
            var mediatorMock = new Mock<IMediator>();
            var mediatorResult = new GetAstronautDutiesByNameResult { Success = true, Message = "ok", ResponseCode = (int)HttpStatusCode.OK };
            mediatorMock.Setup(m => m.Send(It.IsAny<GetAstronautDutiesByName>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mediatorResult);

            var controller = new AstronautDutyController(mediatorMock.Object);

            var result = await controller.GetAstronautDutiesByName("neil");

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objResult.StatusCode);
            var body = Assert.IsType<GetAstronautDutiesByNameResult>(objResult.Value);
            Assert.True(body.Success);
        }

        [Fact]
        public async Task CreateAstronautDuty_ReturnsCreatedResponse_FromMediatorResult()
        {
            var mediatorMock = new Mock<IMediator>();
            var response = new CreateAstronautDutyResult { Success = true, Message = "created", ResponseCode = (int)HttpStatusCode.Created, Id = 123 };
            mediatorMock.Setup(m => m.Send(It.IsAny<CreateAstronautDuty>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(response);

            var controller = new AstronautDutyController(mediatorMock.Object);

            var request = new CreateAstronautDuty { Name = "Buzz Aldrin", Rank = "Commander", DutyTitle = "Pilot", DutyStartDate = System.DateTime.UtcNow.Date };

            var result = await controller.CreateAstronautDuty(request);

            var objResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.Created, objResult.StatusCode);
            var body = Assert.IsType<CreateAstronautDutyResult>(objResult.Value);
            Assert.Equal("created", body.Message);
        }
    }
}
