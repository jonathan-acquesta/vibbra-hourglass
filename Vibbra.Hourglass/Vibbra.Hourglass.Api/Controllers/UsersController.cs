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
    public class UsersController : ControllerBase
    {

        #region Fields

        private readonly IUserService _userService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        #endregion

        #region Methods
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 404)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 422)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 1)
                {
                    return UnprocessableEntity(new ErrorResponseDTO { Message = "ID inválido" });
                }

                var user = await _userService.Find(id);
                return Ok(_mapper.Map<UserResponseDTO>(user));
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
        [ProducesResponseType(typeof(UserResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 409)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 422)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> Create([FromBody] UserRequestDTO userRequestDTO)
        {
            if (userRequestDTO == null)
            {
                return UnprocessableEntity(new ErrorResponseDTO { Message = "Dados do usuário inválidos" });
            }

            try
            {
                var user = await _userService.Add(_mapper.Map<UserDomain>(userRequestDTO));
                return Ok(_mapper.Map<UserRequestDTO>(user));
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

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 404)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 409)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 422)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> Update(int id, [FromBody] UserRequestDTO userRequestDTO)
        {
            if (id < 1)
            {
                return UnprocessableEntity(new ErrorResponseDTO { Message = "ID inválido" });
            }

            if (userRequestDTO == null)
            {
                return UnprocessableEntity(new ErrorResponseDTO { Message = "Dados do usuário inválidos" });
            }

            try
            {
                var user = _mapper.Map<UserDomain>(userRequestDTO);
                user.ID = id;

                var updatedUser = await _userService.Update(user);
                return Ok(_mapper.Map<UserResponseDTO>(updatedUser));
            }
            catch(NotFoundException ex)
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
