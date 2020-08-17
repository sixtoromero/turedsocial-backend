using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using gluky.webapi.Contexts;
using gluky.webapi.Dtos;
using gluky.webapi.Helpers;
using gluky.webapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace gluky.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class usersController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public usersController(IMapper mapper, ApplicationDbContext context, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this._mapper = mapper;
            this._appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public async Task<IActionResult> login(usersDto model)
        {
            Response<usersDto> response = new Response<usersDto>();
            try
            {
                var loginResultDto = new usersDto();
                var userResult = await this.context.users.Where(x => x.email.Equals(model.email) && x.password.Equals(model.password)).FirstOrDefaultAsync();
                if (userResult != null)
                {
                    var iuser = _mapper.Map<usersDto>(userResult);
                    response.Data = iuser;
                    response.IsSuccess = true;
                    response.Message = string.Empty;
                } 
                else
                {
                    response.Data = null;
                    response.IsSuccess = false;
                    response.Message = "Usuario o contraseña incorrectos";
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.Message = ex.Message;

                return BadRequest(response);
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> insert(usersDto model)
        {
            Response<usersDto> response = new Response<usersDto>();

            try
            {
                users iuser = _mapper.Map<users>(model);
                this.context.users.Add(iuser);
                await this.context.SaveChangesAsync();

                response.Data = _mapper.Map<usersDto>(iuser);
                response.IsSuccess = true;
                response.Message = string.Empty;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.Message = ex.Message;

                return BadRequest(response);
            }
        }
    }
}
