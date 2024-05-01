namespace Common.HelperLibrary.Entities
{
    public interface IEntity
    {
        object Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
