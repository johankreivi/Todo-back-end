using Entity;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {

        // Create, interface refrence, constructor and a async Post Method to Create Todo using generic repository pattern and IRepository interface
        private readonly IRepository<Todo> _repository;

        public TodosController(IRepository<Todo> repository)
        {
            _repository = repository;
        }

        // Create a async Post Method to Create Todo using generic repository pattern and IRepository interface
        [HttpPost]
        public async Task<IActionResult> Create(Todo todo)
        {
            await _repository.AddAsync(todo);
            return Ok(todo);
        }
    }
}
