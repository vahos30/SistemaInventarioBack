public class CiudadApiResponse
{
    public string status { get; set; }
    public string message { get; set; }
    public List<CiudadDto> data { get; set; }
}

public class CiudadDto
{
    public int id { get; set; }
    public string code { get; set; }
    public string name { get; set; }
    public string department { get; set; }
}