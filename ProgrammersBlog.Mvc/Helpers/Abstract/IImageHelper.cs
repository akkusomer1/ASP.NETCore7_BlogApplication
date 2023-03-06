using ProgrammersBlog.Core.Concrete.Dtos.HelpersDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Abstract;

namespace ProgrammersBlog.Mvc.Helpers.Abstract
{
    public interface IImageHelper
    {
        Task<IDataResult<UploadedImageDto>> Upload(string name,IFormFile pictureFile,PictureType type, string folderName = null);
        IDataResult<DeleteImageDto> DeleteImage(string pictureName);
    }
}
