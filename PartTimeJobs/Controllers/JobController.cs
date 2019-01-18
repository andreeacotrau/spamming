﻿using PartTimeJobs.App_Settings;
using PartTimeJobs.Authorization;
using PartTimeJobs.BLL.Services;
using PartTimeJobs.DAL.Models;
using PartTimeJobs.JWT;
using PartTimeJobs.Models;
using PartTimeJobs.Models.ModelFactories;
using PArtTimeJobs.BLL.Services;
using PArtTimeJobs.BLL.Validator;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PartTimeJobs.Controllers
{
    public class JobController : BaseController
    {
        private JobService _jobService = new JobService(new JobValidator());
        private UserService _userService = new UserService();

        [HttpGet]
        [UserAuthorize]
        [Route("unassignedJobs")]
        public HttpResponseMessage GetNotAssignedJobs()
        {
            return HandleRequestSafely(() =>
            {
                var jobFactory = new JobFactory();

                var jobs = _jobService.GetNotAssignedJobs();
                if (jobs != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, jobs.Select(jobFactory.GetJobDtoFromJob));
                }
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Something went wrong...");
            });
        }

        [HttpGet]
        [UserAuthorize]
        [Route("createdJobs")]
        public HttpResponseMessage GetUserCreatedJobs()
        {
            return HandleRequestSafely(() =>
            {
                IEnumerable<string> tokenValues = new List<string>();
                Request.Headers.TryGetValues(Settings.TokenKey, out tokenValues);
                var user = _userService.GetUserByEmail(JwtManager.GetEmailFromToken(tokenValues.First()));
                var jobs = user.JobsCreated ?? new List<Job>();
                var jobFactory = new JobFactory();
                return Request.CreateResponse(HttpStatusCode.OK, jobs.Select(jobFactory.GetJobDtoFromJob));
            });
        }

        [HttpGet]
        [UserAuthorize]
        [Route("getJob/{id}")]
        public HttpResponseMessage GetById(int id)
        {
            return HandleRequestSafely(() =>
            {
                var job = _jobService.GetById(id);
                if (job == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "Something went wrong...");
                }
                var jobFactory = new JobFactory();
                return Request.CreateResponse(HttpStatusCode.OK, jobFactory.GetJobDtoFromJob(job));
            });
        }

        [HttpGet]
        [UserAuthorize]
        [Route("assignedJobs")]
        public HttpResponseMessage GetUserAssigned()
        {
            return HandleRequestSafely(() =>
            {
                IEnumerable<string> tokenValues = new List<string>();
                Request.Headers.TryGetValues(Settings.TokenKey, out tokenValues);
                var user = _userService.GetUserByEmail(JwtManager.GetEmailFromToken(tokenValues.First()));
                var jobs = user.JobsAssinged ?? new List<Job>();
                var jobFactory = new JobFactory();
                return Request.CreateResponse(HttpStatusCode.OK, jobs.Select(jobFactory.GetJobDtoFromJob));
            });
        }

        [HttpPost]
        [UserAuthorize]
        [Route("assignedJobs")]
        public HttpResponseMessage AddJob([FromBody] JobDto jobDto)
        {
            return HandleRequestSafely(() =>
            {
                var job = new JobFactory().GetJobFromDto(jobDto);
                _jobService.Add(job);
                return Request.CreateResponse(HttpStatusCode.OK);
            });
        }

        [HttpPut]
        [UserAuthorize]
        [Route("updateJob")]
        public HttpResponseMessage UpdateJob([FromBody] JobDto jobDto)
        {
            return HandleRequestSafely(() =>
            {
                var job = new JobFactory().GetJobFromDto(jobDto);
                _jobService.Update(job);
                return Request.CreateResponse(HttpStatusCode.OK);
            });
        }

        [HttpPut]
        [UserAuthorize]
        [Route("applyToJob/{id}")]
        public HttpResponseMessage Apply(int id)
        {
            return HandleRequestSafely(() =>
            {
                IEnumerable<string> tokenValues = new List<string>();
                Request.Headers.TryGetValues(Settings.TokenKey, out tokenValues);
                var user = _userService.GetUserByEmail(JwtManager.GetEmailFromToken(tokenValues.First()));
                var job = _jobService.GetById(id);
                if (job == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "Something went wrong...");
                }
                job.Asignee = user;
                _jobService.Update(job);
                return Request.CreateResponse(HttpStatusCode.OK);
            });
        }



    }
}