using DataAccessLayer.BaseInterfaces;
using DataAccessLayer.Logger;
using DataTransferObject.Domain.Model;
using DataTransferObject.Requests;
using DataTransferObject.Response;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class FaultyCardDB : GenericRepositoryDL<TrnFaultyCard>, IFaultyCardDB
    {
        protected new readonly ApplicationDbContext _context;
        private readonly DapperContext _contextDP;
        private readonly IDataProtector protector;
        private readonly ILogger<FaultyCardDB> _logger;
        public FaultyCardDB(ApplicationDbContext context, DapperContext contextDP, IDataProtectionProvider dataProtectionProvider, ILogger<FaultyCardDB> logger, DataProtectionPurposeStrings dataProtectionPurposeStrings) : base(context)
        {
            _context = context;
            _contextDP = contextDP;
            _logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.AFSACIdRouteValue);
        }
        public async Task<DTOFaultyCardSaveResponse> SaveFaultyCard(DTOFaultyCardRequest dTO)
        {
            DTOFaultyCardSaveResponse saveResponse = new DTOFaultyCardSaveResponse();
            if (dTO.TrnFaultyCardId > 0)
            {
                TrnFaultyCard? trnFaultyCard = await _context.TrnFaultyCard.FindAsync(dTO.TrnFaultyCardId);
                if (trnFaultyCard != null)
                {
                    trnFaultyCard.ToRemark = dTO.ToRemark;
                    saveResponse.Result = true;
                    saveResponse.Message = "Data Saved";
                }
                else
                {
                    saveResponse.Result = true;
                    saveResponse.Message = "Invalid Id";
                }
                return saveResponse;
            }
            else
            {
                TrnFaultyCard trnFaultyCard = new TrnFaultyCard();
                trnFaultyCard.TrnFaultyCardId = 0;
                trnFaultyCard.RemarksIds = dTO.RemarksIds;
                trnFaultyCard.FromRemark =dTO.FromRemark;
                trnFaultyCard.ToRemark = dTO.ToRemark ?? null;
                trnFaultyCard.CategoryId = dTO.CategoryId;
                trnFaultyCard.RequestId = dTO.RequestId;
                trnFaultyCard.IsActive = dTO.IsActive;
                trnFaultyCard.Updatedby = dTO.Updatedby;
                trnFaultyCard.UpdatedOn = dTO.UpdatedOn;
                await _context.TrnFaultyCard.AddAsync(trnFaultyCard);
                saveResponse.Result = true;
                saveResponse.Message = "Data Updated";
                return saveResponse;
            }
        }
    }
}
