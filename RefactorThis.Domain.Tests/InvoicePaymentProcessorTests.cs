using System;
using System.Collections.Generic;
using NUnit.Framework;
using RefactorThis.Persistence;

namespace RefactorThis.Domain.Tests
{
	[TestFixture]
	public class InvoicePaymentProcessorTests
	{
        private InvoiceRepository repo;
        private InvoicePaymentProcessor paymentProcessor;
        private Payment payment

         [SetUp]
         public void SetUp()
        {
            /* using setup method we need to create sample mock data for testing purposes
                Sample Data :
                1. {
                    AmountDue = 0,
                    AmountPaid = 0,
                    Payments = null
                    InvoiceReference=00001
			    }
                2. {
                    InvoiceReference=00002
                    AmountDue = 10,
                    AmountPaid = 10,
                    Payments = new List<Payment>
                    {
                        new Payment
                        {
                            Amount = 10
                        }
                    }
			       }
                3. {
                    InvoiceReference=00003
                    AmountDue = 50,
                    AmountPaid = 25,
                    Payments = new List<Payment>
                    {
                        new Payment
                        {
                            Amount = 25
                        }
                    }
			       }
                4. {
                    AmountDue = 50,
                    AmountPaid = 0,
                    Payments = null
                    InvoiceReference=00004
			    }

            */
             repo = new InvoiceRepository( );	
			 paymentProcessor= new InvoicePaymentProcessor( repo );
			 payment= new Payment( ) 
             {
				Amount = 10,
                
			 };
        }
		[Test]
		public void ProcessPayment_Should_ThrowException_When_NoInoiceFoundForPaymentReference( )
		{
			String failureMessage = "";
            payment.InvoiceReference="XXX";
			try
			{
				String result = paymentProcessor.ProcessPayment( payment );
			}
			catch ( InvalidOperationException e )
			{
				failureMessage = e.Message;
			}
			Assert.AreEqual( "There is no invoice matching this payment", failureMessage );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_NoPaymentNeeded( )
		{	
			payment.InvoiceReference="0001";
			String result = paymentProcessor.ProcessPayment( payment );
			Assert.AreEqual( "no payment needed or invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_InvoiceAlreadyFullyPaid( )
		{
			payment.InvoiceReference="0002";
			String result = paymentProcessor.ProcessPayment( payment );
			Assert.AreEqual( "no payment needed or invoice was already fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PaymentIsMoreThanAmountDue( )
		{
			payment.InvoiceReference="0003";
            paymentAmount = 100;
			String result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the remaining invoice amount", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnFailureMessage_When_PaymentIsMoreThanBalanceAmount( )
		{
			payment.InvoiceReference="0003";
            paymentAmount = 50;
			String result = paymentProcessor.ProcessPayment( payment );

			Assert.AreEqual( "the payment is greater than the remaining invoice amount", result );
		}
				
		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_PartialPaymentExistsAndAmountPaidIsLessThanAmountDue( )
		{
			payment.InvoiceReference="0003";
            paymentAmount = 10;
			String result = paymentProcessor.ProcessPayment( payment );
			Assert.AreEqual( "another partial payment received, still not fully paid", result );
		}

        public void ProcessPayment_Should_ReturnPaidMessage_When_PartialPaymentExistsAndAmountPaidIsEqualToAmountDue( )
		{
			payment.InvoiceReference="0003";
            paymentAmount = 25;
			String result = paymentProcessor.ProcessPayment( payment );
			Assert.AreEqual( "invoice is now fully paid", result );
		}

		[Test]
		public void ProcessPayment_Should_ReturnPartiallyPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsLessThanInvoiceAmount( )
		{
			payment.InvoiceReference="0004";
            paymentAmount = 10;
			String result = paymentProcessor.ProcessPayment( payment );
			Assert.AreEqual( "another partial payment received, still not fully paid", result );
		}

        public void ProcessPayment_Should_ReturnPaidMessage_When_NoPartialPaymentExistsAndAmountPaidIsEqualToInvoiceAmount( )
		{
			payment.InvoiceReference="0003";
            paymentAmount = 25;
			String result = paymentProcessor.ProcessPayment( payment );
			Assert.AreEqual( "invoice is now fully paid", result );
		}
	}
}
