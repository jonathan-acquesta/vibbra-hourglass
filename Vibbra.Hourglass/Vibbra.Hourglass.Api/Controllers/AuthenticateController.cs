using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Vibbra.Hourglass.Api.DTOs;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Service.Exceptions;
using Vibbra.Hourglass.Service.Interfaces;

namespace Vibbra.Hourglass.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public AuthenticateController(IAuthenticationService authenticationService, IMapper mapper)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        #endregion 

        #region Methods

        [HttpPost()]
        [ProducesResponseType(typeof(LoginResponseDTO), 200)]
        [ProducesResponseType(typeof(ErroResponseDTO), 404)]
        public async Task<ActionResult> Login(
          [FromBody] LoginRequestDTO loginRequestDTO)
        {
            if (loginRequestDTO == null)
                return BadRequest();

            try
            {
                var authUser = await _authenticationService.Authentication(_mapper.Map<UserDomain>(loginRequestDTO));
                LoginResponseDTO loginResponseDTO = new LoginResponseDTO() { Token = authUser.Item1, User = _mapper.Map<UserResponseDTO>(authUser.Item2) };

                return new JsonResult(loginResponseDTO);
            }
            catch (InvalidPasswordException ex)
            {
                return UnprocessableEntity(new ErroResponseDTO() { Message = ex.Message });
            }
            catch (UserNotFoundException ex)
            {
                return Unauthorized(new ErroResponseDTO() { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ErroResponseDTO() { Message = ex.Message });
            }
        }

        #endregion
    }
}
