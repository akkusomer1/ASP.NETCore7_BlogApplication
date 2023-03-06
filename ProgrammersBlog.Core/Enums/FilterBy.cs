using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Enums
{
    public enum FilterBy
    {
        [Display(Name = "Kategori")]
        Cateogory = 0,

        [Display(Name = "Tarih")]
        Date = 1,

        [Display(Name = "Okunma Sayısı")]
        ViewCount = 2,

        [Display(Name = "Yorum Sayısı")]
        CommentCount = 3
    }
}
