using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ForeignExchange
{
	public sealed class BankOfCanada : IBank
	{
		#region Constants
		private const string baseUrl = @"https://www.bankofcanada.ca/valet";
		private const string homeCurrency = "CAD";
		private const string bankName = "Bank of Canada";
		private static readonly DateTime minDate = new DateTime(2017, 1, 3);
		private static readonly IBankResult bankResult = new BankResult()
		{
			ExchangeRate = -1m,
			RateDate = minDate,
			HomeCurrency = homeCurrency,
			BankName = bankName
		};
		#endregion

		#region Properties
		public Dictionary<string, string> SupportedCurrencies => new Dictionary<string, string>
		{
			{"AUD", "Australian dollar"},
			{"BRL", "Brazilian real"},
			{"CNY", "Chinese renminbi"},
			{"EUR", "European euro"},
			{"HKD", "Hong Kong dollar"},
			{"INR", "Indian rupee"},
			{"IDR", "Indonesian rupiah"},
			{"JPY", "Japanese yen"},
			{"MYR", "Malaysian ringgit"},
			{"MXN", "Mexican peso"},
			{"NZD", "New Zealand dollar"},
			{"NOK", "Norwegian krone"},
			{"PEN", "Peruvian new sol"},
			{"RUB", "Russian ruble"},
			{"SAR", "Saudi riyal"},
			{"SGD", "Singapore dollar"},
			{"ZAR", "South African rand"},
			{"KRW", "South Korean won"},
			{"SEK", "Swedish krona"},
			{"CHF", "Swiss franc"},
			{"TWD", "Taiwanese dollar"},
			{"THB", "Thai baht"},
			{"TRY", "Turkish lira"},
			{"GBP", "UK pound sterling"},
			{"USD", "US dollar"},
			{"VND", "Vietnamese dong"}
		};
		#endregion

		#region Methods
		public async Task<IBankResult> GetRateAsync(string currency, DateTime date)
		{
			if (!SupportedCurrencies.ContainsKey(currency)) 
			{
				throw new NotImplementedException($"{bankName} does not support {currency}");
			}

			if (date < minDate) 
			{
				throw new ArgumentOutOfRangeException("date", $"Date must be greater than {minDate:dd-MMM-yyyy}");
			}
			
			//https://www.bankofcanada.ca/valet/observations/FXUSDCAD?start_date=2019-11-05&end_date=2019-11-05
			string startDate = date.ToString("yyyy-MM-dd");
			Uri methodUri = new Uri($"{baseUrl}/observations/FX{currency}CAD?start_date={startDate}&end_date={startDate}");
			using (WebClient client = new WebClient())
			{
				try
				{
					decimal result = 0;
					string json;
					json = await client.DownloadStringTaskAsync(methodUri);
					JObject jo = JsonConvert.DeserializeObject<JObject>(json);
					JArray ja = (JArray)jo.SelectToken("$.observations");
					if (ja.Count != 0)
					{
						string v = ja.SelectToken($"[0].FX{currency}CAD.v").ToString();
						decimal.TryParse(v, out result);

						bankResult.RateDate = date;
						bankResult.ExchangeRate = result;
						return bankResult;
					}
					else
					{
						return await GetRateAsync(currency, date.AddDays(-1));
					}
				}
				catch
				{
					return bankResult;
				}
			}
		}
		#endregion
	}
}