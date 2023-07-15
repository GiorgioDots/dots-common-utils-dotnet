using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManager.DAL.Models;
using UserManager.DTO.Tags;
using UserManager.DTO;

namespace UserManager.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersTagsController : ControllerBase
    {
        private readonly UserManagerEntities _dc;
        private readonly IMapper _mapper;

        public UsersTagsController(UserManagerEntities context, IMapper mapper)
        {
            _dc = context;
            _mapper = mapper;
        }

        // POST api/<TagsController>
        [HttpPost("{idTag}/{idUser}")]
        public async Task Post(int idTag, int idUser)
        {
            var tag = _dc.Tags
                .FirstOrDefault(k => k.Status == (int)eEntityStatus.OK && k.Id == idTag);

            if (tag == null) throw new Exception("Tag not found");

            var user = _dc.Users
                .FirstOrDefault(k => k.Status == (int)eEntityStatus.OK && k.Id == idUser);

            if (user == null) throw new Exception("User not found");

            if(
                _dc.UsersTags
                    .Any(k => 
                        k.IdUser == idUser && 
                        k.IdTag == idTag && 
                        k.Status == (int)eEntityStatus.OK
                    )
            )
            {
                return;
            }

            _dc.Add(new UsersTags()
            {
                IdUser = idUser,
                IdTag = idTag,
            });

            await _dc.Commit();
        }

        // DELETE api/<TagsController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var userTag = await _dc.UsersTags
                .FirstOrDefaultAsync(k =>
                    k.Id == id &&
                    k.Status == (int)eEntityStatus.OK);

            if (userTag == null) throw new Exception("Not found");

            _dc.Remove(userTag);
            await _dc.Commit();
        }
    }
}
