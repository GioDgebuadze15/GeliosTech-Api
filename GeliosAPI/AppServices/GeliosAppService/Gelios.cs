using GeliosAPI.EntityFramework;
using GeliosAPI.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace GeliosAPI.AppServices.GeliosAppService
{
    public class Gelios : IGelios
    {
        private const string url = "url here";
        private static readonly Regex carNameExceptedPattern = new Regex("([A-Z]{2}[-_][0-9]{3}[-_][A-Z]{2})");
        private static readonly Regex carNameCorrectPattern = new Regex("([A-Z]{2}[0-9]{3}[A-Z]{2})");
        private static readonly Regex differentCarNameCorrectPattern = new Regex("([A-Z]{2}[0-9]{3}[A-Z])");
        private static readonly Regex differentCarNamePattern = new Regex("([A-Z]{2}[-_][0-9]{3}[-_][A-Z])");
        private static readonly Regex alsoDifferentCarNameCorrectPattern = new Regex("([A-Z]{2}[0-9]{4})");
        private static readonly Regex alsoDifferentCarNamePattern = new Regex("([A-Z]{2}[-_][0-9]{4})");

        private readonly AppDbContext _ctx;

        public Gelios(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task GetCarInfo()
        {
            if (CountCars() != 0)
            {
                var oldCars = GetAllCars();
                var newCars = await GetCarsFromGelios();

                if (newCars.Count > 0)
                {
                    //delete
                    oldCars.RemoveAll(oldCar => !newCars.Any(newCar => oldCar.CarId == newCar.CarId));

                    //update
                    foreach (var oldCar in oldCars)
                    {
                        var updatedNewCar = newCars.Where(newCar => newCar.CarId == oldCar.CarId && newCar.Name != oldCar.Name).FirstOrDefault();
                        if (updatedNewCar != null)
                        {
                            CarModel updatedCar = FillCarInfo(updatedNewCar);
                            oldCar.Name = updatedCar.Name;
                            oldCar.IsNameValid = updatedCar.IsNameValid;
                        }
                    }

                    //insert
                    oldCars.AddRange(newCars.Where(newCar => !oldCars.Any(oldCar => newCar.CarId == oldCar.CarId)).ToList());


                    //update database
                    await UpdateCarsIntoDatabase(oldCars);
                }
            }
            else
            {
                var cars = await GetCarsFromGelios();
                if (cars.Count > 0)
                    await AddCarsIntoDatabase(cars);
            }
        }

        private static async Task<List<CarModel>> GetCarsFromGelios()
        {
            List<CarModel> cars = new List<CarModel>();
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (response != null)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var getResult = JsonConvert.DeserializeObject<List<CarModel>>(jsonString);

                    if (getResult != null)
                    {
                        for (int i = 0; i < getResult.Count; i++)
                        {
                            CarModel car = new CarModel()
                            {
                                CarId = getResult[i].Id,
                                Creator = getResult[i].Creator,
                                Name = getResult[i].Name

                            };
                            
                            cars.Add(FillCarInfo(car));
                        }
                    }
                }
            }
            return cars;
        }


        public List<CarModel> GetAllCars() => _ctx.Cars.ToList();
        public List<CarModel> GetNameValidCars() => _ctx.Cars.Where(x => x.IsNameValid == true).ToList();


        private async Task AddCarsIntoDatabase(List<CarModel> cars)
        {
            _ctx.Cars.AddRange(cars);
            await _ctx.SaveChangesAsync();

        }

        private async Task UpdateCarsIntoDatabase(List<CarModel> cars)
        {
            _ctx.Cars.UpdateRange(cars);
            await _ctx.SaveChangesAsync();

        }

        public async Task UpdateCar(CarModel updatedCar)
        {
            CarModel? carModel = _ctx.Cars.FirstOrDefault(x => x.Id == updatedCar.Id);
            bool isInspeced = false;
            if (carModel != null)
            {
                if (updatedCar.LastDate != null && updatedCar.NextDate != null)
                {
                    if (updatedCar.LastDate.Contains(':'))
                    {
                        isInspeced = true;
                        updatedCar.LastDate = updatedCar.LastDate.Split(':')[1].Trim();
                    }

                    carModel.LastDate = updatedCar.LastDate;
                    carModel.NextDate = updatedCar.NextDate;
                    carModel.IsInspected = isInspeced;
                    carModel.LastUpdated = DateTime.Now;
                }
                _ctx.Cars.Update(carModel);
                await _ctx.SaveChangesAsync();
            }

        }

        public int CountCars() => _ctx.Cars.Count();
        public int CountInvalidCars() => _ctx.Cars.Where(x => x.IsNameValid == false).Count();

        private static CarModel FillCarInfo(CarModel car)
        {
            string name = car.Name.ToUpper();
            name = Regex.Replace(name, @"\s+", "");
            if (MatchesExceptedPattern(name))
            {
                car.Name = ParseCarName(name);
                car.IsNameValid = true;
            }
            else if (MatchesCorrectPattern(name))
            {

                car.Name = GetCorrectCarName(name);
                car.IsNameValid = true;
            }
            else if (MatchesCorrectDifferentPattern(name))
            {
                car.Name = GetCorrectDifferenctCarName(name);
                car.IsNameValid = true;
            }
            else if (MatchesDifferentPattern(name))
            {
                car.Name = ParseDifferentCarName(name);
                car.IsNameValid = true;
            }
            else if (MatchesCorrectAlsoDifferentPattern(name))
            {
                car.Name = GetCorrectAlsoDifferentCarName(name);
                car.IsNameValid = true;
            }
            else if (MatchesAlsoDifferentPattern(name))
            {
                car.Name = ParseAlsoDifferentCarName(name);
                car.IsNameValid = true;
            }
            else
            {
                car.Name = name;
                car.IsNameValid = false;
            }
            return car;
        }

        private static bool MatchesExceptedPattern(string name) => carNameExceptedPattern.IsMatch(name);
        private static bool MatchesCorrectPattern(string name) => carNameCorrectPattern.IsMatch(name);
        private static bool MatchesCorrectDifferentPattern(string name) => differentCarNameCorrectPattern.IsMatch(name);
        private static bool MatchesDifferentPattern(string name) => differentCarNamePattern.IsMatch(name);
        private static bool MatchesCorrectAlsoDifferentPattern(string name) => alsoDifferentCarNameCorrectPattern.IsMatch(name);
        private static bool MatchesAlsoDifferentPattern(string name) => alsoDifferentCarNamePattern.IsMatch(name);
        private static string GetCorrectCarName(string name) => carNameCorrectPattern.Match(name).Value;
        private static string GetCorrectDifferenctCarName(string name) => differentCarNameCorrectPattern.Match(name).Value;
        private static string GetCorrectAlsoDifferentCarName(string name) => alsoDifferentCarNameCorrectPattern.Match(name).Value;

        private static string ParseCarName(string name)
        {
            string match = carNameExceptedPattern.Match(name).Value;
            StringBuilder sb = new StringBuilder(match);
            sb.Remove(2, 1);
            sb.Remove(5, 1);
            return sb.ToString();

        }

        private static string ParseDifferentCarName(string name)
        {
            string match = differentCarNamePattern.Match(name).Value;
            StringBuilder sb = new StringBuilder(match);
            sb.Remove(2, 1);
            sb.Remove(5, 1);
            return sb.ToString();

        }

        private static string ParseAlsoDifferentCarName(string name)
        {
            string match = alsoDifferentCarNamePattern.Match(name).Value;
            StringBuilder sb = new StringBuilder(match);
            sb.Remove(2, 1);
            return sb.ToString();

        }


    }
}
