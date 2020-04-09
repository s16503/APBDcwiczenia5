using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APBDcwiczenia5.DOTs.Requests
{
    public class EnrollStudentRequest
    {

        [RegularExpression("^s[0-9]+$")]
        [Required]
        public string IndexNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        public string BirthDate { get; set; }

        [Required(ErrorMessage = "Podaj studia")]
        public string Studies { get; set; }
    }
}
