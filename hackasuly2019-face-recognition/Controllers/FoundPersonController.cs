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
        private PersonService _PersonService;
        private string _URL;

        public FoundPersonController(PersonService personService, IDatabaseSettings databaseSettings)
        {
            _PersonService = personService;
            _URL = databaseSettings.FlaskAPI;
            //_FaceRecognitionService = faceRecognitionService;
        }

        [HttpGet]
        public ActionResult Get() => Ok("nice");

        // GET: api/LostPerson/5
        [HttpGet("{id}")]
        public ActionResult Get(string id) => Ok(_PersonService.Get(id, PersonType.Found));

        // POST: api/LostPerson
        [HttpPost]
        public async Task<ActionResult> Post(Person person)
        {
            _PersonService.Create(person, PersonType.Found);

            using (var httpClient = new HttpClient())
            {
                HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        {"force","0" },
                        {"id", person.ID },
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

                        var otherPerson = _PersonService.Get(_id, PersonType.Lost);
                        if (otherPerson == null)
                            continue;

                        var lostSimilar = new SimilarPerson()
                        {
                            Name = otherPerson.Name,
                            Similarity = percentage,
                            ContactPhone = otherPerson.ContactPhone,
                            ImageURL = otherPerson.ImageURL,
                            CreatedAt = otherPerson.CreatedAt
                        };
                        var foundSimilar = new SimilarPerson()
                        {
                            Name = person.Name,
                            Similarity = percentage,
                            ContactPhone = person.ContactPhone,
                            ImageURL = person.ImageURL,
                            CreatedAt = person.CreatedAt
                        };

                        _PersonService.AddNewSimilarPerson(_id, foundSimilar, PersonType.Lost);
                        _PersonService.AddNewSimilarPerson(otherPerson.ID, lostSimilar, PersonType.Found);

                        person.SimilarPeople.Add(lostSimilar);
                    }
                }
            }

            return Ok(person);
        }
    }
}
