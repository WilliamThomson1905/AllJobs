using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace AllJobs.ModelValidation
{
    public class DateGreaterThanNow : ValidationAttribute
    {
        // Custom validation used to determine if the closing date of an Advertisement that is being created is beofre DateTime.Now
        public override bool IsValid(object value)
        {
            DateTime date = Convert.ToDateTime(value);
            return date >= DateTime.Now;
        }
    }


    public class DateBeforeNow : ValidationAttribute
    {
        // Custom validation used to determine if the closing date of an Advertisement that is being created is beofre DateTime.Now
        public override bool IsValid(object value)
        {
            DateTime date = Convert.ToDateTime(value);
            return date < DateTime.Now;
        }
    }


}