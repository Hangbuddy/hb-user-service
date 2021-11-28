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
        [HttpPut]
        public ActionResult<ApplicationUserReadDto> UpdateUser(ApplicationUserUpdateDto userUpdateDto)
        {
            var userId = User.FindFirst("Id")?.Value;
            if (userId != userUpdateDto.UserId)
                return Unauthorized();
            var applicationUser = _mapper.Map<ApplicationUser>(userUpdateDto);
            _repository.UpdateUser(applicationUser);
            _repository.SaveChanges();
            return CreatedAtRoute(nameof(GetUser), new { userId = userUpdateDto.UserId }, userUpdateDto);
        }

        [HttpPost]
        public async Task<ActionResult<ApplicationUserReadDto>> CreateUser(ApplicationUserCreateDto userCreateDto)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByNameAsync(userCreateDto.Username) != null)
                {
                    return BadRequest(new RegisterResultDto()
                    {
                        ErrorCode = ErrorCodes.UsernameAlreadyInUse
                    });
                }
                if (await _userManager.FindByEmailAsync(userCreateDto.Email) != null)
                {
                    return BadRequest(new RegisterResultDto()
                    {
                        ErrorCode = ErrorCodes.EmailAlreadyInUse
                    });
                }

                var newUser = new IdentityUser() { Email = userCreateDto.Email, UserName = userCreateDto.Username };
                var applicationUser = _mapper.Map<ApplicationUser>(newUser);
                _repository.CreateUser(applicationUser);
                var isCreated = await _userManager.CreateAsync(newUser, userCreateDto.Password);
                if (isCreated.Succeeded)
                {
                    return Ok(new RegisterResultDto()
                    {
                        ErrorCode = ErrorCodes.Success
                    });
                }
                else
                {
                    // ToDo: Remove application user
                    // ToDo: Log errors.
                    var errors = isCreated.Errors.Select(x => x.Description).ToList();
                    return BadRequest(new RegisterResultDto()
                    {
                        ErrorCode = ErrorCodes.UnknownError
                    });
                }
            }

            return BadRequest(new RegisterResultDto()
            {
                ErrorCode = ErrorCodes.UnknownError
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApplicationUserReadDto>> Login(ApplicationUserLoginDto userLoginDto)
        {
            if (ModelState.IsValid)
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

            return BadRequest(new LoginResultDto()
            {
                Token = null,
                ErrorCode = ErrorCodes.WrongPassword,
                User = null
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