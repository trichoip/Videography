namespace Videography.Domain.Common.Interfaces
{
    public interface IEntity : IAuditableEntity
    {
        public int Id { get; set; }
    }
}
