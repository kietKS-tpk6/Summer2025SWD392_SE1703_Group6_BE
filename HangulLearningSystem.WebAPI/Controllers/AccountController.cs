using Application.Common.Constants;
using Application.Common.Shared;
using Application.DTOs;
using Application.IServices;
using Application.Usecases.Command;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HangulLearningSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAccountService _accountService;

        public AccountController(IMediator mediator, IAccountService accountService)
        {
            _mediator = mediator;
            _accountService = accountService;
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
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
        [HttpDelete("{accountId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteAccount(string accountId)
        {
            var currentUserId = User.FindFirst("id")?.Value;
            var result = await _accountService.DeleteAccountAsync(accountId, currentUserId);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
        [HttpGet("list-account-with-role-gender-status")]
        public async Task<IActionResult> ListAccountWithRole([FromQuery] GetPaginatedAccountListCommand command, CancellationToken cancellationToken)
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

        [HttpGet("teaching-schedule")]
        public async Task<IActionResult> GetTeachingSchedule()
        {
            var result = await _accountService.GetTeachingSchedule();

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpGet("teaching-schedule-detail/{accountId}")]
        public async Task<IActionResult> GetTeachingScheduleDetailByID(string accountId)
        {
            var result = await _accountService.GetTeachingScheduleDetailByID(accountId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        [HttpGet("get-by-role-actived")]
        public async Task<IActionResult> GetAccountsByRole([FromQuery] AccountRole role)
        {
            var result = await _accountService.GetListAccountByRoleAsync(role);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }
       
       
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetById(string accountId)
        {
            var result = await _accountService.GetAccountByIdAsync(accountId);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result.Data);
        }
        [HttpGet("{accountID}/image")]
        public async Task<IActionResult> GetAccountImage(string accountID)
        {
            var result = await _accountService.GetAccountImageAsync(accountID);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchAccounts([FromQuery] SearchAccountsQueryCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("get-lecturer-free")]
        public async Task<IActionResult> GetFreeLecturers([FromQuery] CheckLecturerFreeCommand request)
        {
            var result = await _mediator.Send(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
