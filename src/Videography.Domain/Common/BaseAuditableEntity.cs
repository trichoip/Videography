using Videography.Domain.Common.Interfaces;

namespace Videography.Domain.Common
{
    public abstract class BaseAuditableEntity : IAuditableEntity
    {
        public string? CreatedBy { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}
