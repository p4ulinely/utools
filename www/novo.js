window.onload = function() {
    // coletarEmpresas();
}

// let card = document.querySelector('#app .container .card .card-body');
let carregando = "<div class='text-center'><div class='spinner-border' style='width: 3rem; height: 3rem;' role='status'><span class='sr-only'>Loading...</span></div></div>";
let card = document.getElementById('card-body');

function buscarEmpresa(tipo='/nome'){

    // e.preventDefault();
    let cnpjNome = document.getElementById('inpt-buscarEmpresa');
    let endereco = 'https://utools-api.herokuapp.com/v1/empresas/';

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

            card.innerHTML = "";
            
			for(res of response){
				let meuP = document.createElement('p');
				let minhaEmpresa = document.createTextNode(res.cnpj + " " + res.nome);

				meuP.appendChild(minhaEmpresa);
				card.appendChild(meuP);
			}
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
                str += "<tr><td>"+r.cnpj+"</td><td>"+r.nome+"</td><td>"+r.municipio+"</td><td>"+r.uf+"</td><td><a href='#' onClick='deletarCNPJ("+rPontuacao(r.cnpj)+")'>REMOVER</a></td></tr>";
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