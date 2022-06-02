using System.Collections.Generic;
using System.Linq;
using RefactorThis.Domain.Model;
using RefactorThis.Domain.Services;

namespace RefactorThis.Persistence
{
    public class Repository<T> : IRepository<T> where T : BaseModel
    {
        protected readonly List<T> _localRepo = new List<T>();
        public void Add(T model)
        {
            _localRepo.Add(model);
        }

        public T Get(string id)
        {
            return _localRepo.FirstOrDefault(p => p.Id == id);
        }

        public void Save(T model)
        {
            //TODO: saves the invoice to the database
        }
    }
}
