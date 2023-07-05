using System;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Movie.Service.Nuget.Interface;
using Movie.Service.Nuget.Model;
using Microsoft.AspNetCore.WebUtilities;

namespace Movie.Service.Nuget.Repository
{
	public class JWTRepo : IJWTRepo
	{
        public string GenerateUserToken(User user)
        {
            var encodedJwtSecret = JWTHandler.encodedJwtSecret;
            string jwtSecret = DecryptToken(encodedJwtSecret);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            var adminClaims = new List<Claim>
            {
                //new Claim(ClaimTypes.Authentication, userType.ToString().ToUpper()),
                new Claim(ClaimTypes.Name, $"{user.FullName}"),
                new Claim(ClaimTypes.Email, $"{user.Email}"),
            };

            //var userRoles = await _userManager.GetRolesAsync(user);
            //adminClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var claim = new ClaimsIdentity(adminClaims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claim,
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                 SecurityAlgorithms.HmacSha256Signature),
            };


            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public string DecryptToken(string token)
        {
            var code = WebEncoders.Base64UrlDecode(token);
            return Encoding.UTF8.GetString(code);
        }
    }
}

