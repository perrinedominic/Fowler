﻿using FowlerSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FowlerSite.Services
{
    public interface IListService
    {
        IEnumerable<Login> GetLoginList();
    }
}
