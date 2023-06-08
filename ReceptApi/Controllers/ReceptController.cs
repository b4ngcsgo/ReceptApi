using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReceptApi.Models;
using ReceptApi.Repos;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ReceptApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly ReceptRepo _receptRepo;
        public ReceptController(IConfiguration config, ReceptRepo receptRepo)
        {
            _config = config;
            _receptRepo = receptRepo;
        }

        [HttpPost("SkapaRecept")]

        public async Task<IActionResult> AddRecept(Recept recept)
        {
             await _receptRepo.SkapaRecept(recept);
            return Ok("Du har skapat ett recept");
        }

        [HttpPut("UppdateraRecept")]

        public async Task<IActionResult> UpdateRecept(Recept recept)
        {
            await _receptRepo.UppdateraRecept(recept);
            return Ok("Du har uppdaterat ett recept");
        }

        [HttpDelete("TaBortRecept")]
        public async Task<IActionResult> DeleteRecept(int receptid, int userid)
        {
            try
            {
                await _receptRepo.TaBortRecept(receptid, userid);
                return Ok("Du har tagit bort ett recept");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("HämtaAllaRecept")]
        public async Task<IActionResult> GetAllRecept()
        {
            var recept = await _receptRepo.HämtaAllaRecept();
            return Ok(recept);
        }

        [HttpPost("betyg")]
        public async Task<IActionResult> BetygsattRecept([FromBody] Betyg betyg)
        {
            try
            {
                await _receptRepo.BetygsattRecept(betyg.receptid, betyg.userid, betyg.betyg);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
