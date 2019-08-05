import * as signalR from '@aspnet/signalr';

const connection = new signalR.HubConnectionBuilder().withUrl('/notifications').build();

connection.on('count', (count: number) => {
  let el = document.getElementById('actions-count');

  if (el != null) {
    let countTag = el.getElementsByClassName('count');
    if (countTag.length > 0) {
      if (count != 0) {
        countTag[0].innerHTML = `${count}`;
        el.classList.remove('hidden')
      } else {
        el.classList.add('hidden');
        countTag[0].innerHTML = '';
      }
    }
  }
});

connection.start().catch(err => console.log(err));