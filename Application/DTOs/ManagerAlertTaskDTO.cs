using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ManagerAlertTaskDTO
    {
        public string Type { get; set; }          
        public string Message { get; set; }   
        public DateTime? Deadline { get; set; }       
        public string Severity { get; set; }      
    }

}
