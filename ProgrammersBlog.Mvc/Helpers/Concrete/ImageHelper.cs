using ProgrammersBlog.Core.Concrete.Dtos.HelpersDto;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Core.Result.Abstract;
using ProgrammersBlog.Core.Result.Concrete;
using ProgrammersBlog.Mvc.Extantions;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using System.Text.RegularExpressions;

namespace ProgrammersBlog.Mvc.Helpers.Concrete
{
    public class ImageHelper : IImageHelper
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _wwwroot;
        private const string imgFolder = "img";
        private const string userImagesFolder  = "userImages";
        private const string  postImageFolder  = "postImages";
        public ImageHelper(IWebHostEnvironment env)
        {
            _env = env;
            _wwwroot = env.WebRootPath;
        }

        public IDataResult<DeleteImageDto> DeleteImage(string pictureName)
        {
            string fileToDelete = Path.Combine(_wwwroot, imgFolder, pictureName);

            if (System.IO.File.Exists(fileToDelete))
            {

                FileInfo fileInfo = new FileInfo(fileToDelete);

                var imageDeletedDto = new DeleteImageDto
                {
                    Extantion = fileInfo.Extension,
                    FullName = pictureName,
                    Path = fileInfo.FullName,
                    Size = fileInfo.Length
                };

                System.IO.File.Delete(fileToDelete);
                return new DataResult<DeleteImageDto>(imageDeletedDto,ResultStatusType.Success);
            }
            return new DataResult<DeleteImageDto>(null, ResultStatusType.Error,"Böyle bir resim bulunamadı.");
        }
        public async Task<IDataResult<UploadedImageDto>> Upload(string name, IFormFile pictureFile,PictureType type,string folderName=null)
        {
            folderName ??= type == PictureType.User ? userImagesFolder : postImageFolder;
            if (!Directory.Exists($"{_wwwroot}/{imgFolder}/{folderName}"))
            {
                Directory.CreateDirectory($"{_wwwroot}/{imgFolder}/{folderName}");
            }

            if (pictureFile != null)
            {
                string oldFileName = Path.GetFileNameWithoutExtension(pictureFile.FileName);
                string fileExtantion = Path.GetExtension(pictureFile.FileName);

                Regex regex = new Regex("[*.,\"'&$#<=>@!?]");
                name=regex.Replace(name,string.Empty);

                DateTime dateTime = DateTime.Now;
                string newFileName = $"{name}_{dateTime.FullDateAndTimeStringWithUnderscore()}_{fileExtantion}";
                string path = Path.Combine(_wwwroot, "img", folderName, newFileName);
                await using (var stream = new FileStream(path, FileMode.Create))
                {
                    await pictureFile.CopyToAsync(stream);
                }
                string message = type == PictureType.User ? $"{name} adlı kullanıcının resmi başarıyla  yüklenmiştir."
                    : $"{name} adlı makalenin resmi başarıyla  yüklenmiştir.";

                return new DataResult<UploadedImageDto>(
                    
                    new UploadedImageDto
                    {
                        Extension = fileExtantion,
                        OldName = oldFileName,
                        FolderName = folderName,
                        FullName = $"{folderName}/{newFileName}",
                        Path = path,
                        Size = pictureFile.Length
                    } ,ResultStatusType.Success,message);
            }
            return new DataResult<UploadedImageDto>(ResultStatusType.Error, "Resim yüklenirken bir hata oluştu lütfen tekrar deneyiniz.");
        }
    }
}
