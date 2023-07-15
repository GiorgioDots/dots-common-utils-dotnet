using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManager.DAL.Models;
using UserManager.DTO;
using UserManager.DTO.Tags;
using UserManager.DTO.Users;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserManager.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManagerEntities _dc;
        private readonly IMapper _mapper;

        public UsersController(UserManagerEntities context, IMapper mapper) 
        {
            _dc = context;
            _mapper = mapper;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IEnumerable<UserDTO>> Get()
        {
            var users = await _dc.Users
                .Include(k =>
                    k.UsersTags
                        .Where(ut =>
                            ut.Status == (int)eEntityStatus.OK &&
                            ut.IdTagNavigation.Status == (int)eEntityStatus.OK
                        )
                )
                .ThenInclude(k => k.IdTagNavigation)
                .Where(k => k.Status == (int)eEntityStatus.OK)
                .ToListAsync();

            var ret = new List<UserDTO>();
            foreach (var user in users)
            {
                var u = _mapper.Map<UserDTO>(user);
                u.Tags = user.UsersTags
                    .Select(k => _mapper.Map<TagDTO>(k.IdTagNavigation))
                    .ToList();
                ret.Add(u);
            }
            return ret;
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<UserDTO> Get(int id)
        {
            var user = await _dc.Users
                .Include(k =>
                    k.UsersTags
                        .Where(ut =>
                            ut.Status == (int)eEntityStatus.OK &&
                            ut.IdTagNavigation.Status == (int)eEntityStatus.OK
                        )
                )
                .ThenInclude(k => k.IdTagNavigation)
                .FirstOrDefaultAsync(k =>
                    k.Id == id &&
                    k.Status == (int)eEntityStatus.OK);
            if (user == null) throw new Exception("Not found");

            var u = _mapper.Map<UserDTO>(user);
            u.Tags = user.UsersTags
                .Select(k => _mapper.Map<TagDTO>(k.IdTagNavigation))
                .ToList();
            return u;
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task Post([FromBody] UserDTO value)
        {
            var user = _mapper.Map<Users>(value);
            _dc.Add(user);
            await _dc.Commit();
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] UserDTO input)
        {
            var user = await _dc.Users
                .FirstOrDefaultAsync(k =>
                    k.Id == id &&
                    k.Status == (int)eEntityStatus.OK);
            if (user == null) throw new Exception("Not found");

            user.FastLoginKey = input.FastLoginKey;
            user.Password = input.Password;
            user.Username = input.Username;
            await _dc.Commit();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var user = await _dc.Users
                .FirstOrDefaultAsync(k =>
                    k.Id == id &&
                    k.Status == (int)eEntityStatus.OK);
            if (user == null) throw new Exception("Not found");

            _dc.Remove(user);
            await _dc.Commit();
        }
    }
}
