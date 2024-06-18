create table ActionDiaryAux
(
    Balance             decimal(18, 2),
    Username            nvarchar(max),
    ActionDate          smalldatetime,
    ActionCode          nvarchar(max),
    RentAccount         nvarchar(max),
    Id                  bigint identity
        primary key nonclustered,
    TimeStamp           datetimeoffset default getdate() not null,
    ActionComment       nvarchar(max),
    Action              nvarchar(max),
    TenancyAgreementRef nvarchar(max)
)
go

create table ActionDiaryHistory
(
    IsRead              bit,
    ActionComment       nvarchar(max),
    Balance             decimal(18, 2),
    Username            nvarchar(max),
    Action              nvarchar(max),
    ActionCode          nvarchar(max),
    RentAccount         nvarchar(max),
    TenancyAgreementRef nvarchar(max),
    Id                  bigint identity
        primary key nonclustered,
    TimeStamp           datetimeoffset default getdate() not null,
    ActionDate          smalldatetime
)
go

create table Adjustment
(
    Id              int identity
        primary key,
    PaymentRef      nvarchar(max),
    TransactionType char(3),
    Amount          decimal(18, 2),
    TransactionDate datetime,
    IsRead          bit,
    TimeStamp       datetimeoffset default getdate() not null
)
go

create table AdjustmentAux
(
    Id              int identity
        primary key,
    PaymentRef      nvarchar(max),
    TransactionType char(3),
    Amount          decimal(18, 2),
    TransactionDate datetime,
    TimeStamp       datetimeoffset default getdate() not null
)
go

create table AuditTrail
(
    Id              bigint identity,
    Username        nvarchar(max),
    ActionPerformed nvarchar(max),
    Timestamp       datetimeoffset default getdate() not null
)
go

create table BatchLog
(
    Id        bigint identity
        primary key nonclustered,
    Type      nvarchar(max),
    StartTime datetimeoffset default getdate() not null,
    EndTime   datetimeoffset,
    IsSuccess bit
)
go

create table BatchLogError
(
    Id         bigint identity,
    Type       nvarchar(max),
    Message    nvarchar(max),
    BatchLogId bigint                           not null
        references BatchLog,
    Timestamp  datetimeoffset default getdate() not null
)
go

create table BatchReport
(
    Id                     int identity
        constraint PK__BatchRep__3214EC078C927B14
            primary key,
    IsSuccess              bit,
    Link                   varchar(max),
    ReportDate             datetime,
    ReportStartDate        datetime,
    TransactionType        varchar(10),
    RentGroup              varchar(10),
    [Group]                varchar(10),
    EndTime                datetimeoffset,
    ReportName             varchar(255),
    ReportYear             int,
    StartTime              datetimeoffset
        constraint DF__BatchRepo__Start__2D9CB955 default getdate() not null,
    ReportEndDate          datetime,
    ReportStartWeekOrMonth int,
    ReportEndWeekOrMonth   int
)
go

create table CalculatedCurrentBalance
(
    TenancyAgreementRef char(11),
    RentAccount         char(20),
    RentGroup           char(3),
    OpeningBalance2021  decimal(18, 2),
    Balance2021Wk1Wk26  decimal(18, 2),
    Balance2021Wk27Wk52 decimal(18, 2),
    OpeningBalance2022  decimal(18, 2),
    Balance2022Wk1Wk52  decimal(18, 2),
    OpeningBalance2023  decimal(18, 2),
    Balance2023Wk1Wk52  decimal(18, 2),
    OpeningBalance2024  decimal(18, 2),
    Balance2024Wk1Wk52  decimal(18, 2),
    CurrentBalance      decimal(18, 2),
    PreviousWeekBalance decimal(18, 2),
    OpeningBalance2025  decimal(18, 2),
    Balance2025Wk1Wk52  decimal(18, 2)
)
go

create table CashSuspenseTransactionAux
(
    RentAccount           nvarchar(max)  not null,
    NewRentAccount        nvarchar(max)  not null,
    Amount                decimal(18, 2) not null,
    IdSuspenseTransaction bigint         not null,
    Id                    bigint identity
        primary key nonclustered,
    Date                  smalldatetime  not null
)
go

create table Charges
(
    RentGroup    char(3),
    Id           bigint identity
        primary key,
    PropertyRef  nvarchar(max),
    ChargePeriod nvarchar(max),
    ChargeType   char(3),
    Amount       decimal(18, 2),
    Active       bit,
    TimeStamp    datetimeoffset default getdate() not null,
    Year         int
)
go

create table ChargesAux
(
    RentGroup   nvarchar(max),
    Id          bigint identity
        primary key,
    PropertyRef nvarchar(max),
    DAT         decimal(18, 2),
    DBR         decimal(18, 2),
    DC4         decimal(18, 2),
    DC5         decimal(18, 2),
    DCB         decimal(18, 2),
    DCC         decimal(18, 2),
    DCE         decimal(18, 2),
    DCI         decimal(18, 2),
    DCO         decimal(18, 2),
    DCP         decimal(18, 2),
    DCT         decimal(18, 2),
    DGA         decimal(18, 2),
    DGM         decimal(18, 2),
    DGR         decimal(18, 2),
    DHA         decimal(18, 2),
    DHE         decimal(18, 2),
    DHM         decimal(18, 2),
    DIN         decimal(18, 2),
    DIT         decimal(18, 2),
    DKF         decimal(18, 2),
    DLL         decimal(18, 2),
    DLP         decimal(18, 2),
    DMC         decimal(18, 2),
    DMJ         decimal(18, 2),
    DMR         decimal(18, 2),
    DR5         decimal(18, 2),
    DRP         decimal(18, 2),
    DRR         decimal(18, 2),
    DSA         decimal(18, 2),
    DSB         decimal(18, 2),
    DSC         decimal(18, 2),
    DSJ         decimal(18, 2),
    DSO         decimal(18, 2),
    DSR         decimal(18, 2),
    DST         decimal(18, 2),
    DTA         decimal(18, 2),
    DTC         decimal(18, 2),
    DTL         decimal(18, 2),
    DTV         decimal(18, 2),
    DVA         decimal(18, 2),
    DWR         decimal(18, 2),
    DWS         decimal(18, 2),
    DWW         decimal(18, 2),
    RCI         decimal(18, 2),
    RPD         decimal(18, 2),
    RSJ         decimal(18, 2),
    RTM         decimal(18, 2),
    RWA         decimal(18, 2),
    WON         decimal(18, 2),
    TimeStamp   datetimeoffset default getdate() not null,
    Year        int
)
go

create table ChargesBatchYears
(
    Id             bigint identity
        primary key nonclustered,
    ProcessingDate date,
    Year           int,
    IsRead         bit
)
go

create table ChargesHistory
(
    Id                  bigint identity
        primary key nonclustered,
    TenancyAgreementRef nvarchar(max),
    PropertyRef         nvarchar(max),
    ChargePeriod        nvarchar(max),
    Date                smalldatetime,
    IsRead              bit,
    ChargesId           bigint
        references Charges,
    ChargeType          char(3),
    Amount              decimal(18, 2),
    TimeStamp           datetimeoffset default getdate() not null,
    FirstWeekAdjustment bit            default 0         not null,
    LastWeekAdjustment  bit            default 0         not null
)
go

create table ChargesHistoryAdjustments
(
    Id               int identity
        constraint ChargesHistoryAdjustments_pk
            primary key,
    StartDate        date          not null,
    EndDate          date,
    ChargeType       varchar(3)    not null,
    AdjustmentFactor decimal(5, 4) not null,
    Description      varchar(max)  not null,
    ExclusionSetRef  int
)
go

create table ChargesHistoryAdjustmentsExclusions
(
    Id              int identity
        constraint PK_ChargesHistoryAdjustmentsExclusions
            primary key,
    ExclusionSetRef int      not null,
    PropertyRef     char(12) not null
)
go

create table CurrentBalance
(
    TenancyAgreementRef   char(11),
    RentAccount           char(20),
    RentGroup             char(3),
    OpeningBalanceWk1     decimal(18, 2),
    Balance2021Wk26       decimal(18, 2),
    Balance2021Wk27Till52 decimal(18, 2),
    Balance2022Wk1Onwards decimal(18, 2),
    Balance2023Wk1Onwards decimal(18, 2),
    PreviousWeekBalance   decimal(18, 2),
    Balance2024Wk1Onwards decimal(18, 2)
)
go

create table DMTransactionEntity
(
    id                       uniqueidentifier,
    target_id                uniqueidentifier,
    target_type              varchar(max),
    period_no                decimal(3),
    financial_year           int,
    financial_month          int,
    transaction_source       varchar(max),
    transaction_type         varchar(max),
    transaction_date         smalldatetime,
    transaction_amount       decimal(9, 2),
    payment_reference        varchar(max),
    bank_account_number      varchar(max),
    is_suspense              bit,
    suspense_resolution_info nvarchar(max),
    paid_amount              decimal(9, 2),
    charged_amount           decimal(9, 2),
    housing_benefit_amount   decimal(9, 2),
    balance_amount           decimal(21, 2),
    person                   nvarchar(max),
    fund                     varchar(max)
)
go

create table DirectDebit
(
    RentAccount nvarchar(max)                    not null,
    Active      bit                              not null,
    DueDay      tinyint                          not null,
    TimeStamp   datetimeoffset default getdate() not null,
    Amount      decimal(18, 2)                   not null,
    Id          bigint identity
        primary key nonclustered
)
go

create table DirectDebitAux
(
    Id          bigint identity
        primary key nonclustered,
    RentAccount nvarchar(max)                    not null,
    Date        smalldatetime                    not null,
    Amount      decimal(18, 2)                   not null,
    TimeStamp   datetimeoffset default getdate() not null
)
go

create table DirectDebitHistory
(
    TimeStamp   datetimeoffset default getdate() not null,
    Date        datetime                         not null,
    Id          bigint identity
        primary key nonclustered,
    IsRead      bit,
    Amount      decimal(18, 2)                   not null,
    RentAccount nvarchar(max)                    not null
)
go

create table DirectDebitSuspenseAccounts
(
    Id                   bigint identity
        primary key nonclustered,
    DirectDebitHistoryId bigint                           not null
        references DirectDebitHistory,
    RentAccount          nvarchar(max)                    not null,
    NewRentAccount       nvarchar(max),
    Amount               decimal(18, 2)                   not null,
    Date                 datetime                         not null,
    IsResolved           bit,
    TimeStamp            datetimeoffset default getdate() not null
)
go

create table ErrorLog
(
    Id                bigint identity
        primary key,
    TableName         nvarchar(max),
    RowId             nvarchar(max),
    UserFriendlyError nvarchar(max),
    ApplicationError  nvarchar(max),
    Timestamp         datetimeoffset default getdate() not null
)
go

create table GoogleFileSetting
(
    Id               int identity
        primary key,
    GoogleIdentifier nvarchar(max)                    not null,
    FileType         nvarchar(max)                    not null,
    Label            nvarchar(max)                    not null,
    StartDate        datetimeoffset default getdate() not null,
    EndDate          datetimeoffset,
    FileYear         int
)
go

create table MABalanceTransactionDetails
(
    tag_ref             varchar(20),
    date                smalldatetime,
    transaction_details varchar(50),
    debit               decimal(18, 2),
    credit              decimal(18, 2),
    balance             decimal(18, 2),
    row_num             int
)
go

create table MAMember
(
    house_ref    char(10)
        constraint DF__MAMember__house___6E8B6712 default space(1) not null,
    person_no    numeric(2)
        constraint DF__MAMember__person__6F7F8B4B default 0        not null,
    gender       char,
    title        char(10)
        constraint DF__MAMember__title__7073AF84 default space(1),
    initials     char(3),
    forename     char(24)
        constraint DF__MAMember__forena__7167D3BD default space(1),
    surname      char(20)
        constraint DF__MAMember__surnam__725BF7F6 default space(1),
    age          numeric(3)
        constraint DF__MAMember__age__73501C2F default 0,
    relationship char,
    responsible  bit
        constraint DF__MAMember__respon__74444068 default 0        not null,
    member_sid   int                                               not null,
    dob          datetime
)
go

create table MAProperty
(
    prop_ref      char(12) not null,
    major_ref     char(12),
    man_scheme    char(11),
    post_code     char(10),
    short_address char(200),
    telephone     char(21),
    ownership     char(10) not null,
    agent         char(3),
    area_office   char(3),
    subtyp_code   char(3),
    letable       bit      not null,
    cat_type      char(3),
    house_ref     char(10),
    occ_stat      char(3),
    post_preamble char(60),
    property_sid  int,
    arr_patch     char(3),
    address1      char(255),
    num_bedrooms  int,
    post_desig    char(60)
)
go

create table MATenancyAgreement
(
    tag_ref         char(11) not null,
    prop_ref        char(12),
    house_ref       char(10),
    tag_desc        char(200),
    cot             smalldatetime,
    eot             smalldatetime,
    tenure          char(3),
    prd_code        char(2),
    present         bit      not null,
    terminated      bit      not null,
    rentgrp_ref     char(3),
    rent            numeric(9, 2),
    service         numeric(9, 2),
    other_charge    numeric(9, 2),
    tenancy_rent    numeric(9, 2),
    tenancy_service numeric(9, 2),
    tenancy_other   numeric(9, 2),
    cur_bal         numeric(9, 2),
    cur_nr_bal      numeric(9, 2),
    occ_status      char(3),
    tenagree_sid    int,
    u_saff_rentacc  char(20),
    high_action     char(3),
    u_notice_served smalldatetime,
    courtdate       smalldatetime,
    u_court_outcome char(3),
    evictdate       smalldatetime,
    agr_type        char
)
go

create table MonthsByYear
(
    YearNo    int,
    MonthNo   int,
    StartDate datetime
)
go

create table OperatingBalance
(
    rentgrp_desc  char(16),
    post_year     int,
    post_prdno    decimal(3),
    total_charged numeric(38, 2),
    total_paid    numeric(38, 2),
    total_hb_paid numeric(38, 2),
    D20           numeric(38, 2),
    D25           numeric(38, 2),
    DAT           numeric(38, 2),
    DBR           numeric(38, 2),
    DBT           numeric(38, 2),
    DC1           numeric(38, 2),
    DC2           numeric(38, 2),
    DC3           numeric(38, 2),
    DC4           numeric(38, 2),
    DC5           numeric(38, 2),
    DCB           numeric(38, 2),
    DCC           numeric(38, 2),
    DCE           numeric(38, 2),
    DCI           numeric(38, 2),
    DCO           numeric(38, 2),
    DCP           numeric(38, 2),
    DCT           numeric(38, 2),
    DGA           numeric(38, 2),
    DGM           numeric(38, 2),
    DGR           numeric(38, 2),
    DHA           numeric(38, 2),
    DHE           numeric(38, 2),
    DHM           numeric(38, 2),
    DHW           numeric(38, 2),
    DIN           numeric(38, 2),
    DIT           numeric(38, 2),
    DKF           numeric(38, 2),
    DLD           numeric(38, 2),
    DLK           numeric(38, 2),
    DLL           numeric(38, 2),
    DLP           numeric(38, 2),
    DMC           numeric(38, 2),
    DMF           numeric(38, 2),
    DMJ           numeric(38, 2),
    DML           numeric(38, 2),
    DMR           numeric(38, 2),
    DPP           numeric(38, 2),
    DPY           numeric(38, 2),
    DR1           numeric(38, 2),
    DR2           numeric(38, 2),
    DR3           numeric(38, 2),
    DR4           numeric(38, 2),
    DR5           numeric(38, 2),
    DRP           numeric(38, 2),
    DRR           numeric(38, 2),
    DSA           numeric(38, 2),
    DSB           numeric(38, 2),
    DSC           numeric(38, 2),
    DSJ           numeric(38, 2),
    DSO           numeric(38, 2),
    DSR           numeric(38, 2),
    DST           numeric(38, 2),
    DTA           numeric(38, 2),
    DTC           numeric(38, 2),
    DTL           numeric(38, 2),
    DTV           numeric(38, 2),
    DVA           numeric(38, 2),
    DWR           numeric(38, 2),
    DWS           numeric(38, 2),
    DWW           numeric(38, 2),
    RBA           numeric(38, 2),
    RBP           numeric(38, 2),
    RBR           numeric(38, 2),
    RCI           numeric(38, 2),
    RCO           numeric(38, 2),
    RCP           numeric(38, 2),
    RCT           numeric(38, 2),
    RDD           numeric(38, 2),
    RDN           numeric(38, 2),
    RDP           numeric(38, 2),
    RDR           numeric(38, 2),
    RDS           numeric(38, 2),
    RDT           numeric(38, 2),
    REF           numeric(38, 2),
    RHA           numeric(38, 2),
    RHB           numeric(38, 2),
    RIT           numeric(38, 2),
    RML           numeric(38, 2),
    ROB           numeric(38, 2),
    RPD           numeric(38, 2),
    RPO           numeric(38, 2),
    RPY           numeric(38, 2),
    RQP           numeric(38, 2),
    RRC           numeric(38, 2),
    RRP           numeric(38, 2),
    RSJ           numeric(38, 2),
    RSO           numeric(38, 2),
    RTM           numeric(38, 2),
    RUC           numeric(38, 2),
    RWA           numeric(38, 2),
    WOF           numeric(38, 2),
    WON           numeric(38, 2)
)
go

create table OperatingBalanceAccounts
(
    rent_account  varchar(20),
    rentgrp_desc  char(16),
    post_year     int,
    total_charged numeric(38, 2),
    total_paid    numeric(38, 2),
    total_hb_paid numeric(38, 2),
    D20           numeric(38, 2),
    D25           numeric(38, 2),
    DAT           numeric(38, 2),
    DBR           numeric(38, 2),
    DBT           numeric(38, 2),
    DC1           numeric(38, 2),
    DC2           numeric(38, 2),
    DC3           numeric(38, 2),
    DC4           numeric(38, 2),
    DC5           numeric(38, 2),
    DCB           numeric(38, 2),
    DCC           numeric(38, 2),
    DCE           numeric(38, 2),
    DCI           numeric(38, 2),
    DCO           numeric(38, 2),
    DCP           numeric(38, 2),
    DCT           numeric(38, 2),
    DGA           numeric(38, 2),
    DGM           numeric(38, 2),
    DGR           numeric(38, 2),
    DHA           numeric(38, 2),
    DHE           numeric(38, 2),
    DHM           numeric(38, 2),
    DHW           numeric(38, 2),
    DIN           numeric(38, 2),
    DIT           numeric(38, 2),
    DKF           numeric(38, 2),
    DLD           numeric(38, 2),
    DLK           numeric(38, 2),
    DLL           numeric(38, 2),
    DLP           numeric(38, 2),
    DMC           numeric(38, 2),
    DMF           numeric(38, 2),
    DMJ           numeric(38, 2),
    DML           numeric(38, 2),
    DMR           numeric(38, 2),
    DPP           numeric(38, 2),
    DPY           numeric(38, 2),
    DR1           numeric(38, 2),
    DR2           numeric(38, 2),
    DR3           numeric(38, 2),
    DR4           numeric(38, 2),
    DR5           numeric(38, 2),
    DRP           numeric(38, 2),
    DRR           numeric(38, 2),
    DSA           numeric(38, 2),
    DSB           numeric(38, 2),
    DSC           numeric(38, 2),
    DSJ           numeric(38, 2),
    DSO           numeric(38, 2),
    DSR           numeric(38, 2),
    DST           numeric(38, 2),
    DTA           numeric(38, 2),
    DTC           numeric(38, 2),
    DTL           numeric(38, 2),
    DTV           numeric(38, 2),
    DVA           numeric(38, 2),
    DWR           numeric(38, 2),
    DWS           numeric(38, 2),
    DWW           numeric(38, 2),
    RBA           numeric(38, 2),
    RBP           numeric(38, 2),
    RBR           numeric(38, 2),
    RCI           numeric(38, 2),
    RCO           numeric(38, 2),
    RCP           numeric(38, 2),
    RCT           numeric(38, 2),
    RDD           numeric(38, 2),
    RDN           numeric(38, 2),
    RDP           numeric(38, 2),
    RDR           numeric(38, 2),
    RDS           numeric(38, 2),
    RDT           numeric(38, 2),
    REF           numeric(38, 2),
    RHA           numeric(38, 2),
    RHB           numeric(38, 2),
    RIT           numeric(38, 2),
    RML           numeric(38, 2),
    ROB           numeric(38, 2),
    RPD           numeric(38, 2),
    RPO           numeric(38, 2),
    RPY           numeric(38, 2),
    RQP           numeric(38, 2),
    RRC           numeric(38, 2),
    RRP           numeric(38, 2),
    RSJ           numeric(38, 2),
    RSO           numeric(38, 2),
    RTM           numeric(38, 2),
    RUC           numeric(38, 2),
    RWA           numeric(38, 2),
    WOF           numeric(38, 2),
    WON           numeric(38, 2)
)
go

create table RentGroupSumr
(
    RentGroup    varchar(3),
    YearNo       int,
    PeriodNo     int,
    TotalCharged decimal(18, 2),
    TotalPaid    decimal(18, 2),
    TotalHB      decimal(18, 2)
)
go

create table SSCurrentRentPosition
(
    Id                         bigint identity
        primary key,
    PropertyRef                nvarchar(max),
    PaymentRef                 nvarchar(max),
    TenancyAgreementRef        nvarchar(max),
    Tenant                     nvarchar(max),
    TenancyStartDate           smalldatetime,
    TenancyType                nvarchar(max),
    DateOfBirth                smalldatetime,
    HomeTel                    nvarchar(max),
    Mobile                     nvarchar(max),
    AddressLine1               nvarchar(max),
    AddressLine2               nvarchar(max),
    AddressLine3               nvarchar(max),
    PostCode                   nvarchar(max),
    UniversalCredit            bit,
    HBClaimRef                 nvarchar(max),
    DirectDebitDate            tinyint,
    Week53Year20ClosingBalance decimal(18, 2),
    Week1Balance               decimal(18, 2),
    Week27Balance              decimal(18, 2),
    TotalRent                  decimal(18, 2),
    HBwc12Oct20                decimal(18, 2),
    SubsequentWeeklyHB         decimal(18, 2),
    NetRent                    decimal(18, 2),
    Week28Payment              decimal(18, 2),
    Week29Payment              decimal(18, 2),
    Week30Payment              decimal(18, 2),
    Week31Payment              decimal(18, 2),
    Week32Payment              decimal(18, 2),
    Week33Payment              decimal(18, 2),
    Week34Payment              decimal(18, 2),
    Week35Payment              decimal(18, 2),
    Week36Payment              decimal(18, 2),
    Week37Payment              decimal(18, 2),
    Week38Payment              decimal(18, 2),
    Week39Payment              decimal(18, 2),
    Week40Payment              decimal(18, 2),
    Week41Payment              decimal(18, 2),
    Week42Payment              decimal(18, 2),
    Week43Payment              decimal(18, 2),
    Week44Payment              decimal(18, 2),
    Week45Payment              decimal(18, 2),
    Week46Payment              decimal(18, 2),
    Week47Payment              decimal(18, 2),
    Week48Payment              decimal(18, 2),
    Week49Payment              decimal(18, 2),
    Week50Payment              decimal(18, 2),
    Week51Payment              decimal(18, 2),
    Week52Payment              decimal(18, 2),
    EstimatedBalance           decimal(18, 2),
    IncreaseArrearsSinceWeek27 decimal(18, 2),
    Timestamp                  datetimeoffset default getdate() not null
)
go

create table SSGarage
(
    Id                  bigint identity
        primary key,
    PropertyRef         nvarchar(max),
    PropertyType        nvarchar(max),
    AreaOffice          nvarchar(max),
    Patch               nvarchar(max),
    Fund                nvarchar(max),
    Status              nvarchar(max),
    VoidDate            smalldatetime,
    Name                nvarchar(max),
    TenancyDate         smalldatetime,
    Tenure              nvarchar(max),
    AddressLine1        nvarchar(max),
    AddressLine2        nvarchar(max),
    AddressLine3        nvarchar(max),
    PostCode            nvarchar(max),
    RentAccount         nvarchar(max),
    Comment             nvarchar(max),
    DdCase              nvarchar(max),
    TenancyAgreementRef nvarchar(max),
    Balance11Oct20      decimal(18, 2),
    Rent                decimal(18, 2),
    VAT                 decimal(18, 2),
    CurrentTotal        decimal(18, 2),
    NewRent             decimal(18, 2),
    NewVAT              decimal(18, 2),
    NewTotal            decimal(18, 2),
    Corr1               nvarchar(max),
    Corr2               nvarchar(max),
    Corr3               nvarchar(max),
    CorrPostCode        nvarchar(max),
    Timestamp           datetimeoffset default getdate() not null
)
go

create table SSLeaseholdAccount
(
    Id                  bigint identity
        primary key,
    TenancyAgreementRef nvarchar(max),
    PaymentRef          nvarchar(max),
    PropertyRef         nvarchar(max),
    RentGroup           nvarchar(max),
    Tenure              nvarchar(max),
    AssignmentStartDate smalldatetime,
    AssignmentEndDate   smalldatetime,
    SoldLeasedDate      smalldatetime,
    AccountType         nvarchar(max),
    AgreementType       nvarchar(max),
    Balance             decimal(18, 2),
    Lessee              nvarchar(max),
    Address             nvarchar(max),
    Timestamp           datetimeoffset default getdate() not null
)
go

create table SSMiniSumr
(
    tag_ref     char(11) not null,
    post_year   int,
    post_prdno  int,
    b_forward   decimal(10, 2),
    tot_deb     decimal(10, 2),
    tot_deb_adj decimal(10, 2),
    tot_rec     decimal(10, 2),
    tot_rec_adj decimal(10, 2)
)
go

create table SSMiniTransaction
(
    tag_ref     char(11),
    prop_ref    char(12),
    rentgroup   char(3),
    post_year   int,
    post_prdno  decimal(3),
    tenure      char(3),
    trans_type  char(3),
    trans_src   char(3),
    real_value  decimal(9, 2),
    post_date   smalldatetime,
    trans_ref   char(12),
    origin      varchar(max),
    origin_desc varchar(max)
)
go

create index ix_ss_transactions
    on SSMiniTransaction (tag_ref) include (prop_ref, rentgroup, post_year, post_prdno, tenure, trans_type, trans_src,
                                            real_value, post_date)
go

create table SSRentBreakdown
(
    Id                  bigint identity
        primary key,
    PropertyRef         nvarchar(max),
    TenancyAgreementRef nvarchar(max),
    AreaOffice          nvarchar(max),
    Patch               nvarchar(max),
    UPRN                nvarchar(max),
    PaymentRef          nvarchar(max),
    Comment             nvarchar(max),
    Comment2            nvarchar(max),
    OccupiedStatus      nvarchar(max),
    FormulaRent202021   decimal(18, 2),
    ActualRent202021    decimal(18, 2),
    Bedrooms            tinyint,
    Type                nvarchar(max),
    Tenure              nvarchar(max),
    Title               nvarchar(max),
    Forename            nvarchar(max),
    Surname             nvarchar(max),
    TenancyStartDate    smalldatetime,
    VoidDate            smalldatetime,
    AddressLine1        nvarchar(max),
    AddressLine2        nvarchar(max),
    AddressLine3        nvarchar(max),
    AddressLine4        nvarchar(max),
    PostCode            nvarchar(max),
    Formula             decimal(18, 2),
    TotalRent           decimal(18, 2),
    Actual              decimal(18, 2),
    WaterRates          decimal(18, 2),
    WaterStandingChrg   decimal(18, 2),
    WatersureReduction  decimal(18, 2),
    TenantsLevy         decimal(18, 2),
    CleaningBlock       decimal(18, 2),
    CleaningEstate      decimal(18, 2),
    LandlordLighting    decimal(18, 2),
    GroundsMaintenance  decimal(18, 2),
    CommunalDigitalTV   decimal(18, 2),
    Concierge           decimal(18, 2),
    Heating             decimal(18, 2),
    HeatingMaintenance  decimal(18, 2),
    TelevisionLicense   decimal(18, 2),
    ContentsInsurance   decimal(18, 2),
    TravellersCharge    decimal(18, 2),
    GarageAttached      decimal(18, 2),
    CarPort             decimal(18, 2),
    GarageVAT           decimal(18, 2),
    Timestamp           datetimeoffset default getdate() not null
)
go

create table SSServiceChargePaymentsReceived
(
    Id                             bigint identity,
    ArrearPatch                    nvarchar(max),
    TenancyAgreementRef            nvarchar(max),
    PaymentRef                     nvarchar(max),
    PropertyRef                    nvarchar(max),
    Tenancy                        nvarchar(max),
    Tenant                         nvarchar(max),
    Address                        nvarchar(max),
    DirectDebitDate                nvarchar(max),
    MonthlyDebit                   decimal(18, 2),
    Sep20DebitToIncludeActuals     decimal(18, 2),
    AdjustmentsToSCDebits          decimal(18, 2),
    DirectDebits15and23Nov20       decimal(18, 2),
    DirectDebitDec20               decimal(18, 2),
    Balance30Sep20                 decimal(18, 2),
    OctoberMovedToJudgement        decimal(18, 2),
    JanuaryMovedToJudgement        decimal(18, 2),
    FebruaryMovedToJudgement       decimal(18, 2),
    MarchMovedToJudgement          decimal(18, 2),
    OctoberSCandMWTransfers        decimal(18, 2),
    MarchSCandMWTransfers          decimal(18, 2),
    OctoberPayments                decimal(18, 2),
    NovemberPayments               decimal(18, 2),
    DecemberPayments               decimal(18, 2),
    JanuaryPayments                decimal(18, 2),
    FebruaryPayments               decimal(18, 2),
    MarchPayments                  decimal(18, 2),
    DisputedAmount                 decimal(18, 2),
    BalanceIncludingDisputedAmount decimal(18, 2),
    Timestamp                      datetimeoffset default getdate() not null
)
go

create table SuspenseTransactionAux
(
    RentAccount           nvarchar(max)  not null,
    Type                  varchar(50),
    NewRentAccount        nvarchar(max)  not null,
    Amount                decimal(18, 2) not null,
    IdSuspenseTransaction bigint         not null,
    Id                    bigint identity
        primary key nonclustered,
    Date                  smalldatetime  not null
)
go

create table TenancyAgreementAux
(
    Title        nvarchar(max),
    Address      nvarchar(max),
    DateOfBirth  smalldatetime,
    PaymentRef   nvarchar(max),
    PropertyRef  nvarchar(max),
    StartDate    smalldatetime,
    NumBedrooms  int,
    RentGroup    nvarchar(max),
    Id           bigint identity
        primary key,
    EndDate      smalldatetime,
    Forename     nvarchar(max),
    TimeStamp    datetimeoffset default getdate() not null,
    Surname      nvarchar(max),
    Tenure       nvarchar(max),
    PostCode     nvarchar(max),
    ShortAddress nvarchar(max)
)
go

create table TenancyAgreementHistory
(
    TimeStamp           datetimeoffset default getdate() not null,
    IsNewRow            bit,
    IsRead              bit,
    DateOfBirth         smalldatetime,
    Surname             nvarchar(24),
    Forename            nvarchar(20),
    Title               nvarchar(10),
    HouseholdRef        nvarchar(10),
    NumBedrooms         int,
    PostCode            nvarchar(10),
    Address             nvarchar(4000),
    ShortAddress        nvarchar(4000),
    PropertyRef         nvarchar(12),
    EndDate             smalldatetime,
    StartDate           smalldatetime,
    Tenure              char(3),
    RentGroup           char(3),
    PaymentRef          nvarchar(20),
    TenancyAgreementRef nvarchar(11),
    Id                  bigint identity
        primary key
)
go

create table Transactions
(
    trans_src     char(3),
    post_date     smalldatetime,
    origin        varchar(max),
    real_value    decimal(9, 2),
    trans_ref     char(12),
    trans_type    char(3),
    origin_source varchar(max),
    tag_ref       char(11),
    prop_ref      char(12),
    rentgroup     char(3),
    post_year     int,
    post_prdno    decimal(3),
    tenure        char(3)
)
go

create table UHAraction
(
    tag_ref             char(11)  not null,
    action_set          int       not null,
    action_no           int       not null,
    action_code         char(3),
    action_date         smalldatetime,
    action_balance      numeric(7, 2),
    action_comment      text,
    username            varchar(40),
    comm_only           bit       not null,
    ole_obj             image,
    araction_sid        int,
    action_deferred     bit,
    deferred_until      smalldatetime,
    deferral_reason     char(3),
    severity_level      int,
    action_nr_balance   numeric(10, 2),
    action_type         char(3),
    act_status          char(3),
    action_cat          char(3),
    action_subno        int,
    action_subcode      char(3),
    action_process_no   int,
    notice_sid          int,
    courtord_sid        int,
    warrant_sid         int,
    action_doc_no       int,
    tstamp              timestamp not null,
    comp_avail          char(200),
    comp_display        char(200),
    u_saff_araction_ref char(30)
)
go

create clustered index ix_araction_id
    on UHAraction (araction_sid)
go

create index ix_araction_trac
    on UHAraction (tag_ref, action_code) include (action_date)
go

create table UHArag
(
    arag_ref             char(15),
    tag_ref              char(11),
    arag_startbal        numeric(10, 2),
    arag_whichbal        char(3),
    arag_startdate       smalldatetime,
    arag_firstno         int,
    arag_firstunit       char(3),
    arag_subno           int,
    arag_subunit         char(10),
    arag_lastcheckbal    numeric(10, 2),
    arag_lastcheckdate   smalldatetime,
    arag_lastexpectedbal numeric(10, 2),
    arag_breached        bit       not null,
    arag_status          char(10),
    arag_cancelbal       numeric(10, 2),
    arag_statusdate      smalldatetime,
    arag_statususer      char(3),
    arag_amount          numeric(10, 2),
    arag_clearby         smalldatetime,
    arag_frequency       char(3),
    arag_comment         text,
    arag_nextcheck       smalldatetime,
    arag_tolerance       int,
    arag_sid             int,
    arag_pmandata        text,
    tstamp               timestamp null,
    comp_avail           char(200),
    comp_display         char(200),
    u_saffron_id         char(8),
    u_saff_rentacc       char(12),
    u_new_book           bit,
    u_pay_start          smalldatetime,
    u_no_payments        int,
    arag_fcadate         smalldatetime,
    arag_noprepay        bit
)
go

create table UHAragdet
(
    arag_sid          int            not null,
    aragdet_sid       int            not null,
    aragdet_amount    numeric(10, 2) not null,
    aragdet_frequency char(3)        not null,
    aragdet_startdate smalldatetime  not null,
    aragdet_enddate   smalldatetime  not null,
    aragdet_comment   text           not null,
    comp_avail        char(200),
    comp_display      char(200)
)
go

create table UHContacts
(
    con_key       int,
    con_ref       char(12),
    con_name      varchar(73),
    con_address   char(200),
    con_phone1    char(21),
    con_phone2    char(21),
    con_phone3    char(21),
    con_postcode  char(10),
    con_type      char,
    tag_ref       char(11),
    prop_ref      char(12),
    email_address char(50),
    app_ref       char(10),
    contacts_sid  int,
    intreason     char(3),
    vunreason     char(3),
    locreason     char(3),
    intcomment    text,
    vuncomment    text,
    loccomment    text,
    tstamp        binary(8),
    comp_avail    char(200),
    comp_display  char(200)
)
go

create table UHDdagacc
(
    ddagree_ref         char(20)       not null,
    tag_ref             char(12)       not null,
    current_debits      numeric(12, 2) not null,
    ent_value           numeric(12, 2) not null,
    arag_amount         numeric(12, 2) not null,
    other_dd            numeric(12, 2) not null,
    current_balance     numeric(12, 2) not null,
    total_due           numeric(12, 2) not null,
    fixed_total_due     numeric(12, 2) not null,
    include_balance     char(3)        not null,
    detail_schedule     bit            not null,
    due_per_period      numeric(12, 2) not null,
    smooth_rough        char(3)        not null,
    ddagacc_sid         int,
    tstamp              timestamp      null,
    comp_avail          char(200),
    comp_display        char(200),
    other_rec           numeric(12, 2) not null,
    reduction_cd        numeric(12, 2) not null,
    reduction_ev        numeric(12, 2) not null,
    reduction_od        numeric(12, 2) not null,
    reduction_cb        numeric(12, 2) not null,
    reduction_or        numeric(12, 2) not null,
    reduction_aa        numeric(12, 2) not null,
    fixed_total_percent numeric(12, 2),
    due_per_period_ta   numeric(12, 2) not null
)
go

create table UHDebType
(
    deb_code      char(3) not null,
    deb_desc      char(20),
    deb_cat       char,
    deb_link      char,
    deb_group     numeric(1),
    vatable       bit     not null,
    apportion     bit     not null,
    freeprd       bit     not null,
    deb_effective char,
    deb_vatrate   char,
    debtype_sid   int,
    void_charge   bit
)
go

create table UHHousehold
(
    house_ref      char(10) not null,
    post_code      char(10),
    telephone      char(21),
    house_size     numeric(2),
    house_kids     numeric(2),
    payment_method char(3),
    house_desc     char(73),
    prop_ref       char(12),
    email_address  varchar(50),
    househ_sid     int
)
go

create table UHMember
(
    house_ref    char(10)   not null,
    person_no    numeric(2) not null,
    gender       char,
    title        char(10),
    initials     char(3),
    forename     char(24),
    surname      char(20),
    age          numeric(3),
    relationship char,
    responsible  bit        not null,
    member_sid   int        not null,
    dob          datetime
)
go

create table UHMiniSumr
(
    tag_ref     char(11) not null,
    post_year   int,
    post_prdno  int,
    b_forward   decimal(10, 2),
    tot_deb     decimal(10, 2),
    tot_deb_adj decimal(10, 2),
    tot_rec     decimal(10, 2),
    tot_rec_adj decimal(10, 2)
)
go

create table UHMiniTransaction
(
    tag_ref    char(11),
    prop_ref   char(12),
    rentgroup  char(3),
    post_year  int,
    post_prdno decimal(3),
    tenure     char(3),
    trans_type char(3),
    trans_src  char(3),
    real_value decimal(9, 2),
    post_date  smalldatetime,
    trans_ref  char(12)
)
go

create index ix_uh_transactions
    on UHMiniTransaction (tag_ref, post_year) include (prop_ref, rentgroup, post_prdno, tenure, trans_type, trans_src,
                                                       real_value, post_date)
go

create index ix_uhminitransaction_period
    on UHMiniTransaction (post_year, tag_ref, rentgroup) include (post_prdno)
go

create index ix_uhminitransaction_yeardate
    on UHMiniTransaction (post_year, post_date) include (tag_ref, post_prdno, tenure, trans_type, trans_src, real_value)
go

create table UHParis
(
    PCRTransactionType varchar(3),
    UHTReceiptType     varchar(3),
    Description        varchar(50)
)
go

create table UHPostCode
(
    post_code    char(10) not null,
    address      varchar(200),
    aline1       char(50),
    aline2       char(50),
    aline3       char(50),
    aline4       char(50),
    postcode_sid int
)
go

create table UHProperty
(
    prop_ref      char(12) not null,
    major_ref     char(12),
    man_scheme    char(11),
    post_code     char(10),
    short_address char(200),
    telephone     char(21),
    ownership     char(10) not null,
    agent         char(3),
    area_office   char(3),
    subtyp_code   char(3),
    letable       bit      not null,
    cat_type      char(3),
    house_ref     char(10),
    occ_stat      char(3),
    post_preamble char(60),
    property_sid  int,
    arr_patch     char(3),
    address1      char(255),
    num_bedrooms  int,
    post_desig    char(60)
)
go

create table UHRecType
(
    rec_code    char(3) not null,
    rec_desc    char(20),
    rec_group   numeric(1),
    rectype_sid int,
    rec_dd      bit     not null
)
go

create table UHRent
(
    prop_ref      char(12) not null,
    rentgrp_ref   char(3),
    occ_status    char(3),
    occ_stat_date smalldatetime,
    prop_type     char(3),
    tenure        char(3),
    house_ref     char(10),
    short_address varchar(120),
    rent          numeric(9, 2),
    service       numeric(9, 2),
    other_charge  numeric(9, 2),
    rent_sid      int
)
go

create table UHRentGroup
(
    rentgrp_ref          char(3) not null,
    rentgrp_desc         char(16),
    prd_code             char(2),
    rg_period_no         int,
    rg_year              int,
    no_of_periods        int,
    rentgrp_sid          int,
    spreadsheet_tab_name nvarchar(40)
)
go

create table UHRentSource
(
    trans_src char(3),
    src_desc  char(30)
)
go

create table UHTenancyAgreement
(
    tag_ref         char(11) not null,
    prop_ref        char(12),
    house_ref       char(10),
    tag_desc        char(200),
    cot             smalldatetime,
    eot             smalldatetime,
    tenure          char(3),
    prd_code        char(2),
    present         bit      not null,
    terminated      bit      not null,
    rentgrp_ref     char(3),
    rent            numeric(9, 2),
    service         numeric(9, 2),
    other_charge    numeric(9, 2),
    tenancy_rent    numeric(9, 2),
    tenancy_service numeric(9, 2),
    tenancy_other   numeric(9, 2),
    cur_bal         numeric(9, 2),
    cur_nr_bal      numeric(9, 2),
    occ_status      char(3),
    tenagree_sid    int,
    u_saff_rentacc  char(20),
    high_action     char(3),
    u_notice_served smalldatetime,
    courtdate       smalldatetime,
    u_court_outcome char(3),
    evictdate       smalldatetime,
    agr_type        char,
    rech_tag_ref    char(11),
    master_tag_ref  char(11)
)
go

create unique clustered index ci_tag_ref
    on UHTenancyAgreement (tag_ref)
go

create table UHTenure
(
    ten_type   char(3) not null,
    ten_desc   char(15),
    ten_cat    char,
    tenure_sid int,
    leasehold  bit
)
go

create table UPCashDumpFileName
(
    Id        bigint identity
        primary key,
    FileName  nvarchar(max)                    not null,
    IsSuccess bit,
    Timestamp datetimeoffset default getdate() not null
)
go

create table UPCashDump
(
    Id                   bigint identity
        primary key,
    FullText             nvarchar(max)                    not null,
    IsRead               bit,
    UPCashDumpFileNameId bigint                           not null
        references UPCashDumpFileName,
    Timestamp            datetimeoffset default getdate() not null
)
go

create table UPCashLoad
(
    Id              bigint identity,
    RentAccount     nvarchar(max),
    PaymentSource   nvarchar(max),
    MethodOfPayment nvarchar(max),
    AmountPaid      decimal(18, 2),
    DatePaid        smalldatetime,
    CivicaCode      nvarchar(max),
    UPCashDumpId    bigint                           not null
        references UPCashDump,
    Timestamp       datetimeoffset default getdate() not null,
    IsRead          bit
)
go

create table UPCashLoadSuspenseAccounts
(
    IsResolved      bit,
    Timestamp       datetimeoffset default getdate() not null,
    CivicaCode      nvarchar(max),
    DatePaid        smalldatetime,
    AmountPaid      decimal(18, 2),
    MethodOfPayment nvarchar(max),
    PaymentSource   nvarchar(max),
    NewRentAccount  nvarchar(max),
    RentAccount     nvarchar(max),
    UPCashDumpId    bigint                           not null
        references UPCashDump,
    Id              int identity
)
go

create table UPHousingCashDumpFileName
(
    Id        bigint identity
        primary key,
    FileName  nvarchar(max)                    not null,
    IsSuccess bit,
    Timestamp datetimeoffset default getdate() not null
)
go

create table UPHousingCashDump
(
    Id                          bigint identity
        primary key,
    FullText                    nvarchar(max)                    not null,
    IsRead                      bit,
    UPHousingCashDumpFileNameId bigint                           not null
        references UPHousingCashDumpFileName,
    Timestamp                   datetimeoffset default getdate() not null
)
go

create table UPHousingCashLoad
(
    Id                  bigint identity,
    AcademyClaimRef     nvarchar(max),
    column2             nvarchar(max),
    RentAccount         nvarchar(max),
    Date                smalldatetime,
    value1              decimal(18, 2),
    value2              decimal(18, 2),
    value3              decimal(18, 2),
    value4              decimal(18, 2),
    value5              decimal(18, 2),
    UPHousingCashDumpId bigint                           not null
        references UPHousingCashDump,
    Timestamp           datetimeoffset default getdate() not null,
    IsRead              bit
)
go

create table UPHousingCashLoadSuspenseAccounts
(
    Id                  bigint identity,
    AcademyClaimRef     nvarchar(max),
    column2             nvarchar(max),
    RentAccount         nvarchar(max),
    NewRentAccount      nvarchar(max),
    Date                smalldatetime,
    value1              decimal(18, 2),
    value2              decimal(18, 2),
    value3              decimal(18, 2),
    value4              decimal(18, 2),
    value5              decimal(18, 2),
    UPHousingCashDumpId bigint                           not null
        references UPHousingCashDump,
    Timestamp           datetimeoffset default getdate() not null,
    IsResolved          bit
)
go

create table WeeksByYear
(
    YearNo    int,
    WeekNo    int,
    StartDate datetime
)
go

create table awsdms_truncation_safeguard
(
    latchTaskName    varchar(128) not null,
    latchMachineGUID varchar(40)  not null,
    LatchKey         char         not null,
    latchLocker      datetime     not null,
    primary key (latchTaskName, latchMachineGUID, LatchKey)
)
go

create table systranschemas
(
    tabid    int            not null,
    startlsn binary(10)     not null,
    endlsn   binary(10)     not null,
    typeid   int default 52 not null
)
go

create unique clustered index uncsystranschemas
    on systranschemas (startlsn)
go

