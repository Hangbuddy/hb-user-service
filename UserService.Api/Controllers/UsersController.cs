using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Configuration;
using UserService.Data;
using UserService.Dtos.Enums;
using UserService.Dtos.Requests;
using UserService.Dtos.Responses;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public UsersController(
            IUserRepo repository,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpGet("health-check", Name = "HealthCheck")]
        public ActionResult<string> HealthCheck()
        {
            return Ok("Service is up and running.");
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{userId}", Name = "GetUser")]
        public ActionResult<ApplicationUserReadDto> GetUser(string userId)
        {
            var userItem = _repository.GetUser(userId);
            if (userItem != null)
            {
                return Ok(_mapper.Map<ApplicationUserReadDto>(userItem));
            }

            return NotFound();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public ActionResult<ApplicationUserReadDto> GetUser()
        {
            var userItem = _repository.GetUser(User.FindFirst("Id")?.Value);
            if (userItem != null)
            {
                return Ok(_mapper.Map<ApplicationUserReadDto>(userItem));
            }

            return NotFound();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("get-users-bulk", Name = "GetUserBulk")]
        public ActionResult<List<ApplicationUserReadDto>> GetUsersBulk(List<string> userIdList)
        {
            var userItemList = _repository.GetUsersBulk(userIdList);
            return Ok(_mapper.Map<List<ApplicationUserReadDto>>(userItemList));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut(Name = "UpdateUser")]
        public async Task<ActionResult<ApplicationUserReadDto>> UpdateUser(ApplicationUserUpdateDto userUpdateDto)
        {
            var userId = User.FindFirst("Id")?.Value;
            var applicationUser = _mapper.Map<ApplicationUser>(userUpdateDto);
            applicationUser.Id = userId;
            var result = await _repository.UpdateUser(applicationUser, userUpdateDto.Password);
            if (result)
            {
                _repository.SaveChanges();
                return CreatedAtRoute(nameof(GetUser), new { userId }, userUpdateDto);
            }
            else
                return BadRequest();
        }


        [HttpPost(Name = "CreateUser")]
        public async Task<ActionResult<ApplicationUserReadDto>> CreateUser(ApplicationUserCreateDto userCreateDto)
        {
            IdentityUser identityUser = new() { Email = userCreateDto.Email, UserName = userCreateDto.Username };
            var applicationUser = _mapper.Map<ApplicationUser>(identityUser);
            ErrorCodes result = await _repository.CreateUser(applicationUser, identityUser, userCreateDto.Password);

            if (result.Equals(ErrorCodes.Success))
            {
                _repository.SaveChanges();
                return Ok(new RegisterResultDto()
                {
                    ErrorCode = ErrorCodes.Success
                });
            }
            else
                return BadRequest(new RegisterResultDto()
                {
                    ErrorCode = result
                });
        }

        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<LoginResultDto>> Login(ApplicationUserLoginDto userLoginDto)
        {
            var existingUser = await _userManager.FindByNameAsync(userLoginDto.Username);
            if (existingUser == null)
            {
                return BadRequest(new LoginResultDto()
                {
                    Token = null,
                    ErrorCode = ErrorCodes.UserNotFound,
                    User = null
                });
            }

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, userLoginDto.Password);
            if (!isCorrect)
            {
                return BadRequest(new LoginResultDto()
                {
                    Token = null,
                    ErrorCode = ErrorCodes.WrongPassword,
                    User = null
                });
            }

            var jwtToken = GenerateJwtToken(existingUser);
            ApplicationUserReadDto applicationUserReadDto = (ApplicationUserReadDto)((OkObjectResult)GetUser(existingUser.Id).Result).Value;
            return Ok(new LoginResultDto()
            {
                Token = jwtToken,
                ErrorCode = ErrorCodes.Success,
                User = applicationUserReadDto
            });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}