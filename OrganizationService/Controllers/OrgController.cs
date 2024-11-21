using Microsoft.AspNetCore.Mvc;
using OrganizationService.Repository;

namespace OrganizationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrgController : ControllerBase
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserRepository _userRepository;
        public OrgController(IOrganizationRepository organizationRepository, IUserRepository userRepository)
        {
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var organizations = _organizationRepository.GetOrganizations();
            return Ok(organizations);
        }
    }
}
