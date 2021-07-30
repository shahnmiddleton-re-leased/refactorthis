using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RefactorThis.Persistence
{
	public class Payment
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Reference { get; set; }

        public virtual Invoice Invoice { get; set; } = null!;
        public Guid InvoiceReference { get; set; }

        public decimal Amount { get; set; }
    }
}