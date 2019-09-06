using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MissingPeople.Services;
using MissingPeople.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MissingPeople.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LostPersonController : ControllerBase
    {
        private PersonService _PersonService;
        private string _URL;

        public LostPersonController(PersonService personService, IDatabaseSettings databaseSettings)
        {
            _PersonService = personService;
            _URL = databaseSettings.FlaskAPI;
            //_FaceRecognitionService = faceRecognitionService;
        }

        // GET: api/LostPerson/5
        [HttpGet("{id}")]
        public ActionResult Get(string id) => Ok(_PersonService.Get(id, PersonType.Lost));

        // POST: api/LostPerson
        [HttpPost]
        public async Task<ActionResult> PostAsync(Person person)
        {
            _PersonService.Create(person, PersonType.Lost);

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

                        var otherPerson = _PersonService.Get(_id, PersonType.Found);
                        if (otherPerson == null)
                            continue;

                        var lostSimilar = new SimilarPerson()
                        {
                            Name = person.Name,
                            Similarity = percentage,
                            ContactPhone = person.ContactPhone,
                            ImageURL = person.ImageURL,
                            CreatedAt = person.CreatedAt
                        };
                        var foundSimilar = new SimilarPerson()
                        {
                            Name = otherPerson.Name,
                            Similarity = percentage,
                            ContactPhone = otherPerson.ContactPhone,
                            ImageURL = otherPerson.ImageURL,
                            CreatedAt = otherPerson.CreatedAt
                        };

                        _PersonService.AddNewSimilarPerson(person.ID, foundSimilar, PersonType.Lost);
                        _PersonService.AddNewSimilarPerson(otherPerson.ID, lostSimilar, PersonType.Found);

                        person.SimilarPeople.Add(lostSimilar);
                    }
                }
            }

            return Ok(person);
        }
    }
}
