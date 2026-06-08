using pizza.api.Domain.Entities;

namespace pizza.api.Infrastructure.Data;

/// <summary>
/// Read access to the pizza catalog. Implemented today by an in-memory mock
/// (<see cref="InMemoryPizzaCatalog"/>); a later spec swaps in an EF Core / PostgreSQL-backed
/// implementation behind this same interface so the feature slices stay unchanged.
/// </summary>
public interface IPizzaCatalog
{
    /// <summary>Returns the pizza with the given id, or <c>null</c> if none exists.</summary>
    Pizza? FindById(int id);
}
