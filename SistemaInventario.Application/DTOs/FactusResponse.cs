using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Application.DTOs
{
    public class FactusResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public FactusResponseData data { get; set; }
    }

    public class FactusResponseData
    {
        public FactusBill bill { get; set; }
        public List<FactusItemResponse> items { get; set; }
        // Puedes agregar más propiedades si lo necesitas
    }

    public class FactusBill
    {
        public int id { get; set; }
        public string number { get; set; }
        public string reference_code { get; set; }
        public string total { get; set; }
        public string observation { get; set; }
        // Otros campos relevantes...
    }

    public class FactusItemResponse
    {
        public string name { get; set; }
        public int quantity { get; set; }
        public string price { get; set; }
        public string tax_rate { get; set; }
        public string tax_amount { get; set; } // Este es el campo IVA que necesitas
        // Otros campos relevantes...
    }
}
