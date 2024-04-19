using DataTransferObject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IApplCloseDB : IGenericRepositoryDL<TrnApplClose>
    {

        public Task<bool> RequestIdExists(TrnApplClose DTo);
    }
}
