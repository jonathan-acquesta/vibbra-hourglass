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
    public class TimeService : ITimeService
    {
        #region Fields

        private readonly IBaseRepository<TimeDomain> _timeRepository;

        private readonly IBaseRepository<UserDomain> _userRepository;

        private readonly IBaseRepository<ProjectDomain> _projectRepository;

        #endregion

        #region Constructor

        public TimeService(
            IBaseRepository<TimeDomain> timeRepository, IBaseRepository<UserDomain> userRepository, IBaseRepository<ProjectDomain> projectRepository)
        {
            _timeRepository = timeRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        #endregion

        #region Methods

        public async Task<TimeDomain> Add(TimeDomain time)
        {
            if (time.StartedAt == DateTime.MinValue || time.EndedAt == DateTime.MinValue || time.ProjectID == 0 || time.UserID == 0)
                throw new RequiredFieldException("Required fields not filled in");

            if (time.StartedAt >= time.EndedAt)
                throw new RequiredFieldException("End date less than start date");

            await ValidateUser(time);
            await ValidateProject(time);
            await ValidateTimeDuplicatedNew(time);

            var result = await _timeRepository.Insert(time);

            await _timeRepository.CommitAsync();

            return result;
        }


        public async Task<List<TimeDomain>> FindAllByProject(int projectID)
        {
            var timesDB = (await _timeRepository.Select(x => x.ProjectID == projectID)).ToList();

            if (timesDB.Count == 0)
                throw new NotFoundException("No time posting found");

            return timesDB;
        }

        public async Task<TimeDomain> Update(TimeDomain time)
        {
            var timeOnDb = await _timeRepository.SelectFirstBy(x => x.ID == time.ID);

            if (timeOnDb == null)
                throw new NotFoundException("Unable to locate the time posting to be updated.");

            if (time.StartedAt == DateTime.MinValue || time.EndedAt == DateTime.MinValue || time.ProjectID == 0 || time.UserID == 0)
                throw new RequiredFieldException("Required fields not filled in");

            if (time.StartedAt >= time.EndedAt)
                throw new RequiredFieldException("End date less than start date");

            timeOnDb.StartedAt = time.StartedAt;
            timeOnDb.EndedAt = time.EndedAt;
            timeOnDb.ProjectID = time.ProjectID;
            timeOnDb.UserID = time.UserID;

            await ValidateUser(time);
            await ValidateProject(time);
            await ValidateTimeDuplicatedUpdate(time);

            await _timeRepository.Update(timeOnDb);

            await _timeRepository.CommitAsync();

            return timeOnDb;
        }

        private async Task ValidateTimeDuplicatedNew(TimeDomain time)
        {
            var timeDB = await _timeRepository.Select(x =>
                                x.UserID == time.UserID &&
                                (
                                    (x.StartedAt <= time.StartedAt && time.StartedAt <= x.EndedAt) ||
                                    (x.StartedAt <= time.EndedAt && time.EndedAt <= x.EndedAt) ||
                                    (time.StartedAt <= x.StartedAt && x.EndedAt <= time.EndedAt)
                                )
                            );

            if (timeDB.Count > 0)
                throw new DuplicateItemException("Release conflicts with other time posting period for this user");
        }

        private async Task ValidateTimeDuplicatedUpdate(TimeDomain time)
        {
            var timeDB = await _timeRepository.Select(x =>
                                x.UserID == time.UserID && x.ID != time.ID &&
                                (
                                    (x.StartedAt <= time.StartedAt && time.StartedAt <= x.EndedAt) ||
                                    (x.StartedAt <= time.EndedAt && time.EndedAt <= x.EndedAt) ||
                                    (time.StartedAt <= x.StartedAt && x.EndedAt <= time.EndedAt)
                                )
                            );

            if (timeDB.Count > 0)
                throw new DuplicateItemException("Release conflicts with other time posting period for this user");
        }

        private async Task ValidateUser(TimeDomain time)
        {
            var user = await _userRepository.SelectFirstBy(x => x.ID == time.UserID);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
        }

        private async Task ValidateProject(TimeDomain time)
        {
            var project = await _projectRepository.SelectFirstBy(x => x.ID == time.ProjectID);
            if (project == null)
            {
                throw new NotFoundException("Project not found");
            }
        }

        #endregion
    }
}
