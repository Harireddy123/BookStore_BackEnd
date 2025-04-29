using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepositoryLayer.Entity
{
    public class Order
    {
        public int OrderId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10,2)")]   
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
    }
}
