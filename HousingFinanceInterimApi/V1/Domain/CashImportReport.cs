using System;

namespace HousingFinanceInterimApi.V1.Domain;


public class CashImportReport
{
    DateTime Date { get; set; }
    decimal IFSTotal { get; set; }
    decimal FileTotal { get; set; }
    decimal GPS { get; set; }
    decimal HGF { get; set; }
    decimal HRA { get; set; }
    decimal LMW { get; set; }
    decimal LSC { get; set; }
    decimal TAG { get; set; }
    decimal TAH { get; set; }
    decimal TRA { get; set; }
    decimal ZZZZZZ { get; set; }
    decimal SSSSSS { get; set; }
}
