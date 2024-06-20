﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartcouponAPI.Context.Identity.UserIdentity;
using SmartcouponAPI.Users.Model;
using SmartcouponAPI.Users.Model.Requests;
using SmartcouponAPI.Users.Model.Responses;
using SmartcouponAPI.Users.Repository;
using System.Diagnostics;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmartcouponAPI.Users.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly UserIdentityDbContext _context;
        private readonly UserRepository _repository;
        //private readonly JWTTokenManager _tokenManager;

        public UserController(UserManager<User> userManager, UserIdentityDbContext context, UserRepository repository)
        {
            _userManager = userManager;
            _context = context;
            _repository = repository;
            //TokenManager
        }

        // Implementar un middleware para personalizar el mensaje de regreso?
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            UserRegisterResponse response = await _repository.Register(request, _userManager, _context);

            if (response.UserName == null)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
