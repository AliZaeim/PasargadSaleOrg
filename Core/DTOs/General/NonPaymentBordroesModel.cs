using DataLayer.Entities.LifeBordro;
using DataLayer.Entities.User;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTOs.General
{
    public class NonPaymentBordroesModel
    {
        /// <summary>
        /// بیمه نامه های پرداخت نشده
        /// </summary>
        public List<NonePaymentBordroesDet> NonePaymentBordroesDets { get; set; }

        public int? RecCount { get; set; }
        public int TotalPages { get; set; }
        public int? CurPage { get; set; }
        public int TotalRecCount { get; set; }
        public string SearchText { get; set; }

        public string SearchField { get; set; }
        public string SearchFieldName { get; set; }
        public int IsDateRange { get; set; }


    }
}
