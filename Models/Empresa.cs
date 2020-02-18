using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace utools.Models
{
    public class Empresa
    {
        [Key]
        public string cnpj { get; set; }
        public string tipo { get; set; }
        public string abertura { get; set; }
        public string nome { get; set; }
        public string fantasia { get; set; }
        public string porte { get; set; }
        public List<Cnae> atividade_principal { get; set; }
        public List<Cnae> atividades_secundarias { get; set; }
        public string natureza_juridica { get; set; }
        public string logradouro { get; set; }
        public int numero { get; set; }
        public string complemento { get; set; }
        public string cep { get; set; }
        public string bairro { get; set; }
        public string municipio { get; set; }
        public string uf { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public string efr { get; set; }
        public string situacao { get; set; }
        public string data_situacao { get; set; }

        // [JsonIgnore]
        // public DateTime DataCriacao { get; set; }

        // public Empresa()
        // {
        //     DataCriacao = DateTime.Now;
        // }
    }
}