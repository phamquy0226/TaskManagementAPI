using Frontend.Models;

namespace Frontend.Repository
{
    public interface IUserApiService
    {
        Task<List<UserViewModel>> GetAllAsync();
    }
}
