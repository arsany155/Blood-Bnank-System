//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Web;

namespace BD_26.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Hospital_Request
    {
        public string commissary_ssn { get; set; }
        public string commissary_name { get; set; }
        public string Bloodtype { get; set; }
        public int Hospital_Id { get; set; }
        public string permission_path { get; set; }
        public int permission_ID { get; set; }
        public string Status { get; set; }

        public HttpPostedFileBase Imagefile { get; set; }

        public virtual Hospital Hospital { get; set; }
    }
}