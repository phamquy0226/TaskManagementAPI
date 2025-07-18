using Frontend.Models;

namespace Frontend.Repository
{
    public interface IDepartmentApiService
    {
        Task<List<DepartmentViewModel>> GetAllAsync();
    }
}
