using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Domain.Services
{
    public interface IRepository<T>
    {
        T Get(string reference);
		void Save(T entity);
        void Add(T entity);
	}
}
