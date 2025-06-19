using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuestionsController (IMediator mediator)
        {
            _mediator = mediator;
        }
       
    }
}
