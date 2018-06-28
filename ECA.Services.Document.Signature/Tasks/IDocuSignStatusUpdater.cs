using System.Threading.Tasks;

namespace ECA.Services.Document.Signature.Tasks
{
    public interface IDocuSignStatusUpdater
    {
        Task Update();
    }
}