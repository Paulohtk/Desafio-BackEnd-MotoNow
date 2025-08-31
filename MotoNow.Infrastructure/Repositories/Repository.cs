using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MotoNow.Domain.Entities;
using MotoNow.Domain.Repositories;
using MotoNow.Infrastructure.Context;

namespace MotoNow.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _ctx;
        protected readonly DbSet<T> _db;

        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
            _db = ctx.Set<T>();
        }

        public Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
            => _db.FirstOrDefaultAsync(e => e.Identifier == id, ct);

        public async Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? await _db.ToListAsync(ct) : await _db.Where(predicate).ToListAsync(ct);

        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => _db.AnyAsync(predicate, ct);

        public Task AddAsync(T entity, CancellationToken ct = default) => _db.AddAsync(entity, ct).AsTask();

        public void Update(T entity) => _db.Update(entity);

        public void Remove(T entity) => _db.Remove(entity);

        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _ctx.SaveChangesAsync(ct);
    }
}
