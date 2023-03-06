using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Entities;

namespace ProgrammersBlog.Mvc.Areas.Admin.Models
{
    public class CategoryUpdateAjaxViewModal 
    {
        public CategoryUpdateDto CategoryUpdateDto { get; set; }
        public string CategoryUpdatePartial { get; set; }
        public CategoryDto CategoryDto { get; set; }
    }
}
