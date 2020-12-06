using System.Collections.Generic;

namespace O2.Identity.Web.Models.ManageViewModels
{
    public class UsersViewModel
    {
        public List<O2User> Users { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}