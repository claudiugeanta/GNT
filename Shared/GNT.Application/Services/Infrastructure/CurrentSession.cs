using GNT.Application.Interfaces;
using GNT.Shared.Dtos.UserManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GNT.Application.Services.Infrastructure;

public class CurrentSession(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : ICurrentSession
{
    public IHttpContextAccessor HttpContextAccessor { get; set; }
    public CurrentUser CurrentUser { get; set; } = new CurrentUser(httpContextAccessor?.HttpContext?.User?.Claims);
    public string HostName { get; set; } = "https://" + httpContextAccessor?.HttpContext?.Request.Host.Value;
    public string ClientUrl { get; set; } = configuration["ClientAppUrl"] ?? string.Empty;
}