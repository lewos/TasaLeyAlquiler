using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TasaLeyAlquiler
{
    public static class ServiceBCRA
    {
        public static async Task<string> GetFechaYTasasAsync(DateTime desde, DateTime hasta) 
        {
            var httpClient = new HttpClient();

            var fechaDesdeGuiones =  desde.ToString("yyyy-MM-dd");
            var fechaDesde = desde.ToString("yyyyMMdd");
            var fechaHastaGuiones = hasta.ToString("yyyy-MM-dd");
            var fechaHasta = hasta.ToString("yyyyMMdd");

            var url = $"http://www.bcra.gov.ar/PublicacionesEstadisticas/Principales_variables_datos.asp?fecha_desde={fechaDesdeGuiones}&fecha_hasta={fechaHastaGuiones}&B1=Enviar&primeravez=1&fecha_desde={fechaDesde}&fecha_hasta={fechaHasta}&serie=7988&serie1=0&serie2=0&serie3=0&serie4=0";
            var response = await httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}
