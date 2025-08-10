namespace OTS.DOMAIN.MobileAccoutingDTO
{
    public class VoucherSummaryRequest
    {
        public string? VoucherType { get; set; } = null;
        public DateTime? FromDate { get; set; } = null;  // ✅ Nullable
        public DateTime? ToDate { get; set; } = null;    // ✅ Nullable
        public int? CreatedBy { get; set; } = null;
    }


}
