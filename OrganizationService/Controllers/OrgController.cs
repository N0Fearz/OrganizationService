using Microsoft.AspNetCore.Mvc;
using OrganizationService.Repository;

namespace OrganizationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrgController : ControllerBase
    {
        private readonly IOrganizationRepository _organizationRepository;
        public OrgController(IOrganizationRepository organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        [HttpGet]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            var organizations = _organizationRepository.GetOrganizations();
            return Ok(organizations);
        }

        [HttpGet("{id}")]
        [ProducesResponseType<int>(StatusCodes.Status200OK)]
        public IActionResult GetOrganization(Guid id)
        {
            var organization = _organizationRepository.GetOrganizationById(id);
            return Ok(organization);
        }
    }
}
