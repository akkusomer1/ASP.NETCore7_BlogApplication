using AutoMapper;
using ProgrammersBlog.Core.Abstract.Interfaces.Services;
using ProgrammersBlog.Core.Abstract.İnterfaces.UnitOfWork;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Abstract;
using ProgrammersBlog.Core.Result.Concrete;
using ProgrammersBlog.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProgrammersBlog.Services.Services
{
    public class CommentService : ServiceManager, ICommentService
    {
        public CommentService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {

        }

        public async Task<IDataResult<CommentDto>> GetAsync(int commentId)
        {
            var comment = await _unitOfWork.GetRepository<Comment>().GetAsync(c => c.Id == commentId, x => x.Article);
            if (comment != null)
            {
                return new DataResult<CommentDto>(new CommentDto
                {
                    Comment = comment
                }, ResultStatusType.Success);
            }
            return new DataResult<CommentDto>(new CommentDto
            {
                Comment = null,
            }, ResultStatusType.Error, "Böyle bir yorum bulunamadı.");
        }

        public async Task<IDataResult<CommentUpdateDto>> GetCommentUpdateDtoAsync(int commentId)
        {
            var result = await _unitOfWork.GetRepository<Comment>().AnyAsync(c => c.Id == commentId);
            if (result)
            {
                var comment = await _unitOfWork.GetRepository<Comment>().GetAsync(c => c.Id == commentId);
                var commentUpdateDto = _mapper.Map<CommentUpdateDto>(comment);
                return new DataResult<CommentUpdateDto>(commentUpdateDto, ResultStatusType.Success);
            }
            else
            {
                return new DataResult<CommentUpdateDto>(null, ResultStatusType.Error, "Böyle bir yorum bulunamadı.");
            }
        }

        public async Task<IDataResult<CommentListDto>> GetAllAsync()
        {
            var comments = await _unitOfWork.GetRepository<Comment>().GetAllAsync(null, x => x.Article);
            if (comments.Count > -1)
            {
                return new DataResult<CommentListDto>(new CommentListDto
                {
                    Comments = comments,
                }, ResultStatusType.Success);
            }
            return new DataResult<CommentListDto>(new CommentListDto
            {
                Comments = null,
            }, ResultStatusType.Error, "Hiç bir yorum bulunamadı.");
        }

        public async Task<IDataResult<CommentListDto>> GetAllByDeletedAsync()
        {
            var comments = await _unitOfWork.GetRepository<Comment>().GetAllAsync(c => c.IsDeleted, x => x.Article);
            if (comments.Count > -1)
            {
                return new DataResult<CommentListDto>(new CommentListDto
                {
                    Comments = comments,
                }, ResultStatusType.Success);
            }
            return new DataResult<CommentListDto>(new CommentListDto
            {
                Comments = null,
            }, ResultStatusType.Error, "Hiç bir yorum bulunamadı.");
        }

        public async Task<IDataResult<CommentListDto>> GetAllByNonDeletedAsync()
        {
            var comments = await _unitOfWork.GetRepository<Comment>().GetAllAsync(c => !c.IsDeleted, x => x.Article);
            if (comments.Count > -1)
            {
                return new DataResult<CommentListDto>(new CommentListDto
                {
                    Comments = comments,
                }, ResultStatusType.Success);
            }
            return new DataResult<CommentListDto>(new CommentListDto { Comments = null }, ResultStatusType.Error, "Hiç bir yorum bulunamadı.");
        }

        public async Task<IDataResult<CommentListDto>> GetAllByNonDeletedAndActiveAsync()
        {
            var comments = await _unitOfWork.GetRepository<Comment>().GetAllAsync(c => !c.IsDeleted && c.IsActive, x => x.Article);
            if (comments.Count > -1)
            {
                return new DataResult<CommentListDto>(new CommentListDto
                {
                    Comments = comments,
                }, ResultStatusType.Success);
            }
            return new DataResult<CommentListDto>(new CommentListDto { Comments = null }, ResultStatusType.Error, "Hiç bir yorum bulunamadı.");
        }

        public async Task<IDataResult<CommentDto>> AddAsync(CommentAddDto commentAddDto)
        {
            var article = await _unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == commentAddDto.ArticleId);
            if (article == null)
            {
                return new DataResult<CommentDto>(null, ResultStatusType.Error, "Böyle bir makale bulunamadı.");
            }
            var comment = _mapper.Map<Comment>(commentAddDto);
            var addedComment = await _unitOfWork.GetRepository<Comment>().AddAsync(comment);

            article.CommentCount += 1; 
            _unitOfWork.GetRepository<Article>().Update(article);
            await _unitOfWork.CommitAsync();
            return new DataResult<CommentDto>(new CommentDto
            {
                Comment = addedComment,
            }, ResultStatusType.Success, $"Sayın {commentAddDto.CreatedByName}, yorumunuz başarıyla eklenmiştir.");
        }
       

        public async Task<IDataResult<CommentDto>> UpdateAsync(CommentUpdateDto commentUpdateDto, string modifiedByName)
        {
            var oldComment = await _unitOfWork.GetRepository<Comment>().GetAsync(c => c.Id == commentUpdateDto.Id);
            var comment = _mapper.Map<CommentUpdateDto, Comment>(commentUpdateDto, oldComment);
            comment.UpdateByName = modifiedByName;
            var updatedComment = _unitOfWork.GetRepository<Comment>().Update(comment);
            updatedComment.Article = await _unitOfWork.GetRepository<Article>().GetAsync(x => x.Id == updatedComment.ArticleId);
            await _unitOfWork.CommitAsync();
            return new DataResult<CommentDto>(new CommentDto
            {
                Comment = updatedComment,
            }, ResultStatusType.Success, $"{comment.CreatedByName} tarafından eklenen yorum başarıyla güncellenmiştir.");
        }

 
        public async Task<IDataResult<CommentDto>> DeleteAsync(int commentId, string modifiedByName)
        {
            var comment = await _unitOfWork.GetRepository<Comment>().GetAsync(c => c.Id == commentId,x=>x.Article);
            if (comment != null)
            {
                var article = comment.Article;

                comment.IsDeleted = true;
                comment.IsActive = false;
                comment.UpdateByName = modifiedByName;
                comment.UpdateDate = DateTime.Now;

                article.CommentCount-=1;

                _unitOfWork.GetRepository<Article>().Update(article);
                var deletedComment = _unitOfWork.GetRepository<Comment>().Update(comment);
                await _unitOfWork.CommitAsync();
                return new DataResult<CommentDto>(new CommentDto
                {
                    Comment = deletedComment,
                }, ResultStatusType.Success, $"{deletedComment.CreatedByName} tarafından eklenen yorum başarıyla silinmiştir.");
            }
            return new DataResult<CommentDto>(new CommentDto
            {
                Comment = null,
            }, ResultStatusType.Error, "Böyle bir yorum bulunamadı.");
        }


        public async Task<IResult> HardDeleteAsync(int commentId)
        {
            var comment = await _unitOfWork.GetRepository<Comment>().GetAsync(c => c.Id == commentId,x=>x.Article);
            if (comment != null)
            {
                if (comment.IsDeleted)
                {
                    await _unitOfWork.GetRepository<Comment>().DeleteAsync(comment);
                    await _unitOfWork.CommitAsync();
                    return new Result(ResultStatusType.Success, $"{comment.CreatedByName} tarafından eklenen yorum başarıyla veritabanından silinmiştir.");
                }
                var article = comment.Article;
                await _unitOfWork.GetRepository<Comment>().DeleteAsync(comment);
                article.CommentCount -= 1;
                _unitOfWork.GetRepository<Article>().Update(article);
                await _unitOfWork.CommitAsync();
                return new Result(ResultStatusType.Success, $"{comment.CreatedByName} tarafından eklenen yorum başarıyla veritabanından silinmiştir.");
            }
            return new Result(ResultStatusType.Error, "Böyle bir yorum bulunamadı.");
        }

        public async Task<IDataResult<int>> CountAsync()
        {
            var commentsCount = await _unitOfWork.GetRepository<Comment>().CountAsync();
            if (commentsCount > -1)
            {
                return new DataResult<int>(commentsCount, ResultStatusType.Success);
            }
            else
            {
                return new DataResult<int>(-1, ResultStatusType.Error, $"Beklenmeyen bir hata ile karşılaşıldı.");
            }
        }

        public async Task<IDataResult<int>> CountByNonDeletedAsync()
        {
            var commentsCount = await _unitOfWork.GetRepository<Comment>().CountAsync(c => !c.IsDeleted);
            if (commentsCount > -1)
            {
                return new DataResult<int>(commentsCount, ResultStatusType.Success);
            }
            else
            {
                return new DataResult<int>(-1, ResultStatusType.Error, $"Beklenmeyen bir hata ile karşılaşıldı.");
            }
        }

        public async Task<IDataResult<CommentDto>> ApproveAsync(int commentId, string updateByName)
        {
            var comment = await _unitOfWork.GetRepository<Comment>().GetAsync(x => x.Id == commentId, x => x.Article);
            if (comment != null)
            {
                comment.IsActive = true;
                comment.UpdateByName = updateByName;
                comment.UpdateDate = DateTime.Now;
                var updatedComment = _unitOfWork.GetRepository<Comment>().Update(comment);
                await _unitOfWork.CommitAsync();
                return new DataResult<CommentDto>(new CommentDto { Comment = updatedComment }, ResultStatusType.Success, $"{comment.Id} no'lu yorum başarıyla onaylanmıştır.");
            }
            return new DataResult<CommentDto>(ResultStatusType.Error, "Böyle bir yorum bulunamadı.");
        }

        public async Task<IDataResult<CommentDto>> UndoDeleteAsync(int commentId, string modifiedByName)
        {
            var comment = await _unitOfWork.GetRepository<Comment>().GetAsync(c => c.Id == commentId, x => x.Article);
            if (comment != null)
            {
                var article = comment.Article;
                comment.IsDeleted = false;
                comment.IsActive = true;
                comment.UpdateByName = modifiedByName;
                comment.UpdateDate = DateTime.Now;
                var deletedComment = _unitOfWork.GetRepository<Comment>().Update(comment);
                
                article.CommentCount += 1;
                _unitOfWork.GetRepository<Article>().Update(article);
               
                await _unitOfWork.CommitAsync();
                return new DataResult<CommentDto>(new CommentDto
                {
                    Comment = deletedComment,
                }, ResultStatusType.Success, $"{deletedComment.CreatedByName} tarafından eklenen yorum  arşivden  başarıyla geri getirilmiştir.");
            }
            return new DataResult<CommentDto>(new CommentDto
            {
                Comment = null,
            }, ResultStatusType.Error, "Böyle bir yorum bulunamadı.");
        }
    }

}
