namespace DataAccess.Abstract
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IStructureNodeRepo StructureNodes { get; }
        IEmployeeRepo Employees { get; }
        Task<int> SaveChangesAsync();
    }
}
