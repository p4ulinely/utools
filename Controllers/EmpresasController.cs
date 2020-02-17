using System;
using utools.Models;
using utools.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
// using System.Net;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Linq;
// using System.Web.Http;

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
            if(cnpj.Length < 14)
            {
                throw new Exception("message: CNPJ precisa ter 14 dígitos");
                // return null;
            }

            return (cnpj.Substring(0,2) + "." +
                cnpj.Substring(2,3) + "." +
                cnpj.Substring(5,3) + "/" +
                cnpj.Substring(8,4) + "-" +
                cnpj.Substring(12,2));
        }// private string MaskCnpj

        [HttpGet]
        [Route("")]
        public dynamic DisplayEmpresas()
        // public async Task<ActionResult<List<Empresa>>> DisplayEmpresas()
        {
            try
            {
                return (dbEmpresas.Empresas
                .Include(e => e.atividade_principal)
                .Include(e => e.atividades_secundarias)
                .ToList());
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, new {message = "Erro ao fazer consulta"});
            }
        }// public List<Empresa> DisplayEmpresas

        [HttpPost]
        [Route("{cnpj}")]
        // public async Task<HttpResponseMessage> CollectEmpresa(string cnpj)
        public IActionResult CollectEmpresa(string cnpj)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://www.receitaws.com.br/v1/cnpj/");
                HttpResponseMessage apiResponse = client.GetAsync(cnpj).Result;

                Empresa empresa = null;

                if (apiResponse.IsSuccessStatusCode)
                {
                    string data =  apiResponse.Content.ReadAsStringAsync().Result;
                    if (!data.Contains("ERROR"))
                    {
                        empresa = JsonToEmpresa(data);
                        dbEmpresas.Empresas.Add(empresa);
                        dbEmpresas.SaveChanges();
                        return Ok(new {message = "CNPJ inserido"});
                    }
                    else
                    {
                        return StatusCode(400, new {message = "CNPJ inválido"});
                    }
                }
                else
                {
                    // throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError){
                    //     Content = new StringContent("API da Receita Offline"),
                    //     ReasonPhrase = "Critical Exception"
                    // });
                    return StatusCode(500, new {message = "API da Receita Offline"});
                }
                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, new {message = "BD não aceitou o CNPJ"});
            }
        }// public IActionResult AddEmpresa

        [HttpGet]
        // [Route("{id:regex(^\\d{{2}}\\d{{3}}\\d{{3}}\\d{{4}}\\d{{2}}$)}")]
        [Route("{id}/{tipo:alpha}")]
        public dynamic GetEmpresa(string id, string tipo)
        {
            try
            {
                string idToQuery = tipo == "cnpj" ? MaskCnpj(id) : id;

                return dbEmpresas.Empresas
                    .Where(e => e.cnpj == idToQuery || e.nome.Contains(idToQuery))
                    .Include(e => e.atividade_principal)
                    .Include(e => e.atividades_secundarias);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, new {message = "Erro ao fazer consulta"});
            }
            
        }// public IQueryable GetEmpresa

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
                // empresa.atividade_principal.id // pegar id do cnpj e remove no bd
                // System.Console.WriteLine(dbEmpresas.Empresas.Remove(new Empresa(){ cnpj = MaskCnpj(id) }));
                dbEmpresas.Remove(empresa);
                dbEmpresas.SaveChanges();
                return Ok(new {message = "CNPJ removido"}); 
            }
            catch (InvalidOperationException)
            {
                return Ok(new {message = "CNPJ não encontrado"});
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return StatusCode(500, new {message = "BD não aceitou o CNPJ"});
            }
        }// public IActionResult RemoveEmpresaByCnpj
    }
}