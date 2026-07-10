using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Constants
{
    public enum ApplicationStepEnum:byte
    {
        [Display(Name = "Drafted/Saved <br/> Appl")]
        DraftedSavedApplication = 1,

        [Display(Name = "Pending Appl <br/> (Approver Level)")]
        PendingApplicationApproverLevel = 2,

        [Display(Name = "Pending Appl <br/> (Verifier Level)")]
        PendingApplicationVerifierLevel = 3,

        [Display(Name = "Appl Status <br/> at ADC")]
        ApplicationStatusAtADC = 4,

        [Display(Name = "Exported")]
        Exported = 5,

        [Display(Name = "I-CARD PRINT")]
        ICardPrint = 6,

        [Display(Name = "Appl Rejected <br/> (Approver Level)")]
        ApplicationRejectedApproverLevel = 7,

        [Display(Name = "Appl Rejected <br/> (Verifier Level)")]
        ApplicationRejectedVerifierLevel = 8,

        [Display(Name = "Appl Rejected <br/> (AFSAC LEVEL)")]
        ApplicationRejectedAFSACLevel = 9,

        [Display(Name = "Print Reject")]
        PrintReject = 10,

        [Display(Name = "Card Dispatch to Regiment / Officer Record Office")]
        CardDispatchToRegimentOrOfficerRecordOffice = 11,

        [Display(Name = "Card in Regiment / Officer Record Office")]
        CardInRegimentOrOfficerRecordOffice = 12,

        [Display(Name = "Card Dispatch to Unit")]
        CardDispatchToUnit = 13,

        [Display(Name = "Card in Unit")]
        CardInUnit = 14,

        [Display(Name = "I-Card Distributed")]
        ICardDistributed = 15
    }
}
