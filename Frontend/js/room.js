const handleLoad = () => {
    console.log(sessionStorage.getItem('LobbyID'))

    const leaveGameButton = document.getElementById('leaveButton');
    leaveGameButton.addEventListener("click", handleLeaveGame);

    const startGameButton = document.getElementById('startButton');
    startGameButton.addEventListener("click", handleStartGame);

    const skipButton = document.getElementById('skipButton');
    skipButton.addEventListener("click", handleSkipMove);

    const outputDiv = document.getElementById("outputdiv");
    let opponentFound = false;
    let gameStarted = false;

    const fetchData = () => {
        const url = `https://localhost:5051/api/Tables/${sessionStorage.getItem('LobbyID')}`;
        fetch(url, {
            method: "GET",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + sessionStorage.getItem("token")
            }
        })
            .then((response) => {
                return response.json();
            })
            .then((roomData) => {
                sessionStorage.setItem('guid', roomData.gameId);
                const ownerID = roomData.ownerPlayerId;
                sessionStorage.setItem('ownerID', ownerID);

                makeElementEmpty(outputDiv);

                if (roomData.gameId == '00000000-0000-0000-0000-000000000000'){
                    gameStarted = false;
                } else {

                    gameStarted = true;

                    let startButton = document.getElementById('startButton');
                    startButton.style.display = 'none';

                    let skipButton = document.getElementById('skipButton');
                    skipButton.style.display = 'block';

                    console.log(sessionStorage.getItem('selectedCard'));
                    console.log(sessionStorage.getItem('selectedPawn'));
                    const gameUrl = `https://localhost:5051/api/Games/${sessionStorage.getItem('guid')}`;

                    fetch(gameUrl, {
                        method: "GET",
                        headers: {
                            'Accept': 'application/json',
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + sessionStorage.getItem("token")
                        }
                    })
                        .then((response) => {
                            const testData = response.json()
                            return testData;
                        })
                        .then((data) => {
                            sessionStorage.setItem('winner',data.winnerPlayerId);
                            sessionStorage.setItem('winnerMethod' , data.winnerMethod);
                            if (data.winnerMethod === null){
                                console.log('no winner method');
                            } else {
                                if (sessionStorage.getItem('player1Id') === data.winnerPlayerId){
                                    outputDiv.appendChild(document.createTextNode(sessionStorage.getItem('player1Name') + " won with " + data.winnerMethod));
                                } else {
                                    outputDiv.appendChild(document.createTextNode(sessionStorage.getItem('player2Name') + " won with " + data.winnerMethod));
                                }
                            }
                            const extra = document.getElementById('extra');
                            const extraMoveCard = data.extraMoveCard;
                            console.log(extraMoveCard);
                            const playerToPlay = data.playerToPlayId;
                            fillBoard(data.playMat.grid);
                            sessionStorage.setItem('toGo',playerToPlay);
                            createCard(extraMoveCard.grid,extra,extraMoveCard.stampColor,extraMoveCard.name);
                            let spelbeurtDiv = document.getElementById('spelbeurt');
                            makeElementEmpty(spelbeurtDiv);
                            spelbeurtDiv.appendChild(document.createTextNode(extraMoveCard.stampColor + " kleur is aan de beurt."));
                        })

                    makeElementEmpty(outputDiv);
                    handleFillCards(roomData.seatedPlayers, ownerID);
                    sessionStorage.setItem('guid', roomData.gameId);
                }

                if (roomData.seatedPlayers.length === 2) {
                    const player1 = roomData.seatedPlayers[0];
                    sessionStorage.setItem('player1Name',roomData.seatedPlayers[0].name);
                    sessionStorage.setItem('player1Id',roomData.seatedPlayers[0].id);
                    const player2 = roomData.seatedPlayers[1];
                    sessionStorage.setItem('player2Name',roomData.seatedPlayers[1].name);
                    sessionStorage.setItem('player2Id',roomData.seatedPlayers[1].id);
                    sessionStorage.setItem('controlColor', roomData.seatedPlayers[0].color);
                    if (roomData.seatedPlayers[0].id === sessionStorage.getItem('userid')) {
                        sessionStorage.setItem('userColor', roomData.seatedPlayers[0].color);
                    } else {
                        sessionStorage.setItem('userColor', roomData.seatedPlayers[1].color);
                    }
                    sessionStorage.setItem('southColor', roomData.seatedPlayers[1].color);
                    const player1Box = createPlayerBox(player1);
                    const player2Box = createPlayerBox(player2);

                    if (player1Box && player2Box) {
                        outputDiv.appendChild(player1Box);
                        outputDiv.appendChild(player2Box);

                        if (sessionStorage.getItem('userid') === player2.id ) {
                            if (!gameStarted){
                                let pElement = document.createElement('p');
                                pElement.textContent = "Waiting for the owner to start the game...";
                                outputDiv.appendChild(pElement);
                            }
                        }
                        // Opponent found
                        opponentFound = true;
                    }
                } else if (!opponentFound) {
                    let pElement = document.createElement('p');
                    outputDiv.appendChild(pElement);
                    pElement.appendChild(document.createTextNode("Searching for opponent..."))
                } else {
                    let pElement = document.createElement('p');
                    outputDiv.appendChild(pElement);
                    pElement.appendChild(document.createTextNode("Opponent left."))
                }
            })
            .catch((error) => {
                console.error(error);
            });
    };
    if (!gameStarted){
        setInterval(fetchData, 3750);
    }
};

const handleLeaveGame = () => {
    const lobbyID = sessionStorage.getItem('LobbyID');
    const url = `https://localhost:5051/api/Tables/${lobbyID}/leave`;

    fetch(url, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + sessionStorage.getItem("token")
        }
    })
        .then((response) => {
            if (response.ok) {
                sessionStorage.removeItem('LobbyID');
                window.location.href = './lobby.html';
            } else {
                console.error("Error leaving the game:", response.statusText);
            }
        })
        .catch((error) => {
            console.error("Error leaving the game:", error.message);
        })
        .finally(() => {
            opponentFound = false; // Reset opponentFound when leaving the game
        });
};

const handleSkipMove = () => {
    const outputdiv = document.getElementById('outputdiv');

    let url = `https://localhost:5051/api/Games/${sessionStorage.getItem('guid')}/skip-movement`;
    fetch(url, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + sessionStorage.getItem("token")
        }
    })
        .then((response) => {
            if (response.status === 400) {
                outputdiv.appendChild(document.createTextNode("You can't skip movement, you have available cards to play!"));
            } else if (response.ok) {
                outputdiv.appendChild(document.createTextNode("Movement skipped!"));
            } else {
                outputdiv.appendChild(document.createTextNode("An unexpected error occurred."));
            }
        })
};


const createPlayerBox = (player) => {
    if (!player) {
        console.error("Player data is missing.");
        return null;
    }

    const playerBox = document.createElement("div");
    playerBox.classList.add("player-box");
    if (player.color) {
        playerBox.style.backgroundColor = player.color;
    } else {
        console.error("Player color is missing or undefined.");
        return null;
    }

    const playerName = document.createElement("span");
    playerName.classList.add("player-name");

    if (player.name) {
        playerName.textContent = player.name;
    } else {
        console.error("Player name is missing or undefined.");
        return null;
    }

    playerBox.appendChild(playerName);

    return playerBox;
};

function fillBoard(grid) {
    // go through grid to find positions
    for (i in grid){
        for (j in grid[i]){
            // if grid element isn't null we're gonna add a pawn or a master
            if (grid[i][j] === null){
                let first = i
                let second = j
                const row = document.getElementById(`row${first}-${second}`)
                makeElementEmpty(row)
            } else {
                // here we decide what color of pawn/master to render
                if (sessionStorage.getItem('ownerID') === grid[i][j].OwnerId){
                    const img = document.createElement('img')
                    // is it a master or a pawn?
                    if (grid[i][j].Type === 0){
                        img.src = "assets/pink-master.png"
                    } else {
                        img.src = "assets/pink-pawn.png"
                    }
                    img.id = grid[i][j].Id
                    let first = Number(i)
                    let second = Number(j)
                    const row = document.getElementById(`row${first}-${second}`)
                    makeElementEmpty(row)
                    img.onclick = function () {selectPawn(grid[first][second])}
                    //add image to board
                    row.appendChild(img)
                } else {
                    const img = document.createElement('img')
                    // is it a master or a pawn?
                    if (grid[i][j].Type === 0){
                        img.src = "assets/blue-master.png"
                    } else {
                        img.src = "assets/blue-pawn.png"
                    }
                    img.id = grid[i][j].Id
                    let first = i
                    let second = j
                    const row = document.getElementById(`row${first}-${second}`)
                    makeElementEmpty(row)
                    img.onclick = function () {
                        selectPawn(grid[first][second])}
                    //add image to board
                    row.appendChild(img)
                }
            }
        }
    }
}

const handleFillCards = (seatedPlayers, ownerID) => {
    makeElementEmpty(document.querySelector('.opponent-cards'));
    makeElementEmpty(document.querySelector('.my-cards'));

    seatedPlayers.forEach(player => {
        const isOwner = player.id === ownerID;

        const playerColor = player.color.toLowerCase();

        const cardContainer = document.querySelector(isOwner ? '.my-cards' : '.opponent-cards');
        player.moveCards.forEach(card => {
            const cardElement = document.createElement('div');
            createCard(card.grid,cardElement,playerColor,card.name)
            cardContainer.appendChild(cardElement);
        });
    });
};

const handleStartGame = () => {
    sessionStorage.setItem('selectedCard',null)
    sessionStorage.setItem('selectedPawn', null)
    const outputDiv = document.getElementById("outputdiv");
    const LobbyID = sessionStorage.getItem('LobbyID');
    const extra = document.getElementById('extra');
    makeElementEmpty(extra)
    const ownerID = sessionStorage.getItem('ownerID');
    const currentUserID = sessionStorage.getItem('userid');
    if (ownerID !== currentUserID) {
        makeElementEmpty(outputDiv);
        outputDiv.appendChild(document.createTextNode("Error: Only the owner of the table can start the game."));
        return;
    }

    const url = `https://localhost:5051/api/Tables/${LobbyID}/start-game`;

    fetch(url, {
        method: "POST",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + sessionStorage.getItem("token")
        }
    })
        .then((response) => {
            if (!response.ok) {
                makeElementEmpty(outputDiv);
                outputDiv.appendChild(document.createTextNode("Error: No opponent found, wait for opponent."));
            }
            return response.json();
        })
        .then((response) => {
            console.log("Game started");

            handleFillCards(response.seatedPlayers, ownerID);

        })
        .catch((error) => {
            console.error("Error starting the game:", error.message);
        });
};

function selectPawn(pawn) {
    const outputDiv = document.getElementById("outputdiv");
    const currentPlayerId = sessionStorage.getItem('userid');
    const selectedPawnOwnerId = pawn.OwnerId;

    if (currentPlayerId === selectedPawnOwnerId) {
        console.log('Your Pawn is selected: ' + pawn.Id);
        sessionStorage.setItem('selectedPawn', pawn.Id);

        if (sessionStorage.getItem('selectedCard') !== null) {
            console.log('started');
            const possibleMovesUrl = `https://localhost:5051/api/Games/${sessionStorage.getItem('guid')}/possible-moves-of/${sessionStorage.getItem('selectedPawn')}/for-card/${sessionStorage.getItem('selectedCard')}`;

            fetch(possibleMovesUrl, {
                method: "GET",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + sessionStorage.getItem("token")
                }
            })
                .then((response) => {
                    return response.json();
                })
                .then((posMoves) => {
                    for (let i in posMoves) {
                        console.log(posMoves[i].to);
                        let row = posMoves[i].to.row;
                        let col = posMoves[i].to.column;
                        const cell = document.getElementById(`row${Number(row)}-${col}`);
                        cell.style.backgroundColor = "grey";
                        cell.onclick = function () { movePawn(row, col) };
                    }
                });
        }
    } else {
        if (pawn.OwnerId !== null) {
            console.log('Opponent\'s Pawn selected: ' + pawn.Id);
            console.log('You are eating opponent\'s pawn!');
        } else {
            outputDiv.appendChild(document.createTextNode('This is not your pawn to select'));
        }
    }
}

function selectCard(cardName){
    const outputDiv = document.getElementById("outputdiv");
    console.log('Card selected: ' + cardName);
    if (sessionStorage.getItem('userid')=== sessionStorage.getItem('toGo')){
        sessionStorage.setItem('selectedCard', cardName);
        console.log('its your turn and you selected ' + cardName );
        sessionStorage.setItem('selectedCard', cardName);
        const card = document.getElementById(cardName);
        console.log(("" + card.className).toUpperCase());
        console.log(("" + sessionStorage.getItem('controlColor')).toUpperCase());
        if(("" + card.className).toUpperCase() === ("" + sessionStorage.getItem('userColor')).toUpperCase()) {
            card.style.backgroundColor = "blue";
        } else{
            outputDiv.appendChild(document.createTextNode('this is not your card to select'));
        }
    } else {
        console.log('its not your turn but you selected ' + cardName);
    }
}

function createCard(grid,location,color, name){
    if((color + "").toUpperCase() === (sessionStorage.getItem('southColor') + "").toUpperCase()) {
        for(i in grid) {
            grid[i].reverse()
        }
        grid.reverse();
    }
    if (location.querySelector("table") === null){
        tbl = document.createElement('table');
        tbl.id = name;
        tbl.className = color;
        tbl.style.width = "200px"
        tbl.style.height = ""
        tbl.style.border = "1px solid black"
        tbl.style.margin = "0px"
        tbl.onclick = function () {
            console.log('Card clicked: ' + name);
            selectCard(name)}
        for (i in grid) {
            tr = document.createElement('tr');
            for (j in grid[i]) {
                td = document.createElement('td');
                td.style.width = "20%";
                td.style.height = "27px";
                if (grid[i][j] === 1){
                    td.style.backgroundColor = color
                } else if (grid[i][j] === 2){
                    td.style.backgroundColor = "black";
                    td.style.color = "white";
                    td.style.textAlign = "center";
                    td.style.fontSize = "8px";
                    td.textContent = name;
                } else if (grid[i][j] === 0) {
                    td.style.backgroundColor = "white";
                }
                tr.appendChild(td);
            }
            tbl.appendChild(tr);
            tbl.style.float = "left";
        }
        location.appendChild(tbl)
    }
}

function movePawn(row,column) {
    let data = {
        "pawnId": "" + sessionStorage.getItem("selectedPawn"),
        "moveCardName": "" + sessionStorage.getItem("selectedCard"),
        "to": {
            "row": row,
            "column": column
        }
    }
    const url = `https://localhost:5051/api/games/${sessionStorage.getItem('guid')}/move-pawn`;

    fetch(url, {
        method: "POST",
        body: JSON.stringify(data),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + sessionStorage.getItem("token")
        }
    })
    const board = document.getElementById("board");
    const cells = board.getElementsByTagName('td')
    for (i in cells){
        cells[i].style.backgroundColor = "#c2b280"
    }
}

function makeElementEmpty(element) {
    while (element.hasChildNodes()) {
        element.removeChild(element.firstChild);
    }
}

window.addEventListener('load', handleLoad);