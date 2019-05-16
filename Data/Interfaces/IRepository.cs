using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IRepository<T> where T : class
    {
        string CollectionName { get; set; }

        T GetOne(Expression<Func<T, bool>> expression);

        void UpdateOne(Expression<Func<T, bool>> expression);

        void DeleteOne(Expression<Func<T, bool>> expression);

        void InsertOne(T item);

        IQueryable<T> Page(IQueryable<T> source, int page, int pageSize);

        IQueryable<T> Filter(Expression<Func<T, bool>> expression);

        IQueryable<T> Sort(IQueryable<T> source, string sortBy, string sortDirection);

    }
}
