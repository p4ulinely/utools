using System;
using utools.Models;
using utools.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Linq;

namespace utools.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class EmpresasController : ControllerBase
    {
        private DataContext dbEmpresas = new DataContext();

        // metodo para parsear json para objeto Empresa
        private Empresa JsonToEmpresa(string json)
        {
            var deserializedEmpresa = new Empresa();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var ser = new DataContractJsonSerializer(deserializedEmpresa.GetType());
            deserializedEmpresa = ser.ReadObject(ms) as Empresa;
            ms.Close();
            return deserializedEmpresa;
        }// private Empresa JsonToEmpresa

        // metodo para parsear objeto Empresa para json
        private string EmpresaToJson(Empresa obj)
        {
            // var obj = new Empresa(){
            //     cnpj = {}
            // };

            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(Empresa));
            ser.WriteObject(ms, obj);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }// private string EmpresaToJson

        // metodo para aplicar mascara no cnpj
        private string MaskCnpj(string cnpj)
        {
            if(cnpj.Length != 14) throw new Exception();

            return cnpj.Substring(0,2) + "." +
                cnpj.Substring(2,3) + "." +
                cnpj.Substring(5,3) + "/" +
                cnpj.Substring(8,4) + "-" +
                cnpj.Substring(12,2);
            
        }// private string MaskCnpj

        [HttpGet]
        [Route("")]
        public dynamic DisplayEmpresas()
        {
            try
            {
                var data = dbEmpresas.Empresas
                .Include(e => e.atividade_principal)
                .Include(e => e.atividades_secundarias)
                .ToList();

                if (data.Count() > 0)
                {
                    return data;
                }
                else
                {
                    return StatusCode(400, new {message = "Não há CNPJs cadastrados"});
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, new {message = "Erro ao fazer consulta"});
            }
        }// public dynamic DisplayEmpresas

        [HttpPost]
        [Route("{cnpj}")]
        public async Task<IActionResult> CollectEmpresa(string cnpj)
        {
            try
            {
                HttpClient client = new HttpClient();
                
                // url da api que fornece os dados do CNPJ
                client.BaseAddress = new Uri("https://www.receitaws.com.br/v1/cnpj/");
                
                HttpResponseMessage apiResponse = await client.GetAsync(cnpj);

                if (apiResponse.IsSuccessStatusCode)
                {
                    string data = await apiResponse.Content.ReadAsStringAsync();

                    if (data.Contains("CNPJ inválido")) return StatusCode(400, new {message = "CNPJ inválido"});

                    Empresa empresa = JsonToEmpresa(data);
                    dbEmpresas.Empresas.Add(empresa);
                    dbEmpresas.SaveChanges();

                    return Ok(new {message = "CNPJ inserido"});
                }
                else
                {
                    return StatusCode(500, new {message = "API da Receita Offline"});
                }
                
            }
            catch (DbUpdateException ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(400, new {message = "CNPJ já existe"});
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, new {message = "BD não aceitou o CNPJ"});
            }
        }// public async Task<IActionResult> CollectEmpresa

        [HttpGet]
        [Route("{id}/{tipo:alpha}")]
        // [Route("{id:regex(^\\d{{2}}\\d{{3}}\\d{{3}}\\d{{4}}\\d{{2}}$)}")]
        public dynamic GetEmpresa(string id, string tipo)
        {
            try
            {
                string idToQuery = tipo == "cnpj" ? MaskCnpj(id) : id;

                /* caso o tipo passado nao seja cnpj, buscara' pelo atributo 'nome'.
                    se for cnpj, buscara' pelo atributo 'cnpj'.
                */
                var data = dbEmpresas.Empresas
                    // busca pelos dois, mas apenas um retorna algo. assim, evita uma nova consulta.
                    .Where(e => e.cnpj == idToQuery || e.nome.Contains(idToQuery))
                    .Include(e => e.atividade_principal)
                    .Include(e => e.atividades_secundarias);

                // caso a consulta retorne vazia
                if (!(data.Count() > 0)) return StatusCode(400, new {message = "CNPJ ou NOME não encontrado"});

                return data;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, new {message = "Erro com CNPJ fornecido"});
            }
            
        }// public dynamic GetEmpresa

        [HttpDelete]
        [Route("{id}")]
        public IActionResult RemoveEmpresaByCnpj(string id)
        {
            try
            {
                var empresa = dbEmpresas.Empresas
                    .Include(e => e.atividade_principal)
                    .Include(e => e.atividades_secundarias)
                    .First(e => e.cnpj == MaskCnpj(id));
                
                var cnaePrincipal = dbEmpresas.Cnaes
                    .First(c => c.Id == empresa.atividade_principal[0].Id);
                
                var atividadesSecundarias = empresa.atividades_secundarias;
                
                dbEmpresas.Remove(empresa); // remove empresa
                dbEmpresas.Remove(cnaePrincipal); // remove CNAE principal
    
                foreach (Cnae cnae in atividadesSecundarias) // remove CNAEs secundarios
                {
                    dbEmpresas.Remove(dbEmpresas.Cnaes
                    .First(c => c.Id == cnae.Id));
                }

                dbEmpresas.SaveChanges();

                return Ok(new {message = "CNPJ removido"}); 
            }
            catch (InvalidOperationException ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(400, new {message = "CNPJ não encontrado"});
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, new {message = "Erro no BD"});
            }
        }// public IActionResult RemoveEmpresaByCnpj
    }
}