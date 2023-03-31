using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using Trucks.Common;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType("Truck")]
    public class ExportTruckDto
    {
        [XmlElement("RegistrationNumber")]
        //[MinLength(GlobalConstants.TRUCK_RegistrationNumber_MAX_LENGTH)]
        //[MaxLength(GlobalConstants.TRUCK_RegistrationNumber_MAX_LENGTH)]
        //[RegularExpression(GlobalConstants.TRUCK_RegistrationNumber_REGEX)]
        public string RegistrationNumber { get; set; } = null!;

        [XmlElement("Make")]
        //[Range(GlobalConstants.TRUCK_MakeType_MIN_VALUE, GlobalConstants.TRUCK_MakeType_MAX_VALUE)]
        public string Make { get; set; } = null!;
    }
}
