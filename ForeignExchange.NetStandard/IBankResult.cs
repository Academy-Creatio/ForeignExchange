using System;

namespace ForeignExchange
{
	public interface IBankResult
	{
		/// <summary>
		/// The Bank home currency
		/// </summary>
		string HomeCurrency { get; set; }

		/// <summary>
		/// Exchange rate is stated in units of foreign currency per one unit of home currency
		/// </summary>
		decimal ExchangeRate { get; set; }

		/// <summary>
		/// Observation Date
		/// </summary>
		DateTime RateDate { get; set; }

		/// <summary>
		/// Name of the Bank providing observation
		/// </summary>
		string BankName { get; set; }
	}
}