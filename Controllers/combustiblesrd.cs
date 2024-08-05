using CombustiblesrdBack.Models;
using CombustiblesrdBack.Response;
using CombustiblesrdBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace CombustiblesrdBack.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Combustiblesrd : ControllerBase
    {

        private readonly ICombustibleService _combustibleService;

        public Combustiblesrd(ILogger<Combustiblesrd> logger, ICombustibleService combustibleService)
        {
            this._combustibleService = combustibleService;
        }
        [HttpGet("getHistory")]
        public async Task<IActionResult> GetHistoricAsync() 
        {
            IEnumerable<List<Combustible>> response = await _combustibleService.GetCombustiblesHistory();  
            return Ok(response);
        }

        [HttpGet("getPrices")]
        public async Task<IActionResult> GetAsync()
        {
            var response = new ApiResponse<IEnumerable<Combustible>>();
            try
            {
                response.Combustibles = await _combustibleService.GetCombustiblesLocalAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Se produjo un error al obtener los datos";
            }
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}
