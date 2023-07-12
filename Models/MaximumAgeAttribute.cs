using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BD_26.Models
{
    public class MaximumAgeAttribute : ValidationAttribute
    {
        int _maximumAge;
        public MaximumAgeAttribute(int maximumAge)
        {
            _maximumAge = maximumAge;
        }

       public override bool IsValid(object value)
       {
         DateTime date;
         if (DateTime.TryParse(value.ToString(), out date))
         {
            return date.AddYears(_maximumAge) > DateTime.Now;
         }
            return false;
       }
    }
}