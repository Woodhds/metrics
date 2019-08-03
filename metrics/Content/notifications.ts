import * as signalR from '@aspnet/signalr';

const connection = new signalR.HubConnectionBuilder().withUrl('/notifications').build();

connection.on('count', args => {
  console.log(args)
});

connection.start().catch(err => console.log(err))