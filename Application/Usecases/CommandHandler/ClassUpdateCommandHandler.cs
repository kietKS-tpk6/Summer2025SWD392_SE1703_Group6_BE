using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Constants;
using Application.IServices;
using Application.Usecases.Command;
using MediatR;

namespace Application.Usecases.CommandHandler
{
    public class ClassUpdateCommandHandler : IRequestHandler<ClassUpdateCommand, OperationResult<bool>>
    {
        private readonly IClassService _classService;

        public ClassUpdateCommandHandler(IClassService classService)
        {
            _classService = classService;
        }
        public async Task<OperationResult<bool>> Handle(ClassUpdateCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra số lượng học sinh đã đăng ký
            var enrolledCount = await _classService.GetEnrollmentCountAsync(request.ClassID);

            if (enrolledCount > 0)
            {
                return OperationResult<bool>.Fail("Không thể cập nhật lớp học vì đã có học sinh đăng ký.");
            }

            return await _classService.UpdateClassAsync(request);
        }

    }
}
