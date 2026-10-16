const Success = 200;
const BadRequest = 400;
const InternalServerError = 500;
const DataSave = 1;
const DataUpdate = 2;
const DataExists = 3;
const IncorrectData = 4;
const spnISIO = 1;
const spnISCO = 1;
const spnIntRO = 1;
const spnIntORO = 1;
const spnIntAfsacCell = 1;
const spnIntAfsaccellUnitId = 85;
const spnMI11UnitId = 85;
const spnHQ54UnitId = 85;

const ApplicantPostingOut = 50;
const ApplicantClose = 51;
const FaultyCardRequest = 52;
const HoltlistCardRequest = 53;
const LostCardRequest = 54;
const DistributeCardRequest = 55;
const DestructionCardRequest = 56;


const Command = 1;
const Corps = 2;
const Div = 3;
const Bde = 4;
const Formation = 5;
const Appt = 6;
const Rank = 7;
const RankJCo = 8;
const ArmyType = 9;
const ArmyCat = 10;
const FmnBranches = 11;
const PSO = 12;
const SubDte = 13;
const PostingReason = 14;
const RecordOffice = 15;
const IssuingAuthority = 16;
const Regimental = 17;
const BloodGroup = 18;
const FaultyStage = 19;
const DispatchMode = 20;
const AllRegimental = 21;
const ORO = 22;
const HostUrlDGISToken = 'https://dgisapp.army.mil:55102'; /*localhost*/
const HostUrl = 'https://localhost:7023';

const MReg_Self = 1;
const MReg_Offrs_SameUnit = 2;
const MReg_Offrs_OtherUnit = 3;
const MReg_GCs_LCs_Cadets = 4;
const MReg_JCOs_OR_SameUnit = 6;
const MReg_JCOs_OR_OtherUnit = 7;
const MReg_Rect = 9;
const MReg_AMC_ADC_Offrs = 10;

const MICardType_First_time = 1;
const MICardType_Damaged = 2;
const MICardType_Change_Rank = 3;
const MICardType_Change_ArmyNo = 4;
const MICardType_LostCard = 5;

const MApplyFor_Offrs = 1;
const MApplyFor_JCO_ORs = 2;