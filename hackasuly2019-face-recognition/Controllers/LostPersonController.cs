using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using hackasuly2019_face_recognition.Services;
using MissingPeople.Models;

namespace hackasuly2019_face_recognition.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LostPersonController : ControllerBase
    {
        private LostPersonService _LostPersonService;
        public LostPersonController(LostPersonService lostPersonService)
        {
            _LostPersonService = lostPersonService;
        }

        // GET: api/LostPerson
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/LostPerson/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/LostPerson
        [HttpPost]
        public async void Post([FromBody] Person value)
        {
            await _LostPersonService.CreateAsync(value);
        }

        // PUT: api/LostPerson/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
