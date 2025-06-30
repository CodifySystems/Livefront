using CartonCaps.Domain.Common;

namespace CartonCaps.Application.Repositories;

/// <summary>
/// Interface for base repository operations.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBaseRepository<T> where T : BaseEntity
{

}