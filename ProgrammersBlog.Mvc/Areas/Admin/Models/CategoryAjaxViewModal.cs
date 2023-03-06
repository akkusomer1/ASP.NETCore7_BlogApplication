using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    public class CategoryAjaxViewModal
    {
        public CategoryAddDto? CategoryAddDto { get; set; }
        public string CategoryAddPartial { get; set; }
        public CategoryDto CategoryDto { get; set; }
    }
}
