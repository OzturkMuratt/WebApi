using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,ITokenService tokenService,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user =  _userManager.Users.FirstOrDefault(x => x.UserName==loginDto.Username.ToLower());
            if (user == null) {return Unauthorized("Invalid Username");}

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password,false);
            if (!result.Succeeded) { return Unauthorized("İnvalid Password"); }

            return Ok(
                new NewUserDto 
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = _tokenService.CreateToken(user) 
                });
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var appUser = new AppUser
                {

                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };

                var createUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto 
                            {
                                Email = appUser.Email,
                                UserName = appUser.UserName, 
                                Token = _tokenService.CreateToken(appUser)
                            }
                            );
                    }
                    else { return BadRequest("User Not Created"); }
                }
                else
                {
                    return StatusCode(500, createUser.Errors);
                }
            }
            catch (Exception e)
            {

                return StatusCode(500, e);
            }

        }
    }
}
