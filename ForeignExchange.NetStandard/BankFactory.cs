namespace ForeignExchange
{
	public static class BankFactory
	{
		#region Enum
		public enum SupportedBanks
		{
			BOC = 0,
			CBR = 1,
			NBU = 2,
			ECB = 3,
			BOMX = 4,
			RBA = 5,
			BOE=6

			
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get Bank implementation for one of the supported banks
		/// </summary>
		/// <param name="supportedBanks"></param>
		/// <param name="secret">Optional API Token</param>
		/// <returns>Implementation of a Bank</returns>
		public static IBank GetBank(SupportedBanks supportedBanks, string secret="")
		{
			switch (supportedBanks)
			{
				case SupportedBanks.BOC:
					return new BankOfCanada();

				case SupportedBanks.CBR:
					return new CentralBankOfRussia();

				case SupportedBanks.NBU:
					return new NationalBankOfUkraine();

				case SupportedBanks.ECB:
					return new EuropeanCentralBank(); 
				
				case SupportedBanks.BOMX:
					return new BankOfMexico(secret);
				
				case SupportedBanks.RBA:
					return new BankOfAustralia();

				case SupportedBanks.BOE:
					return new BankOfEngland();
				default: return null;
			}
		}
		#endregion
	}
}