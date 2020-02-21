using utools.Models;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System;

namespace utools.Services
{
    public static class Conversion
    {
        // metodo para parsear json para objeto Empresa
        public static Empresa JsonToEmpresa(string json)
        {
            var deserializedEmpresa = new Empresa();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var ser = new DataContractJsonSerializer(deserializedEmpresa.GetType());
            deserializedEmpresa = ser.ReadObject(ms) as Empresa;
            ms.Close();
            return deserializedEmpresa;
        }// private Empresa JsonToEmpresa

        // metodo para parsear objeto Empresa para json
        public static string EmpresaToJson(Empresa obj)
        {
            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(typeof(Empresa));
            ser.WriteObject(ms, obj);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }// private string EmpresaToJson

        // metodo para aplicar mascara no cnpj
        public static string MaskCnpj(string cnpj)
        {
            if(cnpj.Length != 14) throw new Exception();

            return cnpj.Substring(0,2) + "." +
                cnpj.Substring(2,3) + "." +
                cnpj.Substring(5,3) + "/" +
                cnpj.Substring(8,4) + "-" +
                cnpj.Substring(12,2);
            
        }// private string MaskCnpj
    }
}