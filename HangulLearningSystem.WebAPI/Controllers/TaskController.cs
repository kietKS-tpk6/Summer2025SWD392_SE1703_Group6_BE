﻿using Application.DTOs;
using Application.Usecases.Command;
using Application.Usecases.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static HangulLearningSystem.WebAPI.Controllers.TestController;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpGet("lecturer/{lecturerId}")]
        public async Task<IActionResult> GetTasksByLecturerId(string lecturerId)
        {
            var query = new GetTasksByLecturerIdQuery { LecturerId = lecturerId };
            var result = await _mediator.Send(query);

            if (result.Success)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(string taskId)
        {
            var query = new GetTaskByIdQuery { TaskId = taskId };
            var result = await _mediator.Send(query);

            if (result.Success)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpGet("all")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllTasks()
        {
            var query = new GetAllTasksQuery();
            var result = await _mediator.Send(query);

            if (result.Success)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpPut("{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus(string taskId, [FromBody] UpdateTaskStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new UpdateTaskStatusCommand
            {
                TaskId = taskId,
                Status = request.Status,
                DateStart = request.DateStart,
                Deadline = request.Deadline
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = result.Message
            });
        }

        [HttpPut("{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(string taskId, [FromBody] CompleteTaskRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new CompleteTaskCommand
            {
                TaskId = taskId,
                LecturerID = request.LecturerID,
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = result.Message
            });
        }


        [HttpDelete("{taskId}")]
        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            var command = new DeleteTaskCommand { TaskId = taskId };
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = result.Message
            });
        }
    }
}