using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.BaseInterfaces
{
    public interface IFaultyCardDB : IGenericRepositoryDL<TrnFaultyCard>
    {
        public Task<DTOFaultyCardSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO);
    }
}
