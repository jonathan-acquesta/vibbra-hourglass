using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vibbra.Hourglass.Api.DTOs;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Service.Exceptions;
using Vibbra.Hourglass.Service.Interfaces;

namespace Vibbra.Hourglass.Api.Controllers
{
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TimesController : ControllerBase
    {
        #region Fields

        private readonly ITimeService _timeService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public TimesController(ITimeService timeService, IMapper mapper)
        {
            _timeService = timeService;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        [HttpGet("{projectID}")]
        [ProducesResponseType(typeof(List<TimeResponseDTO>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 404)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 422)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> Get(int projectID)
        {
            try
            {
                if (projectID < 1)
                {
                    return UnprocessableEntity(new ErrorResponseDTO { Message = "invalid project id" });
                }

                var times = await _timeService.FindAllByProject(projectID);
                return Ok(_mapper.Map<List<TimeResponseDTO>>(times));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponseDTO { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDTO { Message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(TimeResponseDTO), 201)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 409)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 422)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> Create([FromBody] TimeRequestDTO timeRequestDTO)
        {
            if (timeRequestDTO == null)
            {
                return UnprocessableEntity(new ErrorResponseDTO { Message = "Invalid time posting data" });
            }

            try
            {
                var time = await _timeService.Add(_mapper.Map<TimeDomain>(timeRequestDTO));
                return Ok(_mapper.Map<TimeResponseDTO>(time));
            }
            catch (DuplicateItemException ex)
            {
                return Conflict(new ErrorResponseDTO() { Message = ex.Message });
            }
            catch (RequiredFieldException ex)
            {
                return UnprocessableEntity(new ErrorResponseDTO() { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDTO() { Message = ex.Message });
            }
        }

        [HttpPut("{timeID}")]
        [ProducesResponseType(typeof(TimeResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 404)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 422)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> Update(int timeID, [FromBody] TimeRequestDTO timeRequestDTO)
        {
            if (timeRequestDTO == null)
            {
                return UnprocessableEntity(new ErrorResponseDTO { Message = "Invalid time posting data" });
            }

            try
            {
                var time = _mapper.Map<TimeDomain>(timeRequestDTO);
                time.ID = timeID;

                var updatedTime = await _timeService.Update(time);
                return Ok(_mapper.Map<TimeResponseDTO>(updatedTime));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ErrorResponseDTO() { Message = ex.Message });
            }
            catch (DuplicateItemException ex)
            {
                return Conflict(new ErrorResponseDTO() { Message = ex.Message });
            }
            catch (RequiredFieldException ex)
            {
                return UnprocessableEntity(new ErrorResponseDTO() { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDTO() { Message = ex.Message });
            }
        }

        #endregion
    }
}
