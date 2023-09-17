using Entity;
using System;

namespace Infrastructure.Repositories;

public class TodoRepository : Repository<Todo>, ITodoRepository
{
    public TodoRepository(TodoContext context) : base(context)
    {
    }

    // Add any specific methods only relevant to Todo
}