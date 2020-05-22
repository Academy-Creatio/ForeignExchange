using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace ForeignExchange
{
    public sealed class BankOfMexico : IBank
    {
        #region Constants
        private const string baseUrl = @"https://www.banxico.org.mx/SieAPIRest/service/v1/series";
        private const string homeCurrency = "MXN";
        private const string bankName = "Bank of Mexico";
        private readonly string bmxToken;
        #endregion

        #region Properties
        public Dictionary<string, string> SupportedCurrencies => new Dictionary<string, string>
        {
            {"CAD", "Canadian dollar"},
            {"EUR", "European euro"},
            {"JPY", "Japanese yen"},
            {"GBP", "UK pound sterling"},
            {"USD", "US dollar"},
            {"CNY", "Chinese yuan"},
        };
        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="secret">bmxToken from https://www.banxico.org.mx/SieAPIRest/service/v1/token </param>
        public BankOfMexico(string secret) 
        {
            bmxToken = secret;
        }
        public async Task<IBankResult> GetRateAsync(string currency, DateTime date)
        {
            if (!SupportedCurrencies.ContainsKey(currency))
                throw new NotImplementedException($"{bankName} does not support {currency}");

            IBankResult bankResult = new BankResult()
            {
                ExchangeRate = -1m,
                RateDate = date,
                HomeCurrency = homeCurrency,
                BankName = bankName
            };

            //https://www.banxico.org.mx/SieAPIRest/service/v1/series/SF60632/datos/2020-03-01/2020-04-08?locale=es
            string startDate = date.ToString("yyyy-MM-dd");
            Uri methodUri = new Uri($"{baseUrl}/{GetSeriesName(currency)}/datos/{startDate}/{startDate}?locale=en");

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Bmx-Token", bmxToken);
                try
                {
                    //decimal result = 0;
                    string json = "";
                    json = await client.DownloadStringTaskAsync(methodUri);
                    JObject jo = JsonConvert.DeserializeObject<JObject>(json);
                    
                    JObject series = (JObject)jo.SelectToken("$.bmx.series[0]");
                    if (series.Property("datos") == null) {
                        return await GetRateAsync(currency, date.AddDays(-1));
                    }

                    string rate = jo.SelectToken("$.bmx.series[0].datos[0].dato").ToString();
                    string rateDatestr = jo.SelectToken("$.bmx.series[0].datos[0].fecha").ToString();
                    if (!decimal.TryParse(rate, out decimal result)) 
                    {
                        return await GetRateAsync(currency, date.AddDays(-1));
                    }

                    bankResult.ExchangeRate = result;
                    DateTime.TryParseExact(rateDatestr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime rateDate);
                    
                    bankResult.RateDate = rateDate;
                    return bankResult;
                }
                catch (JsonException ex)
                {
                    Console.WriteLine(ex.Message);
                    return bankResult;
                }
            }
        }
        private string GetSeriesName(string CurrencyCode) {

            switch (CurrencyCode)
            {
                case "USD" : return "SF46405";
                case "EUR":return "SF46410";
                case "CAD": return "SF60632";
                case "JPY": return "SF46406";
                case "GBP": return "SF46407";
                case "CNY":return "SF290383";
                default: return "MXN";
            }
        }
        #endregion
    }
}