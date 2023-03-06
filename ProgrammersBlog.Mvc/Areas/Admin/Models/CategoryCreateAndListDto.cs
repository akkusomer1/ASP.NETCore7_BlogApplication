using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    public class CategoryCreateAndListDto
    {
        public CategoryListDto CategoryListDto { get; set; }
        public CategoryAddDto CategoryAddDto { get; set; }
    }
}
