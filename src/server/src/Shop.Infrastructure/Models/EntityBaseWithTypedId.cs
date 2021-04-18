namespace Shop.Infrastructure.Models
{
    public abstract class EntityBaseWithTypedId<TId> : ValidatableObject, IEntityWithTypedId<TId>
    {
        public virtual TId Id { get; set; } //protected set; redis 反序列化时如果是受保护的，则无法设置值
    }
}
