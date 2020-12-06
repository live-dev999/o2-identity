using System.Collections.Generic;

namespace O2.Identity.Web.Models.ManageViewModels
{
    public class SubscriptionsViewModel
    {
        public PageViewModel PageViewModel { get; set; }
        public List<SubscriptionViewModel> Subscriptions { get; set; }

        public SubscriptionsViewModel()
        {
            Subscriptions = new List<SubscriptionViewModel>();
        }
    }
}