using System.Collections.Generic;

namespace WbyCodeLibrary.ExcelOperation
{
    public class ControlElementDto
    {
        public string ControlElementName { get; set; }
        public List<ControlElementDetail> ControlElementDetails { get; set; }
    }

    public class ControlElementDetail
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public bool IsChecked { get; set; } = true;
    }
}
