using GeliosAPI.Models;

namespace GeliosAPI.AppServices.GeliosAppService
{
    public interface IGelios
    {
        Task GetCarInfo();

        List<CarModel> GetAllCars();
        List<CarModel> GetNameValidCars();

        Task UpdateCar(CarModel updatedCar);

        int CountInvalidCars();
    }
}
