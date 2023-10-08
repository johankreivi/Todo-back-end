using Entity;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Api.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Serilog;
using System.Net;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {

        private readonly IRepository<Todo> _repository;
        private readonly IMapper _mapper;

        public TodosController(IRepository<Todo> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTodo(Todo todo)
        {
            try
            {
                await _repository.AddAsync(todo);
                var todoDto = _mapper.Map<TodoDto>(todo);

                return Ok(todoDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while creating a todo.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while creating the todo. Please try again later.");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update(Todo todo)
        {
            try
            {
                await _repository.UpdateAsync(todo);
                var todoDto = _mapper.Map<TodoDto>(todo);

                return Ok(todoDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while updating a todo.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while updating the todo. Please try again later.");
            }
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodos(int pageNumber, int pageSize)
        {
            var todos = await _repository.GetAllAsync(pageNumber, pageSize);
            var todosDto = _mapper.Map<IEnumerable<TodoDto>>(todos);

            if (todosDto.Count() == 0)
            {
                return NoContent();
            }

            return Ok(todosDto);
        }

        [HttpGet("count")]
        public async Task<ActionResult> GetTodoCount()
        {

            try
            {
                var count = await _repository.GetCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while fetching the todo count.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while fetching the todo count. Please try again later.");
            }
        }

        [HttpPut("deadline")]
        public async Task<ActionResult> ChangeDeadline([FromBody] ChangeDeadlineRequest request)
        {
            await _repository.UpdateDeadline(request.id, request.deadline);

            return Ok();
        }
    }



}

