using Gymwebproject.DB;
using Gymwebproject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Gymwebproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Gymdbcontext obj;

        public UserController(Gymdbcontext context)
        {
            obj = context;
        }
        [HttpGet]
        [Route("List")]

        public async Task<ActionResult<IEnumerable<Userinformation>>> GetUserinformation()
        {
            return await obj.Userinformation.ToListAsync();
        }



        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> PostUserinformation(Userinformation userInformation)
        {
            if (userInformation == null)
            {
                return BadRequest(new { message = "Invalid user data" });
            }

            try
            {
                obj.Userinformation.Add(userInformation);
                await obj.SaveChangesAsync();

                return Ok(new { message = "User registered successfully!", userId = userInformation.ID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new { message = "Email and Password are required!" });
            }

            try
            {
                // Check if user exists in UserInformation table
                var user = await obj.Userinformation
                    .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.password == loginRequest.Password);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid email or password!" });
                }

                return Ok(new
                {
                    message = "Login successful!",
                    userId = user.ID,
                    fullname = user.Fullname
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
            }
        }


        [HttpGet("CheckEmail")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var userExists = await obj.Userinformation.AnyAsync(u => u.Email == email);
            return Ok(new { exists = userExists });
        }




    }
}



