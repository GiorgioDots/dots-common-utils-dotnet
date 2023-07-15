using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserManager.DAL.Models;
using UserManager.DTO.Users;
using UserManager.DTO;
using UserManager.DTO.Tags;
using Microsoft.EntityFrameworkCore;

namespace UserManager.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController
    {
        private readonly UserManagerEntities _dc;
        private readonly IMapper _mapper;

        public TagsController(UserManagerEntities context, IMapper mapper)
        {
            _dc = context;
            _mapper = mapper;
        }

        // GET: api/<TagsController>
        [HttpGet]
        public async Task<IEnumerable<TagDTO>> Get()
        {
            var tags = await _dc.Tags
                .Include(k =>
                    k.UsersTags
                        .Where(ut =>
                            ut.Status == (int)eEntityStatus.OK &&
                            ut.IdUserNavigation.Status == (int)eEntityStatus.OK
                        )
                )
                .ThenInclude(k => k.IdUserNavigation)
                .Where(k => k.Status == (int)eEntityStatus.OK)
                .ToListAsync();

            var ret = new List<TagDTO>();
            foreach (var tag in tags)
            {
                var t = _mapper.Map<TagDTO>(tag);
                t.Users = tag.UsersTags
                    .Select(k => _mapper.Map<UserDTO>(k.IdUserNavigation))
                    .ToList();
                ret.Add(t);
            }
            return ret;
        }

        // GET api/<TagsController>/5
        [HttpGet("{id}")]
        public async Task<TagDTO> Get(int id)
        {
            var tag = await _dc.Tags
                .Include(k => 
                    k.UsersTags
                        .Where(ut => 
                            ut.Status == (int)eEntityStatus.OK &&
                            ut.IdUserNavigation.Status == (int)eEntityStatus.OK
                        )
                )
                .ThenInclude(k => k.IdUserNavigation)
                .FirstOrDefaultAsync(k =>
                    k.Id == id &&
                    k.Status == (int)eEntityStatus.OK);
            if (tag == null) throw new Exception("Not found");

            var t = _mapper.Map<TagDTO>(tag);
            t.Users = tag.UsersTags
                .Select(k => _mapper.Map<UserDTO>(k.IdUserNavigation))
                .ToList();
            return t;
        }

        // POST api/<TagsController>
        [HttpPost]
        public async Task Post([FromBody] TagDTO value)
        {
            var tag = _mapper.Map<Tags>(value);
            _dc.Add(tag);
            await _dc.Commit();
        }

        // PUT api/<TagsController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] TagDTO input)
        {
            var tag = await _dc.Tags
                .FirstOrDefaultAsync(k =>
                    k.Id == id &&
                    k.Status == (int)eEntityStatus.OK);
            if (tag == null) throw new Exception("Not found");

            tag.Name = input.Name;
            await _dc.Commit();
        }

        // DELETE api/<TagsController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var tag = await _dc.Tags
                .FirstOrDefaultAsync(k =>
                    k.Id == id &&
                    k.Status == (int)eEntityStatus.OK);
            if (tag == null) throw new Exception("Not found");

            _dc.Remove(tag);
            await _dc.Commit();
        }
    }
}
