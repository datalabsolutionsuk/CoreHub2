using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CoreHub.Application.Extensions;
using CoreHub.Application.Interfaces;
using CoreHub.Domain.Entities;
using CoreHub.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CoreHub.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation with organization-level filtering
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    private readonly bool _hasOrganizationId;

    /// <summary>
    /// Gets the current organization ID from the authenticated user's claims
    /// </summary>
    protected Guid? CurrentOrganizationId
    {
        get
        {
            var user = _httpContextAccessor?.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
                return null;

            if (user.TryGetOrganizationId(out var organizationId))
                return organizationId;

            return null;
        }
    }

    public Repository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _dbSet = context.Set<T>();
        
        // Check if entity type has OrganizationId property
        _hasOrganizationId = typeof(T).GetProperty("OrganizationId") != null;
    }

    /// <summary>
    /// Applies organization filter to query if entity has OrganizationId property
    /// </summary>
    protected IQueryable<T> ApplyOrganizationFilter(IQueryable<T> query)
    {
        if (!_hasOrganizationId || CurrentOrganizationId == null)
            return query;

        // Use reflection to build the filter expression
        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, "OrganizationId");
        var organizationIdValue = Expression.Constant(CurrentOrganizationId.Value);
        var equals = Expression.Equal(property, organizationIdValue);
        var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

        return query.Where(lambda);
    }

    /// <summary>
    /// Validates that entity belongs to current user's organization
    /// </summary>
    protected void ValidateOrganizationAccess(T entity)
    {
        if (!_hasOrganizationId || CurrentOrganizationId == null)
            return;

        var orgIdProperty = typeof(T).GetProperty("OrganizationId");
        if (orgIdProperty == null)
            return;

        var entityOrgId = orgIdProperty.GetValue(entity);
        if (entityOrgId is Guid orgId && orgId != CurrentOrganizationId.Value)
        {
            throw new UnauthorizedAccessException(
                $"Access denied. Entity belongs to a different organization.");
        }
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query.Where(predicate).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Set OrganizationId if entity has the property and it's not already set
        if (_hasOrganizationId && CurrentOrganizationId.HasValue)
        {
            var orgIdProperty = typeof(T).GetProperty("OrganizationId");
            if (orgIdProperty != null)
            {
                var currentValue = orgIdProperty.GetValue(entity);
                if (currentValue == null || (currentValue is Guid guid && guid == Guid.Empty))
                {
                    orgIdProperty.SetValue(entity, CurrentOrganizationId.Value);
                }
            }
        }

        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Validate organization access before updating
        ValidateOrganizationAccess(entity);

        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Validate organization access before deleting
        ValidateOrganizationAccess(entity);

        _dbSet.Remove(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            await DeleteAsync(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        return await query.AnyAsync(e => e.Id == id);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        var query = ApplyOrganizationFilter(_dbSet);
        
        if (predicate == null)
            return await query.CountAsync();

        return await query.CountAsync(predicate);
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
