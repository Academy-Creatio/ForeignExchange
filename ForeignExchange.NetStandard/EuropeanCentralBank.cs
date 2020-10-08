using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace ForeignExchange
{
	public sealed class EuropeanCentralBank : IBank
	{
		#region Constants
		private const string baseUrl = @"https://sdw-wsrest.ecb.europa.eu/service";
		private const string resource = "data";
		private const string flowRef = "EXR";
		private const string homeCurrency = "EUR";
		private const string bankName = "European Central Bank";
		#endregion

		#region Fields
		private static readonly DateTime minDate = new DateTime(1999, 1, 4);
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
			{"USD", "US dollar"},
			{"JPY", "Japanese yen"},
			{"BGN", "Bulgarian lev"},
			{"CZK", "Czech koruna"},
			{"DKK", "Danish krone"},
			{"GBP", "Pound sterling"},
			{"HUF", "Hungarian forint"},
			{"PLN", "Polish zloty"},
			{"RON", "Romanian leu"},
			{"SEK", "Swedish krona"},
			{"CHF", "Swiss franc"},
			{"ISK", "Icelandic krona"},
			{"NOK", "Norwegian krone"},
			{"HRK", "Croatian kuna"},
			{"RUB", "Russian rouble"},
			{"TRY", "Turkish lira"},
			{"AUD", "Australian dollar"},
			{"BRL", "Brazilian real"},
			{"CAD", "Canadian dollar"},
			{"CNY", "Chinese yuan renminbi"},
			{"HKD", "Hong Kong dollar"},
			{"IDR", "Indonesian rupiah"},
			{"ILS", "Israeli shekel"},
			{"INR", "Indian rupee"},
			{"KRW", "South Korean won"},
			{"MXN", "Mexican peso"},
			{"MYR", "Malaysian ringgit"},
			{"NZD", "New Zealand dollar"},
			{"PHP", "Philippine peso"},
			{"SGD", "Singapore dollar"},
			{"THB", "Thai baht"},
			{"ZAR", "South African rand"}
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

			//https://sdw-wsrest.ecb.europa.eu/service/data/EXR/D.USD.EUR.SP00.A?startPeriod=2019-11-01&endPeriod=2019-11-04
			string startDate = date.ToString("yyyy-MM-dd");
			Uri methodUri = new Uri($"{baseUrl}/{resource}/{flowRef}/D.{currency}.EUR.SP00.A?startPeriod={startDate}&endPeriod={startDate}");
			using (WebClient client = new WebClient())
			{
				try
				{
					decimal result = 0;
					string xml = await client.DownloadStringTaskAsync(methodUri);

					if (!string.IsNullOrEmpty(xml))
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(xml);
						XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
						namespaceManager.AddNamespace("message", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message");
						namespaceManager.AddNamespace("generic", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/data/generic");
						namespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
						namespaceManager.AddNamespace("common", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/common");
						string xmlPath = $"/message:GenericData/message:DataSet/generic:Series/generic:Obs/generic:ObsValue";
						string rate = xmlDocument.SelectSingleNode(xmlPath, namespaceManager).Attributes[0].Value;
						decimal.TryParse(rate.ToString(), out result);
						bankResult.ExchangeRate = decimal.Round(1/result, 4);
						bankResult.RateDate = date;
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
				return bankResult;
			}
		}
		#endregion
	}
}