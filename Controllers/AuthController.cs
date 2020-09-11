using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rpg_api.Data;
using rpg_api.Dtos.User;
using rpg_api.Models;

namespace rpg_api.Controllers
{
    // to access this controller /character/auth
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;

        }

        // to access this controller method /character/auth/register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto request) 
        {
            // Wrapper for register call to auth repo interface
            ServiceResponse<int> response = await _authRepo.Register(
                new User { Username = request.Username}, request.Password
            );
            // if response data is not successful - unable to register
            if (!response.Success) {
                return BadRequest(response);
            }
            // if response is successful return 200 OK
            return Ok(response);
        }

        // to access this controller method /character/auth/login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto request) 
        {
            // Wrapper for login call to auth repo interface
            ServiceResponse<string> response = await _authRepo.Login(
                request.Username, request.Password
            );
            // if response data is not successful - unable to login
            if (!response.Success) {
                return BadRequest(response);
            }
            // if response is successful return 200 OK
            return Ok(response);
        }


    }
}