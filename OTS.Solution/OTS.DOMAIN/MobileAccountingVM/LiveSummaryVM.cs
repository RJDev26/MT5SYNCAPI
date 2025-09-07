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

        // Stored procedure returns abbreviated column names (BQty, BAmt, etc.).
        // Map them here while preserving the expected camel‑cased JSON names.
        [System.Text.Json.Serialization.JsonPropertyName("buyQty")]
        public decimal BQty { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("buyAmt")]
        public decimal BAmt { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("sellQty")]
        public decimal SQty { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("sellAmt")]
        public decimal SAmt { get; set; }

        public decimal Commission { get; set; }
        public decimal CloseQty { get; set; }
        public decimal CloseRate { get; set; }
        public decimal CloseAmt { get; set; }
        public decimal GrossMTM { get; set; }
        public decimal NetAmt { get; set; }
    }
}
