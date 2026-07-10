using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Response
{ 
    public class ICardHistoryResponse
    {
        public int? TrnFwdId { get; set; }
        public byte StepId { get; set; }
        public string? FromDomain { get; set; }
        public string? FromProfile { get; set; }
        public string? FromRank { get; set; }
        public string? FromArmyNo { get; set; }
        public DateTime? FromDate { get; set; }
        public string? ToDomain { get; set; }
        public string? ToProfile { get; set; }
        public string? ToRank { get; set; }
        public int ToAspNetUsersId { get; set; }
        public string? Status { get; set; }
        public string? UpdatedOn { get; set; }
        public string? Remark { get; set; }
        public int IsComplete { get; set; }
        public string? Remarks2 { get; set; }
        public string? Reason { get; set; }
        public string? Authority { get; set; }
        public string? UnitName { get; set; }
        public int RequestId { get; set; }
    }
    public class ICardHistoryPostingOutResponse
    {
        public int? TrnFwdId { get; set; }
        public string? Reason { get; set; }
        public string? Authority { get; set; }
        public string? UnitName { get; set; }
        public string? FromUnit { get; set; }
    }
    public class ICardHistoryFaultyCardResponse
    {
        public int? TrnFwdId { get; set; }
        public string? FaultyStage { get; set; }
        public int CategoryId { get; set; }
        public string? RemarksNameList { get; set; }
    }
    public class ICardApplCloseCardResponse
    {
        public int? RequestId { get; set; }
        public string? Authority { get; set; }
        public string? Remarks { get; set; }
        public string? Reasons { get; set; }
    }
    public class ICardHistoryResponseAll
    {
        public DTOBasicDetailForCompleteClosed BasicDetail { get; set; }
        public List<ICardHistoryResponse> ICardHistory { get; set; }
        public List<ICardHistoryPostingOutResponse> PostingOut { get; set; }
        public List<ICardHistoryFaultyCardResponse> FaultyCard { get; set; }
        public ICardApplCloseCardResponse CloseCard { get; set; }
    }
}
