using System;
using System.Runtime.CompilerServices;
using Marketplace.Framework;

[assembly: InternalsVisibleTo("Marketplace.Tests")]
namespace Marketplace.Domain
{
    public class PictureNote : Entity<PictureNoteId>
    {
        internal PictureId ParentId { get; private set; }
        internal string Note { get; private set; }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case Events.PictureNoteAddedToPicture e:
                    ParentId = new PictureId(e.PictureId);
                    Id = new PictureNoteId(e.PictureNoteId);
                    Note = e.Note;
                    break;
                case Events.PictureNoteUpdated e:
                    Note = e.Note;
                    break;
            }
        }
        
        public void UpdateNote(string note)
            => Apply(new Events.PictureNoteUpdated{Note = note});

        public PictureNote(Action<object> applier) : base(applier)
        {
        }
    }

    public class PictureNoteId : Value<PictureNoteId>
    {
        public PictureNoteId(Guid value) => Value = value;

        public Guid Value { get; }
    }
}