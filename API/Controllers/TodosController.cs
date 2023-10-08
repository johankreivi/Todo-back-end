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
            await _repository.AddAsync(todo);
            var todoDto = _mapper.Map<TodoDto>(todo);

            return Ok(todoDto);
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
            await _repository.UpdateAsync(todo);
            var todoDto = _mapper.Map<TodoDto>(todo);

            return Ok(todoDto);
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

        // Create a async Get Method to Get Todo count using generic repository pattern and IRepository interface
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

