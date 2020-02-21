using System;
using utools.Models;
using utools.Data;
using utools.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Linq;

namespace utools.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class EmpresasController : ControllerBase
    {

        [HttpGet]
        [Route("")]
        public dynamic DisplayEmpresas([FromServices] DataContext context)
        {
            try
            {
                var data = context.Empresas
                .Include(e => e.atividade_principal)
                .Include(e => e.atividades_secundarias)
                .ToList();

                if (!(data.Count() > 0)) return Ok(new {message = "Não há CNPJs cadastrados"});
                
                return data;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return BadRequest(new {message = "Erro na consulta"});
            }
        }// public dynamic DisplayEmpresas

        [HttpPost]
        [Route("{cnpj}")]
        public IActionResult CollectEmpresa([FromServices] DataContext context, string cnpj)
        {
            try
            {
                HttpClient client = new HttpClient();
                
                // url da api que fornece os dados do CNPJ
                client.BaseAddress = new Uri("https://www.receitaws.com.br/v1/cnpj/");
                
                HttpResponseMessage apiResponse = client.GetAsync(cnpj).Result;

                if (apiResponse.IsSuccessStatusCode)
                {
                    string data = apiResponse.Content.ReadAsStringAsync().Result;
                    Empresa empresa = Conversion.JsonToEmpresa(data);
                    context.Empresas.Add(empresa);
                    context.SaveChanges();

                    return Ok(new {message = "CNPJ inserido"});
                }
                else
                {
                    return BadRequest(new {message = "API da Receita Offline"});
                }
                
            }
            catch (DbUpdateException ex)
            {
                System.Console.WriteLine(ex);
                return BadRequest(new {message = "CNPJ já existe"});
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return BadRequest(new {message = "BD não aceitou o CNPJ"});
            }
        }// public async Task<IActionResult> CollectEmpresa

        [HttpGet]
        [Route("{id}/{tipo:alpha}")]
        public dynamic GetEmpresa([FromServices] DataContext context, string id, string tipo)
        {
            try
            {
                string idToQuery = tipo == "cnpj" ? Conversion.MaskCnpj(id) : id;

                /* caso o tipo passado nao seja cnpj, buscara' pelo atributo 'nome'.
                    se for cnpj, buscara' pelo atributo 'cnpj'.
                */
                var data = context.Empresas
                    // busca pelos dois, mas apenas um retorna algo. assim, evita uma nova consulta.
                    .Where(e => e.cnpj == idToQuery || e.nome.Contains(idToQuery))
                    .Include(e => e.atividade_principal)
                    .Include(e => e.atividades_secundarias);

                // caso a consulta retorne vazia
                if (!(data.Count() > 0)) return NotFound(new {message = "CNPJ ou NOME não encontrado"});

                return data;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return BadRequest(new {message = "Erro com CNPJ fornecido"});
            }
            
        }// public dynamic GetEmpresa

        [HttpDelete]
        [Route("{id}")]
        public IActionResult RemoveEmpresaByCnpj([FromServices] DataContext context, string id)
        {
            try
            {
                var empresa = context.Empresas
                    .Include(e => e.atividade_principal)
                    .Include(e => e.atividades_secundarias)
                    .First(e => e.cnpj == Conversion.MaskCnpj(id));
                
                var cnaePrincipal = context.Cnaes
                    .First(c => c.Id == empresa.atividade_principal[0].Id);
                
                var atividadesSecundarias = empresa.atividades_secundarias;
                
                context.Remove(empresa); // remove empresa
                context.Remove(cnaePrincipal); // remove CNAE principal
    
                foreach (Cnae cnae in atividadesSecundarias) // remove CNAEs secundarios
                {
                    context.Remove(context.Cnaes
                    .First(c => c.Id == cnae.Id));
                }

                context.SaveChanges();

                return Ok(new {message = "CNPJ removido"}); 
            }
            catch (InvalidOperationException ex)
            {
                System.Console.WriteLine(ex);
                return NotFound(new {message = "CNPJ não encontrado"});
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return BadRequest(new {message = "Erro no BD"});
            }
        }// public IActionResult RemoveEmpresaByCnpj
    }
}