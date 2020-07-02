namespace Db.Interfaces
{
    public interface IEntity<T>
    {
        T Id { get; set; }
    }
}