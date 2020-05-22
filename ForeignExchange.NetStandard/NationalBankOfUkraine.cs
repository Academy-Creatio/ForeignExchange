using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Globalization;
namespace ForeignExchange
{
    public sealed class NationalBankOfUkraine : IBank
    {
        #region Constants
        private const string baseUrl = @"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?valcode=";
        private const string homeCurrency = "UAH";
        private const string bankName = "National Bank of Ukraine";
        private readonly DateTime minDate = new DateTime(1996, 1, 6);
        #endregion

        #region Properties
        public string HomeCurrency => homeCurrency;
        public Dictionary<string, string> SupportedCurrencies => new Dictionary<string, string>
        {
            {"AUD","(036) - Австралійський долар"},
            {"CAD","(124) - Канадський долар"},
            {"CNY","(156) - Юань Женьмiньбi"},
            {"HRK","(191) - Куна"},
            {"CZK","(203) - Чеська крона"},
            {"DKK","(208) - Данська крона"},
            {"HKD","(344) - Гонконгівський долар"},
            {"HUF","(348) - Форинт"},
            {"INR","(356) - Індійська рупія"},
            {"IDR","(360) - Рупія"},
            {"IRR","(364) - Іранський ріал"},
            {"ILS","(376) - Новий ізраїльський шекель"},
            {"JPY","(392) - Єна"},
            {"KZT","(398) - Теньге"},
            {"KRW","(410) - Вона"},
            {"MXN","(484) - Мексіканський песо"},
            {"MDL","(498) - Молдовський лей"},
            {"NZD","(554) - Новозеландський долар"},
            {"NOK","(578) - Норвезька крона"},
            {"RUB","(643) - Російський рубль"},
            {"SAR","(682) - Саудівський рiял"},
            {"SGD","(702) - Сінгапурський долар"},
            {"ZAR","(710) - Ренд"},
            {"SEK","(752) - Шведська крона"},
            {"CHF","(756) - Швейцарський франк"},
            {"EGP","(818) - Єгипетський фунт"},
            {"GBP","(826) - Фунт стерлінгів"},
            {"USD","(840) - Долар США"},
            {"BYN","(933) - Бiлоруський рубль"},
            {"AZN","(944) - Азербайджанський манат"},
            {"RON","(946) - Румунський лей"},
            {"TRY","(949) - Турецька ліра"},
            {"XDR","(960) - СПЗ(спеціальні права запозичення)"},
            {"BGN","(975) - Болгарський лев"},
            {"EUR","(978) - Євро"},
            {"PLN","(985) - Злотий"},
            {"DZD","(012) - Алжирський динар"},
            {"BDT","(050) - Така"},
            {"AMD","(051) - Вiрменський драм"},
            {"IQD","(368) - Іракський динар"},
            {"KGS","(417) - Сом"},
            {"LBP","(422) - Ліванський фунт"},
            {"LYD","(434) - Лівійський динар"},
            {"MYR","(458) - Малайзійський ринггіт"},
            {"MAD","(504) - Марокканський дирхам"},
            {"PKR","(586) - Пакистанська рупія"},
            {"VND","(704) - Донг"},
            {"THB","(764) - Бат"},
            {"AED","(784) - Дирхам ОАЕ"},
            {"TND","(788) - Туніський динар"},
            {"UZS","(860) - Узбецький сум"},
            {"TWD","(901) - Новий тайванський долар"},
            {"TMT","(934) - Туркменський новий манат"},
            {"GHS","(936) - Ганських седі"},
            {"RSD","(941) - Сербський динар"},
            {"TJS","(972) - Сомонi"},
            {"GEL","(981) - Ларi"},
            {"XAU","(959) - Золото"},
            {"XAG","(961) - Срiбло"},
            {"XPT","(962) - Платина"},
            {"XPD","(964) - Паладiй"}

        };
        #endregion

        #region Methods
        public async Task<IBankResult> GetRateAsync(string currency, DateTime date)
        {
            IBankResult bankResult = new BankResult()
            {
                ExchangeRate = -1m,
                RateDate = date,
                HomeCurrency = homeCurrency,
                BankName = bankName
            };

            if (date < minDate)
                return bankResult;

            if (SupportedCurrencies.ContainsKey(currency))
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        string startDate = date.ToString("yyyyMMdd");
                        Uri methodUri = new Uri($"{baseUrl}{currency}&date={startDate}&json");
                        string json = await client.DownloadStringTaskAsync(methodUri);

                        JArray ja = (JArray)JsonConvert.DeserializeObject(json);
                        if (ja.Count != 0)
                        {
                            decimal result = decimal.Zero;
                            decimal.TryParse(ja.SelectToken("$[0].rate").ToString(), out result);
                            bankResult.ExchangeRate = result;
                            DateTime.TryParseExact(ja.SelectToken("$[0].exchangedate").ToString(), "dd.MM.yyyy", CultureInfo.InvariantCulture, 
                                DateTimeStyles.None, out DateTime rateDate);
                            
                            bankResult.RateDate = rateDate;
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
                };
            }
            else
            {
                return bankResult;
            }
        }
        #endregion
    }
}
