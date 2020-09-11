using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using rpg_api.Models;

namespace rpg_api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;

        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            // wrapper
            ServiceResponse<string> response = new ServiceResponse<string>();
            // find user in database with username param
            User user = await _context.Users.FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));

            // no user found
            if (user == null)
            {
                response.Success = false;
                response.Message = "Login failed.";
            }// check if passwords match
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Login failed.";
            }// create token
            else
            {
                response.Data = CreateToken(user);
            }

            return response;

        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            // wrapper
            ServiceResponse<int> response = new ServiceResponse<int>();

            // check if the user exists by checking if username exists
            if (await UserExists(user.Username))
            {
                // return message
                response.Success = false;
                response.Message = "User already exists.";
                return response;
            }

            // hash password by passing password and returning hash and salt
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // add user to database and save changes
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // return wrapper data as well
            response.Data = user.Id;
            response.Message = "User created.";
            return response;
        }

        public async Task<bool> UserExists(string username)
        {
            // using Linq, check if any users in db match the username
            if (await _context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower()))
            {
                // user exists
                return true;
            }
            // user does not exist
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            // hashing password with HMACSHA512
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                // generating salt
                passwordSalt = hmac.Key;
                // hashing password with salt
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // rehash password and check if db stored hash matches rehashed password
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        // passwords do not match
                        return false;
                    }
                }
                // passwords match
                return true;
            }
        }

        private string CreateToken(User user)
        {
            // declare a list of claims (userId and username)
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Create a symmetric security key with the secret key using IConfiguration for access
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)
            );

            // create new signing credentials and use the HmacSha512Signature Algorithm for that
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // This object gets the information used to create the final token.
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // userId and username
                Expires = DateTime.Now.AddDays(1),  // expires in 1 day
                SigningCredentials = creds // secret key + hmacsha512 algorithm
            };

            // JWT security token handler and use this token handler and the token descriptor to create a security token.
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            // return the json Web token as a string
            return tokenHandler.WriteToken(token);
        }

    }
}