using Abstracciones.Asistentes;
using Abstracciones.Conferencistas;
using Abstracciones.Eventos;
using Abstracciones.Inscripciones;
using DA.Asistentes;
using DA.Conferencistas;
using DA.Contexto;
using DA.Eventos;
using DA.Inscripciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DA;

public static class RegistroDA
{
    public static IServiceCollection AddDatos(this IServiceCollection servicios, string cadenaConexion)
    {
        servicios.AddDbContext<EventosContexto>(o => o.UseSqlServer(cadenaConexion));

        // Módulo Conferencistas
        servicios.AddScoped<IConferencistaDA, ConferencistaDA>();
        // Módulo Eventos
        servicios.AddScoped<IEventoDA, EventoDA>();
        // Módulo Asistentes
        servicios.AddScoped<IAsistenteDA, AsistenteDA>();
        // Módulo Inscripciones (Muchos a Muchos)
        servicios.AddScoped<IInscripcionDA, InscripcionDA>();

        return servicios;
    }
}
