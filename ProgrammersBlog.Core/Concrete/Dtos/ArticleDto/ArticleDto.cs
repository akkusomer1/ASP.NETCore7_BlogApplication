using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Concrete.Dtos.ArticleDto
{
    public class ArticleDto:BaseDto
    {
        public Article Article { get; set; }
    }
}
