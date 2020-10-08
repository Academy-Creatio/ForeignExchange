using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;

namespace ForeignExchange
{
	class BankOfAustralia : IBank
	{
		#region Constants
		private const string csvCurrent = @"https://www.rba.gov.au/statistics/tables/csv/f11.1-data.csv";
		private const string homeCurrency = "AUD";
		private const string bankName = "Reserve Bank of Australia";
		#endregion

		#region Fields
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
		private DataTable Dt { get; }
		private bool parsed = false;
		public Dictionary<string, string> SupportedCurrencies => new Dictionary<string, string>
		{
			{"USD", "US dollar"},
			{"CNY", "Chinese renminbi"},
			{"JPY", "Japanese yen"},
			{"EUR", "European euro"},
			{"KRW", "South Korean won"},
			{"GBP", "UK pound sterling"},
			{"SGD", "Singapore dollar"},
			{"INR", "Indian rupee"},
			{"THB", "Thai baht"},
			{"NZD", "New Zealand dollar"},
			{"TWD", "Taiwanese dollar"},
			{"MYR", "Malaysian ringgit"},
			{"IDR", "Indonesian rupiah"},
			{"VND", "Vietnamese dong"},
			{"AED", "United Arab Emirates Dirham"},
			{"PGK", "Papua New Guinean Kina"},
			{"HKD", "Hong Kong dollar"},
			{"CAD", "Canadian dollar"},
			{"CHF", "Swiss franc"},
			{"SDR", "Unit of account of the IMF"}
		};
		#endregion

		#region Constructor
		public BankOfAustralia()
		{
			Dt = new DataTable("ExchageRates");
		}
		#endregion

		#region Methods
		public Task<IBankResult> GetRateAsync(string currency, DateTime date)
		{
			if (date < minDate)
			{
				throw new ArgumentOutOfRangeException("date", $"Date must be greater than {minDate:dd-MMM-yyyy}");
			}

			if (!SupportedCurrencies.ContainsKey(currency))
			{
				throw new NotImplementedException($"{bankName} does not support {currency}");
			}

			DownloadCsvFile();
			
			while (!parsed) {  }
			var fxRate = decimal.Zero;
			int d = 0;
			DataView dv = new DataView(Dt);
			dv.Sort = "Date";
			while (fxRate == decimal.Zero)
			{
				int rowIndex = dv.Find(date.AddDays(-d).Date);
				if(rowIndex > 0)
				{
					fxRate = (decimal)dv[rowIndex][currency];
				}
				else { 
					d++;
				}
				
			}
			bankResult.ExchangeRate = decimal.Round(1 / fxRate, 4);
			bankResult.RateDate = date.AddDays(-d);
			return Task.FromResult(bankResult);
		}
		private void DownloadCsvFile()
		{
			var client = new WebClient();
			client.DownloadDataCompleted += Client_DownloadDataCompleted;
			client.DownloadDataAsync(new Uri(csvCurrent));
		}
		private void Client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
		{
			string csv = Encoding.Default.GetString(e.Result);
			char[] separator = new[] { '\r', '\n' };
			string[] lines = csv.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in lines)
			{
				List<string> cells = line.Split(new[] {','}).ToList();
				if(cells.Count() != 0)
				{
					if (cells[0] == "Units")
					{
						DataColumn dateColumn = new DataColumn()
						{
							ColumnName = "Date",
							Caption = "Date",
							DataType = typeof(DateTime),
							Unique = true
						};
						Dt.Columns.Add(dateColumn);
						Dt.PrimaryKey = new DataColumn[]{ dateColumn};
						
						for(int i = 1; i < cells.Count(); i++)
						{
							if (!string.IsNullOrEmpty(cells[i]))
							{
								DataColumn dc = new DataColumn()
								{
									ColumnName = cells[i],
									Caption = cells[i],
									DataType = typeof(decimal),
									AllowDBNull = true
								};
								Dt.Columns.Add(dc);
							}
						}
					}
					if(DateTime.TryParseExact(cells[0], "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
					{
						DataRow row = Dt.NewRow();
						row["Date"] = date;

						for (int i = 1; i < cells.Count(); i++)
						{
							if (!string.IsNullOrEmpty(cells[i]))
							{
								if (decimal.TryParse(cells[i], out decimal rate))
								{
									row[i] = rate;
								}
								else
								{
									row[i] = DBNull.Value;
								}
							}
						};
						Dt.Rows.Add(row);
					}
				}
			}
			parsed = true;
		}
		#endregion
	}
}
