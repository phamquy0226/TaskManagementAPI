using Frontend.Models;

namespace Frontend.Services
{
    public interface IDepartmentApiService
    {
        Task<List<DepartmentViewModel>> GetAllAsync();
    }
}
