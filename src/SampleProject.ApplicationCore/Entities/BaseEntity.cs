namespace SampleProject.ApplicationCore.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public DateTime UpdatedDatetime { get; set; }
        public BaseEntity()
        {
            CreatedDatetime = DateTime.UtcNow;
            UpdatedDatetime = DateTime.UtcNow;
        }
    }
}
