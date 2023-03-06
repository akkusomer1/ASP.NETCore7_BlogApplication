using AutoMapper;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Abstract.İnterfaces.UnitOfWork;
using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Abstract;
using ProgrammersBlog.Core.Result.Concrete;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProgrammersBlog.Services.Services
{
    public class CategoryService : ServiceManager, ICategoryService
    {
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public async Task<IDataResult<CategoryDto>> Add(CategoryAddDto addDto, string createdByName)
        {
            Category addCategory = _mapper.Map<Category>(addDto);
            addCategory.CreatedByName = createdByName;
            addCategory.UpdateByName = createdByName;

            var addedCategory = await _unitOfWork.GetRepository<Category>().AddAsync(addCategory);
            await _unitOfWork.CommitAsync();
            return new DataResult<CategoryDto>(new CategoryDto
            {
                Category = addCategory,
                ResultStatusType = ResultStatusType.Success,
                Message = $"{addDto.Name} adlı kategori başarıyla eklenmiştir."
            }, ResultStatusType.Success, $"{addDto.Name} adlı kategori başarıyla eklenmiştir.");
        }

        public async Task<IDataResult<int>> CountAsync()
        {
            var categoriesCount = await _unitOfWork.GetRepository<Category>().CountAsync();

            if (categoriesCount > -1)
            {
                return new DataResult<int>(categoriesCount, ResultStatusType.Success);
            }
            return new DataResult<int>(-1, ResultStatusType.Error, "Beklenmeyen bir hata ile karşılaşıldı.");
        }

        public async Task<IDataResult<int>> CountByNonDeletedAsync()
        {
            var categoriesCount = await _unitOfWork.GetRepository<Category>().CountAsync(x => !x.IsDeleted);

            if (categoriesCount > -1)
            {
                return new DataResult<int>(categoriesCount, ResultStatusType.Success);
            }
            return new DataResult<int>(-1, ResultStatusType.Error, "Beklenmeyen bir hata ile karşılaşıldı.");
        }

        public async Task<IDataResult<CategoryDto>> Delete(int id, string updateName)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetAsync(c => c.Id == id);
            if (category != null)
            {
                category.IsDeleted = true;
                category.IsActive = false;
                category.UpdateByName = updateName;
                category.UpdateDate = DateTime.Now;
                var deletedCategory = _unitOfWork.GetRepository<Category>().Update(category);
                await _unitOfWork.CommitAsync();

                _unitOfWork.GetRepository<Category>().Update(deletedCategory);
                await _unitOfWork.CommitAsync();
                return new DataResult<CategoryDto>(new CategoryDto
                {
                    Category = deletedCategory,
                    ResultStatusType = ResultStatusType.Success,
                    Message = $"{deletedCategory.Name} adlı kategori başarıyla silimiştir."
                }, ResultStatusType.Success, $"{deletedCategory.Name} adlı kategori başarıyla silimiştir.");
            }
            return new DataResult<CategoryDto>(new CategoryDto
            {
                Category = null,
                ResultStatusType = ResultStatusType.Success,
                Message = "Böyle bir kategori bulunamadı"
            }, ResultStatusType.Error, "Böyle bir kategori bulunamadı");

        }


        public async Task<IDataResult<CategoryDto>> Get(int id)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetAsync(x => x.Id == id, x => x.Articles);

            if (category != null)
            {
                return new DataResult<CategoryDto>(new CategoryDto()
                {
                    Category = category,
                    ResultStatusType = ResultStatusType.Success
                }, ResultStatusType.Success);
            }
            return new DataResult<CategoryDto>(new CategoryDto()
            {
                Category = null,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir kategori bulunamadı."
            }, ResultStatusType.Error, "Hiç bir kategori bulunamadı.");
        }

        //CateogryService
        public async Task<IDataResult<CategoryListDto>> GetAll()
        {
            var query = _unitOfWork.GetRepository<Category>().GetAssQueryable();
            IQueryable<Category> categoriesList = query.Include(x => x.Articles).ThenInclude(x => x.Comments);
            IList<Category> categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(null!, x => x.Articles);

            if (categories.Count > -1)
            {
                return new DataResult<CategoryListDto>(new CategoryListDto
                {
                    Categories = categories,
                    ResultStatusType = ResultStatusType.Success
                }, ResultStatusType.Success);
            }
            return new DataResult<CategoryListDto>(new CategoryListDto()
            {
                Categories = null,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir kategori bulunamadı."
            }, ResultStatusType.Error, "Hiç bir kategori bulunamadı.");
        }

        public async Task<IDataResult<CategoryListDto>> GetAllByDeleted()
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(x => x.IsDeleted, x => x.Articles);

            if (categories.Count > -1)
            {

                return new DataResult<CategoryListDto>(new CategoryListDto()
                {
                    Categories = categories,
                    ResultStatusType = ResultStatusType.Success
                }, ResultStatusType.Success);
            }
            return new DataResult<CategoryListDto>(new CategoryListDto()
            {
                Categories = null,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir kategori bulunamadı."
            }, ResultStatusType.Error, "Hiç bir kategori bulunamadı.");
        }

        public async Task<IDataResult<CategoryListDto>> GetAllByNonDeleted()
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync(x => !x.IsDeleted, x => x.Articles);

            if (categories.Count > -1)
            {

                return new DataResult<CategoryListDto>(new CategoryListDto()
                {
                    Categories = categories.ToList(),
                    ResultStatusType = ResultStatusType.Success
                }, ResultStatusType.Success);
            }
            return new DataResult<CategoryListDto>(new CategoryListDto()
            {
                Categories = null,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir kategori bulunamadı."
            }, ResultStatusType.Error, "Hiç bir kategori bulunamadı.");
        }

        public async Task<IDataResult<CategoryUpdateDto>> GetCategoryUpdateDto(int id)
        {
            bool result = await _unitOfWork.GetRepository<Category>().AnyAsync(x => x.Id == id);
            if (result)
            {
                var category = await _unitOfWork.GetRepository<Category>().GetAsync(x => x.Id == id);
                CategoryUpdateDto categoryUpdateDto = _mapper.Map<CategoryUpdateDto>(category);
                return new DataResult<CategoryUpdateDto>(categoryUpdateDto, ResultStatusType.Success);
            }
            return new DataResult<CategoryUpdateDto>(null!, ResultStatusType.Error, "Böyle bir kategori bulunamadı.");
        }

        public async Task<IResult> HardDelete(int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetAsync(c => c.Id == categoryId);
            if (category != null)
            {
                await _unitOfWork.GetRepository<Category>().DeleteAsync(category);
                await _unitOfWork.CommitAsync();
                return new Result(ResultStatusType.Success, $"{category.Name} adlı kategori başarıyla veritabanından silinmiştir.");
            }
            return new Result(ResultStatusType.Error, "Böyle bir kategori bulunamadı.");
        }

        public async Task<IDataResult<CategoryDto>> UndoDelete(int id, string updateName)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetAsync(c => c.Id == id);
            if (category != null)
            {
                category.IsDeleted = false;
                category.IsActive = true;
                category.UpdateByName = updateName;
                category.UpdateDate = DateTime.Now;
                var deletedCategory = _unitOfWork.GetRepository<Category>().Update(category);
                await _unitOfWork.CommitAsync();

                _unitOfWork.GetRepository<Category>().Update(deletedCategory);
                await _unitOfWork.CommitAsync();
                return new DataResult<CategoryDto>(new CategoryDto
                {
                    Category = deletedCategory,
                    ResultStatusType = ResultStatusType.Success,
                    Message = $"{deletedCategory.Name} adlı kategori arşivden  başarıyla geri getirilmiştir."
                }, ResultStatusType.Success, $"{deletedCategory.Name} adlı kategori arşivden  başarıyla geri getirilmiştir.");
            }
            return new DataResult<CategoryDto>(new CategoryDto
            {
                Category = null,
                ResultStatusType = ResultStatusType.Success,
                Message = "Böyle bir kategori bulunamadı"
            }, ResultStatusType.Error, "Böyle bir kategori bulunamadı");
        }

        public async Task<IDataResult<CategoryDto>> Update(CategoryUpdateDto updateDto, string updatedByName)
        {
            Category oldCategory = await _unitOfWork.GetRepository<Category>().GetAsync(x => x.Id == updateDto.Id);

            var category = _mapper.Map<CategoryUpdateDto, Category>(updateDto, oldCategory);
            category.UpdateByName = updatedByName;

            Category updateCategory = _unitOfWork.GetRepository<Category>().Update(category);
            await _unitOfWork.CommitAsync();


            return new DataResult<CategoryDto>(new CategoryDto
            {
                Category = updateCategory,
                ResultStatusType = ResultStatusType.Success,
                Message = $"{updateDto.Name} adlı kategori başarıyla güncellenmiştir."
            }, ResultStatusType.Success, $"{updateDto.Name} adlı kategori başarıyla güncellenmiştir.");
        }
    }
}
