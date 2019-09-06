using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MissingPeople.Models;
using MissingPeople.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MissingPeople.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoundPersonController : ControllerBase
    {
        private LostPersonService _LostPersonService;
        private FoundPersonService _FoundPersonService;
        private string _URL;

        public FoundPersonController(FoundPersonService foundPersonService, LostPersonService lostPersonService, IDatabaseSettings databaseSettings)
        {
            _LostPersonService = lostPersonService;
            _FoundPersonService = foundPersonService;
            _URL = databaseSettings.FlaskAPI;
            //_FaceRecognitionService = faceRecognitionService;
        }

        // GET: api/LostPerson/5
        [HttpGet("{id}")]
        public ActionResult Get(string id) => Ok(_FoundPersonService.Get(id));

        // POST: api/LostPerson
        [HttpPost]
        public ActionResult Post(Person person)
        {
            _FoundPersonService.Create(person);
            Task.Run(async () =>
            {
                string id = person.ID;
                using (var httpClient = new HttpClient())
                {
                    HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        {"force","0" },
                        {"id", id },
                        {"gender", ((int)person.Gender).ToString() },
                        {"image", person.ImageURL }
                    });
                    var response = await httpClient.PostAsync(_URL, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<JObject>(responseString);
                    if (obj["result"] is JArray result)
                    {
                        foreach (var res in result)
                        {
                            string _id = res[0].ToString();
                            float percentage = float.Parse(res[1].ToString());
                            var p = _LostPersonService.AddNewSimilarPerson(_id, new SimilarPerson()
                            {
                                Similarity = percentage,
                                ContactPhone = person.ContactPhone,
                                ImageURL = person.ImageURL
                            });
                            Console.WriteLine(p);
                        }
                    }
                }
            });
            return Ok(person);
        }
    }
}
