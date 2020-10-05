using System;
using System.Linq;
using Marketplace.Domain;
using Xunit;

namespace Marketplace.Tests
{
    public class ClassifiedAd_Publish_Spec
    {
        private readonly ClassifiedAd _classifiedAd;
        
        public ClassifiedAd_Publish_Spec()
        {
            _classifiedAd = new ClassifiedAd(
                new ClassifiedAdId(Guid.NewGuid()), 
                new UserId(Guid.NewGuid()));
        }

        [Fact]
        public void Can_publish_a_valid_ad()
        {
            _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
            _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my stuff"));
            _classifiedAd.UpdatePrice(
                Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));
            _classifiedAd.AddPicture(new Uri("http://localhost/storage/123.jpg"), new PictureSize(1200, 620));

            _classifiedAd.ResizePicture(_classifiedAd.Pictures.First().Id, new PictureSize(1300, 720));


            _classifiedAd.RequestToPublish();

            Assert.Equal(ClassifiedAd.ClassifiedAdState.PendingReview,
                _classifiedAd.State);
        }

        [Fact]
        public void Cannot_publish_without_title()
        {
            _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my stuff"));
            _classifiedAd.UpdatePrice(
                Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));
            
            Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
        }

        [Fact]
        public void Cannot_publish_without_text()
        {
            _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
            _classifiedAd.UpdatePrice(
                Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));
            
            Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
        }

        [Fact]
        public void Cannot_publish_without_price()
        {
            _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
            _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my stuff"));
            
            Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
        }

        [Fact]
        public void Cannot_publish_with_zero_price()
        {
            _classifiedAd.SetTitle(ClassifiedAdTitle.FromString("Test ad"));
            _classifiedAd.UpdateText(ClassifiedAdText.FromString("Please buy my stuff"));
            _classifiedAd.UpdatePrice(
                Price.FromDecimal(0.0m, "EUR", new FakeCurrencyLookup()));
            
            Assert.Throws<InvalidEntityStateException>(() => _classifiedAd.RequestToPublish());
        }

        [Fact]
        public void Can_resize_picture()
        {
            // Arrange
            _classifiedAd.AddPicture(new Uri("http://localhost/storage/123.jpg"), new PictureSize(1200, 620));
            //_classifiedAd.ClearChanges();
            var sut = _classifiedAd.Pictures.First();
            var expected  = new PictureSize(1100, 600);

            // Act
            sut.Resize(expected);
            var actual =  sut.Size;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Resize_picture_in_classifiedAd_changed_list()
        {
            // Arrange
            _classifiedAd.AddPicture(new Uri("http://localhost/storage/123.jpg"), new PictureSize(1200, 620));
            _classifiedAd.ClearChanges();
            var sut = _classifiedAd.Pictures.First();
            var expected = new PictureSize(1100, 600);

            // Act
            sut.Resize(new PictureSize(1100, 600));
            var actual = _classifiedAd.GetChanges();

            // Assert
            Assert.IsType<Events.ClassifiedAdPictureResized>(actual.First());
           
        }

        [Fact]
        public void Can_resize_picture_from_classifiedAd()
        {
            // Arrange
            _classifiedAd.AddPicture(new Uri("http://localhost/storage/123.jpg"), new PictureSize(1200, 620));
            //_classifiedAd.ClearChanges();
            var sut = _classifiedAd.Pictures.First();
            var expected = new PictureSize(1100, 600);

            // Act
            _classifiedAd.ResizePicture(sut.Id, new PictureSize(1100, 600));
            var actual = sut.Size;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Resize_picture__from_classifiedAd_in_classifiedAd_changed_list()
        {
            // Arrange
            _classifiedAd.AddPicture(new Uri("http://localhost/storage/123.jpg"), new PictureSize(1200, 620));
            _classifiedAd.ClearChanges();
            var sut = _classifiedAd.Pictures.First();
            var expected = new PictureSize(1100, 600);

            // Act
            _classifiedAd.ResizePicture(sut.Id, new PictureSize(1100, 600));
            var actual = _classifiedAd.GetChanges();

            // Assert
            Assert.IsType<Events.ClassifiedAdPictureResized>(actual.First());

        }
    }
}