using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ForeignExchange
{
    public sealed class BankOfEngland : IBank
    {
        #region Constants
        private const string baseUrl = @"https://www.bankofengland.co.uk/boeapps/database/Rates.asp?";
        private const string homeCurrency = "GBP";
        private const string bankName = "Bank of England";
        private readonly DateTime earliestDate = new DateTime(2017, 1, 3);
        #endregion

        #region Properties
        public Dictionary<string, string> SupportedCurrencies => new Dictionary<string, string>
        {
            {"AUD","Australian Dollar"},
            {"CAD","Canadian Dollar"},
            {"CNY","Chinese Yuan"},
            {"CZK","Czech Koruna"},
            {"DKK","Danish Krone"},
            {"EUR","Euro"},
            {"HKD","Hong Kong Dollar"},
            {"HUF","Hungarian Forint"},
            {"INR","Indian Rupee"},
            {"ILS","Israeli Shekel"},
            {"JPY","Japanese Yen"},
            {"MYR","Malaysian ringgit"},
            {"NZD","Zealand Dollar"},
            {"NOK","Norwegian Krone"},
            {"PLN","Polish Zloty"},
            {"RUB","Russian Ruble"},
            {"SAR","Saudi Riyal"},
            {"SGD","Singapore Dollar"},
            {"ZAR","South African Rand"},
            {"KRW","South Korean Won"},
            {"SEK","Swedish Krona"},
            {"CHF","Swiss Franc"},
            {"TWD","Taiwan Dollar"},
            {"THB","Thai Baht"},
            {"TRY","Turkish Lira"},
            {"USD","US Dollar"}
        };
        #endregion

        #region Methods
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
            if (date < earliestDate) return bankResult;

            //TD=11&TM=May&TY=2020&into=GBP&rateview=D
            
            string D = date.ToString("dd", CultureInfo.InvariantCulture);
            string M = date.ToString("MMM", CultureInfo.InvariantCulture);
            string Y = date.ToString("yyyy", CultureInfo.InvariantCulture);

            
            Uri methodUri = new Uri($"{baseUrl}TD={D}&TM={M}&TY={Y}&into=GBP&rateview=D");
            using (WebClient client = new WebClient())
            {
                try
                {
                    string htmlContent = client.DownloadString(methodUri);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlContent);

                    
                    var error = doc.GetElementbyId("editorial").ChildNodes.Where(x => 
                        x.Name == "p"
                        && x.Attributes.Where(a=> a.Value=="error").Count() >0
                    );


                    if (error.Count() != 0)
                    {
                        return await GetRateAsync(currency, date.AddDays(-1));
                    }


                    var header = doc.GetElementbyId("editorial").ChildNodes
                        .Where(x => x.Name == "table").FirstOrDefault().ChildNodes
                        .Where(x => x.Name == "thead").FirstOrDefault().ChildNodes
                        .Where(x => x.Name == "tr").FirstOrDefault().ChildNodes
                        .Where(x => x.Name == "th");
                    
                    List<string> caption = new List<string>();
                    foreach(var item in header)
                    {
                        if(item.ChildNodes.Count == 1)
                        {
                            caption.Add(item.ChildNodes.FirstOrDefault().InnerText);
                        }

                        if (item.ChildNodes.Count == 3)
                        {
                            string val = item.ChildNodes.Where(
                                x => x.Name == "#text" && !x.InnerText.Contains(";") && !x.InnerText.Contains("/")
                                ).FirstOrDefault().InnerText.Trim();
                            caption.Add(val);
                        }
                    }

                    var rows = doc.GetElementbyId("editorial").ChildNodes
                        .Where(x => x.Name == "table").FirstOrDefault().ChildNodes
                        .Where(x => x.Name == "tr");

                    foreach(var row in rows)
                    {
                        object[] r = new object[caption.Count];
                        int i = 0;
                        foreach(var cell in row.ChildNodes.Where(x => x.Name == "td"))
                        {
                            string cellValue = cell.InnerText.Trim();
                            if(SupportedCurrencies.Where(v => v.Value == cellValue).Count() == 1)
                            {
                                string key = SupportedCurrencies.Where(v => v.Value == cellValue).FirstOrDefault().Key;
                                r[i] = key;
                            }

                            if(decimal.TryParse(cell.InnerText, out decimal rate))
                            {
                                r[i] = rate;
                            }
                            i++;
                        }

                        if (r[0]?.ToString() == currency)
                        {
                            bankResult.ExchangeRate = (decimal)r[1];
                            if(DateTime.TryParseExact(caption[1].Trim(), "dd MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime rateDate))
                            {
                                bankResult.RateDate = rateDate;
                            }
                            return bankResult;
                        }
                    }

                    return bankResult;
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


/***
 * 
 * //https://www.bankofengland.co.uk/boeapps/database/fromshowcolumns.asp?Travel=NIxRSxSUx&FromSeries=1&ToSeries=50&DAT=RNG&
            //FD=1&FM=May&FY=2019&TD=1&TM=May&TY=2020&FNY=&CSVF=TT&html.x=1&html.y=24&C=ECL
            string D = date.ToString("dd", CultureInfo.InvariantCulture);
            string M = date.ToString("MMM", CultureInfo.InvariantCulture);
            string Y = date.ToString("yyyy", CultureInfo.InvariantCulture);

            
            Uri methodUri = new Uri($"{baseUrl}FD={D}&FM={M}&FY={Y}&TD={D}&TM={M}&TY={Y}&FNY=&CSVF=TT&html.x=1&html.y=24&C=ECL");
 * 
 * 
                     var cells = statsTable.ChildNodes.Where(x => x.Name == "tbody")
                        .FirstOrDefault().ChildNodes.Where(x => x.Name == "tr")
                        .FirstOrDefault().ChildNodes.Where(x => x.Name == "td");

                    foreach (var cell in cells)
                    {
                        if (DateTime.TryParseExact(cell.InnerHtml, "dd MMM yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateResult))
                        {
                            bankResult.RateDate = dateResult;
                        }

                        if (decimal.TryParse(cell.InnerHtml, out decimal rateResult))
                        {
                            bankResult.ExchangeRate = rateResult;
                        }
                    }
                    return bankResult;
     
     */
