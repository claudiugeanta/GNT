using GNT.Shared.Dtos.UserManagement;
using Microsoft.AspNetCore.Http;

namespace GNT.Application.Interfaces;

public interface ICurrentSession
{
    CurrentUser CurrentUser { get; set; }
    IHttpContextAccessor HttpContextAccessor { get; set; }
    public string HostName { get; set; }
    public string ClientUrl { get; set; }
}