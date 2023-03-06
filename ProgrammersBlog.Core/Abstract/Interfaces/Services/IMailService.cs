using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProgrammersBlog.Core.Concrete.Dtos;
using ProgrammersBlog.Core.Result.Abstract;

namespace ProgrammersBlog.Core.Abstract.Interfaces.Services
{
    public interface IMailService
    {
        IResult Send(EmailSendDto sendDto);
        IResult SendContactEmail(EmailSendDto sendDto);
    }
}
