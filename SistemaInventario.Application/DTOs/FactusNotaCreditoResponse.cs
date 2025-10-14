public class FactusNotaCreditoResponse
{
    public string status { get; set; }
    public string message { get; set; }
    public FactusNotaCreditoData data { get; set; }
}

public class FactusNotaCreditoData
{
    public FactusCreditNote credit_note { get; set; }
    // Puedes agregar más propiedades si las necesitas
}

public class FactusCreditNote
{
    public int id { get; set; }
    public string number { get; set; }
    public string reference_code { get; set; }
    public int status { get; set; }
    public int send_email { get; set; }
    public string validated { get; set; }
    public string observation { get; set; }
    public string total { get; set; }
    // Agrega más campos si los necesitas
}