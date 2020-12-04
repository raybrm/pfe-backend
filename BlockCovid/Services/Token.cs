using BlockCovid.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlockCovid.Services
{
    public static class Token
    {

        public static JwtSecurityToken createToken(Participant participant)
        {
            string SECRET_KEY = "PFE_BACKEND_2020_GRP_13";
            var SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
            var signingCredentials = new SigningCredentials(SIGNING_KEY, SecurityAlgorithms.HmacSha256);

            var role = participant.Participant_Type.ToString();
            var claims = new List<Claim>();
            claims.Add(new Claim("login", participant.Login));
            claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenJWT = new JwtSecurityToken(
                issuer: "GROUPE_13",
                audience: "readers",
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials,
                claims: claims
                );
            return tokenJWT;
        }
    }
}
