
using System.Collections.Generic;
namespace University.Bussiness.Models.ViewModel
{
    public class FavouriteBooks_vm
    {
        public List<int> BookIds { get; set; }
        public string BookName { get; set; }
        public string PostedType { get; set; }
        public string PostedDate { get; set; }
        public int DaysAgo { get; set; }
    }
}
