using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BD_26.Models
{
    public class ResetPasswordModel
    {
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false)]
        public string NewPassword { get; set; }


        [Required]
        public string ResetCode { get; set; }
    }
}