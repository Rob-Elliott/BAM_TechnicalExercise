using System.Net;
using System.Xml.Linq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;

namespace StargateAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PersonController> _logger;
        public PersonController(IMediator mediator, ILogger<PersonController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("")] // GET /api/People 
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople() { });

                _logger.LogInformation($"GetPeople(): Success");
                return this.GetResponse(result);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"GetPeople(): BadRequest", ex);
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetPeople(): ServerError", ex);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
                });

                _logger.LogInformation($"GetPersonByName({name}): Success");
                return this.GetResponse(result);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"GetPersonByName({name}): BadRequest", ex);
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetPersonByName({name}): ServerError", ex);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            try
            {
                var result = await _mediator.Send(new CreatePerson()
                {
                    Name = name
                });

                _logger.LogInformation($"CreatePerson({name}): Success");

                return this.GetResponse(result);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"CreatePerson({name}): BadRequest", ex);
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CreatePerson({name}): ServerError", ex);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }

        }

        [HttpPut("")]
        public async Task<IActionResult> UpdatePerson([FromBody] UpdatePerson request)
        {
            try
            {
                var result = await _mediator.Send(request);
                _logger.LogInformation($"UpdatePerson({request.Id}): Success");
                return this.GetResponse(result);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError($"UpdatePerson({request.Id}): BadRequest", ex);
                return this.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"UpdatePerson({request.Id}): ServerError", ex);
                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}