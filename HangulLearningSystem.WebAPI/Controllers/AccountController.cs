using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAccountService _accountService;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create-account")]
        public async Task<IActionResult> Create([FromBody] CreateAccountCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (result)
            {
                return Ok(OperationMessages.CreateSuccess);
            }
            else
            {
                return BadRequest(OperationMessages.CreateFail);
            }
        }
        [HttpGet("list-account-with-role-gender-status")]
        public async Task<IActionResult> ListAccountWithRole([FromBody] GetPaginatedAccountListCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _mediator.Send(command, cancellationToken);

                // Kiểm tra xem có dữ liệu không
                if (result != null && result.Items != null)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Lấy danh sách tài khoản thành công",
                        Data = result
                    });
                }
                else
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Không có dữ liệu",
                        Data = new PagedResult<AccountForManageDTO>
                        {
                            Items = new List<AccountForManageDTO>(),
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi lấy danh sách tài khoản",
                    Error = ex.Message
                });
            }
        }

    }
}
