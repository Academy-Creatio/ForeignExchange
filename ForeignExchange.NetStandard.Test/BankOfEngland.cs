using System;
using System.Threading.Tasks;
using Xunit;

namespace ForeignExchange.NetStandard.Test
{
	public class BankOfEngland
	{
		[Theory(DisplayName ="Should_Return"), ClassData(typeof(BOE_TestData_ShouldRetun))]
		public async Task BOE_GetRateAsyncShoudReturn(decimal expectedExchangeRate, 
			DateTime requestedRateDate, DateTime expectedRateDate, string currency)
		{
			#region Arrange
			IBankResult expected = new BankResult()
			{
				BankName = "Bank of England",
				HomeCurrency = "GBP",
				ExchangeRate = expectedExchangeRate,
				RateDate = expectedRateDate
			};
			#endregion

			#region Act
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOE);
			IBankResult actual = await ibank.GetRateAsync(currency, requestedRateDate);
			#endregion

			#region Assert
			//Assert.Equal(expected, actual);
			Assert.Equal(expected.RateDate, actual.RateDate);
			Assert.Equal(expected.ExchangeRate, actual.ExchangeRate);
			Assert.Equal(expected.BankName, actual.BankName);
			Assert.Equal(expected.HomeCurrency, actual.HomeCurrency);
			
			#endregion
		}

		[Fact]
		public async Task BOE_ShoudThrowCurrency()
		{
			#region Arrange
			DateTime date = new DateTime(2020, 1, 8);
			string expectedMessage = $"Bank of England does not support XXX";
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOE);
			#endregion

			#region Act
			var exception = Record.ExceptionAsync(
				async () => await ibank.GetRateAsync("XXX", date));
			exception.Wait();
			#endregion

			#region Assert

			Assert.Equal(expectedMessage, exception.Result.Message);
			Assert.IsType<NotImplementedException>(exception.Result);
			await Assert.ThrowsAsync<NotImplementedException>(
				async () => await ibank.GetRateAsync("XXX", date));
			#endregion
		}

		[Fact]
		public async Task BOE_ShoudThrowDate()
		{
			#region Arrange
			DateTime date = new DateTime(1999, 1, 3);
			string expectedMessage = @"Date must be greater than 04-Jan-2010 (Parameter 'date')";
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOE);
			#endregion

			#region Act
			var exception = Record.ExceptionAsync(
				async () => await ibank.GetRateAsync("USD", date));
			exception.Wait();
			#endregion

			#region Assert
			Assert.Equal(expectedMessage, exception.Result.Message);
			Assert.IsType<ArgumentOutOfRangeException>(exception.Result);
			await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
				async () => await ibank.GetRateAsync("USD", date));
			#endregion
		}
	}

	/// <summary>
	/// p1. Expected Exchange Rate
	/// p2. Rrequested RateDate
	/// p3. Expected RateDate
	/// p4. Requested Currency
	/// </summary>
	public class BOE_TestData_ShouldRetun : TheoryData<decimal, DateTime, DateTime, string>
	{
		public BOE_TestData_ShouldRetun()
		{
			Add(1.7646M, new DateTime(2010, 1, 4), new DateTime(2010, 1, 4), "AUD");
			Add(1.6695M, new DateTime(2010, 1, 4), new DateTime(2010, 1, 4), "CAD");
			Add(1.1166M, new DateTime(2010, 1, 4), new DateTime(2010, 1, 4), "EUR");
			Add(1.6121M, new DateTime(2010, 1, 4), new DateTime(2010, 1, 4), "USD");

			Add(1.8792M, new DateTime(2020, 1, 3), new DateTime(2020, 1, 3), "AUD");
			Add(1.6971M, new DateTime(2020, 1, 3), new DateTime(2020, 1, 3), "CAD");
			Add(1.1710M, new DateTime(2020, 1, 3), new DateTime(2020, 1, 3), "EUR");
			Add(1.3072M, new DateTime(2020, 1, 3), new DateTime(2020, 1, 3), "USD");

			Add(1.8792M, new DateTime(2020, 1, 4), new DateTime(2020, 1, 3), "AUD");
			Add(1.6971M, new DateTime(2020, 1, 4), new DateTime(2020, 1, 3), "CAD");
			Add(1.1710M, new DateTime(2020, 1, 4), new DateTime(2020, 1, 3), "EUR");
			Add(1.3072M, new DateTime(2020, 1, 4), new DateTime(2020, 1, 3), "USD");

			Add(1.8792M, new DateTime(2020, 1, 5), new DateTime(2020, 1, 3), "AUD");
			Add(1.6971M, new DateTime(2020, 1, 5), new DateTime(2020, 1, 3), "CAD");
			Add(1.1710M, new DateTime(2020, 1, 5), new DateTime(2020, 1, 3), "EUR");
			Add(1.3072M, new DateTime(2020, 1, 5), new DateTime(2020, 1, 3), "USD");

			Add(1.8978M, new DateTime(2020, 1, 6), new DateTime(2020, 1, 6), "AUD");
			Add(1.7072M, new DateTime(2020, 1, 6), new DateTime(2020, 1, 6), "CAD");
			Add(1.1761M, new DateTime(2020, 1, 6), new DateTime(2020, 1, 6), "EUR");
			Add(1.3159M, new DateTime(2020, 1, 6), new DateTime(2020, 1, 6), "USD");
		}
	}
}
