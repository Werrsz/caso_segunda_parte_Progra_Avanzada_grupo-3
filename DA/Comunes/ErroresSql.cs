using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DA.Comunes;

/// <summary>Traduce errores de SQL Server a casos de negocio.</summary>
public static class ErroresSql
{
    // 2601 y 2627: violación de índice único / llave única.
    public static bool EsDuplicado(DbUpdateException ex) =>
        ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);

    // 547: conflicto con restricción FOREIGN KEY / REFERENCE.
    public static bool EsLlaveForanea(DbUpdateException ex) =>
        ex.InnerException is SqlException sql && sql.Number == 547;
}
