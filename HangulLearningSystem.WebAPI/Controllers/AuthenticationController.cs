﻿using System.Threading;
using Application.Common.Constants;
using Application.Usecases.Command;
using Application.Usecases.CommandHandler;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!string.IsNullOrEmpty(result?.Token))
            {
                return Ok(result); // Trả về token cho client
            }
            else
            {
                return BadRequest(OperationMessages.CreateFail); // Token rỗng => đăng nhập thất bại
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (result == null)
            {
                return BadRequest("");
            }
            return Ok(result);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!string.IsNullOrEmpty(result))
            {
                return Ok(result); // Xác thực thành công, trả về message
            }
            else
            {
                return BadRequest(OperationMessages.CreateFail); // Xác thực thất bại
            }
        }

    }

}
