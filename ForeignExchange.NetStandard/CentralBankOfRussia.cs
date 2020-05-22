using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.Threading.Tasks;
using ForeignExchange.CbrDailyInfo;

namespace ForeignExchange
{
    public sealed class CentralBankOfRussia : IBank
    {
        #region Constants
        private const string homeCurrency = "RUB";
        private const string bankName = "Central bank of the Russian Federation";
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
                throw new NotImplementedException($"{bankName} does not support {currency}");

            IBankResult bankResult = new BankResult()
            {
                ExchangeRate = -1m,
                RateDate = date,
                HomeCurrency = homeCurrency,
                BankName = bankName
            };

            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endPointAddress = new EndpointAddress("http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx");
            DailyInfoSoap client = new DailyInfoSoapClient(binding, endPointAddress);

            //ArrayOfXElement fxRates = await client.GetCursOnDateAsync(date);
            Task<DataSet> fxRates = client.GetCursOnDateAsync(date);
            fxRates.Wait();
            
            //var list = fxRates.Nodes.Find(n => n.Name.LocalName == "diffgram");

            DataTable table = fxRates.Result.Tables["ValuteCursOnDate"];


            string filterExpression = $"VchCode = '{currency}'";
            if (table == null || table?.Rows.Count == 0)
                return bankResult;
            DataRow[] rows = table.Select(filterExpression);

            decimal.TryParse(rows[0]["Vnom"].ToString(), out decimal multiplier);
            decimal.TryParse(rows[0]["Vcurs"].ToString(), out decimal rate);
            decimal fxRate = rate / multiplier;
            bankResult.ExchangeRate = fxRate;

            return bankResult;
        }
        #endregion
    }
}