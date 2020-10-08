using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using ForeignExchange.CbrDailyInfo;

namespace ForeignExchange
{
	public sealed class CentralBankOfRussia : IBank
	{
		#region Constants
		private const string homeCurrency = "RUB";
		private const string bankName = "Central bank of the Russian Federation";

		#endregion

		#region Fields
		private static readonly DateTime minDate = new DateTime(1992, 7, 1);
		private readonly static IBankResult bankResult = new BankResult()
		{
			ExchangeRate = -1m,
			RateDate = DateTime.Now,
			HomeCurrency = homeCurrency,
			BankName = bankName
		};
		#endregion

		#region Properties
		public Dictionary<string, string> SupportedCurrencies => new Dictionary<string, string>
		{
			{"AUD", "Australian dollar"},
			{"AZN", "Azerbaijan Manat"},
			{"AMD", "Armenia Dram"},
			{"BYN", "Belarussian Ruble"},
			{"BGN", "Bulgarian lev"},
			{"BRL", "Brazil Real"},
			{"HUF", "Hungarian forint"},
			{"KRW", "South Korean won"},
			{"HKD", "Hong Kong dollar"},
			{"DKK", "Danish Krone"},
			{"USD", "US dollar"},
			{"EUR", "Euro"},
			{"INR", "Indian rupee"},
			{"KZT", "Kazakhstan Tenge"},
			{"CAD", "Canadian dollar"},
			{"KGS", "Kyrgyzstan Som"},
			{"CNY", "Chinese yuan renminbi"},
			{"MDL", "Moldova Lei"},
			{"TMT", "New Turkmenistan Manat"},
			{"NOK", "Norwegian krone"},
			{"PLN", "Polish zloty"},
			{"RON", "Romanian leu"},
			{"XDR", "SDR"},
			{"SGD", "Singapore dollar"},
			{"TJS", "Tajikistan Ruble"},
			{"TRY", "Turkish lira"},
			{"UZS", "Uzbekistan Sum"},
			{"UAH", "Ukrainian Hryvnia"},
			{"GBP", "Pound sterling"},
			{"CZK", "Czech koruna"},
			{"SEK", "Swedish krona"},
			{"CHF", "Swiss franc"},
			{"ZAR", "South African rand"},
			{"JPY", "Japanese yen"}
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
			BasicHttpBinding binding = new BasicHttpBinding();
			EndpointAddress endPointAddress = new EndpointAddress("http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx");
			DailyInfoSoap client = new DailyInfoSoapClient(binding, endPointAddress);

			ArrayOfXElement fxRates = await client.GetCursOnDateAsync(date);
			var valuesNode = fxRates.Nodes
				.Where(n => n.Name.LocalName == "diffgram").FirstOrDefault().Elements()
				.Where(e => e.Name == "ValuteData").Elements("ValuteCursOnDate")
				.Where(ee => ee.Elements("VchCode").FirstOrDefault().Value.Trim() == currency).FirstOrDefault();

			if (!valuesNode.IsEmpty)
			{
				string Vnom = valuesNode.Elements("Vnom").FirstOrDefault().Value.Trim();
				string Vcurs = valuesNode.Elements("Vcurs").FirstOrDefault().Value.Trim();
				string VchCode = valuesNode.Elements("VchCode").FirstOrDefault().Value.Trim();

				decimal.TryParse(Vnom, out decimal multiplier);
				decimal.TryParse(Vcurs, out decimal rate);
				decimal resultFxRate = rate / (multiplier == 0 ? 1: multiplier);

				bankResult.RateDate = date;
				bankResult.ExchangeRate = resultFxRate;
			}
			return bankResult;
		}
		#endregion
	}
}