﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Core.Concrete.Entities
{
    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string Message { get; set; }
    }
}
