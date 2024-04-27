using System.ComponentModel.DataAnnotations.Schema;

namespace Common.HelperLibrary.Entities
{
    public abstract class BaseEntity<T> : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }

        object IEntity.Id
        {
            get => Id;
            set { }
        }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
      
    }
}
