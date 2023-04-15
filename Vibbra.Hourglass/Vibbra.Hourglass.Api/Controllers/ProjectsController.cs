using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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
    public class ProjectsController : ControllerBase
    {
        #region Fields

        private readonly IProjectService _projectService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public ProjectsController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 404)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 422)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id < 1)
                {
                    return UnprocessableEntity(new ErrorResponseDTO { Message = "id inválido" });
                }

                var project = await _projectService.Find(id);
                return Ok(_mapper.Map<ProjectResponseDTO>(project));
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

        [HttpGet()]
        [ProducesResponseType(typeof(ProjectResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 404)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var projects = await _projectService.FindAll();
                return Ok(_mapper.Map<List<ProjectResponseDTO>>(projects));
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
        [ProducesResponseType(typeof(ProjectResponseDTO), 201)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 409)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 422)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> Create([FromBody] ProjectRequestDTO projectRequestDTO)
        {
            if (projectRequestDTO == null)
            {
                return UnprocessableEntity(new ErrorResponseDTO { Message = "Dados do projeto inválido" });
            }

            try
            {
                var project = await _projectService.Add(_mapper.Map<ProjectDomain>(projectRequestDTO));
                return Ok(_mapper.Map<ProjectResponseDTO>(project));
            }
            catch (DuplicateItemException ex)
            {
                return Conflict(new ErrorResponseDTO() { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDTO() { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectResponseDTO), 200)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 404)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 422)]
        [ProducesResponseType(typeof(ErrorResponseDTO), 500)]
        public async Task<IActionResult> Update(int id, [FromBody] ProjectRequestDTO projectRequestDTO)
        {
            if (projectRequestDTO == null)
            {
                return UnprocessableEntity(new ErrorResponseDTO { Message = "Dados do projeto inválido" });
            }

            try
            {
                var project = _mapper.Map<ProjectDomain>(projectRequestDTO);
                project.ID = id;

                var updatedProject = await _projectService.Update(project);
                return Ok(_mapper.Map<ProjectResponseDTO>(updatedProject));
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
