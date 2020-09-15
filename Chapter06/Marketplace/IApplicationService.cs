using System.Threading.Tasks;
using Marketplace.Contracts;

namespace Marketplace
{
    public interface IApplicationService
    {
        Task Create(Marketplace.Contracts.ClassifiedAds.V1.Create request);


        Task SetTitle(ClassifiedAds.V1.SetTitle request);


        Task UpdateText(ClassifiedAds.V1.UpdateText request);


        Task UpdatePrice(ClassifiedAds.V1.UpdatePrice request);


        Task RequestToPublish(ClassifiedAds.V1.RequestToPublish request);
    }
}