using NSubstitute;
using OrganizationService;
using OrganizationService.Repository;
using OrganizationService.Services;

namespace OrganizationTests;

public class UnitTest1
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IOrganizationService _organizationService;

    public UnitTest1()
    {
        _organizationRepository = Substitute.For<IOrganizationRepository>();
        _organizationService = new OrganizationService.Services.OrganizationService(_organizationRepository);
    }
    [Fact]
    public void AddOrganization_Should_Return_SchemaName()
    {
        //arrange
        var organizationMessage =
            "{\"id\":\"ac4ec53c-45da-4bec-939a-08e9a360f546\",\"name\":\"organization\",\"alias\":\"organization\",\"enabled\":true,\"description\":\"\",\"redirectUrl\":\"\",\"attributes\":{},\"domains\":[{\"name\":\"1233.bsr\",\"verified\":false}]}";
        //act
        var result = _organizationService.AddOrganization(organizationMessage);
        //assert
        Assert.Equal("schema_organization", result);
    }
    
    [Fact]
    public void AddOrganization_Should_Return_error()
    {
        //arrange
        var organizationMessage =
            "{\"id\":\"ac4ec53c-45da-4bec-939a-08e9a360f546\",\"alias\":\"organization\",\"enabled\":true,\"description\":\"\",\"redirectUrl\":\"\",\"attributes\":{},\"domains\":[{\"name\":\"1233.bsr\",\"verified\":false}]}";
        //act and assert
        Assert.Throws<ArgumentNullException>(() => _organizationService.AddOrganization(organizationMessage));
    }
}ganizationMessage));
    }
}