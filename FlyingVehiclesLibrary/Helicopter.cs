using System;

namespace FlyingVehiclesLibrary
{
    public class Helicopter : IFlyingVehicle
    {
        public bool TakeOff()
        {
            Console.WriteLine("Вертолет взлетает...");
            return true;
        }

        public bool Land()
        {
            Console.WriteLine("Вертолет приземляется...");
            return true;
        }
    }
}
