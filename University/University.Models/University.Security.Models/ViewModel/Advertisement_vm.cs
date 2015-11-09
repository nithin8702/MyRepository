using University.Common.Models.Enums;

namespace University.Security.Models.ViewModel
{
    public class Advertisement_vm
    {
        public int AdvertisementId { get; set; }
        public string Screen { get; set; }
        public Place Place { get; set; }
        public Frequency Frequency { get; set; }
        public Mode Mode { get; set; }
        public string ImagePath { get; set; }
        public string CustomField01 { get; set; }
        public string CustomField02 { get; set; }
        public string CustomField03 { get; set; }
        public string CustomField04 { get; set; }
        public string CustomField05 { get; set; }

    }
}
