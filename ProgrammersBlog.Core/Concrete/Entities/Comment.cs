using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgrammersBlog.Core.Abstract.Interfaces.IEntity;

namespace ProgrammersBlog.Core.Concrete.Entities
{
    public class Comment : BaseEntity, IEntity
    {
        public string Text { get; set; }
        public int ArticleId { get; set; }
        public Article? Article { get; set; }
    }
}
