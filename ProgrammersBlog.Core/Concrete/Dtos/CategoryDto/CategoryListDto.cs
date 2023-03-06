using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Core.Concrete.Dtos.CategoryDto
{
    public class CategoryListDto:BaseDto
    {
        public IList<Category> Categories { get; set; }
    
    }
}
