namespace Infrastructure.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly TodoContext _context;

    public TodoRepository(TodoContext context)
    {
        _context = context;
    }

}