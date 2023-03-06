using AutoMapper;
using ProgrammersBlog.Core.Abstract.İnterfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Services.Services
{
    public class ServiceManager
    {
        protected IUnitOfWork _unitOfWork;
        protected IMapper _mapper;

        public ServiceManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
    }
}
