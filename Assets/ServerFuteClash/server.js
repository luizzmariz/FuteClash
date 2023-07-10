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
    io.to(challenged).emit('startMatch', {oponent: players.find(e => e.id === socketId), oponentTeam: team});
    io.to(socketId).emit('startMatch', {oponent: players.find(e => e.id === challenged), oponentTeam: playerChallenged.team});
    console.log(playersInMatch);
  }
}

function GetQueueInfo(socketId)
{
  io.to(socketId).emit('playersList', {playersList: playersOnQueue, queueList: true});
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
      Queue(socket.id, data.team);
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
});

const port = process.env.PORT || 6900;
http.listen(port, () => 
{
  console.log(`Escutando no ip local, na porta:${port}`);
});