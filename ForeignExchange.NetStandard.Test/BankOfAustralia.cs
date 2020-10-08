using System;
using System.Threading.Tasks;
using Xunit;

namespace ForeignExchange.NetStandard.Test
{
	public class BankOfAustralia
	{
		[Fact]
		public async Task RBA_GetRateAsyncShoudReturn()
		{
			DateTime date = new DateTime(2018, 1, 17);

			#region Arrange
			IBankResult expected = new BankResult()
			{
				BankName = "Reserve Bank of Australia",
				HomeCurrency = "AUD",
				ExchangeRate = 1.2571M,
				RateDate = date

			};
			#endregion

			#region Act
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.RBA);
			IBankResult actual = await ibank.GetRateAsync("USD", date);
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
		public async Task RBA_GetRateAsyncShoudReturnWithOffSetDate()
		{
			DateTime date = new DateTime(2018, 2, 8);

			#region Arrange
			IBankResult expected = new BankResult()
			{
				BankName = "Reserve Bank of Australia",
				HomeCurrency = "AUD",
				ExchangeRate = 0.0012M,
				RateDate = date
			};
			#endregion

			#region Act
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.RBA);
			IBankResult actual = await ibank.GetRateAsync("KRW", date);
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
		public async Task RBA_ShoudThrowDate()
		{
			#region Arrange
			DateTime date = new DateTime(2017, 1, 2);
			string expectedMessage = @"Date must be greater than 03-Jan-2017 (Parameter 'date')";
			#endregion

			#region Act
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.RBA);
			#endregion

			#region Assert
			var exception = Record.ExceptionAsync(
				async () => await ibank.GetRateAsync("KRW", date));
			exception.Wait();

			Assert.Equal(expectedMessage, exception.Result.Message);
			Assert.IsType<ArgumentOutOfRangeException>(exception.Result);
			await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
				async () => await ibank.GetRateAsync("KRW", date));
			#endregion
		}

		[Fact]
		public async Task RBA_ShoudThrowCurrency()
		{
			#region Arrange
			DateTime date = new DateTime(2020, 1, 8);
			string expectedMessage = $"Reserve Bank of Australia does not support XXX";
			#endregion

			#region Act
			IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.RBA);
			#endregion

			#region Assert
			var exception = Record.ExceptionAsync(
				async () => await ibank.GetRateAsync("XXX", date));
			exception.Wait();

			Assert.Equal(expectedMessage, exception.Result.Message);
			Assert.IsType<NotImplementedException>(exception.Result);
			await Assert.ThrowsAsync<NotImplementedException>(
				async () => await ibank.GetRateAsync("XXX", date));
			#endregion
		}
	}
}
