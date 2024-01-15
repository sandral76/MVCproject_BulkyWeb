using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T: class
    {
        //T- Category, na primer
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null,string? includeProperties = null);
        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool? tracked = false);
        void Add(T entity);
        //void Update(T entity); implementacija se moze razlikovati od klase do klase
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }

}
