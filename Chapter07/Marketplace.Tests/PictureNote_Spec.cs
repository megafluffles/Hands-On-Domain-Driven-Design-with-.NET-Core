using System;
using System.Linq;
using Marketplace.Domain;
using Xunit;

namespace Marketplace.Tests
{
    public class PictureNote_Spec
    {
        private readonly ClassifiedAd _classifiedAd;
        private readonly Picture _picture;

        public PictureNote_Spec()
        {
            _classifiedAd = new ClassifiedAd(
                new ClassifiedAdId(Guid.NewGuid()), 
                new UserId(Guid.NewGuid()));
            _classifiedAd.AddPicture(new Uri("http://localhost/storage/123.jpg"), new PictureSize(1200, 620));
            _picture = _classifiedAd.Pictures.First();
            _classifiedAd.ClearChanges();

        }

        [Fact]
        public void Can_add_picture_note_from_classifiedAd()
        {
            // Arrange
            var expected = "Note";
            _classifiedAd.AddPictureNote(_picture.Id, expected);
            var sut = _classifiedAd.Pictures.First(a => a.Id == _picture.Id).Notes.First();

            // Act
            _classifiedAd.AddPictureNote(_picture.Id, expected);
            var actual = _classifiedAd.Pictures.First(a => a.Id == _picture.Id).Notes.First().Note;

            // Assert
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void Picture_note_added_in_classifiedAd_changed_list()
        {
            // Act
            _classifiedAd.AddPictureNote(_picture.Id, "Note");

            // Assert
            Assert.IsType<Events.PictureNoteAddedToPicture>(_classifiedAd.GetChanges().First());

        }



        [Fact]
        public void Can_update_picture_from_classifiedAd_note()
        {
            // Arrange
            _classifiedAd.AddPictureNote(_picture.Id, "Note");
            PictureNote sut = _classifiedAd.Pictures.First(a => a.Id == _picture.Id).Notes.First();
            var expected = "Ny note";

            // Act
            _classifiedAd.UpdatePictureNote(sut.Id, "Ny note");
            var actual = _classifiedAd.Pictures.First(a => a.Id == _picture.Id).Notes.First().Note; 

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Picture_note_update_in_classifiedAd_changed_list()
        {
            // Arrange
            _classifiedAd.AddPictureNote(_picture.Id, "Note");
            _classifiedAd.ClearChanges();
            PictureNote sut = _classifiedAd.Pictures.First(a => a.Id == _picture.Id).Notes.First();

            // Act
            _classifiedAd.UpdatePictureNote(sut.Id, "Ny note");
            var actual = _classifiedAd.GetChanges();

            // Assert
            Assert.IsType<Events.PictureNoteUpdated>(actual.First());

        }       
    }
}