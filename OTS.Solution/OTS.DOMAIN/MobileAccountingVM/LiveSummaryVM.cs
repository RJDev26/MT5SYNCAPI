using System;

namespace OTS.DOMAIN.MobileAccountingVM
{
    public class LiveSummaryVM
    {
        public long Login { get; set; }
        public string? Symbol { get; set; }
        public decimal OpenQty { get; set; }
        public decimal OpenRate { get; set; }
        public decimal OpenAmt { get; set; }

        // Map database column names to camel‑cased JSON names.
        // The stored procedure returns BuyQty/BuyAmt/SellQty/SellAmt.
        [System.Text.Json.Serialization.JsonPropertyName("buyQty")]
        public decimal BuyQty { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("buyAmt")]
        public decimal BuyAmt { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("sellQty")]
        public decimal SellQty { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("sellAmt")]
        public decimal SellAmt { get; set; }

        public decimal Commission { get; set; }
        public decimal CloseQty { get; set; }
        public decimal CloseRate { get; set; }
        public decimal CloseAmt { get; set; }
        public decimal GrossMTM { get; set; }
        public decimal NetAmt { get; set; }
    }
}
