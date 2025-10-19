using Fixopolis.Application.Abstractions;


public class CategoryValidatorService : ICategoryValidatorService
{
    private readonly IAppDbContext _db;

    public CategoryValidatorService(IAppDbContext db)
    {
        _db = db;
    }

    public bool BeUniqueName(string name)
        => !_db.Categories.Any(p => p.Name == name);
}
