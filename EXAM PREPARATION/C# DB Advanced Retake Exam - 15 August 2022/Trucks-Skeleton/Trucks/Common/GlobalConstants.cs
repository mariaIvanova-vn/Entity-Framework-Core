
namespace Trucks.Common
{
    public class GlobalConstants
    {
        //Truck
        public const int TRUCK_RegistrationNumber_MAX_LENGTH = 8;
        public const int TRUCK_VINNUMBER_MAX_LENGTH = 17;

        public const int TRUCK_TankCapacity_MIN_LENGTH = 950;
        public const int TRUCK_TankCapacity_MAX_LENGTH = 1420;

        public const int TRUCK_CargoCapacity_MIN_LENGTH = 5000;
        public const int TRUCK_CargoCapacity_MAX_LENGTH = 29000;

        public const string TRUCK_RegistrationNumber_REGEX = @"^[A-Z]{2}\d{4}[A-Z]{2}$";

        public const int TRUCK_CategoryType_MIN_VALUE = 0;
        public const int TRUCK_CategoryType_MAX_VALUE = 3;
        public const int TRUCK_MakeType_MIN_VALUE = 0;
        public const int TRUCK_MakeType_MAX_VALUE = 4;

        //Despatcher
        public const int Despatcher_NAME_MIN_LENGTH = 2;
        public const int Despatcher_NAME_MAX_LENGTH = 40;
    }
}
