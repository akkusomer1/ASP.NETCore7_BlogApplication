using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Core.Abstract.İnterfaces.Repository;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Data.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(ProgrammersBlogContext context) : base(context)
        {
        }
    }
}
