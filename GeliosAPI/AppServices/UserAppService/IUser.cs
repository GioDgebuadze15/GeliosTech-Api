using GeliosAPI.Models;

namespace GeliosAPI.AppServices.UserAppService
{
    public interface IUser
    {
        public List<CarModel> GetUserCars(int id );
    }
}
