import Vue from 'vue';
import UserComponent from './Components/usercomponent.vue';
import './notifications';

document.addEventListener("DOMContentLoaded", function () {
  const user = document.getElementById('user');
  if(user) {
    new Vue({
      render: h => h(UserComponent)
    }).$mount('#user');
  }
});