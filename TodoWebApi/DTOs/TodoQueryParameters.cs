namespace TodoWebApi.DTOs
{
    public class TodoQueryParameters
    {
        public string? Category {  get; set; }
        public string? SortOrder { get; set; } = "asc";
    }
}
