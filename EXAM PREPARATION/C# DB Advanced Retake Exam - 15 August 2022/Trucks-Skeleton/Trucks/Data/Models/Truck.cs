using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trucks.Common;
using Trucks.Data.Models.Enums;

namespace Trucks.Data.Models
{
    public class Truck
    {
        public Truck()
        {
            this.ClientsTrucks = new HashSet<ClientTruck>();
        }
        [Key]
        public int Id { get; set; }

        [MaxLength(GlobalConstants.TRUCK_RegistrationNumber_MAX_LENGTH)]
        public string RegistrationNumber { get; set; }

        [Required]
        [MaxLength(GlobalConstants.TRUCK_VINNUMBER_MAX_LENGTH)]
        public string VinNumber { get; set; } = null!;

        //[MaxLength(GlobalConstants.TRUCK_TankCapacity_MAX_LENGTH)]
        public int TankCapacity { get; set; }

        //[MaxLength(GlobalConstants.TRUCK_CargoCapacity_MAX_LENGTH)]
        public int CargoCapacity { get; set; }

        public CategoryType CategoryType { get; set; }

        public MakeType MakeType { get; set; }

        [ForeignKey(nameof(Despatcher))]
        public int DespatcherId { get; set; }
        public Despatcher Despatcher { get; set; } = null!;

        public virtual ICollection<ClientTruck> ClientsTrucks { get; set; }
    }
}
