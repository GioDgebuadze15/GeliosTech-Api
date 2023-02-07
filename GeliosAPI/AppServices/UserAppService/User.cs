using GeliosAPI.EntityFramework;
using GeliosAPI.Models;

namespace GeliosAPI.AppServices.UserAppService
{
    public class User: IUser
    {
        private readonly AppDbContext _ctx;

        public User(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public List<CarModel> GetUserCars(int id) => _ctx.Cars.Where(x => x.Creator == id).ToList();
        
    }
}
