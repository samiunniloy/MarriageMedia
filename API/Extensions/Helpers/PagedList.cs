using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PagedList<T>:List<T>
    {
        public PagedList(IEnumerable<T>items,int count,int pageNumber,int pageSize) {
              
            CurrentPage= pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);

        }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public PagedListDto<T> ToDto()
        {
            return new PagedListDto<T>
            {
                Items = this.ToList(),
                CurrentPage = this.CurrentPage,
                TotalPages = this.TotalPages,
                PageSize = this.PageSize,
                TotalCount = this.TotalCount
            };
        }
    }
    public class PagedListDto<T>
    {
        public List<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedListDto()
        {
            Items = new List<T>();
        }

        // Add conversion method to create PagedList
        public PagedList<T> ToPagedList()
        {
            return new PagedList<T>(Items, TotalCount, CurrentPage, PageSize);
        }
    }
}
