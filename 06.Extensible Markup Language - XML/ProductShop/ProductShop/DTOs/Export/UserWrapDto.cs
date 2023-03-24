using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    [XmlType("Users")]
    public class UserWrapDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlElement("users")]
        public UserDto[] Users { get; set; } = null!;
    }
}
