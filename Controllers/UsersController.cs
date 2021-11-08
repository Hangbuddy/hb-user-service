using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _repository;
        private readonly IMapper _mapper;

        public UsersController(
            IUserRepo repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("get-user", Name = "GetUser")]
        public ActionResult<UserReadDto> GetUser(int userId)
        {
            var userItem = _repository.GetUser(userId);
            if (userItem != null)
            {
                return Ok(_mapper.Map<UserReadDto>(userItem));
            }

            return NotFound();
        }

        [HttpPost("create-user")]
        public ActionResult<UserReadDto> CreateUser(UserCreateDto userCreateDto)
        {
            var userModel = _mapper.Map<User>(userCreateDto);
            _repository.CreateUser(userModel);
            _repository.SaveChanges();

            var userReadDto = _mapper.Map<UserReadDto>(userModel);

            return CreatedAtRoute(nameof(GetUser), new { id = userReadDto.Username }, userReadDto);
        }

        [HttpPost("update-user")]
        public ActionResult<UserReadDto> UpdateUser(UserUpdateDto userUpdateDto)
        {
            var userModel = _mapper.Map<User>(userUpdateDto);
            _repository.UpdateUser(userModel);
            _repository.SaveChanges();
            return CreatedAtRoute(nameof(GetUser), new { id = userUpdateDto.UserId }, userUpdateDto);
        }

        [HttpPost("login")]
        public ActionResult<UserReadDto> Login(UserLoginDto userLoginDto)
        {
            var userModel = _mapper.Map<User>(userLoginDto);
            var userLogin = _repository.Login(userModel);
            if (userLogin is null)
                return BadRequest();
            return Ok(_mapper.Map<User>(userLogin));
        }
    }
}