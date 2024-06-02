using System;
using System.Threading.Tasks;

namespace FlyingVehiclesLibrary
{
    public class Airplane : IFlyingVehicle
    {
        public int RunwayLength { get; set; }

        public event EventHandler<int> TakeOffProgressChanged;

        public async Task<bool> SimulatedTakeOff()
        {
            Console.WriteLine("Самолет начинает разбег...");

            int steps = 0;
            Random random = new Random();
            while (steps < 10)
            {
                await Task.Delay(500);
                steps++;
                Console.WriteLine($"Самолет прошёл {steps * 10}% расстояния.");

                TakeOffProgressChanged?.Invoke(this, steps * 10);

                if (random.Next(0, 100) < 3)
                {
                    return false;
                }
            }

            Console.WriteLine("Самолет взлетает...");
            return true;
        }

        public bool TakeOff()
        {
            if (RunwayLength >= 2000)
            {
                return SimulatedTakeOff().Result;
            }
            else
            {
                Console.WriteLine("Недостаточно длины взлётной полосы!");
                return false;
            }
        }

        public bool Land()
        {
            Console.WriteLine("Самолет приземляется...");
            return true;
        }
    }
}
