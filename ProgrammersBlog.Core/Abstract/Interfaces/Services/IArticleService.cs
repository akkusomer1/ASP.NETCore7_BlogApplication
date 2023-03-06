using ProgrammersBlog.Core.Abstract.İnterfaces.Repository;
using ProgrammersBlog.Core.Concrete.Dtos.ArticleDto;
using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Abstract.Interfaces.Services
{
    public interface IArticleService
    {
        Task<IDataResult<ArticleListDto>> GetAllAsyncV2(int? categoryId, int? userId, bool? isActive, bool? isDeleted,
            int currentPage, int pageSize,
            OrderByGeneral orderBy, bool isAscending, bool includeCategory, bool includeComments, bool includeUser);
        Task<IDataResult<ArticleDto>> GetByIdAsync(int articleId, bool includeCategory, bool includeComments,bool includeUser);
        Task<IDataResult<ArticleListDto>> GetAllByUserIdOnFilter(int userId, FilterBy filterBy, OrderBy orderBy,
                bool isAscending, int takeSize, int categoryId, DateTime startAt, DateTime endAt, int minViewCount,
                                                 int maxViewCount, int minCommentCount, int maxCommentCount);

        Task<IResult> IncreaseViewCountAsync(int articleId);
        Task<IDataResult<ArticleListDto>> SearchAsync(string keyword, int currentPage = 1, int PageSize = 5, bool isAscending = false);
        Task<IDataResult<ArticleListDto>> GetAllByPagingAsync(int? categoryId, int currentPage = 1, int PageSize = 5, bool isAscending = false);
        Task<IDataResult<ArticleListDto>> GetAllViewCountAsync(bool isAscending, int? takeSize);
        Task<IResult> UndoDeleteAsync(int id, string updateName);
        Task<IDataResult<ArticleListDto>> GetAllByDeletedAsync();
        Task<IResult> Add(ArticleCreateDto addDto, string createdByName, int userId);
        Task<IDataResult<ArticleUpdateDto>> ArticleUpdateDtoAsync(int articleId);
        Task<IDataResult<int>> CountByNonDeletedAsync();
        Task<IDataResult<int>> CountAsync();
        Task<IDataResult<ArticleDto>> Get(int articleId);
        Task<IDataResult<ArticleListDto>> GetAll();
        Task<IDataResult<ArticleListDto>> GetAllByNonDeleted();
        Task<IDataResult<ArticleListDto>> GetAllByNonDeletedByActive();
        Task<IDataResult<ArticleListDto>> GetAllByCategory(int categoryId);
        Task<IResult> Update(ArticleUpdateDto updateDto, string updatedByName);
        Task<IResult> Delete(int id, string updateName);

        Task<IResult> HardDelete(int id);
    }
}
