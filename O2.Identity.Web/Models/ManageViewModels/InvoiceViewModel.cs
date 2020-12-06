using System;

namespace O2.Identity.Web.Models.ManageViewModels
{
    public class InvoiceViewModel
    {
        public int InvoiceID;
        public DateTime DueDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string BillingPeriod { get; set; }
        public string AmountDue { get; set; }
        public string TotalAmount { get; set; }
        public string Status { get; set; }
    }
}