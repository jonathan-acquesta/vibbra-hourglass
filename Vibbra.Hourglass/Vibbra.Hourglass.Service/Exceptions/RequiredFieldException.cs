﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vibbra.Hourglass.Service.Exceptions
{
    public class RequiredFieldException : Exception
    {
        public RequiredFieldException(string message) : base(message) { }
    }
}
