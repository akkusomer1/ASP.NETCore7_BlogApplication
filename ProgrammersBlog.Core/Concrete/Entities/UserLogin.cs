using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Concrete.Entities
{
    //ASP.NET Identity sistemi tarafından, kullanıcının hangi dış oturum açma sağlayıcılarına hesabını bağladığını belirlemek için kullanılır.
    public class UserLogin:IdentityUserLogin<int>
    {
    }
}
