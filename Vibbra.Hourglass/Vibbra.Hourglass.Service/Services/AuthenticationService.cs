using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vibbra.Hourglass.CrossCutting;
using Vibbra.Hourglass.Domain.Domains;
using Vibbra.Hourglass.Infra.Interfaces;
using Vibbra.Hourglass.Service.Exceptions;
using Vibbra.Hourglass.Service.Interfaces;

namespace Vibbra.Hourglass.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Fields

        private readonly IMapper _mapper;

        private readonly IConfiguration _configuration;

        private readonly IBaseRepository<UserDomain> _userRepository;

        #endregion

        #region Constructor

        public AuthenticationService(
            IMapper mapper,
            IBaseRepository<UserDomain> userRepository,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        #endregion

        #region Methods
        public async Task<(string, UserDomain)> Authentication(UserDomain user)
        {
            if ((String.IsNullOrEmpty(user.Login) || String.IsNullOrEmpty(user.Password)))
                throw new InvalidPasswordException("E-mail ou senha não preenchidos.");

            UserDomain userDB = await _userRepository.SelectFirstBy(x => x.Login == user.Login && x.Password == user.Password);

            if (userDB == null)
                throw new NotFoundException("Não foi possível logar, usuário não localizado.");

            var jwtSecret = _configuration.GetSection("JwtSecret").Value ?? string.Empty;

            string token = Token.CreateToken(new Dictionary<string, string>()
            {
                {"email", user.Email},
                {"id", user.ID.ToString() },
            }, jwtSecret);

            return (token, userDB);
        }

        #endregion
    }
}
