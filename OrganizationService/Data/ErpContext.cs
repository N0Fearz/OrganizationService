using Microsoft.EntityFrameworkCore;
using OrganizationService.Models;

namespace OrganizationService.Data;

public class ErpContext(DbContextOptions<ErpContext> options) : DbContext(options)
{
    
}