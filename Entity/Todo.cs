﻿namespace Entity
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Completed { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
