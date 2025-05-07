using Frontend.Models;

namespace Frontend.Services
{
    public interface IUserApiService
    {
        Task<List<UserViewModel>> GetAllAsync();
    }
}
