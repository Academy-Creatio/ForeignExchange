using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForeignExchange
{
	public interface IBank
	{
		Dictionary<string, string> SupportedCurrencies { get; }
		Task<IBankResult> GetRateAsync(string currency, DateTime date);
	}
}