using BgmRodotec.Escala.Infra.Database;
using FluentMigrator.Runner.Processors;

namespace Teste
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {                


                var x1 = new DatabaseSetup();
                x1.UpAll();
            }
            catch (System.Exception ex)
            {

                throw ex;
            }


        }
    }
}
