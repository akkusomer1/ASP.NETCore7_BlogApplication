using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Core.Abstract.İnterfaces.Repository;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Data.Context;

namespace ProgrammersBlog.Data.Repositories
{
    public class ArticleRepository : GenericRepository<Article>, IArticleRepository
    {
        public ArticleRepository(ProgrammersBlogContext context) : base(context)
        {
        }
    }
}
