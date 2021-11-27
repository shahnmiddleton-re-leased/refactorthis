namespace RefactorThis.Persistence
{
    public interface IRepository<T>
    {

        T Get(string reference);

        void Save();

        void Add(T invoice);

    }
}