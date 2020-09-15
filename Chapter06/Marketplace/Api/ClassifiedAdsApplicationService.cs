using System;
using System.Threading.Tasks;
using Marketplace.Domain;
using static Marketplace.Contracts.ClassifiedAds;

namespace Marketplace.Api
{
    public class ClassifiedAdsApplicationService
        : IApplicationService
    {
        private readonly ICurrencyLookup _currencyLookup;
        private readonly IClassifiedAdRepository _repository;

        public ClassifiedAdsApplicationService(
            IClassifiedAdRepository repository,
            ICurrencyLookup currencyLookup
        )
        {
            _repository = repository;
            _currencyLookup = currencyLookup;
        }

        //public Task Handle(object command) =>
        //    command switch
        //    {
        //        V1.Create cmd =>
        //            HandleCreate(cmd),
        //        V1.SetTitle cmd =>
        //            HandleUpdate(
        //                cmd.Id,
        //                c => c.SetTitle(
        //                    ClassifiedAdTitle.FromString(cmd.Title)
        //                )
        //            ),
        //        V1.UpdateText cmd =>
        //            HandleUpdate(
        //                cmd.Id,
        //                c => c.UpdateText(
        //                    ClassifiedAdText.FromString(cmd.Text)
        //                )
        //            ),
        //        V1.UpdatePrice cmd =>
        //            HandleUpdate(
        //                cmd.Id,
        //                c => c.UpdatePrice(
        //                    Price.FromDecimal(
        //                        cmd.Price,
        //                        cmd.Currency,
        //                        _currencyLookup
        //                    )
        //                )
        //            ),
        //        V1.RequestToPublish cmd =>
        //            HandleUpdate(
        //                cmd.Id,
        //                c => c.RequestToPublish()
        //            ),
        //        _ => Task.CompletedTask
        //    };



        //private async Task HandleCreate(V1.Create cmd)
        //{
        //    if (await _repository.Exists(cmd.Id.ToString()))
        //        throw new InvalidOperationException($"Entity with id {cmd.Id} already exists");

        //    var classifiedAd = new ClassifiedAd(
        //        new ClassifiedAdId(cmd.Id),
        //        new UserId(cmd.OwnerId)
        //    );

        //    await _repository.Save(classifiedAd);
        //}

        //private async Task HandleUpdate(
        //    Guid classifiedAdId,
        //    Action<ClassifiedAd> operation
        //)
        //{
        //    var classifiedAd = await _repository.Load(
        //        classifiedAdId.ToString()
        //    );
        //    if (classifiedAd == null)
        //        throw new InvalidOperationException(
        //            $"Entity with id {classifiedAdId} cannot be found"
        //        );

        //    operation(classifiedAd);

        //    await _repository.Save(classifiedAd);
        //}
        async Task IApplicationService.Create(V1.Create request)
        {
            throw new NotImplementedException();
        }

        async Task IApplicationService.RequestToPublish(V1.RequestToPublish request)
        {
            throw new NotImplementedException();
        }

        async Task IApplicationService.SetTitle(V1.SetTitle request)
        {
            var classifiedAd = await _repository.Load(new
                ClassifiedAdId(request.Id));

            classifiedAd.SetTitle(ClassifiedAdTitle.FromString(request.Title));
        }

        async Task IApplicationService.UpdatePrice(V1.UpdatePrice request)
        {
            throw new NotImplementedException();
        }

        async Task IApplicationService.UpdateText(V1.UpdateText request)
        {
            throw new NotImplementedException();
        }
    }
}