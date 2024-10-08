// Get the modal
let modal = document.getElementById("myModal");

// Get the button that opens the modal
let btn = document.querySelector(".OpenLobbyModalButton");

// Get the <span> element that closes the modal
let span = document.getElementsByClassName("close")[0];

// Get the custom message modal
let customMessageModal = document.getElementById("customMessageModal");

// When the user clicks the button, open the modal
btn.onclick = function() {
    modal.style.display = "block";
}

// When the user clicks on <span> (x), close the modal
span.onclick = function() {
    modal.style.display = "none";
}

// When the user clicks anywhere outside the modal, close it
window.onclick = function(event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}

// Create lobby button functionality
document.getElementById("createLobbyBtn").addEventListener("click", function() {
    let lobbyName = document.getElementById("lobbyNameInput").value;
    // Check if lobby name is not empty
    if (lobbyName.trim() !== "") {
            // Close modal
            modal.style.display = "none";
            let data = {
                "numberOfPlayers": 2,
                "playerMatSize": 5,
                "moveCardSet": 0
            }
            let outputDiv = document.getElementById("errormessagediv");
            let url = 'https://localhost:5051/api/Tables';
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
                    return response.json();
                })
                .then((lobbyData) => {
                    sessionStorage.setItem('LobbyID', lobbyData.id);
                    window.location.href = './room.html';
                })
                .catch((error) => {
                    outputDiv.appendChild(document.createTextNode(error.message));
                });
        }
        else {
            // Display custom message for empty lobby name
            displayCustomMessage("Please enter a lobby name");
            }
    } 
  )

// Function to display custom message
function displayCustomMessage(message) {
    let customMessageText = document.getElementById("errormessagediv");
    customMessageText.textContent = message;
}

const handleLoad = () => {
    let logOutButton = document.getElementById('logout');
    logOutButton.addEventListener('click', logOut);
    const token = sessionStorage.getItem('token');
    
    if (!token) {
      alert('Session expired, Please log in again.');
    }

  
    let outputDiv = document.getElementById("errormessagediv");
    let url = 'https://localhost:5051/api/Tables/with-available-seats';
    fetch(url, {
        method: "GET",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + token
        }
    })
        .then((response) => {
            return response.json();
        })
        .then((availableRooms) => {
            console.log(availableRooms); // Controleren of de gegevens correct zijn ontvangen
            // Loop over beschikbare kamers en voeg ze toe aan HTML
            availableRooms.forEach(room => {
                let newLobby = document.createElement("div");
                newLobby.classList.add("lobby-item");
                newLobby.innerHTML = '<h3><span class="lobby-name" data-name="' + room.id + '">' + room.name + '</span></h3>' +
                    '<p>Warrior name: <span class="player-name">' + room.seatedPlayers[0].name + '</span></p>' +
                    '<button class="join-room-btn" data-room-id="' + room.id + '"> Enter Room ' +
                    '<svg viewBox="0 0 16 16" class="bi bi-arrow-right" fill="currentColor" height="20" width="20" xmlns="http://www.w3.org/2000/svg">' +
                    '<path d="M1 8a.5.5 0 0 1 .5-.5h11.793l-3.147-3.146a.5.5 0 0 1 .708-.708l4 4a.5.5 0 0 1 0 .708l-4 4a.5.5 0 0 1-.708-.708L13.293 8.5H1.5A.5.5 0 0 1 1 8z" fill-rule="evenodd"></path>' +
                    '</svg>' +
                    '</button>';
                document.querySelector(".scrolbox").appendChild(newLobby);
            });
            let joinRoomBtns = document.querySelectorAll(".join-room-btn");
            joinRoomBtns.forEach(btn => {
                btn.addEventListener("click", handleJoinRoom);
            });
        })
        .catch((error) => {
            outputDiv.appendChild(document.createTextNode(error.message));
        });
}

const handleJoinRoom = (event) => {
    let roomId = event.target.getAttribute("data-room-id");
    sessionStorage.setItem('LobbyID', roomId);
    let joinUrl = `https://localhost:5051/api/Tables/${roomId}/join`;

    fetch(joinUrl, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + sessionStorage.getItem("token")
        }
    })
        .then((reponse) => {
            sessionStorage.setItem('LobbyID', roomId);
            window.location.href = './room.html';
        })
        .catch((error) => {
            console.error(error);
        });
}

function logOut() {
  sessionStorage.clear();
  window.location.href = './index.html';
 
}

window.addEventListener('load', handleLoad);