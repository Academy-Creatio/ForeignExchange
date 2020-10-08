using System;
using System.Threading.Tasks;
using Xunit;

namespace ForeignExchange.NetStandard.Test
{
	public class BankOfCanada
	{
		[Theory(DisplayName ="Should_Return"), ClassData(typeof(BOC_TestData_ShouldRetun))]
		public async Task BOC_GetRateAsyncShoudReturn(decimal expectedExchangeRate, 
			DateTime requestedRateDate, DateTime expectedRateDate, string currency)
		{
			#region Arrange
			IBankResult expected = new BankResult()
			{
				BankName = "Bank of Canada",
				HomeCurrency = "CAD",
				ExchangeRate = expectedExchangeRate,
				RateDate = expectedRateDate
			};
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOC);
			#endregion

			#region Act
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
		public async Task BOC_ShoudThrowCurrency()
		{
			#region Arrange
			DateTime date = new DateTime(2020, 1, 8);
			string expectedMessage = $"Bank of Canada does not support XXX";
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOC);
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
		public async Task BOC_ShoudThrowDate()
		{
			#region Arrange
			DateTime date = new DateTime(2017, 1, 2);
			string expectedMessage = @"Date must be greater than 03-Jan-2017 (Parameter 'date')";
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOC);
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
	public class BOC_TestData_ShouldRetun : TheoryData<decimal, DateTime, DateTime, string>
	{
		public BOC_TestData_ShouldRetun()
		{
			Add(1.3934M, new DateTime(2020, 5, 10), new DateTime(2020, 5, 8), "USD");
			Add(1.3934M, new DateTime(2020, 5, 9), new DateTime(2020, 5, 8), "USD");
			Add(1.3934M, new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "USD");
			
			Add(1.5109M, new DateTime(2020, 5, 10), new DateTime(2020, 5, 8), "EUR");
			Add(1.5109M, new DateTime(2020, 5, 9), new DateTime(2020, 5, 8), "EUR");
			Add(1.5109M, new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "EUR");
			
			Add(0.01307M, new DateTime(2020, 5, 10), new DateTime(2020, 5, 8), "JPY");
			Add(0.01307M, new DateTime(2020, 5, 9), new DateTime(2020, 5, 8), "JPY");
			Add(0.01307M, new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "JPY");
			

		}
		[Fact]
		public void BOC_ShoudThrowDate()
		{
			#region Arrange
			DateTime date = new DateTime(2017, 1, 2);
			string expectedMessage = @"Date must be greater than 03-Jan-2017 (Parameter 'date')";
			#endregion

			#region Act
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOC);
			#endregion

			#region Assert
			var exception = Record.ExceptionAsync(
				async () => await ibank.GetRateAsync("USD", date));
			exception.Wait();

			Assert.IsType<ArgumentOutOfRangeException>(exception.Result);
			Assert.Equal("date", ((ArgumentException)exception.Result).ParamName);
			Assert.Equal(exception.Result.Message, expectedMessage);
			#endregion
		}
	}
}
