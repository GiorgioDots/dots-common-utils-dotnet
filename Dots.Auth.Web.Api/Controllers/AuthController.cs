using Azure.Core;
using Dots.Auth.DAL.Models;
using Dots.Auth.DTO.Auth;
using Dots.Auth.Web.Api.Services;
using Dots.Commons;
using Dots.Commons.Auth;
using Dots.Commons.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dots.Auth.Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DotsAuthContext dc;
        private readonly TokenService tokenSvc;
        private readonly IConfiguration config;
        private string passwordKey => this.config.GetSection("Security:passwordKey").Get<string>() ?? "";

        public AuthController(DotsAuthContext dc, TokenService tokenSvc, IConfiguration config)
        {
            this.dc = dc;
            this.tokenSvc = tokenSvc;
            this.config = config;
        }

        [HttpPost]
        [Route("signup")]
        public async Task<ActionResult<MessageDTO>> SignUp(
            SignUpDTO request
        )
        {
            if (!request.Valid)
            {
                throw new UnprocessableEntityException("Request not valid");
            }

            var existingUsernameOrEmail = await dc.Accounts
                .FirstOrDefaultAsync(k => k.Status == (int)eEntityStatus.OK &&
                    k.Email == request.Email || k.Username == request.Username);
            if (existingUsernameOrEmail != null)
            {
                throw new UnprocessableEntityException("Email or username already exists");
            }

            var account = new Accounts()
            {
                Username = request.Username,
                Email = request.Email,
                Password = PasswordUtils.EncryptString(
                    passwordKey,
                    request.Password)
            };

            dc.Accounts.Add(account);

            await dc.Commit();

            return Ok(new MessageDTO() { Message= "Signed up!"});
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<object>> Login(
            [FromBody] LoginDTO login
        )
        {
            if(!login.Valid)
            {
                throw new UnprocessableEntityException("Request not valid");
            }

            var encryptedPwd = PasswordUtils.EncryptString(passwordKey, login.Password);
            var existingUser = await dc.Accounts
                .Include(k => k.AccountsApplications)
                .ThenInclude(k => k.IdApplicationNavigation)
                .FirstOrDefaultAsync(k =>
                    k.Status == (int)eEntityStatus.OK &&
                    (k.Email == login.Login || k.Username == login.Login) && 
                    k.Password == encryptedPwd
                );

            if (existingUser == null)
            {
                throw new UnprocessableEntityException("Wrong credentials");
            }

            var accessToken = tokenSvc.CreateToken(existingUser);

            var refreshTokenDateTo = DateTime.UtcNow.AddDays(7);
            var refreshToken = tokenSvc.GenerateRefreshToken();

            existingUser.AccountsTokens.Add(new AccountsTokens()
            {
                DateTo = refreshTokenDateTo,
                Token = refreshToken,
                Type = (int)eAccountTokenType.REFRESH_TOKEN
            });

            await dc.Commit();

            return Ok(new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
            });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<ActionResult<object>> RefreshToken(
            [FromBody] AuthResponseDTO req
        )
        {
            if (string.IsNullOrEmpty(req.RefreshToken))
            {
                throw new UnprocessableEntityException("Request not valid");
            }

            var existingToken = await dc.AccountsTokens
                .Include(k => k.IdAccountNavigation)
                .ThenInclude(k => k.AccountsApplications)
                .ThenInclude(k => k.IdApplicationNavigation)
                .FirstOrDefaultAsync(k =>
                    k.Status == (int)eEntityStatus.OK &&
                    k.IdAccountNavigation.Status == (int)eEntityStatus.OK &&
                    k.DateTo > DateTime.Now && k.Token == req.RefreshToken);

            if (existingToken == null)
            {
                throw new UnprocessableEntityException("Refresh token not valid");
            }

            dc.AccountsTokens.Remove(existingToken);
            var existingUser = existingToken.IdAccountNavigation;

            var accessToken = tokenSvc.CreateToken(existingUser);

            var refreshTokenDateTo = DateTime.UtcNow.AddDays(7);
            var refreshToken = tokenSvc.GenerateRefreshToken();

            existingUser.AccountsTokens.Add(new AccountsTokens()
            {
                DateTo = refreshTokenDateTo,
                Token = refreshToken,
                Type = (int)eAccountTokenType.REFRESH_TOKEN
            });

            await dc.Commit();

            return Ok(new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
            });
        }
    }
}
