using System.ComponentModel.DataAnnotations;

namespace SecurityEssentialsDemo.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class SampleModel : DbContext
    {
        public SampleModel()
            : base("name=SampleModel")
        {
        }

        public virtual DbSet<InjectionSampleItem> InjectionSampleItems { get; set; }
        public virtual DbSet<XSSSampleItem> XSSSampleItems { get; set; }
    }

    public class InjectionSampleItem
    {
        [Key]
        public Guid Id { get; set; }
        public string Text { get; set; }
    }

    public class XSSSampleItem
    {
        [Key]
        public Guid Id { get; set; }
        public string Text { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}