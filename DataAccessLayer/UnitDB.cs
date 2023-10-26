﻿using DataAccessLayer.BaseInterfaces;
using DataTransferObject.Domain.Master;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class UnitDB : GenericRepositoryDL<MUnit>, IUnitDB
    {
        protected readonly ApplicationDbContext _context;
        public UnitDB(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        private readonly IConfiguration configuration;

        public async Task<bool> GetByName(MUnit Data)
        {
            var ret = _context.MUnit.Any(p => p.Unit_desc.ToUpper() == Data.Unit_desc.ToUpper());
            return ret;
        }

        public async Task<MUnit> GetBySusNo(string Sus_no)
        {
            return _context.MUnit.Where(P => P.Sus_no + P.Suffix == Sus_no).SingleOrDefault();
        }
    }
 }
