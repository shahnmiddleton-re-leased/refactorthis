using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RefactorThis.Persistence
{
    public class Invoice
    {
        public Invoice()
        {
            Payments = new HashSet<Payment>();
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Reference { get; set; }

        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}