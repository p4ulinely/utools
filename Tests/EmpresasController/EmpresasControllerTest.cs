using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using utools.Controllers;
using utools.Data;
using utools.Services;

namespace utools.Tests.Empresas.Tests
{
    [TestFixture]
    public class EmpresasControllerTest
    {   
        public DbContextOptions<DataContext> Options { get; set; }

        [SetUp]
        public void Setup(){
            this.Options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "tests_utools")
            .Options;
        }

        // deve inserir o CNPJ no BD InMemory e retornar dois CNPJs cadastrados
        [Test]
        public void PostRoute_EmpresasController()
        {

            using (var context = new DataContext(this.Options))
            {
                var empresa = new EmpresasController();

                // metodo do POST
                // insere duas empresas
                empresa.CollectEmpresa(context, "11886096000153");
                empresa.CollectEmpresa(context, "38020632000190");
            }

            using (var context = new DataContext(this.Options))
            {
                Assert.AreEqual(2, context.Empresas.CountAsync().Result);
            }
        }

        // pega a empresa retornada pelo metodo GetEmpresa (por 'cnpj') e 
        // compara diretamente com a empresa retornada pelo EF (vem direto do bd InMemory)
        [TestCase("38020632000190")]
        public void GetRoute_EmpresasController_Por_Cnpj(string cnpj)
        {
            dynamic retorno = null;
            var empresa = new EmpresasController();

            using (var context = new DataContext(this.Options))
            {
                empresa.CollectEmpresa(context, cnpj);
            }

            using (var context = new DataContext(this.Options))
            {
                // metodo que retorna empresa por cnpj
                // retorna o objeto da empresa encontrada
                retorno = empresa.GetEmpresa(context, cnpj, "cnpj");

                //compara o retorno do metodo GetEmpresa diretamente com o do BD
                Assert.AreEqual(retorno, context.Empresas
                    .Where(e => e.cnpj == Conversion.MaskCnpj(cnpj))
                    .Include(e => e.atividade_principal)
                    .Include(e => e.atividades_secundarias));
            }
        }

        // mesmo teste que o anterior, mas dessa vez testa o GetEmpresa pelo 'nome'
        [Test]
        public void GetRoute_EmpresasController_Por_Nome()
        {
            dynamic retorno = null;
            var empresa = new EmpresasController();

            string nome = "CASA AMARELA COMIDA CASEIRA LTDA";
            string cnpj = "38020632000190";

            using (var context = new DataContext(this.Options))
            {
                empresa.CollectEmpresa(context, cnpj);
            }

            using (var context = new DataContext(this.Options))
            {
                // metodo que retorna empresa por nome
                // retorna o objeto da empresa encontrada
                retorno = empresa.GetEmpresa(context, nome, "nome");

                //compara o retorno do metodo GetEmpresa diretamente com o do BD
                Assert.AreEqual(retorno, context.Empresas
                    .Where(e => e.nome.Contains(nome))
                    .Include(e => e.atividade_principal)
                    .Include(e => e.atividades_secundarias));
            }
        }

        // deve remover uma empresa
        [Test]
        public void DeleteRoute_EmpresasController()
        {
            dynamic retorno = null;
            var empresa = new EmpresasController();

            string cnpj = "38020632000190";

            using (var context = new DataContext(this.Options))
            {
                empresa.CollectEmpresa(context, cnpj);
            }

            using (var context = new DataContext(this.Options))
            {
                //testa que uma empresa foi inserida
                Assert.AreEqual(1, context.Empresas.CountAsync().Result);

                // metodo que remove por cnpj
                retorno = empresa.RemoveEmpresaByCnpj(context, cnpj);

                // testa que nao ha nenhuma empresa
                Assert.AreEqual(0, context.Empresas.CountAsync().Result);
            }
        }
    }
}