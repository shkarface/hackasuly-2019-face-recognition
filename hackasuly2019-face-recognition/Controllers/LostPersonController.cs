using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MissingPeople.Services;
using MissingPeople.Models;
using System.Net.Http;

namespace MissingPeople.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LostPersonController : ControllerBase
    {
        private LostPersonService _LostPersonService;
        private string _URL;

        public LostPersonController(LostPersonService lostPersonService, IDatabaseSettings databaseSettings)
        {
            _LostPersonService = lostPersonService;
            _URL = databaseSettings.FlaskAPI;
            //_FaceRecognitionService = faceRecognitionService;
        }

        // GET: api/LostPerson/5
        [HttpGet("{id}")]
        public ActionResult Get(string id) => Ok(_LostPersonService.Get(id));

        // POST: api/LostPerson
        [HttpPost]
        public ActionResult Post(Person person)
        {
            _LostPersonService.Create(person);
            Task.Run(async () =>
            {
                string id = person.ID;
                using (var httpClient = new HttpClient())
                {
                    HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        {"force","1" },
                        {"id", id },
                        {"gender", ((int)person.Gender).ToString() },
                        {"image", person.ImageURL }
                    });

                    var response = await httpClient.PostAsync(_URL, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                }

            });
            return Ok(person);
        }
    }
}
