using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Core.Abstract.İnterfaces.Repository;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Data.Context;

namespace ProgrammersBlog.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ProgrammersBlogContext context) : base(context)
        {
        }
    }
}
