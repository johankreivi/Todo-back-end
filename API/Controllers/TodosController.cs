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

    }
}
