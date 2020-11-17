using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BasePackageModule1.Models
{
    public class Pincode
    {
        public int Id { get; set; }

        [DisplayName("Pin Code")]
        [MinLength(6)]
        [MaxLength(6)]
        [DataType(DataType.PhoneNumber)]
        public string Code { get; set; }
    }
}
