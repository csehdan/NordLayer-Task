namespace partycli.Models
{
	internal class VpnServerQuery
	{
		public int? Protocol { get; set; }

		public int? CountryId { get; set; }

		public int? CityId { get; set; }

		public int? RegionId { get; set; }

		public int? SpecificServcerId { get; set; }

		public int? ServerGroupId { get; set; }

		public VpnServerQuery(int? protocol, int? countryId, int? cityId, int? regionId, int? specificServcerId, int? serverGroupId)
		{
			Protocol = protocol;
			CountryId = countryId;
			CityId = cityId;
			RegionId = regionId;
			SpecificServcerId = specificServcerId;
			ServerGroupId = serverGroupId;
		}
	}
}
