using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using Trucks.Common;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Truck")]
    public class ImportTruckDto
    {
        [XmlElement("RegistrationNumber")]
        [MinLength(GlobalConstants.TRUCK_RegistrationNumber_MAX_LENGTH)]
        [MaxLength(GlobalConstants.TRUCK_RegistrationNumber_MAX_LENGTH)]
        [RegularExpression(GlobalConstants.TRUCK_RegistrationNumber_REGEX)]
        public string RegistrationNumber { get; set; }

        [XmlElement("VinNumber")]
        [Required]
        [MaxLength(GlobalConstants.TRUCK_VINNUMBER_MAX_LENGTH)]
        public string VinNumber { get; set; } = null!;

        [XmlElement("TankCapacity")]       
        [Range(GlobalConstants.TRUCK_TankCapacity_MIN_LENGTH, GlobalConstants.TRUCK_TankCapacity_MAX_LENGTH)]
        public int TankCapacity { get; set; }

        [XmlElement("CargoCapacity")]
        [Range(GlobalConstants.TRUCK_CargoCapacity_MIN_LENGTH, GlobalConstants.TRUCK_CargoCapacity_MAX_LENGTH)]
        public int CargoCapacity { get; set; }

        [XmlElement("CategoryType")]
        [Range(GlobalConstants.TRUCK_CategoryType_MIN_VALUE, GlobalConstants.TRUCK_CategoryType_MAX_VALUE)]
        public int CategoryType { get; set; }

        [XmlElement("MakeType")]
        [Range(GlobalConstants.TRUCK_MakeType_MIN_VALUE, GlobalConstants.TRUCK_MakeType_MAX_VALUE)]
        public int MakeType { get; set; }
    }
}
