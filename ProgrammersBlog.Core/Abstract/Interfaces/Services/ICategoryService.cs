using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Abstract.Interfaces.Services
{
  
    public interface ICategoryService
    {
        Task<IDataResult<CategoryListDto>> GetAllByDeleted();
        Task<IDataResult<CategoryDto>> UndoDelete(int id, string updateName);
        Task<IDataResult<int>> CountByNonDeletedAsync();
        Task<IDataResult<int>> CountAsync();
        Task<IDataResult<CategoryDto>> Get(int id);
        Task<IDataResult<CategoryUpdateDto>> GetCategoryUpdateDto(int id);
        Task<IDataResult<CategoryListDto>> GetAll();
        Task<IDataResult<CategoryListDto>> GetAllByNonDeleted();
       
        Task<IDataResult<CategoryDto>> Add(CategoryAddDto addDto,string createdByName);
        Task<IDataResult<CategoryDto>> Update(CategoryUpdateDto updateDto,string updatedByName);
        Task<IDataResult<CategoryDto>> Delete(int id, string updateName);
      
        Task<IResult> HardDelete(int id);
    }
}
