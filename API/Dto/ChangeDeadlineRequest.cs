namespace Api.Dto
{
    public class ChangeDeadlineRequest
    {
        public int id { get; set; }
        public DateTime? deadline { get; set; }
    }
}
