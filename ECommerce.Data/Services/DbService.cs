

namespace ECommerce.Data.Services;

public class DbService
{
    public readonly ECommerceDbContext db;
    private readonly IMapper _mapper;



    public DbService(ECommerceDbContext database, IMapper mapper)
    {
        db = database;
        _mapper = mapper;
    }


    public async Task<TDto> SingleAsync<TEntity, TDto>(int id) where TEntity : class, IEntity where TDto : class
    {
        IncludeNavigations<TEntity>();
        var entity = await db.Set<TEntity>().SingleOrDefaultAsync(e => e.Id == id);
        
        return _mapper.Map<TDto>(entity);
    }

    public async Task<List<TDto>> GetAsync<TEntity, TDto>() where TEntity : class where TDto : class
    {
        IncludeNavigations<TEntity>();
        var entities = await db.Set<TEntity>().ToListAsync();
        return _mapper.Map<List<TDto>>(entities);
    }

    public async Task<TEntity> AddAsync<TEntity, TDto>(TDto dto) where TEntity : class, IEntity where TDto : class
    {
        var entity = _mapper.Map<TEntity>(dto);
        if (typeof(TEntity) == typeof(Customer))
        {
            var customer = entity as Customer;
            if (customer != null)
            {
                Console.WriteLine($"{customer.FirstName}");
            }
        }


        await db.Set<TEntity>().AddAsync(entity);
        return entity;
    }


    public void Update<TEntity, TDto>(TDto dto) where TEntity : class, IEntity where TDto : class
    {
        var entity = _mapper.Map<TEntity>(dto);
        db.Set<TEntity>().Update(entity);
    }

    public async Task<bool> DeleteAsync<TEntity>(int id) where TEntity : class, IEntity
    {
        try
        {
            var entity = await db.Set<TEntity>().SingleOrDefaultAsync(e => e.Id == id);
            if (entity is null) return false;
            db.Remove(entity);
        }
        catch { return false; }

        return true;
    }

    public async Task<bool> SaveChangesAsync() => await db.SaveChangesAsync() >= 0;


    public void IncludeNavigations<TEntity>() where TEntity : class
    {
        var entityType = db.Model.FindEntityType(typeof(TEntity));
        if (entityType == null) return;

        var skipNavigationProperties = entityType.GetDeclaredSkipNavigations().Select(s => s.Name);
        var navigationProperties = entityType.GetNavigations().Select(s => s.Name);

        IQueryable<TEntity> query = db.Set<TEntity>();

        foreach (var name in navigationProperties.Union(skipNavigationProperties))
        {
            if (name == "Orders")
            {
                // Include OrderItems and its navigation properties
                query = query.Include($"{name}.OrderItems.Product");
            }
            else
            {

                query = query.Include(name);
            }
        }
        query.Load();
    }
}

