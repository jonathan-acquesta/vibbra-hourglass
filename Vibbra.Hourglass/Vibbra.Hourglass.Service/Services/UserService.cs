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
    public class UserService : IUserService
    {
        #region Fields

        private readonly IBaseRepository<UserDomain> _userRepository;

        #endregion

        #region Constructor

        public UserService(
            IBaseRepository<UserDomain> userRepository)
        {
            _userRepository = userRepository;
        }

        #endregion

        #region Methods

        public async Task<UserDomain> Add(UserDomain user)
        {
            if (String.IsNullOrEmpty(user.Email) || String.IsNullOrEmpty(user.Login) || String.IsNullOrEmpty(user.Name) || String.IsNullOrEmpty(user.Password))
                throw new RequiredFieldException("Required fields not filled in");

            await ValidateEmailDuplicatedNewUser(user);
            await ValidateLoginDuplicatedNewUser(user);

            var result = await _userRepository.Insert(user);

            await _userRepository.CommitAsync();

            return result;
        }

        public async Task<UserDomain> Find(int id)
        {
            var userDB = await _userRepository.SelectFirstBy(x => x.ID == id);

            if (userDB == null)
                throw new NotFoundException("User not found");

            return userDB;
        }

        public async Task<UserDomain> Update(UserDomain user)
        {
            var userOnDb = await _userRepository.SelectFirstBy(x => x.ID == user.ID);

            if (userOnDb == null)
                throw new NotFoundException("Could not find the user to be updated");

            if (String.IsNullOrEmpty(user.Email) || String.IsNullOrEmpty(user.Login) || String.IsNullOrEmpty(user.Name) || String.IsNullOrEmpty(user.Password))
                throw new RequiredFieldException("Required fields not filled in");

            userOnDb.Email = user.Email;
            userOnDb.Name = user.Name;
            userOnDb.Login = user.Login;
            userOnDb.Password = user.Password;

            await ValidateEmailDuplicatedUpdateUser(user);
            await ValidateLoginDuplicatedUpdateUser(user);

            await _userRepository.Update(userOnDb);

            await _userRepository.CommitAsync();

            return userOnDb;
        }

        private async Task ValidateEmailDuplicatedNewUser(UserDomain user)
        {
            var userDB = await _userRepository
               .Select(x => x.Email == user.Email);

            if (userDB.Count > 0)
                throw new DuplicateItemException("E-mail already registered");
        }

        private async Task ValidateLoginDuplicatedNewUser(UserDomain user)
        {
            var userDB = await _userRepository
               .Select(x => x.Login == user.Login);

            if (userDB.Count > 0)
                throw new DuplicateItemException("Login already registered");
        }

        private async Task ValidateEmailDuplicatedUpdateUser(UserDomain user)
        {
            var userDB = await _userRepository
               .Select(x => x.Email == user.Email && x.ID != user.ID);

            if (userDB.Count > 0)
                throw new DuplicateItemException("E-mail already registered");
        }

        private async Task ValidateLoginDuplicatedUpdateUser(UserDomain user)
        {
            var userDB = await _userRepository
               .Select(x => x.Login == user.Login && x.ID != user.ID);

            if (userDB.Count > 0)
                throw new DuplicateItemException("Login already registered");
        }

        #endregion
    }
}
