const express = require('express');
const app = express();
const http = require('http').Server(app);
const io = require('socket.io')(http);

app.get('/', (req, res) => {
  res.send('<h1>Hello world</h1>');
});

io.use((socket, next) => {
  if (socket.handshake.query.token === "UNITY") {
      next();
  } else {
      next(new Error("Authentication error"));
  }
});

let players = [];
let playersOnQueue = [];
let playersTeam = [];
let playersInMatch = [];
let match = [];

const playerDimension = 0.30;
const ballRadius = 0.15;
const gameHeight = 5.00;
const gameLength = 10.00;

/*
let posX = gameLength / 2;
let posY = gameHeight / 2;
let speedX = 0;
let speedY = 0;
let AGoalKeeperX = 0; //leftPaddleY
let BGoalKeeperX = 0; //rightPaddleY
let ALeftPlayerX = 0;
let BLeftPlayerX = 0;
let ALeftPlayerZ = 0;
let BLeftPlayerZ = 0;
let ARightPlayerX = 0;
let BRightPlayerX = 0;
let ARightPlayerZ = 0;
let BRightPlayerZ = 0;
let leftGoalKeeperX = 0;
let gameIsGoing = false;
let rightScore = 0;
let leftScore = 0;
*/

function playersIn(socketId, username, email)
{
  var usernameVerification = true;
  var emailVerification = true;
  var signed = false;
  if(players.length === 0)
  {
    signed = true;
    players.push(
      {
        id: socketId,
        playerUsername: username,
        playerEmail: email
      });
    io.to(socketId).emit('signIn', {signedCheck: signed, usernameCheck: usernameVerification, emailCheck: emailVerification});
    io.emit('playersList', {playersList: players, queueList: false});
    //onlinePlayers: players.length , 
    console.log(username + ' entered the game.');
  }
  else 
  {
    for(i = 0; i < players.length; i++)
    {
      if(players[i].playerUsername === username)
      {
        usernameVerification = false;
      }
      if(players[i].playerEmail === email)
      {
        emailVerification = false;
      }
    }
    if(usernameVerification === true && emailVerification === true)
    {
      signed = true;
      players.push(
        {
          id: socketId,
          playerUsername: username,
          playerEmail: email
        });
      io.to(socketId).emit('signIn', {signedCheck: signed, usernameCheck: usernameVerification, emailCheck: emailVerification});
      io.emit('playersList', {playersList: players, queueList: false});
      console.log(username + ' entered the game.');
    }
    else{
      io.to(socketId).emit('signIn', {signedCheck: signed, usernameCheck: usernameVerification, emailCheck: emailVerification});
      console.log('wtf');
    }
  }
};

function playersOut(socketId)
{
  if(!playersOnQueue.findIndex(e => e.id === socketId) === -1)
  {
    Queue(socketId);
  }
  for(i = players.findIndex(e => e.id === socketId); i < (players.length -1); i++)
  {
    players[i] = players[i+1];
  }
  players.pop()
  io.emit('playersList', {playersList: players, queueList: false});
  console.log("Alguém saiu, agr tem " + players.length + " jogadores Online");
}

function message(socketId, chatId, messageContent)
{
  var sender = players.find(e => e.id === socketId);
  if(chatId == 'global')
  {
    var messageFormat =
    {
      senderId: sender.playerUsername,
      chatId: 'global',
      message: messageContent
    }
    io.emit('message', {Message: messageFormat});
  }
  else
  {
    var messageFormat =
    {
      senderId: sender.playerUsername,
      chatId: socketId,
      message: messageContent
    }
    io.to(chatId).emit('message', {Message: messageFormat});
    console.log(`msg foi 2`);
  }
}

function Queue(socketId, team)
{
  var playerQueueing = players.find(e => e.id === socketId);
  if(playersOnQueue.length === 0)
  {
    playersOnQueue.push(playerQueueing);
    playersTeam.push(
      {
        player: playerQueueing,
        team: team
      });
    io.emit('playersList', {playersList: playersOnQueue, queueList: true});
    console.log(playerQueueing.playerUsername + " entrou da fila pelo metodo 1, agr tem " + playersOnQueue.length + " na fila");
  }
  else
  {
    if(playersOnQueue.includes(playerQueueing))
    {
      for(i = playersOnQueue.findIndex(e => e === playerQueueing); i < (playersOnQueue.length -1); i++)
      {
        if(i === playersOnQueue.findIndex(e => e === playerQueueing))
        {
          console.log(playersOnQueue[i].playerUsername + " saiu da fila, agr tem " + playersOnQueue.length + " na fila");
        }
        playersOnQueue[i] = playersOnQueue[i+1];
        playersTeam[i] = playersTeam[i+1]
      }
      playersOnQueue.pop();
      playersTeam.pop();
      io.emit('playersList', {playersList: playersOnQueue, queueList: true});
    }
    else
    {
      playersOnQueue.push(playerQueueing);
      playersTeam.push(
        {
          player: playerQueueing,
          team: team
        });
      io.emit('playersList', {playersList: playersOnQueue, queueList: true});
      console.log(playerQueueing.playerUsername + " entrou da fila pelo metodo 2, agr tem " + playersOnQueue.length + " na fila");
    }
  }
}

function CreatingMatch(socketId, team, challenged)
{
  if((playersInMatch.findIndex(e => e.id === challenged) === -1))
  {
    var playerChallenged = playersTeam.find(e => e.player === players.find(e => e.id === challenged));
    playersInMatch.push(players.find(e => e.id === challenged));
    Queue(challenged);
    playersInMatch.push(players.find(e => e.id === socketId));
    match.push(
      {
        id: socketId+challenged,
        leftPlayer: socketId,
        rightPlayer: challenged,
        posX: 0,
        posY: 0,
        speedX: 0,
        speedY: 0,
        AGoalKeeperX: 0,
        BGoalKeeperX: 0,
        ALeftPlayerZ: 0,
        BLeftPlayerZ: 0,
        ARightPlayerZ: 0,
        BRightPlayerZ: 0,
        leftGoalKeeperX: 0,
        gameIsGoing: false,
        rightScore: 0,
        leftScore: 0
      });
    io.to(challenged).emit('startMatch', {oponent: players.find(e => e.id === socketId), oponentTeam: team, matchId: match[match.length - 1].Id});
    io.to(socketId).emit('startMatch', {oponent: players.find(e => e.id === challenged), oponentTeam: playerChallenged.team, matchId: match[match.length - 1].Id});
    console.log(playersInMatch);
  }
}

function GetQueueInfo(socketId)
{
  io.to(socketId).emit('playersList', {playersList: playersOnQueue, queueList: true});
}

// function score(score, match)
// {
//   if(score === 'left')
//   {
//     match.leftScore = match.leftScore + 1;
//     io.to(match.leftPlayer).emit('match', {pontuatiuon: "" + match.leftScore + " : " + match.rightScore})
//     io.to(match.rightPlayer).emit('match', {pontuatiuon: "" + match.leftScore + " : " + match.rightScore})
//     if(leftScore === 7)
//     {
//       endGame('left', match)
//     }
//     else
//     {
//       ballControl();
//     }
//   }
//   if(score === 'right')
//   {
//     rightScore = rightScore + 1;
//     io.to(match.leftPlayer).emit('match', {pontuatiuon: "" + match.leftScore + " : " + match.rightScore})
//     io.to(match.rightPlayer).emit('match', {pontuatiuon: "" + match.leftScore + " : " + match.rightScore})
//     if(rightScore === 7)
//     {
//       endGame('right', match);
//     }
//     else
//     {
//       ballControl();
//     }
//   }
// };

// function ballControl()
// {
//   if(gameIsGoing == true)
//   {
//     posX = gameLength / 2;
//     posY = gameHeight / 2;
//     speedX = -3;
//     speedY = 3;
//   }
//   if(gameIsGoing == false)
//   {
//     gameIsGoing = true;
//     speedX = 3;
//     speedY = -3;
//     updateBall();
//   }
// };

// function updateBall(match) 
// {
//   if(gameIsGoing)
//   {
//     match.posX += match.speedX;
//     match.posY += match.speedY;
//     checkColision(match);
//     setTimeout(() => 
//     {
//       updateBall(match);
//     }, 100);
//   }
// };

// function checkColision(match)
// {
//   if(match.posY < -2.5 + ballRadius)
//   {
//     match.speedY *= -1;
//   }
//   if(match.posY > 2.5 - ballRadius)
//   {
//     match.speedY *= -1;
//   }
//   if(match.posX >= 5 - ballRadius)
//   {
//     score('right', match);
//   }
//   if(match.posX <= (-5 + ballRadius))
//   {
//     score('left', match);
//   }
//   if((match.posX < (match.AGoalKeeperZ + (playerDimension/2))) && (match.posX >= (match.AGoalKeeperZ - (playerDimension/2) - ballRadius)))
//   {
//     if(match.posY > (match.AGoalKeeperX - (playerDimension/2) -ballRadius) && match.posY < (match.AGoalKeeperX + (playerDimension/2)))
//     {
//       match.speedX *= -1;
//       match.posX = match.AGoalKeeperZ - (playerDimension/2) - ballRadius;
//     }
//   }
//   if(match.posX > (match.BGoalKeeperZ - (playerDimension/2)) && match.posX <= (match.BGoalKeeperZ + (playerDimension/2) + ballRadius))
//   {
//     if(match.posY > (match.BGoalKeeperX - (playerDimension/2) -ballRadius) && match.posY < (match.BGoalKeeperX + (playerDimension/2)))
//     {
//       match.speedX *= -1;
//       match.posX = gameLength - paddleWidth - ballRadius;
//     }
//   }
// };

// function endGame(winner, match)
// {
//   if(winner === 'left')
//   {

//   }
//   match.gameIsGoing = false;
//   match.rightScore, leftScore = 0;
//   posX = gameLength / 2;
//   posY = gameHeight / 2;
//   speedX = 0;
//   speedY = 0;
//   io.to(match.leftPlayer).emit('endGame', {winner: ""})
// }

function UpdateMatch(socketId, data)
{
  let matchI = match.findIndex(e => e.id === data.matchId);
  if(socketId === match.leftPlayer)
  {
    match[matchI].AGoalKeeperX = data.goalKeeperX * -1;
    match[matchI].ALeftPlayerZ = data.leftPlayerZ * -1;
    match[matchI].ARightPlayerZ = data.rightPlayerZ * -1;

    io.to(match.rightPlayer).emit('matchPositions', {goalKeeperX: match[matchI].AGoalKeeperX, leftPlayerZ: match[matchI].ALeftPlayerZ, rightPlayerZ: match[matchI].ARightPlayerZ});
  }
  else
  {
    match[matchI].BGoalKeeperX = data.goalKeeperX;
    match[matchI].BLeftPlayerZ = data.leftPlayerZ;
    match[matchI].BRightPlayerZ = data.rightPlayerZ;

    io.to(match.leftPlayer).emit('matchPositions', {goalKeeperX: match[matchI].BGoalKeeperX, leftPlayerZ: match[matchI].BLeftPlayerZ, rightPlayerZ: match[matchI].BRightPlayerZ});
  }
}

io.on('connection', (socket) => 
{

  console.log('a user connected');
  socket.on('disconnect', () =>
  {
    playersOut(socket.id)
    console.log('a user disconnected');
  })

  socket.on('signIn', (data) => 
  {
    playersIn(socket.id, data.playerUsername, data.playerEmail);
  })

  socket.on('message', (data) => 
  {
    message(socket.id, data.chatId, data.message);
  })

  socket.on('onQueue', (data) => 
  {
      Queue(socket.id, data);
  })

  socket.on('challengeSomeone', (data) => 
  {
    if((playersInMatch.findIndex(e => e.id === data.oponent.id) === -1))
    {
      CreatingMatch(socket.id, data.team, data.oponent.id);
    }
  })

  socket.on('getQueueInfo', (data) => 
  {
    GetQueueInfo(socket.id);
  })

  socket.on('match', (data) => 
  {
    UpdateMatch(socket.id, data);
  })
});

const port = process.env.PORT || 6900;
http.listen(port, () => 
{
  console.log(`Escutando no ip local, na porta:${port}`);
});