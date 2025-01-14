using OrganizationService.Models;

namespace OrganizationService.Services;

public interface ILogPublisher
{
    void SendMessage(LogMessage logMessage);
}