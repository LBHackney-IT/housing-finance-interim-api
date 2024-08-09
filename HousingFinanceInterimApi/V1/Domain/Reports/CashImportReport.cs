using System;

namespace HousingFinanceInterimApi.V1.Domain.Reports;


public class CashImportReport
{
    public DateTime Date { get; set; }
    public decimal IFSTotal { get; set; }
    public decimal FileTotal { get; set; }
    public decimal GPS { get; set; }
    public decimal HGF { get; set; }
    public decimal HRA { get; set; }
    public decimal LMW { get; set; }
    public decimal LSC { get; set; }
    public decimal TAG { get; set; }
    public decimal TAH { get; set; }
    public decimal TRA { get; set; }
    public decimal ZZZZZZ { get; set; }
    public decimal SSSSSS { get; set; }

    public string[] ToRow()
    {
        return new string[] {
            Date.ToString("dd/MM/yyyy"), IFSTotal.ToString("0.00"), FileTotal.ToString("0.00"),
            GPS.ToString("0.00"), HGF.ToString("0.00"), HRA.ToString("0.00"), LMW.ToString("0.00"),
            LSC.ToString("0.00"), TAG.ToString("0.00"), TAH.ToString("0.00"), TRA.ToString("0.00"),
            ZZZZZZ.ToString("0.00"), SSSSSS.ToString("0.00")
        };
    }
}
