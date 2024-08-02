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

        private readonly ILogger<Combustiblesrd> _logger;
        private readonly ICombustibleService _combustibleService;

        public Combustiblesrd(ILogger<Combustiblesrd> logger, ICombustibleService combustibleService)
        {
            _logger = logger;
            this._combustibleService = combustibleService;
        }

        [HttpGet("getPrices")]
        public IActionResult Get()
        {
            var response = new ApiResponse<List<Combustible>>();
            try
            {
                response.Combustibles = this._combustibleService.GetCombustible();
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
