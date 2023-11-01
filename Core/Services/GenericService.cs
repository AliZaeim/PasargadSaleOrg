using Core.Services.Interfaces;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class GenericService<TEntity> : IGenericService<TEntity> where TEntity : class
    {
        private readonly MyContext _context;
        public GenericService(MyContext context)
        {
            _context = context;
        }

        public void Create(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void Delete(int id)
        {
            var entityToDelete = _context.Set<TEntity>().Find(id);
            if (entityToDelete != null)
            {
                _context.Set<TEntity>().Remove(entityToDelete);
            }
        }

        public void Edit(TEntity entity)
        {
            
            _context.Set<TEntity>().Update(entity);
        }

        public IEnumerable<TEntity> Filter()
        {
            return _context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Filter(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate);
        }

        public async Task<IEnumerable<TEntity>> FilterAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>()
                .Where(predicate)
                .ToListAsync();
            
        }

        public IQueryable<TEntity> GetAll()
        {
            var query = _context.Set<TEntity>().AsQueryable();
            int c = _context.Model.FindEntityType(typeof(TEntity)).GetNavigations().Count();
            foreach (var property in _context.Model.FindEntityType(typeof(TEntity)).GetNavigations())
                query = query.Include(property.Name);
            return query;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var query=_context.Set<TEntity>().AsQueryable();
            foreach (var property in _context.Model.FindEntityType(typeof(TEntity)).GetNavigations())
                query = query.Include(property.Name);
            return await query.ToListAsync();
        }

        public TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            
            var query = await _context.Set<TEntity>().FindAsync(id);
            //foreach (var property in _context.Model.FindEntityType(typeof(TEntity)).GetNavigations())
            //    query = query.Include(property.Name);
            return query;
        }

        public int GetKey<T>(T entity)
        {
            var keyName = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties
        .Select(x => x.Name).Single();

            return (int)entity.GetType().GetProperty(keyName).GetValue(entity, null);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
