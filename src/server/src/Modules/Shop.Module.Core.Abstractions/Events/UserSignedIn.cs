using MediatR;

namespace Shop.Module.Core.Events
{
    public class UserSignedIn : INotification
    {
        public int UserId { get; set; }
    }
}
