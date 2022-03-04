using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefactorThis.Persistence.Model
{
	public class Payment
	{
		public decimal Amount { get; set; }
		public string Reference { get; set; }
	}
}
