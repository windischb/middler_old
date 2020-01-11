using middler.Common.Enums;

namespace middler.Common.Models
{
    public class MiddlerRulePermission
    {
        public string PrincipalName { get; set; }
        public PrincipalType Type { get; set; }
        public AccessMode AccessMode { get; set; }
        public string Client { get; set; }
        public string SourceAddress { get; set; }

    }

}
