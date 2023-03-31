using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Footballer")]
    public class ImportFootballerDto
    {
        [Required]
        [XmlElement("Name")]
        [MinLength(2)]
        [MaxLength(40)]
        public string Name { get; set; } = null!;

        [XmlElement("ContractStartDate")]
        public string? ContractStartDate { get; set; }

        [XmlElement("ContractEndDate")]
        public string? ContractEndDate { get; set; }

        [XmlElement("BestSkillType")]
        [Range(0,4)]
        public int BestSkillType { get; set; }

        [XmlElement("PositionType")]
        [Range(0,3)]
        public int PositionType { get; set; }
    }
}
