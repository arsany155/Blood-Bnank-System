//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BD_26.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class BloodBag
    {
        public int BloodBag_id { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Blood type is required")]
        public string bloodtype { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Ssn is required")]
        public string DSsn { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Bnak id is required")]
        public int bank_id { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = " BloodBag date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public System.DateTime BloodBag_Donation { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = " BloodBag exp  date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public System.DateTime BloodBag_Expiration { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = " Status is required")]
        public string Status { get; set; }

        public virtual BloodBank BloodBank { get; set; }
        public virtual Donor Donor { get; set; }
    }
}