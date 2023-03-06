using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using ProgrammersBlog.Core.Concrete.Dtos.UserDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Extantions;
using ProgrammersBlog.Mvc.Helpers.Abstract;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IImageHelper _imageHelper;
        private readonly IToastNotification _toastNotification;
        public UserController(UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager, IImageHelper imageHelper, IToastNotification toastNotification)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _imageHelper = imageHelper;
            _toastNotification = toastNotification;
        }


        [Authorize(Roles = "SuperAdmin,User.Read")]
        public async Task<IActionResult> Index()
        {

            List<User> users = await _userManager.Users.ToListAsync();

            UserListDto userListDto = new UserListDto { Users = users, ResultStatusType = ResultStatusType.Success };
            return View(userListDto);
        }

        [Authorize(Roles = "SuperAdmin,User.Create")]
        [HttpGet]
        public IActionResult Add()
        {
            return PartialView("UserAddPartial");
        }

        [Authorize(Roles = "SuperAdmin,User.Create")]
        [HttpPost]
        public async Task<IActionResult> Add(UserAddDto userAddDto)
        {
            var dataResult = await _imageHelper.Upload(userAddDto.UserName, userAddDto.PictureFile!, PictureType.User);
            userAddDto.Picture = dataResult.ResultStatus == ResultStatusType.Success ? dataResult.Data.FullName : "UserImages/default.User.jpg";


            if (ModelState.IsValid)
            {
                User user = _mapper.Map<User>(userAddDto);

                IdentityResult result = await _userManager.CreateAsync(user, userAddDto.Password);

                if (result.Succeeded)
                {
                    var userAddAjaxViewModel = new UserAddAjaxViewModal
                    {
                        UserDto = new UserDto
                        {
                            User = user,
                            ResultStatusType = ResultStatusType.Success,
                            Message = $"{user.UserName} adlı kullanıcı başarıyla eklenmiştir."
                        },
                        UserAddPartial = await this.RenderViewToStringAsync("UserAddPartial", userAddDto)
                    };

                    string jsonUserAddAjaxModel = JsonSerializer.Serialize(userAddAjaxViewModel);
                    return Json(jsonUserAddAjaxModel);
                }

                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    var userAddErrorAjaxViewModel = new UserAddAjaxViewModal
                    {
                        UserAddDto = userAddDto,
                        UserAddPartial = await this.RenderViewToStringAsync("UserAddPartial", userAddDto)
                    };

                    string jsonUserErrorAddAjaxModel = JsonSerializer.Serialize(userAddErrorAjaxViewModel);
                    return Json(jsonUserErrorAddAjaxModel);
                }
            }

            var userAddModalStateAjaxView = new UserAddAjaxViewModal
            {
                UserAddDto = userAddDto,
                UserAddPartial = await this.RenderViewToStringAsync("UserAddPartial", userAddDto)
            };

            string jsonUserModalStaeAddAjaxModel = JsonSerializer.Serialize(userAddModalStateAjaxView);
            return Json(jsonUserModalStaeAddAjaxModel);
        }


        [Authorize(Roles = "SuperAdmin,User.Delete")]
        [HttpPost]
        public async Task<JsonResult> Delete(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            IdentityResult result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                if (user.Picture!= "userImages/default.User.jpg")
                {
                    _imageHelper.DeleteImage(user.Picture);
                }
                var jsonDeleteUser = JsonSerializer.Serialize(new UserDto
                {
                    User = user,
                    ResultStatusType = ResultStatusType.Success,
                    Message = $"{user.UserName} adlı kullanıcı adına sahip kullanıcı başarıyla silinmiştir."
                });
                return Json(jsonDeleteUser);
            }

            string errorMessages = string.Empty;
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
                errorMessages += $"{error.Description}\n";
            }

            var jsonErrorDeleteUser = JsonSerializer.Serialize(new UserDto
            {
                User = user,
                ResultStatusType = ResultStatusType.Error,
                Message = $"{user.UserName} adına sahip kullanıcı eklenirken bazı hatalar oluştu.\n{errorMessages}"
            });
            return Json(jsonErrorDeleteUser);
        }

        [Authorize(Roles = "SuperAdmin,User.Read")]
        public async Task<JsonResult> GetAllUsers()
        {
            List<User> users = await _userManager.Users.ToListAsync();

            UserListDto userListDto = new UserListDto { Users = users, ResultStatusType = ResultStatusType.Success };

            var jsonUserListDto = JsonSerializer.Serialize(userListDto, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return Json(jsonUserListDto);
        }


        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpGet]
        public async Task<IActionResult> Update(int userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            UserUpdateDto userUpdateDto = _mapper.Map<UserUpdateDto>(user);
            return PartialView("UserUpdatePartial", userUpdateDto);
        }

    
        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateDto userUpdateDto)
        {
            if (ModelState.IsValid)
            {
                bool isNewPictureUploaded = false;
                var oldUser = await _userManager.FindByIdAsync(userUpdateDto.Id.ToString());
                var oldPicture = oldUser.Picture;

                if (userUpdateDto.PictureFile is not null)
                {
                    var dataResult = await _imageHelper.Upload(userUpdateDto.UserName, userUpdateDto.PictureFile!, PictureType.User);
                    userUpdateDto.Picture = dataResult.ResultStatus == ResultStatusType.Success ? dataResult.Data.FullName : "UserImages/default.User.jpg";

                    if (oldPicture != "UserImages/default.User.jpg")
                    {
                        isNewPictureUploaded = true;
                    }
                }

                User user = _mapper.Map<UserUpdateDto, User>(userUpdateDto, oldUser);

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (isNewPictureUploaded)
                    {
                        _imageHelper.DeleteImage(oldPicture);
                    }


                    var userUpdateViewModal = new UserUpdateAjaxViewModal
                    {
                        UserDto = new UserDto
                        {
                            User = user,
                            ResultStatusType = ResultStatusType.Success,
                            Message = $"{user.UserName} adlı kullanıcı başarıyla güncellenmiştir.."
                        },
                        UserUpdatePartial = await this.RenderViewToStringAsync("UserUpdatePartial", userUpdateDto)
                    };

                    var jsonModel = JsonSerializer.Serialize(userUpdateViewModal);

                    return Json(jsonModel);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);

                }

                var userUpdateErrorViewModel = JsonSerializer.Serialize(new UserUpdateAjaxViewModal
                {
                    UserUpdateDto = userUpdateDto,
                    UserUpdatePartial = await this.RenderViewToStringAsync("UserUpdatePartial", userUpdateDto)
                });
                return Json(userUpdateErrorViewModel);
            }
            var userUpdateModalStateModel = JsonSerializer.Serialize(new UserUpdateAjaxViewModal
            {
                UserUpdateDto = userUpdateDto,
                UserUpdatePartial = await this.RenderViewToStringAsync("UserUpdatePartial", userUpdateDto)
            });
            return Json(userUpdateModalStateModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<ViewResult> ChangeDetails()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            UserUpdateDto userUpdateDto = _mapper.Map<UserUpdateDto>(user);
            return View(userUpdateDto);
        }

         [Authorize]
        [HttpPost]
        public async Task<ViewResult> ChangeDetails(UserUpdateDto userUpdateDto)
        {
            if (ModelState.IsValid)
            {
                bool isNewPictureUploaded = false;
                var oldUser = await _userManager.GetUserAsync(HttpContext.User);
                var oldPicture = oldUser.Picture;

                if (userUpdateDto.PictureFile is not null)
                {
                    var dataResult = await _imageHelper.Upload(userUpdateDto.UserName, userUpdateDto.PictureFile!, PictureType.User);
                    userUpdateDto.Picture = dataResult.ResultStatus == ResultStatusType.Success ? dataResult.Data.FullName : "UserImages/default.User.jpg";

                    if (oldPicture != "UserImages/default.User.jpg")
                    {
                        isNewPictureUploaded = true;
                    }
                }

                User user = _mapper.Map<UserUpdateDto, User>(userUpdateDto, oldUser);

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    if (isNewPictureUploaded)
                    {
                        _imageHelper.DeleteImage(oldPicture);
                    }

                    _toastNotification.AddSuccessToastMessage($"{user.UserName} adlı kullanıcı başarıyla eklenmiştir.");
                    return View(userUpdateDto);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);

                }

                return View(userUpdateDto);
            }

            return View(userUpdateDto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PasswordChange()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PasswordChange(UserPasswordChangeDto userPasswordChangeDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var verifiedPassword = await _userManager.CheckPasswordAsync(user, userPasswordChangeDto.CurrentPassword);

                if (verifiedPassword)
                {
                    IdentityResult result = await _userManager.ChangePasswordAsync(user, userPasswordChangeDto.CurrentPassword, userPasswordChangeDto.NewPassword);

                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);

                        await _signInManager.SignOutAsync();
                        await _signInManager.PasswordSignInAsync(user, userPasswordChangeDto.NewPassword, true, false);

                        _toastNotification.AddSuccessToastMessage("Şifreniz başarıyla  değiştirilmiştir.");

                        return RedirectToAction("Index", "Home");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(userPasswordChangeDto);
                }
                ModelState.AddModelError(" ", "Lütfen, şu anki şifrenizi kontrol ediniz.");
                return View(userPasswordChangeDto);
            }
            return View(userPasswordChangeDto);
        }



        [Authorize(Roles = "SuperAdmin,User.Read")]
        [HttpGet]
        public async Task<PartialViewResult> GetDetail(int userId)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            return PartialView("_GetDetailPartial", new UserDto { User = user });
        }

    }
}
