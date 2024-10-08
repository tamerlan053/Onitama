function checkLogin() {
    let token = window.sessionStorage.getItem('token');
    if (token) {
        window.location.href = './lobby.html';
    }
}

function login(event) {
    event.preventDefault();
    let url = 'https://localhost:5051/api/Authentication/token';
    let data = {
        email : document.getElementById('input1').value,
        password : document.getElementById('input2').value,
    }
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
            if (response.status == 200) {
                response.json().then(data => {console.log(
                    sessionStorage.setItem("token", data.token));
                    sessionStorage.setItem("userid", data.user.id)
                    dataString = JSON.stringify(data.user).split(",")
                    warriorname = dataString[2].split(":")
                    window.sessionStorage.setItem('warriorname',warriorname[1].replace('"',"").replace('"}',''))
                })
                window.location.replace('./lobby.html');
            }
            //verkeerde inloggevens
            else if (response.status == 400 || response.status == 401) {
                ErrorMessage(1);
            } else {
                ErrorMessage()
            }
        })
}

function ErrorMessage(modifier) {
    if (modifier == 1) {
        let error = document.getElementById("error")
        error.innerHTML = "<span style='color: red;'>"+ "Your email or password is incorrect" + "</span>"
    } else {
        let error = document.getElementById("error")
        error.innerHTML = "<span style='color: red;'>"+ "Unexpected error" + "</span>"
    }
}

function fillEmailIn() {
    let emailAdress = sessionStorage.getItem('userEmail');
    if (emailAdress) {
        let emailVeld = document.querySelector('input');
        emailVeld.value = emailAdress;
    }
}

const handleLoad = () => {
    fillEmailIn();
    checkLogin();
    let form = document.querySelector('form');
    form.addEventListener('submit', login);
}

window.addEventListener('load', handleLoad);