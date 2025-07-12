using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, bool>
    {
        private readonly IAccountService _accountService;

        public CreateAccountCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public async Task<bool> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra email đã tồn tại chưa
            var accByEmail = await _accountService.CheckEmailExistAsync(request.Email);
            if (accByEmail)
                throw new ArgumentException("Email đã được sử dụng, vui lòng chọn email khác.");

            // Kiểm tra số điện thoại đã tồn tại chưa
            var accByPhone = await _accountService.CheckPhoneExistAsync(request.PhoneNumber);
            if (accByPhone )
                throw new ArgumentException("Số điện thoại đã được sử dụng, vui lòng chọn số điện thoại khác.");

            // Tạo tài khoản
            return await _accountService.CreateAccountByManager(request);
        }

    }
}
