namespace RefactorThis.Persistence {
	public class InvoiceRepository
	{
		

		public Invoice GetInvoice( String invoiceNumberReference )
		{
			Invoice invoiceToBeReturned=new Invoice();
            // fetch the invoice details as per the reference passed and store in the invoice class 
            if(invoiceToBeReturned==null)
                throw new InvalidOperationException( "There is no invoice matching this payment" );

            return invoiceToBeReturned;
		}

		public void SaveInvoice( Invoice invoice )
		{
			//saves the invoice to the database 
            
		}

	}
}
