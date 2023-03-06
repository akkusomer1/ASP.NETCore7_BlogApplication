using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ProgrammersBlog.Core.Abstract.Interfaces.IEntity;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Abstract.İnterfaces.UnitOfWork;
using ProgrammersBlog.Core.Concrete.Dtos.ArticleDto;
using ProgrammersBlog.Core.Concrete.Dtos.CategoryDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Abstract;
using ProgrammersBlog.Core.Result.Concrete;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProgrammersBlog.Services.Services
{
    public class ArticleService : ServiceManager, IArticleService
    {
        private readonly UserManager<User> _userManager;
        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager) : base(unitOfWork, mapper)
        {
            _userManager = userManager;
        }

        public async Task<IResult> Add(ArticleCreateDto addDto, string createdByName, int userId)
        {
            var article = _mapper.Map<Article>(addDto);
            article.CreatedByName = createdByName;
            article.UpdateByName = createdByName;
            article.UserId = userId;
            await _unitOfWork.GetRepository<Article>().AddAsync(article);
            await _unitOfWork.CommitAsync();

            return new Result(ResultStatusType.Success, $"{article.Title} başlıklı makale başarıyla eklenmiştir.");
        }

        public async Task<IDataResult<ArticleUpdateDto>> ArticleUpdateDtoAsync(int articleId)
        {
            bool result = await _unitOfWork.GetRepository<Article>().AnyAsync(x => x.Id == articleId);
            if (result)
            {
                var article = await _unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == articleId);
                ArticleUpdateDto articleUpdateDto = _mapper.Map<ArticleUpdateDto>(article);
                return new DataResult<ArticleUpdateDto>(articleUpdateDto, ResultStatusType.Success);
            }
            return new DataResult<ArticleUpdateDto>(null!, ResultStatusType.Error, "Böyle bir kategori bulunamadı.");
        }

        public async Task<IDataResult<int>> CountAsync()
        {
            var articleCount = await _unitOfWork.GetRepository<Article>().CountAsync();

            if (articleCount > -1)
            {
                return new DataResult<int>(articleCount, ResultStatusType.Success);
            }
            return new DataResult<int>(-1, ResultStatusType.Error, "Beklenmeyen bir hata ile karşılaşıldı.");
        }

        public async Task<IDataResult<int>> CountByNonDeletedAsync()
        {
            var articleCount = await _unitOfWork.GetRepository<Article>().CountAsync(x => !x.IsDeleted);

            if (articleCount > -1)
            {
                return new DataResult<int>(articleCount, ResultStatusType.Success);
            }
            return new DataResult<int>(-1, ResultStatusType.Error, "Beklenmeyen bir hata ile karşılaşıldı.");
        }

        public async Task<IResult> Delete(int id, string updateName)
        {
            bool AnyArticle = await _unitOfWork.GetRepository<Article>().AnyAsync(x => x.Id == id);

            if (AnyArticle)
            {
                Article article = await _unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == id);

                article.IsDeleted = true;
                article.IsActive = false;
                article.UpdateByName = updateName;
                article.UpdateDate = DateTime.Now;

                _unitOfWork.GetRepository<Article>().Update(article);
                await _unitOfWork.CommitAsync();
                return new Result(ResultStatusType.Success, $"{article.Title} başlıklı makale başarıyla silinmiştir.");
            }
            return new Result(ResultStatusType.Error, "Böyle bir makale bulunamadı.");
        }


        public async Task<IDataResult<ArticleDto>> Get(int articleId)
        {
            var article = await _unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == articleId, x => x.User, x => x.Category);

            if (article is not null)
            {
                article.Comments = await _unitOfWork.GetRepository<Comment>().GetAllAsync(a => a.ArticleId == articleId && !a.IsDeleted && a.IsActive);
                return new DataResult<ArticleDto>(new ArticleDto()
                {
                    Article = article,
                    ResultStatusType = ResultStatusType.Success
                }, ResultStatusType.Success);
            }
            return new DataResult<ArticleDto>(new ArticleDto()
            {
                Article = null!,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir makale bulunamadı."
            }, ResultStatusType.Error, "Hiç bir makale bulunamadı.");
        }

        public async Task<IDataResult<ArticleListDto>> GetAll()
        {
            var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(null, x => x.User, x => x.Category);

            if (articles.Count > -1)
            {

                return new DataResult<ArticleListDto>(new ArticleListDto()
                {
                    Articles = articles,
                    ResultStatusType = ResultStatusType.Success
                }, ResultStatusType.Success);
            }

            return new DataResult<ArticleListDto>(new ArticleListDto()
            {
                Articles = null!,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir makale bulunamadı."
            }, ResultStatusType.Error, "Hiç bir makele bulunamadı.");
        }


        public async Task<IDataResult<ArticleListDto>> GetAllByCategory(int categoryId)
        {
            var AnyCategory = await _unitOfWork.GetRepository<Article>().AnyAsync(x => x.Id == categoryId);

            if (AnyCategory)
            {
                var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(x => x.Id == categoryId && !x.IsDeleted && x.IsActive, x => x.User, x => x.Category);
                if (articles.Count > -1)
                {
                    return new DataResult<ArticleListDto>(new ArticleListDto()
                    {
                        Articles = articles,
                        ResultStatusType = ResultStatusType.Success
                    }, ResultStatusType.Success);
                }

                return new DataResult<ArticleListDto>(new ArticleListDto()
                {
                    Articles = null!,
                    ResultStatusType = ResultStatusType.Error,
                    Message = "Hiç bir makale bulunamadı."
                }, ResultStatusType.Error, "Hiç bir makale bulunamadı.");


            }
            return new DataResult<ArticleListDto>(new ArticleListDto()
            {
                Articles = null!,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir kategori bulunamadı."
            }, ResultStatusType.Error, "Hiç bir kategori bulunamadı.");



        }

        public async Task<IDataResult<ArticleListDto>> GetAllByDeletedAsync()
        {
            var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(x => x.IsDeleted, x => x.User, x => x.Category);

            if (articles.Count > -1)
            {
                return new DataResult<ArticleListDto>(new ArticleListDto()
                {
                    Articles = articles,
                    ResultStatusType = ResultStatusType.Success
                }, ResultStatusType.Success);
            }

            return new DataResult<ArticleListDto>(new ArticleListDto()
            {
                Articles = null!,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir makale bulunamadı."
            }, ResultStatusType.Error, "Hiç bir makale bulunamadı.");
        }

        public async Task<IDataResult<ArticleListDto>> GetAllByNonDeleted()
        {
            var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(x => !x.IsDeleted, x => x.User, x => x.Category);

            if (articles.Count > -1)
            {
                return new DataResult<ArticleListDto>(new ArticleListDto()
                {
                    Articles = articles,
                    ResultStatusType = ResultStatusType.Success
                }, ResultStatusType.Success);
            }

            return new DataResult<ArticleListDto>(new ArticleListDto()
            {
                Articles = null!,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir makale bulunamadı."
            }, ResultStatusType.Error, "Hiç bir makale bulunamadı.");

        }

        public async Task<IDataResult<ArticleListDto>> GetAllByNonDeletedByActive()
        {
            var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(x => !x.IsDeleted && x.IsActive, x => x.User, x => x.Category);

            if (articles.Count > -1)
            {
                return new DataResult<ArticleListDto>(new ArticleListDto()
                {
                    Articles = articles,
                    ResultStatusType = ResultStatusType.Success
                }, ResultStatusType.Success);
            }

            return new DataResult<ArticleListDto>(new ArticleListDto()
            {
                Articles = null!,
                ResultStatusType = ResultStatusType.Error,
                Message = "Hiç bir makale bulunamadı."
            }, ResultStatusType.Error, "Hiç bir makale bulunamadı.");

        }


        public async Task<IDataResult<ArticleListDto>> GetAllByPagingAsync(int? categoryId, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;
            var articles = categoryId == null
                ? await _unitOfWork.GetRepository<Article>().GetAllAsync(x => x.IsActive && !x.IsDeleted, a => a.Category, a => a.User)
                : await _unitOfWork.GetRepository<Article>().GetAllAsync(x => x.CategoryId == categoryId && x.IsActive && !x.IsDeleted, a => a.Category, a => a.User);

            var sortedArticles = isAscending
                ? articles.OrderBy(x => x.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : articles.OrderByDescending(x => x.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();


            return new DataResult<ArticleListDto>(new ArticleListDto
            {
                Articles = sortedArticles,
                CategoryId = categoryId == null ? null : categoryId.Value,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = articles.Count,
                IsAscending = isAscending
            }, ResultStatusType.Success);
        }


       
        public async Task<IDataResult<ArticleDto>> GetByIdAsync(int articleId, bool includeCategory, bool includeComments, bool includeUser)
        {
            List<Expression<Func<Article, bool>>> predicates = new List<Expression<Func<Article, bool>>>();

            List<Expression<Func<Article, object>>> includes = new List<Expression<Func<Article, object>>>();

            if (includeCategory) includes.Add(x => x.Category);
            if (includeComments) includes.Add(x => x.Comments);
            if (includeUser) includes.Add(x => x.User);

            predicates.Add(x => x.Id == articleId);

            var article = await _unitOfWork.GetRepository<Article>().GetAsyncV2(predicates, includes);

            if (article == null)
            {
                return new DataResult<ArticleDto>(null, ResultStatusType.Warning, $"{articleId} numaraya sahip makale bulunamadı",
                    new List<ValidationError>
                    {
                        new ValidationError
                        {
                            PropertyName = "articleId", Message = $"{articleId} numaraya sahip makale bulunamadı"
                        }
                    });
            }
            return new DataResult<ArticleDto>(new ArticleDto { Article = article }, ResultStatusType.Success);
        }

        public async Task<IDataResult<ArticleListDto>> GetAllAsyncV2(int? categoryId, int? userId, bool? isActive, bool? isDeleted, int currentPage, int pageSize,
            OrderByGeneral orderBy, bool isAscending, bool includeCategory, bool includeComments, bool includeUser)
        {
          
            List<Expression<Func<Article, bool>>> predicates = new List<Expression<Func<Article, bool>>>();
            List<Expression<Func<Article, object>>> includes = new List<Expression<Func<Article, object>>>();

            if (categoryId.HasValue)
            {
                if (!await _unitOfWork.GetRepository<Article>().AnyAsync(x => x.CategoryId == categoryId.Value))
                {
                    return new DataResult<ArticleListDto>(null, ResultStatusType.Warning, $"{categoryId.Value} numaraya sahip makale bulunamadı",
                        new List<ValidationError>
                        {
                            new ValidationError
                            {
                                PropertyName = "articleId", Message = $"{categoryId.Value} kategori koduna sahip makale bulunamadı"
                            }
                        });
                }
                predicates.Add(x => x.Id == categoryId.Value);
            }


            if (userId.HasValue)
            {
                if (!await _userManager.Users.AnyAsync(x => x.Id == userId.Value))
                {
                    return new DataResult<ArticleListDto>(null, ResultStatusType.Warning, $"{userId.Value} numaraya sahip makale bulunamadı",
                        new List<ValidationError>
                        {
                            new ValidationError
                            {
                                PropertyName = "userId", Message = $"{userId.Value}  koduna sahip kullanıcı bulunamadı"
                            }
                        });
                }
                predicates.Add(x => x.Id == userId.Value);
            }

            if (isActive.HasValue) predicates.Add(x => x.IsActive);
            if (isActive.HasValue) predicates.Add(x => x.IsDeleted);

            if (includeCategory) includes.Add(x => x.Category);
            if (includeComments) includes.Add(x => x.Comments);
            if (includeUser) includes.Add(x => x.User);

            var articles = await _unitOfWork.GetRepository<Article>().GetAllAsyncV2(predicates, includes);

            IOrderedEnumerable<Article> sortedArticles;

            switch (orderBy)
            {
                case OrderByGeneral.Id:
                    sortedArticles = isAscending ? articles.OrderBy(x => x.Id) : articles.OrderByDescending(x => x.Id);
                    break;

                case OrderByGeneral.Az:
                    sortedArticles = isAscending ? articles.OrderBy(x => x.Title) : articles.OrderByDescending(x => x.Title);
                    break;

                //CreatedDate
                default:
                    sortedArticles = isAscending ? articles.OrderBy(x => x.CreatedDate) : articles.OrderByDescending(x => x.CreatedDate);
                    break;
            }

            return new DataResult<ArticleListDto>(new ArticleListDto
            {
                Articles = sortedArticles.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList(),
                CategoryId = categoryId.HasValue ? categoryId.Value :null,
                CurrentPage = currentPage,
                PageSize = pageSize,
                IsAscending = isAscending,
                TotalCount = articles.Count
            },ResultStatusType.Success);
        }


        public async Task<IDataResult<ArticleListDto>> GetAllByUserIdOnFilter(int userId, FilterBy filterBy, OrderBy orderBy, bool isAscending, int takeSize, int categoryId, DateTime startAt, DateTime endAt, int minViewCount, int maxViewCount, int minCommentCount, int maxCommentCount)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new DataResult<ArticleListDto>(ResultStatusType.Error, $"{user.UserName} kullanıcı bulunamadı.");
            }
            var userArticles = await _unitOfWork.GetRepository<Article>().GetAllAsync(x => x.UserId == userId && !x.IsDeleted & x.IsActive);

            List<Article> sortedArticle = new List<Article>();
            switch (filterBy)
            {
                case FilterBy.Cateogory:

                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticle = isAscending ? userArticles.Where(x => x.CategoryId == categoryId).OrderBy(x => x.Date).Take(takeSize).ToList()
                            : userArticles.Where(x => x.CategoryId == categoryId).OrderByDescending(x => x.Date).Take(takeSize).ToList();
                            break;

                        case OrderBy.ViewCount:
                            sortedArticle = isAscending ? userArticles.Where(x => x.CategoryId == categoryId).OrderBy(x => x.ViewsCount).Take(takeSize).ToList()
                           : userArticles.Where(x => x.CategoryId == categoryId).OrderByDescending(x => x.ViewsCount).Take(takeSize).ToList();
                            break;

                        case OrderBy.CommentCount:
                            sortedArticle = isAscending ? userArticles.Where(x => x.CategoryId == categoryId).OrderBy(x => x.CommentCount).Take(takeSize).ToList()
                           : userArticles.Where(x => x.CategoryId == categoryId).OrderByDescending(x => x.CommentCount).Take(takeSize).ToList();
                            break;


                        default:
                            break;
                    }
                    break;

                case FilterBy.Date:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticle = isAscending ? userArticles.Where(x => x.Date >= startAt && x.Date <= endAt).OrderBy(x => x.Date).Take(takeSize).ToList()
                            : userArticles.Where(x => x.Date >= startAt && x.Date <= endAt).OrderByDescending(x => x.Date).Take(takeSize).ToList();
                            break;

                        case OrderBy.ViewCount:
                            sortedArticle = isAscending ? userArticles.Where(x => x.Date >= startAt && x.Date <= endAt).OrderBy(x => x.ViewsCount).Take(takeSize).ToList()
                           : userArticles.Where(x => x.Date >= startAt && x.Date <= endAt).OrderByDescending(x => x.ViewsCount).Take(takeSize).ToList();
                            break;

                        case OrderBy.CommentCount:
                            sortedArticle = isAscending ? userArticles.Where(x => x.Date >= startAt && x.Date <= endAt).OrderBy(x => x.CommentCount).Take(takeSize).ToList()
                           : userArticles.Where(x => x.Date >= startAt && x.Date <= endAt).OrderByDescending(x => x.CommentCount).Take(takeSize).ToList();
                            break;


                        default:
                            break;
                    }
                    break;


                case FilterBy.ViewCount:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticle = isAscending ? userArticles.Where(x => x.ViewsCount >= minViewCount && x.ViewsCount <= maxViewCount).OrderBy(x => x.Date).Take(takeSize).ToList()
                            : userArticles.Where(x => x.ViewsCount >= minViewCount && x.ViewsCount <= maxViewCount).OrderByDescending(x => x.Date).Take(takeSize).ToList();
                            break;

                        case OrderBy.ViewCount:
                            sortedArticle = isAscending ? userArticles.Where(x => x.ViewsCount >= minViewCount && x.ViewsCount <= maxViewCount).OrderBy(x => x.ViewsCount).Take(takeSize).ToList()
                           : userArticles.Where(x => x.ViewsCount >= minViewCount && x.ViewsCount <= maxViewCount).OrderByDescending(x => x.ViewsCount).Take(takeSize).ToList();
                            break;

                        case OrderBy.CommentCount:
                            sortedArticle = isAscending ? userArticles.Where(x => x.ViewsCount >= minViewCount && x.ViewsCount <= maxViewCount).OrderBy(x => x.CommentCount).Take(takeSize).ToList()
                           : userArticles.Where(x => x.ViewsCount >= minViewCount && x.ViewsCount <= maxViewCount).OrderByDescending(x => x.CommentCount).Take(takeSize).ToList();
                            break;


                        default:
                            break;
                    }
                    break;


                case FilterBy.CommentCount:
                    switch (orderBy)
                    {
                        case OrderBy.Date:
                            sortedArticle = isAscending ? userArticles.Where(x => x.CommentCount >= minCommentCount && x.CommentCount <= maxCommentCount).OrderBy(x => x.Date).Take(takeSize).ToList()
                            : userArticles.Where(x => x.CommentCount >= minCommentCount && x.CommentCount <= maxCommentCount).OrderByDescending(x => x.Date).Take(takeSize).ToList();
                            break;

                        case OrderBy.ViewCount:
                            sortedArticle = isAscending ? userArticles.Where(x => x.CommentCount >= minCommentCount && x.CommentCount <= maxCommentCount).OrderBy(x => x.ViewsCount).Take(takeSize).ToList()
                           : userArticles.Where(x => x.CommentCount >= minCommentCount && x.CommentCount <= maxCommentCount).OrderByDescending(x => x.ViewsCount).Take(takeSize).ToList();
                            break;

                        case OrderBy.CommentCount:
                            sortedArticle = isAscending ? userArticles.Where(x => x.CommentCount >= minCommentCount && x.CommentCount <= maxCommentCount).OrderBy(x => x.CommentCount).Take(takeSize).ToList()
                           : userArticles.Where(x => x.CommentCount >= minCommentCount && x.CommentCount <= maxCommentCount).OrderByDescending(x => x.CommentCount).Take(takeSize).ToList();
                            break;


                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }

            return new DataResult<ArticleListDto>(new ArticleListDto
            {
                Articles = sortedArticle
            }, ResultStatusType.Success);
        }


        public async Task<IDataResult<ArticleListDto>> GetAllViewCountAsync(bool isAscending, int? takeSize)
        {
            var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(x => x.IsActive && !x.IsDeleted, a => a.User, a => a.Category);

            var sortedArticles = isAscending
                ? articles.OrderBy(x => x.ViewsCount)
                : articles.OrderByDescending(x => x.ViewsCount);

            return new DataResult<ArticleListDto>(new ArticleListDto
            {
                Articles = takeSize == null ? sortedArticles.ToList() : sortedArticles.Take(takeSize!.Value).ToList(),

            }, ResultStatusType.Success);
        }

        public async Task<IResult> HardDelete(int id)
        {
            bool AnyArticle = await _unitOfWork.GetRepository<Article>().AnyAsync(x => x.Id == id);

            if (AnyArticle)
            {
                Article article = await _unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == id);

                await _unitOfWork.GetRepository<Article>().DeleteAsync(article);
                await _unitOfWork.CommitAsync();
                return new Result(ResultStatusType.Success, $"{article.Title} başlıklı makale başarıyla silinmiştir.");
            }
            return new Result(ResultStatusType.Error, "Böyle bir makale bulunamadı.");

        }

        public async Task<IResult> IncreaseViewCountAsync(int articleId)
        {
            var article = await _unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == articleId);
            if (article == null)
            {
                return new Result(ResultStatusType.Error, "Böyle bir makale bulunamadı.");
            }
            article.ViewsCount += 1;
            _unitOfWork.GetRepository<Article>().Update(article);
            await _unitOfWork.CommitAsync();
            return new Result(ResultStatusType.Success);
        }

        public async Task<IDataResult<ArticleListDto>> SearchAsync(string keyword, int currentPage = 1, int pageSize = 5, bool isAscending = false)
        {
            pageSize = pageSize > 20 ? 20 : pageSize;
            if (string.IsNullOrWhiteSpace(keyword))
            {
                var articles = await _unitOfWork.GetRepository<Article>().GetAllAsync(x => x.IsActive && !x.IsDeleted, a => a.Category, a => a.User);

                var sortedArticles = isAscending
                ? articles.OrderBy(x => x.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                : articles.OrderByDescending(x => x.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                return new DataResult<ArticleListDto>(new ArticleListDto
                {
                    Articles = sortedArticles,
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalCount = articles.Count,
                    IsAscending = isAscending
                }, ResultStatusType.Success);
            }

            var searchedArticles = await _unitOfWork.GetRepository<Article>().SearchAsync(new List<Expression<Func<Article, bool>>>
            {
                (a)=>a.Title.Contains(keyword),
                (a)=>a.Category.Name.Contains(keyword),
                (a)=>a.Content.Contains(keyword),
                (a)=>a.SeoTags.Contains(keyword),
                (a)=>a.User.UserName.Contains(keyword),
                (a) => a.SeoDescription.Contains(keyword),
                (a) => a.SeoAuthor.Contains(keyword),
            }, x => x.Category, x => x.User);

            var sortedSearchedArticles = isAscending
                     ? searchedArticles.OrderBy(x => x.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList()
                     : searchedArticles.OrderByDescending(x => x.Date).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new DataResult<ArticleListDto>(new ArticleListDto
            {
                Articles = sortedSearchedArticles,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = searchedArticles.Count,
                IsAscending = isAscending
            }, ResultStatusType.Success);
        }

        public async Task<IResult> UndoDeleteAsync(int id, string updateName)
        {
            bool AnyArticle = await _unitOfWork.GetRepository<Article>().AnyAsync(x => x.Id == id);

            if (AnyArticle)
            {
                Article article = await _unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == id);
                article.IsDeleted = false;
                article.IsActive = true;
                article.UpdateByName = updateName;
                article.UpdateDate = DateTime.Now;

                _unitOfWork.GetRepository<Article>().Update(article);
                await _unitOfWork.CommitAsync();
                return new Result(ResultStatusType.Success, $"{article.Title} başlıklı makale  arşivden  başarıyla geri getirilmiştir.");
            }
            return new Result(ResultStatusType.Error, "Böyle bir makale bulunamadı.");
        }
        public async Task<IResult> Update(ArticleUpdateDto updateDto, string updatedByName)
        {
            var oldArticle = await _unitOfWork.GetRepository<Article>().GetAsync(a => a.Id == updateDto.Id);
            var updateArticle = _mapper.Map<ArticleUpdateDto, Article>(updateDto, oldArticle);
            updateArticle.UpdateByName = updatedByName;
            _unitOfWork.GetRepository<Article>().Update(updateArticle);
            await _unitOfWork.CommitAsync();
            return new Result(ResultStatusType.Success, $"{updateArticle.Title} başlıklı makale başarıyla güncellenmiştir.");
        }
    }
}
