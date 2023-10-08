using Entity;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Api.Dto;
using AutoMapper;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {

        // Create, interface refrence, constructor and a async Post Method to Create Todo using generic repository pattern and IRepository interface
        private readonly IRepository<Todo> _repository;
        private readonly IMapper _mapper;

        public TodosController(IRepository<Todo> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // Create a async Post Method to Create Todo using generic repository pattern and IRepository interface, use automapper profile to map TodoDto to Todo
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

        // Create a async Get Method to Get All Todos using generic repository pattern and IRepository interface, use automapper profile to map Todo to TodoDto
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
            var count = await _repository.GetCountAsync();

            return Ok(count);
        }

        [HttpPut("deadline")]
        public async Task<ActionResult> ChangeDeadline([FromBody] ChangeDeadlineRequest request)
        {
            await _repository.UpdateDeadline(request.id, request.deadline);

            return Ok();
        }
    }



}

