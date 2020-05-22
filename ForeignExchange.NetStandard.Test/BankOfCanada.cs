using System;
using Xunit;
using ForeignExchange;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;

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
            #endregion

            #region Act
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOC);
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
            #endregion

            #region Act
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOC);
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
            
            Add(-1M, new DateTime(1900, 1, 1), new DateTime(1900, 1, 1), "USD");

        }
    }
   



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
            DateTime date = new DateTime(2017, 1, 8);
            string expectedMessage = @"Date must be greater than 01-Jan-2018";
            #endregion

            #region Act
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.RBA);
            #endregion

            #region Assert
            var exception = Record.ExceptionAsync(
                async () => await ibank.GetRateAsync("KRW", date));
            exception.Wait();

            Assert.Equal(exception.Result.Message, expectedMessage);
            Assert.IsType<NotImplementedException>(exception.Result);
            await Assert.ThrowsAsync<NotImplementedException>(
                async()=>await ibank.GetRateAsync("KRW", date));
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

            Assert.Equal(exception.Result.Message, expectedMessage);
            Assert.IsType<NotImplementedException>(exception.Result);
            await Assert.ThrowsAsync<NotImplementedException>(
                async () => await ibank.GetRateAsync("XXX", date));
            #endregion
        }
    }
}
