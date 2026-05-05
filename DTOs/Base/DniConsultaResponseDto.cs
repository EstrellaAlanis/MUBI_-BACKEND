namespace Mubi.Api.DTOs.Base;
public class DniConsultaResponseDto
{
    public string Dni { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Fuente { get; set; } = "Modo demo local";
    public bool Encontrado { get; set; }
}
