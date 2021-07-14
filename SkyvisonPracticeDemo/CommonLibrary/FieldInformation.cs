using System;

namespace CommonLibrary
{
    public class FieldInformation
    {
        public int Length { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }
        public bool IsSystemField { get; set; }
        public bool DomainFixed { get; set; }
        public bool Editable { get; set; }
        public bool Nullable { get; set; }
        public bool RegisteredRenderIndex { get; set; }
        public string FieldType { get; set; }
        public dynamic DefaultValue { get; set; }

        public DomainInformation Domain { get; set; }
        public GeometryInformation GeometryDef { get; set; }
    }
}
