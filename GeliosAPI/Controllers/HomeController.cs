using GeliosAPI.AppServices.GeliosAppService;
using GeliosAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeliosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IGelios _iGelios;
        public HomeController(IGelios iGelios)
        {
            _iGelios = iGelios;
        }

        [HttpGet]
        public async Task<List<CarModel>> GetData()
        {
            await _iGelios.GetCarInfo();
            return _iGelios.GetNameValidCars();
        }

        [HttpPut]
        public async Task UpdateCar(CarModel updatedCar)=> await _iGelios.UpdateCar(updatedCar);

    }
}
