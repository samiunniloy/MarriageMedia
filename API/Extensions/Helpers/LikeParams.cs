namespace API.Extensions.Helpers
{
    public class LikeParams: PaginationParams
    {

        public int UserId { get; set; }
        public string Predicate { get; set; }   
    }
}
