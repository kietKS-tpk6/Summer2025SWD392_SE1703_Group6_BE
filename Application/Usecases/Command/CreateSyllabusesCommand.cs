using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Usecases.Command
{
    public class CreateSyllabusesCommand : IRequest<bool>
    {
        public string SubjectID { get; set;}

        public string AccountID { get; set;}

        public string Description { get; set; }

        public string Note { get; set; }


    }
}
