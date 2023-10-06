using Videography.Domain.Common.Interfaces;

namespace Videography.Domain.Common
{
    public abstract class BaseEntity : BaseAuditableEntity, IEntity
    {
        public int Id { get; set; }

    }
}
