using System.Xml.Serialization;

namespace Plugin.Sample.USPS.Entities
{
    [XmlRoot("AddressValidateRequest")]
    public class USPSAddressValidationRequest
    {
        public USPSAddressValidationRequest()
        {
            Revision = "1";
            Address = new USPSAddress();
        }

        [XmlAttribute("USERID")]
        public string UserId { get; set; }

        [XmlElement("Revision")]
        public string Revision { get; set; }

        [XmlElement("Address")]
        public USPSAddress Address { get; set; }
    }

    [XmlRoot("Address")]
    public class USPSAddress
    {
        public USPSAddress()
        {
            //Initialize fields as all fields are required by the USPS API even if blank
            Id = "0";
            Address1 = string.Empty;
            Address2 = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Zip4 = string.Empty;
            Zip5 = string.Empty;
        }

        [XmlAttribute("ID")]
        public string Id { get; set; }

        [XmlElement("Address1", IsNullable = true)]
        public string Address1 { get; set; }

        [XmlElement("Address2", IsNullable = true)]
        public string Address2 { get; set; }

        [XmlElement("City", IsNullable = true)]
        public string City { get; set; }

        [XmlElement("State", IsNullable = true)]
        public string State { get; set; }

        [XmlElement("Zip5", IsNullable = true)]
        public string Zip5 { get; set; }

        [XmlElement("Zip4", IsNullable = true)]
        public string Zip4 { get; set; }
    }

    [XmlRoot("AddressValidateResponse")]
    public class USPSAddressValidationResponse
    {
        [XmlElement("Address")]
        public USPSAddress Address { get; set; }
    }
}
