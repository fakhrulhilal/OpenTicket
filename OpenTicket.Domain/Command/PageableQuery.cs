namespace OpenTicket.Domain.Command
{
    public abstract class PageableQuery
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 20;
    }
}
