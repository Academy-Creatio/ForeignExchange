using System;
using System.Threading.Tasks;
using Xunit;

namespace ForeignExchange.NetStandard.Test
{
	public class CentralBankOfRussia
	{
		[Theory(DisplayName ="Should_Return"), ClassData(typeof(CBR_TestData_ShouldRetun))]
		public async Task CBR_GetRateAsyncShoudReturn(decimal expectedExchangeRate, 
			DateTime requestedRateDate, DateTime expectedRateDate, string currency)
		{
			#region Arrange
			IBankResult expected = new BankResult()
			{
				BankName = "Central bank of the Russian Federation",
				HomeCurrency = "RUB",
				ExchangeRate = expectedExchangeRate,
				RateDate = expectedRateDate
			};
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.CBR);
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
		public async Task CBR_ShoudThrowCurrency()
		{
			#region Arrange
			DateTime date = new DateTime(2020, 1, 8);
			string expectedMessage = $"Central bank of the Russian Federation does not support XXX";
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.CBR);
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
		public async Task CBR_ShoudThrowDate()
		{
			#region Arrange
			DateTime date = new DateTime(1992, 6, 1);
			string expectedMessage = @"Date must be greater than 01-Jul-1992 (Parameter 'date')";
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
	public class CBR_TestData_ShouldRetun : TheoryData<decimal, DateTime, DateTime, string>
	{
		public CBR_TestData_ShouldRetun()
		{
			Add(73.8725M, new DateTime(2020, 5, 10), new DateTime(2020, 5, 10), "USD");
			Add(73.8725M, new DateTime(2020, 5, 9), new DateTime(2020, 5, 9), "USD");
			Add(74.1169M, new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "USD");
			
			Add(80.0039M, new DateTime(2020, 5, 10), new DateTime(2020, 5, 10), "EUR");
			Add(80.0039M, new DateTime(2020, 5, 9), new DateTime(2020, 5, 9), "EUR");
			Add(80.0611M, new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "EUR");
			
			Add(0.694813M, new DateTime(2020, 5, 10), new DateTime(2020, 5, 10), "JPY");
			Add(0.694813M, new DateTime(2020, 5, 9), new DateTime(2020, 5, 9), "JPY");
			Add(0.696587M, new DateTime(2020, 5, 8), new DateTime(2020, 5, 8), "JPY");
			

		}
	}
}
