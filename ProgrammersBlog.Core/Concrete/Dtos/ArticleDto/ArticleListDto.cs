using ProgrammersBlog.Core.Concrete.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Concrete.Dtos.ArticleDto
{
    public class ArticleListDto : BaseDto
    {
        public IList<Article> Articles { get; set; }
        public int? CategoryId { get; set; }
    }  
}
 