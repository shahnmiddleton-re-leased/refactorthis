using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
    [TestFixture]
    public class InvoiceTests
    {
        [Test]
        public void CreateInvoice_Should_ThrowException_When_WhenInitializeWithNegativeValue( )
        {
            var failureMessage = "";

            try
            {
                var invoice = new Invoice( )
                {
                    Amount = -1
                };
            }
            catch ( InvalidOperationException e )
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual( "Invoice amount should be positive and greater than the paid amount", failureMessage );
        }
		
        [Test]
        public void CreateInvoice_Should_ThrowException_When_WhenTryToReduceTheInvoiceAmount( )
        {
            var failureMessage = "";
            try
            {
                var invoice = new Invoice( )
                {
                    Amount = 10
                };
				
                invoice.AddPayment(new Payment() {Amount = 5});
				
                invoice.Amount = 4;
            }
            catch ( InvalidOperationException e )
            {
                failureMessage = e.Message;
            }

            Assert.AreEqual( "Invoice amount should be positive and greater than the paid amount", failureMessage );
        }
    }
}