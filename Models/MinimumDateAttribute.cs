using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BD_26.Models
{
    public class MinimumDateAttribute : ValidationAttribute
    {
        int _minimumAge;

        public MinimumDateAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        public override bool IsValid(object value)
        {
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date))
            {
                return date.AddDays(_minimumAge) < DateTime.Now;
            }
            return false;
        }
    }
}