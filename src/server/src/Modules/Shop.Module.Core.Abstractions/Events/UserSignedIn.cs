using MediatR;

namespace Shop.Module.Core.Abstractions.Events
{
    public class UserSignedIn : INotification
    {
        public int UserId { get; set; }
    }
}
