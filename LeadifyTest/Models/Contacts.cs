using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeadifyTest.Models
{
    public class Contacts
    {
        [Key]
        public int ContactId { get; set; }

        [Required(ErrorMessage = "First Name is Required")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required.")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number is Required")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Phone Number is not valid.")]
        public string Cellphone { get; set; }

        [Required(ErrorMessage = "Email Address is Required")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Email Address is not valid.")]
        public string Email { get; set; }

        public int UserId { get; set; }

    }
}