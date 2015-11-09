using University.Common.Models.Enums;

namespace University.Common.Models.ViewModel
{
    public class News_vm
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public NewsType NewsType { get; set; }

        public string CustomField01 { get; set; }
        public string CustomField02 { get; set; }
        public string CustomField03 { get; set; }
        public string CustomField04 { get; set; }
        public string CustomField05 { get; set; }

    }
}
