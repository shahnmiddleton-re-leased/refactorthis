
namespace RefactorThis.Domain.Interfaces
{
    /// <summary>
    /// Describes a generic repository.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        void Update(TEntity entity);
    }
}
