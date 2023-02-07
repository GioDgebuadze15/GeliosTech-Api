using GeliosAPI.AppServices.UserAppService;
using GeliosAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeliosAPI.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class UserController: ControllerBase
    {
        private readonly IUser _iUser;

        public UserController(IUser iUser)
        {
            _iUser = iUser;
        }

        [HttpGet("{id}")]
        public List<CarModel> GetUserCars(int id) => _iUser.GetUserCars(id);

    }
}
