using Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly WebApisContext context;
        private DbSet<T> entities;
        string errorMessage = string.Empty;
        private IConfiguration _config;
        private string v;

        public Repository(WebApisContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public Repository(IConfiguration config, string v)
        {
            _config = config;
            this.v = v;
        }

        public string CollectionName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void DeleteOne(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Filter(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public T GetOne(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void InsertOne(T item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Page(IQueryable<T> source, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Sort(IQueryable<T> source, string sortBy, string sortDirection)
        {
            throw new NotImplementedException();
        }

        public void UpdateOne(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
