public class FactusFacturaRequest
{
    public int numbering_range_id { get; set; }
    public string reference_code { get; set; }
    public string observation { get; set; }
    public string payment_form { get; set; }
    public string payment_due_date { get; set; }
    public string payment_method_code { get; set; }
    public int operation_type { get; set; }
    public bool send_email { get; set; }
    public FactusOrderReference order_reference { get; set; }
    public FactusBillingPeriod billing_period { get; set; }
    public FactusEstablishment establishment { get; set; }
    public FactusCustomer customer { get; set; }
    public List<FactusItem> items { get; set; }
    // Puedes agregar related_documents, credit_notes, debit_notes si los necesitas
}

public class FactusOrderReference
{
    public string reference_code { get; set; }
    public string issue_date { get; set; }
}

public class FactusBillingPeriod
{
    public string start_date { get; set; }
    public string start_time { get; set; }
    public string end_date { get; set; }
    public string end_time { get; set; }
}

public class FactusEstablishment
{
    public string name { get; set; }
    public string address { get; set; }
    public string phone_number { get; set; }
    public string email { get; set; }
    public string municipality_id { get; set; }
}

public class FactusCustomer
{
    public string identification { get; set; }
    public string dv { get; set; }
    public string company { get; set; }
    public string trade_name { get; set; }
    public string names { get; set; }
    public string address { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string legal_organization_id { get; set; }
    public string tribute_id { get; set; }
    public string identification_document_id { get; set; }
    public int municipality_id { get; set; }
}

public class FactusItem
{
    public string scheme_id { get; set; }
    public string note { get; set; }
    public string code_reference { get; set; }
    public string name { get; set; }
    public int quantity { get; set; }
    public decimal discount_rate { get; set; }
    public decimal price { get; set; }
    public string tax_rate { get; set; } 
    public int unit_measure_id { get; set; }
    public int standard_code_id { get; set; }
    public int is_excluded { get; set; }
    public int tribute_id { get; set; }
    public List<FactusWithholdingTax> withholding_taxes { get; set; }
    public FactusMandate? mandate { get; set; } // <-- Hazlo nullable
}

public class FactusWithholdingTax
{
    public string code { get; set; }
    public string withholding_tax_rate { get; set; }
}

public class FactusMandate
{
    public int? identification_document_id { get; set; }
    public string identification { get; set; }
}