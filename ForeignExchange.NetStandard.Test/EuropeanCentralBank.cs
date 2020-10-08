using System;
using System.Threading.Tasks;
using Xunit;

namespace ForeignExchange.NetStandard.Test
{
	public class EuropeanCentralBank
	{
		[Theory(DisplayName ="Should_Return"), ClassData(typeof(ECB_TestData_ShouldRetun))]
		public async Task ECB_GetRateAsyncShoudReturn(decimal expectedExchangeRate, 
			DateTime requestedRateDate, DateTime expectedRateDate, string currency)
		{
			#region Arrange
			IBankResult expected = new BankResult()
			{
				BankName = "European Central Bank",
				HomeCurrency = "EUR",
				ExchangeRate = expectedExchangeRate,
				RateDate = expectedRateDate
			};
			#endregion

			#region Act
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.ECB);
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
		public async Task ECB_ShoudThrowCurrency()
		{
			#region Arrange
			DateTime date = new DateTime(2020, 1, 8);
			string expectedMessage = $"European Central Bank does not support XXX";
			#endregion

			#region Act
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.ECB);
			#endregion

			#region Assert
			var exception = Record.ExceptionAsync(
				async () => await ibank.GetRateAsync("XXX", date));
			exception.Wait();

			Assert.Equal(exception.Result.Message, expectedMessage);
			Assert.IsType<NotImplementedException>(exception.Result);
			await Assert.ThrowsAsync<NotImplementedException>(
				async () => await ibank.GetRateAsync("XXX", date));
			#endregion
		}

		[Fact]
		public async Task ECB_ShoudThrowDate()
		{
			#region Arrange
			DateTime date = new DateTime(1999, 1, 3);
			string expectedMessage = @"Date must be greater than 04-Jan-1999 (Parameter 'date')";
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.ECB);
			#endregion

			#region Act
			var exception = Record.ExceptionAsync(
				async () => await ibank.GetRateAsync("USD", date));
			exception.Wait();
			#endregion

			#region Assert
			Assert.Equal(exception.Result.Message, expectedMessage);
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
	public class ECB_TestData_ShouldRetun : TheoryData<decimal, DateTime, DateTime, string>
	{
		public ECB_TestData_ShouldRetun()
		{
			Add(decimal.Round(1/1.0843M,4), new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "USD");
			Add(decimal.Round(1/115.34M,4), new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "JPY");
			Add(decimal.Round(1/27.2510M, 4), new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "CZK");
			Add(decimal.Round(1/10.5875M,4), new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "SEK");
			Add(decimal.Round(1/16229.26M,4), new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "IDR");
			
			Add(decimal.Round(1/1.0843M,4), new DateTime(2020, 5, 9), new DateTime(2020, 5, 8), "USD");
			Add(decimal.Round(1/115.34M,4), new DateTime(2020, 5, 9), new DateTime(2020, 5, 8), "JPY");
			Add(decimal.Round(1/27.2510M, 4), new DateTime(2020, 5, 9), new DateTime(2020, 5, 8), "CZK");
			Add(decimal.Round(1/10.5875M,4), new DateTime(2020, 5, 9), new DateTime(2020, 5, 8), "SEK");
			Add(decimal.Round(1/16229.26M,4), new DateTime(2020, 5, 9), new DateTime(2020, 5, 8), "IDR");
			
			Add(decimal.Round(1/1.0843M,4), new DateTime(2020, 5, 10), new DateTime(2020, 5, 8), "USD");
			Add(decimal.Round(1/115.34M,4), new DateTime(2020, 5, 10), new DateTime(2020, 5, 8), "JPY");
			Add(decimal.Round(1/27.2510M, 4), new DateTime(2020, 5, 10), new DateTime(2020, 5, 8), "CZK");
			Add(decimal.Round(1/10.5875M,4), new DateTime(2020, 5, 10), new DateTime(2020, 5, 8), "SEK");
			Add(decimal.Round(1/16229.26M,4), new DateTime(2020, 5, 10), new DateTime(2020, 5, 8), "IDR");

		}
	}
}
