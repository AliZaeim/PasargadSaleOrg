using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.General
{
    public class PasargadCommissionViewModel
    {
        public string InsNO { get; set; }
        public string DueDate { get; set; }
        public string Insurer { get; set; }
        public string PaidDate { get; set; }
        public string Percent { get; set; }
        public string LifePremium { get; set; }
        public string SupPermium { get; set; }
        public string InsPermium { get; set; }
        public string LifeCommission { get; set; }
        public string SupCommission { get; set; }
        public string SumCommission { get; set; }
        public string Tax { get; set; }
        public string Deductions { get; set; }
        public string Vat { get; set; }
        public string MunicipalTax { get; set; }
        public string TotalVat { get; set; }
        public string NetCommission { get; set; }
        public string DeductionDisc { get; set; }

    }
}
