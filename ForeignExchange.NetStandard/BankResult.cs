using System;

namespace ForeignExchange
{
	public sealed class BankResult : IBankResult
	{
		#region Properties
		public string HomeCurrency { get; set; }
		public decimal ExchangeRate { get; set; }
		public DateTime RateDate { get; set; }
		public string BankName { get; set; }
		#endregion
	}
}