using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MasterDetails.Models
{
    [Table("Category")]
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required, Display(Name = "Category")]
        public string Name { get; set; }

        //[ForeignKey("CategoryID")] -- You can reference foreign key here
        public virtual IList<Item> Items { get; set; }
    }
    [Table("Item")]
    public class Item
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required, Display(Name = "Product Name")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Column(TypeName = "image")]
        public byte[] Image { get; set; }

        [Required, DataType(DataType.Date), Column(TypeName = "date"), Display(Name = "Entry Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EntryDate { get; set; }

        [Required]
        public long Quantity { get; set; }

        [ForeignKey("Category")]
        public long CategoryID { get; set; }

        //[ForeignKey("CategoryID")] -- You can reference foreign key here
        public virtual Category Category { get; set; }
    }

}