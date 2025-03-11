using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace Gymwebproject.Model
{
    public class Userinformation
    {
        [Key]

        public int ID {get; set;}

        public required string Fullname { get; set; }

        public required string Email { get; set; }

        public required string password { get; set; }

       public required DateTime dob { get; set; }


    }
}
