using System.Collections.Generic;

namespace O2.Identity.Web.Models.ManageViewModels
{
    public class InvoicesViewModel
    {
        public PageViewModel PageViewModel { get; set; }
        public List<InvoiceViewModel> Invoices { get; set; }
    }
}