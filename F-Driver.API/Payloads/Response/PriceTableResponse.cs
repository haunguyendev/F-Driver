using F_Driver.Service.BusinessModels;

namespace F_Driver.API.Payloads.Response
{
    public class PriceTableResponse
    {
        public PriceTableModel PriceTable { get; set; } = null!;
    }
}
