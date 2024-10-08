function registerUser(event){
    event.preventDefault();

    let outputDiv = document.getElementById('div_output');
    makeElementEmpty(outputDiv);

    let password = document.getElementsByTagName('input')[2].value;
    if (password.length >= 6){
        let confirmPassword = document.getElementsByTagName('input')[3].value;
        if (password === confirmPassword){
            let data = {
                email : document.getElementsByTagName('input')[1].value,
                password : password,
                wariorName : document.getElementsByTagName('input')[0].value
            }

            let url = ` https://localhost:5051/api/Authentication/register`;
            fetch(url,
                {
                    method: "POST",
                    body: JSON.stringify(data),
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                        'Authorization':'Bearer ' + sessionStorage.getItem("token")
                    }
                })
                .then((response) => {
                    if (response.status === 400){
                        throw new Error('This email has already been registered.');
                    }
                    else {
                        sessionStorage.setItem('userEmail', data.email);
                        window.location.replace('./index.html');
                    }
                })
                .catch((error) => {
                    outputDiv.appendChild(document.createTextNode(error.message));
                });
        } else {
            outputDiv.appendChild(document.createTextNode('The passwords do not match.'));
        }
    } else {
        outputDiv.appendChild(document.createTextNode('Your password must contain a minimum of 6 characters.'));
    }
}

function checkLogin(){
    let token = sessionStorage.getItem('token');
    if (token){
        window.location.href = './lobby.html';
    }
}

function makeElementEmpty(element) {
    while (element.hasChildNodes()) {
        element.removeChild(element.firstChild);
    }
}

const handleLoad = () => {
    checkLogin();

    let form = document.querySelector('form');
    form.addEventListener('submit', registerUser);
}

window.addEventListener('load', handleLoad);