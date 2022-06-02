namespace RefactorThis.Domain.Services
{
    public interface IRepository<T>
    {
        T Get(string id);
		void Save(T entity);
        void Add(T entity);
	}
}
