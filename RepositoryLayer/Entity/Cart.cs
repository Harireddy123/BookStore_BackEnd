using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace RepositoryLayer.Entity
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Book")]
        public int BookId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public bool IsPurchased { get; set; } = false;

        public virtual Book Book { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }

}
