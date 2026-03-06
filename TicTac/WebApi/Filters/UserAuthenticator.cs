using Datasource.Iinterfaces;
using Datasource.Security;
using Di;
using Domain.Interfaces;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using WebApi.DTO;

namespace WebApi;


[ApiController]
[Route("auth")]
public class UserAuthenticator : IAuthorizationFilter
{
    private readonly IAuthService _authService;
    private readonly IJwtProvider _jwtProvider;
    private readonly string _auth = "Authorization";
    private readonly string _base = "base ";
    private readonly string _bearer = "Bearer ";
    public UserAuthenticator()
    {
        _authService = Configuration.AuthService;
        _jwtProvider = Configuration.JwtProvider;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        //if (context.ActionDescriptor.EndpointMetadata
        //.OfType<AllowAnonymousAttribute>()
        //.Any())
        //{
        //        return;
        //}
        var endpoint = context.HttpContext.GetEndpoint();

        if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
        {
            return;
        }
        var httpContext = context.HttpContext;
        if(!httpContext.Request.Headers.TryGetValue(_auth,out var header)) // AlwaysOn
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        var authHeader = header.ToString();
        if (!authHeader.StartsWith(_bearer, StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var accessToken = authHeader.Substring(_bearer.Length).Trim();

        try
        {
            if (_jwtProvider.ValidationAccessToken(accessToken))
            {
                context.HttpContext.Items["UserId"] = _jwtProvider.GetUUIDFromAccessToken(accessToken); ;
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
        catch (SecurityTokenExpiredException)
        {
            context.Result = new UnauthorizedResult();
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}