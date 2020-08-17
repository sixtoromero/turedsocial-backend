using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using gluky.webapi.Contexts;
using gluky.webapi.Dtos;
using gluky.webapi.Helpers;
using gluky.webapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace gluky.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class postsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;
        public static IWebHostEnvironment _environment;

        public postsController(IMapper mapper, 
                               ApplicationDbContext context, 
                               IOptions<AppSettings> appSettings, 
                               IWebHostEnvironment environment)
        {
            this.context = context;
            this._mapper = mapper;
            this._appSettings = appSettings.Value;
            _environment = environment;
        }


        [HttpPost("insert")]
        public async Task<IActionResult> insert(postsDto model)
        {
            Response<postsDto> resp = new Response<postsDto>();
            
            try
            {
                posts ipost = _mapper.Map<posts>(model);

                this.context.posts.Add(ipost);
                await this.context.SaveChangesAsync();

                resp.Data = _mapper.Map<postsDto>(ipost);
                resp.IsSuccess = true;
                resp.Message = string.Empty;

                return Ok(resp);

            }
            catch (Exception ex)
            {
                resp.Data = null;
                resp.IsSuccess = false;
                resp.Message = ex.Message.ToString();

                return BadRequest(resp);
            }
        }

        [HttpGet("getPostByUserID")]
        public async Task<IActionResult> getPostByUserID(string user_id)
        {
            Response<IEnumerable<postsDto>> response = new Response<IEnumerable<postsDto>>();
            try
            {
                var iPost = await this.context.posts.Where(x => x.user_id.Equals(user_id)).ToListAsync();

                response.Data = _mapper.Map<IEnumerable<postsDto>>(iPost);
                response.IsSuccess = true;
                response.Message = string.Empty;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.Message = ex.Message;
                return BadRequest(ex);
            }
        }

        [HttpGet("getPostsPagination")]
        public async Task<IActionResult> getPostsPagination(int user_id, int limit)
        {
            Response<List<postsDto>> response = new Response<List<postsDto>>();
            try
            {
                var respPost = await this.context.posts.Where(x => x.user_id.Equals(user_id)).OrderByDescending(r => r.id).ToListAsync();
                var respPostsDto = _mapper.Map<List<postsDto>>(respPost);

                #region Consumiendo API
                string urlAPI = this._appSettings.ApiURL + "/posts?access-token=" + this._appSettings.Token;
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(urlAPI);
                wrGETURL.Method = "GET";
                wrGETURL.ContentType = @"application/json; charset=utf-8";

                HttpWebResponse webresponse = wrGETURL.GetResponse() as HttpWebResponse;
                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                // read response stream from response object
                StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
                // read string from stream data
                string Result = loResponseStream.ReadToEnd();
                // close the stream object
                loResponseStream.Close();
                // close the response object
                webresponse.Close();

                var result_posts = JsonConvert.DeserializeObject<ResultDto>(Result);

                //result_posts = result_posts.OrderByDescending(x => x.id).ToList();
                #endregion

                #region Configuración de la Data
                foreach (var item in respPostsDto)
                {
                    if (item.url != null && item.url != string.Empty)
                    {
                        item.isVisible = true;
                    }
                    else
                    {
                        item.isVisible = false;
                    }
                }

                foreach (var item in result_posts.data)
                {
                    item.isVisible = false;
                    respPostsDto.Add(item);
                }

                List<postsDto> respPostSend = new List<postsDto>();

                if (limit <= respPostsDto.Count)
                {
                    for (int i = 0; i < limit; i++)
                    {
                        respPostSend.Add(respPostsDto[i]);
                    }
                }
                else
                {
                    respPostSend = respPostsDto;
                }
                #endregion

                response.Data = respPostSend; //_mapper.Map<List<postsDto>>(respPost);
                response.IsSuccess = true;
                response.Message = string.Empty;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Data = null;
                response.IsSuccess = false;
                response.Message = ex.Message;
                return BadRequest(ex);
            }
        }

        [HttpPost("fileupload"), DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("StaticFiles", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
