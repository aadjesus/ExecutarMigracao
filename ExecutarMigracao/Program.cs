using System;

namespace ExecutarMigracao
{
    class Program
    {
        static void Main(string[] args)
        {
            var parametros = new Parametros(args);
            if (!parametros.Validar())
                return;

            var migracao = new Migracao(parametros);
            if (!migracao.ExtrairMigrateExe())
                return;

            migracao.Executar();
        }
    }
}
