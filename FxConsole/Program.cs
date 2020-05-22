using ForeignExchange;
using FxConsole.Properties;
using System;
using System.Threading.Tasks;

namespace FxConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            DateTime date = new DateTime(2020, 5, 21);

            Task BOC = RateBOC("USD", date);
            Task ECB = RateECB("USD", date);
            Task NBU = RateNBU("USD", date);
            Task BOMX = RateBOMX("USD", date);
            Task CBR = RateCBR("USD", date);
            Task RBA = RateRBA("USD", date);
            Task BOE = RateBOE("USD", date);
            await Task.WhenAll(BOC, ECB, NBU, BOMX, RBA, CBR, BOE);

            Console.ReadLine();
        }

        private static async Task RateBOE(string Currency, DateTime date)
        {
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOE);
            IBankResult result = await ibank.GetRateAsync(Currency, date);
            PrintReSult(Currency, result);
        }


        private static async Task RateBOMX(string Currency, DateTime date)
        {
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOMX, Resources.bmxSecret);
            IBankResult result = await ibank.GetRateAsync(Currency, date);
            PrintReSult(Currency, result);
        }

        private static async Task RateCBR(string Currency, DateTime date)
        {
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.CBR);
            IBankResult result = await ibank.GetRateAsync(Currency, date);
            PrintReSult(Currency, result);
        }

        private static async Task RateNBU(string Currency, DateTime date)
        {
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.NBU);
            IBankResult result = await ibank.GetRateAsync(Currency, date);
            PrintReSult(Currency, result);
        }

        private static async Task RateECB(string Currency, DateTime date)
        {
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.ECB);
            IBankResult result = await ibank.GetRateAsync(Currency, date);
            PrintReSult(Currency, result);
        }
        
        private static async Task RateBOC(string Currency, DateTime date)
        {
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.BOC);
            IBankResult result = await ibank.GetRateAsync(Currency, date);
            PrintReSult(Currency, result);
        }
        
        private static async Task RateRBA(string Currency, DateTime date)
        {
            IBank ibank = BankFactory.GetBank(BankFactory.SupportedBanks.RBA);
            IBankResult result = await ibank.GetRateAsync(Currency, date);
            PrintReSult(Currency, result);
        }

        private static void PrintReSult(string Currency, IBankResult result)
        {
            string resString = $"1 {Currency} = {result.ExchangeRate}\t{result.HomeCurrency} on {result.RateDate:dd-MMM-yyyy} provided by the {result.BankName}";
            Console.WriteLine(resString);
        }

    }
}
