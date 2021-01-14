using System.Threading.Tasks;

namespace KidsAreaApp.Services
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}
