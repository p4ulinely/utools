
// let card = document.getElementById('card-body');
let card = document.querySelector('#meuCard .card .card-body');
let backupFormularioVisualizacao = card.innerHTML;
let carregando = "<div class='text-center'><div class='spinner-border' style='width: 3rem; height: 3rem;' role='status'><span class='sr-only'>Loading...</span></div></div>";

window.onload = function() {

}

//consome o endpoint de busca, mas nesse front-end, tambem eh usado pra visualizar os dados das empresas
function buscarEmpresa(source, tipo='/cnpj'){

    // e.preventDefault();
    let cnpjNome = document.getElementById((source || 'inpt-buscarEmpresa'));
    let endereco = 'https://utools-api.herokuapp.com/v1/empresas/';
    let isPorNome = document.getElementById('isPesquisaNome').checked;

    // para gantarir a visualizacao da empresa, quando nao for busca
    if(isPorNome && source == undefined) tipo = "/nome";

    //se aplica apenas para o form de busca
    if(cnpjNome.value.length < 2){
        let helper = document.getElementById('inpt-buscarEmpresas-Help');
        helper.className = "form-text text-danger";
        helper.innerHTML = "Dados inválidos.";
        return cnpjNome.focus();
    }

    card.innerHTML = carregando;
    
    api(endereco+cnpjNome.value+tipo)
        .then(function(response) {
        
            if(response.message){
                card.innerHTML = "Não encontrado";
                return null;
            }
        
            // restaura html do fomulario
            card.innerHTML = backupFormularioVisualizacao;
        
            document.getElementById('cnpj').value = response[0].cnpj;
            document.getElementById('nome').value = response[0].nome;
            document.getElementById('abertura').value = response[0].abertura;
            document.getElementById('fantasia').value = response[0].fantasia;
            document.getElementById('porte').value = response[0].porte;
            document.getElementById('atividade_principal_code').value = response[0].atividade_principal[0].code;
            document.getElementById('atividade_principal_text').value = response[0].atividade_principal[0].text;
            document.getElementById('natureza_juridica').value = response[0].natureza_juridica;
            document.getElementById('logradouro').value = response[0].logradouro;
            document.getElementById('numero').value = response[0].numero;
            document.getElementById('complemento').value = response[0].complemento;
            document.getElementById('bairro').value = response[0].bairro;
            document.getElementById('cep').value = response[0].cep;
            document.getElementById('municipio').value = response[0].municipio;
            document.getElementById('uf').value = response[0].uf;
            document.getElementById('telefone').value = response[0].telefone;
            document.getElementById('email').value = response[0].email;
            document.getElementById('situacao').value = response[0].situacao;
            document.getElementById('data_situacao').value = response[0].data_situacao;
            
            // seta a div para ser mostrada
            document.getElementById('formVisualizacao').style = "display: block;";
        
        })
        .catch(function (err) {
			console.warn(err);
		})
}

function mostrarEmpresas() {

    let endereco = 'https://utools-api.herokuapp.com/v1/empresas';

    card.innerHTML = carregando;

	api(endereco)
		.then(function(response) {

            if(response.message){
                card.innerHTML = "Não há CNPJs cadastrados";
                return null;
            }

            let str = "<table class='table table-striped'><thead><tr><th scope='col'>CNPJ</th><th scope='col'>NOME</th><th scope='col'>CIDADE</th><th scope='col'>UF</th><th scope='col'>ACOES</th></tr></thead><tbody>";

			for(let r of response){
                str += "<tr><td>"+r.cnpj+"<input type='hidden' id='cnpj_"+rPontuacao(r.cnpj)+"' value='"+rPontuacao(r.cnpj)+"'></td><td><a href='#' onClick='buscarEmpresa(\"cnpj_"+rPontuacao(r.cnpj)+"\")'>"+r.nome+"</a></td><td>"+r.municipio+"</td><td>"+r.uf+"</td><td><a href='#' class='btn btn-danger btn-sm' onClick='deletarCNPJ(\""+rPontuacao(r.cnpj)+"\")'>REMOVER</a></td></tr>";
            }

            str += "</tbody></table>";

            card.innerHTML = str

		})
		.catch(function (err) {
			console.warn(err);
		})
}

function deletarCNPJ(cnpj){

    if(!confirm("Tem certeza?")) return null;

    card.innerHTML = carregando;

    api("https://utools-api.herokuapp.com/v1/empresas/"+cnpj, 'DELETE')
        .then(function(response){
            // card.innerHTML = response.message;
            alert(response.message);
            location.reload();
        })
        .catch(function (err) {
			console.warn(err);
        });
}

function cadastrarEmpresa(){

    let cnpj = document.getElementById('inpt-cadastrarEmpresas');
    
    if(cnpj.value.length < 14) {
        let helper = document.getElementById('inpt-cadastrarEmpresas-Help');
        helper.className = "form-text text-danger";
        helper.innerHTML = "CNPJ é composto de 14 dígitos.";
        
        return cnpj.focus();
    }

    card.innerHTML = carregando;

    api("https://utools-api.herokuapp.com/v1/empresas/"+rPontuacao(cnpj.value), 'POST')
        .then(function(response){
            // card.innerHTML = response.message;
            alert(response.message);
            location.reload();
        })
        .catch(function (err) {
			console.warn(err);
        });
}

function api(url, verb='GET') {
    
	return new Promise(function (resolve, reject) {
		let xhr = new XMLHttpRequest();
		xhr.open(verb, url);
		xhr.send(null);

		xhr.onreadystatechange = function () {
			if(xhr.readyState === 4){
                switch (xhr.status) {
                    case 200:
                        resolve(JSON.parse(xhr.responseText));
                        break;
                    case 404:
                        resolve(JSON.parse(xhr.responseText));
                        break;
                    case 400:
                        resolve(JSON.parse(xhr.responseText));
                        break;
                    default:
                        reject(JSON.stringify({message: 'Erro na requisicao'}));
                        break;
                }

                // if (xhr.status === 200) {
                //     resolve(JSON.parse(xhr.responseText));
                // } else {
                //     reject(JSON.stringify({message: 'Erro na requisicao'}));
                // }
			}
		}
	});
} // function api

function rPontuacao(str) {
    return str.replace(/(~|`|!|@|#|$|%|^|&|\*|\(|\)|{|}|\[|\]|;|:|\"|'|<|,|\.|>|\?|\/|\\|\||-|_|\+|=)/g,"")
}