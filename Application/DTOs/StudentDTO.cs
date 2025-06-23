using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
namespace Application.DTOs
{
    public class StudentDTO
    {
        public string StudentID { get; set; }             
        public string FullName { get; set; }               
        public Gender  Gender{ get; set; }              
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateOnly BirthDate { get; set; }
        public string ImageUrl { get; set; }
    }

}
