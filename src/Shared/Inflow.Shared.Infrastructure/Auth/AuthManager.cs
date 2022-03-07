using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Inflow.Shared.Abstractions.Auth;
using Inflow.Shared.Abstractions.Time;
using Microsoft.IdentityModel.Tokens;

namespace Inflow.Shared.Infrastructure.Auth;

public class AuthManager : IAuthManager
{
    private static readonly Dictionary<string, IEnumerable<string>> EmptyClaims = new();
    private readonly AuthOptions _options;
    private readonly IClock _clock;
    private readonly SigningCredentials _signingCredentials;
    private readonly string _issuer;

    public AuthManager(AuthOptions options, IClock clock)
    {
        _options = options;
        _clock = clock;
        _signingCredentials =
            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.IssuerSigningKey)),
                SecurityAlgorithms.HmacSha256);
        _issuer = options.Issuer;
    }

    public JsonWebToken CreateToken(Guid userId, string role = null, string audience = null, IDictionary<string, IEnumerable<string>> claims = null)
    {
        var now = _clock.CurrentDate();
        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeMilliseconds().ToString())
        };

        if (!string.IsNullOrWhiteSpace(role))
        {
            jwtClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (!string.IsNullOrWhiteSpace(audience))
        {
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));
        }

        if (claims?.Any() is true)
        {
            var customClaims = new List<Claim>();
            foreach (var (claim, values) in claims)
            {
                customClaims.AddRange(values.Select(x => new Claim(claim, x)));
            }
            
            jwtClaims.AddRange(customClaims);
        }

        var expires = now.Add(_options.Expiry);

        var jwt = new JwtSecurityToken(_issuer, claims: jwtClaims, notBefore: now, expires: expires,
            signingCredentials: _signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new JsonWebToken()
        {
            AccessToken = token,
            Claims = claims ?? EmptyClaims,
            UserId = userId,
            Expiry = new DateTimeOffset(expires).ToUnixTimeMilliseconds(),
            Role = role ?? string.Empty
        };
    }
}