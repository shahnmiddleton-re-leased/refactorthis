using System;
using System.Collections.Generic;
using System.Linq;
using RefactorThis.Domain.Model;
using RefactorThis.Domain.Services;

namespace RefactorThis.Persistence
{
    public class Repository<T> : IRepository<T> where T : BaseModel
    {
        private readonly List<T> _localRepo;
        public void Add(T model)
        {
            _localRepo.Add(model);
        }

        public T Get(string reference)
        {
            return _localRepo.FirstOrDefault(p => p.Reference == reference);
        }

        public void Save(T model)
        {
            //TODO: saves the invoice to the database
        }
    }
}
