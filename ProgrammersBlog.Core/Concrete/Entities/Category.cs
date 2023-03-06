using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgrammersBlog.Core.Abstract.Interfaces.IEntity;

namespace ProgrammersBlog.Core.Concrete.Entities
{
    public class Category:BaseEntity,IEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Article>?
            Articles { get; set; }      
    } 
}
