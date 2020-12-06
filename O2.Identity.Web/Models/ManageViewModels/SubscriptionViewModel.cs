namespace O2.Identity.Web.Models.ManageViewModels
{
    public enum TypeSubscription
    {
        Free,
        Basic,
        Vip
    }
    public class SubscriptionViewModel
    {
        public string AppName { get; set; }
        public decimal Cost { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool Term { get; set; }
        public TypeSubscription TypeSubscription { get; set; } = TypeSubscription.Free;
    }
}